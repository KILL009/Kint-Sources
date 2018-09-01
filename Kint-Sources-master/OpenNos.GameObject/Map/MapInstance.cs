/*
 * This file is part of the OpenNos Emulator Project. See AUTHORS file for Copyright information
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 */

using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.PathFinder;
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace OpenNos.GameObject
{
    public class MapInstance : BroadcastableBase
    {
        #region Members

        private readonly ThreadSafeSortedList<int, int> _mapMonsterIds;

        private readonly ThreadSafeSortedList<int, int> _mapNpcIds;

        private readonly ThreadSafeSortedList<long, MapMonster> _monsters;

        private readonly ThreadSafeSortedList<long, MapNpc> _npcs;

        private readonly Random _random;

        private bool _isSleeping;

        private bool _isSleepingRequest;
            
        #endregion

        #region Instantiation

        public MapInstance(Map map, Guid guid, bool shopAllowed, MapInstanceType type, InstanceBag instanceBag)
        {
            Buttons = new List<MapButton>();
            XpRate = 1;
            DropRate = 1;
            InstanceMusic = map.Music;
            ShopAllowed = shopAllowed;
            MapInstanceType = type;
            _isSleeping = true;
            LastUserShopId = 0;
            InstanceBag = instanceBag;
            Clock = new Clock(3);
            _random = new Random();
            Map = map;
            MapInstanceId = guid;
            ScriptedInstances = new List<ScriptedInstance>();
            OnCharacterDiscoveringMapEvents = new List<Tuple<EventContainer, List<long>>>();
            OnMoveOnMapEvents = new ThreadSafeGenericList<EventContainer>();
            OnAreaEntryEvents = new ThreadSafeGenericList<ZoneEvent>();
            WaveEvents = new List<EventWave>();
            OnMapClean = new List<EventContainer>();
            _monsters = new ThreadSafeSortedList<long, MapMonster>();
            _npcs = new ThreadSafeSortedList<long, MapNpc>();
            _mapMonsterIds = new ThreadSafeSortedList<int, int>();
            _mapNpcIds = new ThreadSafeSortedList<int, int>();
            DroppedList = new ThreadSafeSortedList<long, MapItem>();
            Portals = new List<Portal>();
            UserShops = new Dictionary<long, MapShop>();
            StartLife();
        }

       

        #endregion

        #region Properties

        public List<MapButton> Buttons { get; set; }

        public bool IsMute { get; set; }

        public Clock Clock { get; set; }

        public ThreadSafeSortedList<long, MapItem> DroppedList { get; }

        public int DropRate { get; set; }

        public InstanceBag InstanceBag { get; set; }

        public int InstanceMusic { get; set; }

        public bool IsPvp { get; set; }

        public bool IsDancing { get; set; }

        public bool IsPVP { get; set; }

        public bool IsSleeping
        {
            get
            {
                if (_isSleepingRequest && !_isSleeping && LastUnregister.AddSeconds(30) < DateTime.Now)
                {
                    _isSleeping = true;
                    _isSleepingRequest = false;
                    return true;
                }
                return _isSleeping;
            }
            set
            {
                if (value)
                {
                    _isSleepingRequest = true;
                }
                else
                {
                    _isSleeping = false;
                    _isSleepingRequest = false;
                }
            }
        }

        public long LastUserShopId { get; set; }

        public Map Map { get; set; }

        public byte MapIndexX { get; set; }

        public byte MapIndexY { get; set; }

        public Guid MapInstanceId { get; set; }

        public MapInstanceType MapInstanceType { get; set; }

        public List<MapMonster> Monsters => _monsters.GetAllItems();

        public List<MapNpc> Npcs => _npcs.GetAllItems();

        public ThreadSafeGenericList<ZoneEvent> OnAreaEntryEvents { get; set; }

        public List<Tuple<EventContainer, List<long>>> OnCharacterDiscoveringMapEvents { get; set; }

        public List<EventContainer> OnMapClean { get; set; }

        public ThreadSafeGenericList<EventContainer> OnMoveOnMapEvents { get; set; }

        public List<Portal> Portals { get; }

        public List<ScriptedInstance> ScriptedInstances { get; set; }

        public bool ShopAllowed { get; set; }

        public Dictionary<long, MapShop> UserShops { get; }

        public List<EventContainer> UnlockEvents { get; set; }

        public List<EventWave> WaveEvents { get; set; }

        public int XpRate { get; set; }

        #endregion

        #region Methods

        public void AddMonster(MapMonster monster) => _monsters[monster.MapMonsterId] = monster;

        public void AddNPC(MapNpc npc) => _npcs[npc.MapNpcId] = npc;

        public void DespawnMonster(int monsterVnum)
        {
            Parallel.ForEach(_monsters.Where(s => s.MonsterVNum == monsterVnum), monster =>
            {
                monster.IsAlive = false;
                monster.LastMove = DateTime.Now;
                monster.CurrentHp = 0;
                monster.CurrentMp = 0;
                monster.Death = DateTime.Now;
                Broadcast(StaticPacketHelper.Out(UserType.Monster, monster.MapMonsterId));
            });
        }

        public void DropItemByMonster(long? owner, DropDTO drop, short mapX, short mapY)
        {
            try
            {
                short localMapX = mapX;
                short localMapY = mapY;
                List<MapCell> possibilities = new List<MapCell>();

                for (short x = -1; x < 2; x++)
                {
                    for (short y = -1; y < 2; y++)
                    {
                        possibilities.Add(new MapCell { X = x, Y = y });
                    }
                }

                foreach (MapCell possibilitie in possibilities.OrderBy(s => ServerManager.RandomNumber()))
                {
                    localMapX = (short)(mapX + possibilitie.X);
                    localMapY = (short)(mapY + possibilitie.Y);
                    if (!Map.IsBlockedZone(localMapX, localMapY))
                    {
                        break;
                    }
                }

                MonsterMapItem droppedItem = new MonsterMapItem(localMapX, localMapY, drop.ItemVNum, drop.Amount, owner ?? -1);
                DroppedList[droppedItem.TransportId] = droppedItem;
                Broadcast($"drop {droppedItem.ItemVNum} {droppedItem.TransportId} {droppedItem.PositionX} {droppedItem.PositionY} {(droppedItem.GoldAmount > 1 ? droppedItem.GoldAmount : droppedItem.Amount)} 0 0 -1");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public void DropItems(List<Tuple<short, int, short, short>> list)
        {
            // TODO: Parallelize, if possible.
            foreach (Tuple<short, int, short, short> drop in list)
            {
                MonsterMapItem droppedItem = new MonsterMapItem(drop.Item3, drop.Item4, drop.Item1, drop.Item2);
                DroppedList[droppedItem.TransportId] = droppedItem;
                Broadcast($"drop {droppedItem.ItemVNum} {droppedItem.TransportId} {droppedItem.PositionX} {droppedItem.PositionY} {(droppedItem.GoldAmount > 1 ? droppedItem.GoldAmount : droppedItem.Amount)} 0 0 -1");
            }
        }

        public IEnumerable<string> GenerateNPCShopOnMap() => (from npc in Npcs where npc.Shop != null select $"shop 2 {npc.MapNpcId} {npc.Shop.ShopId} {npc.Shop.MenuType} {npc.Shop.ShopType} {npc.Shop.Name}").ToList();

        public IEnumerable<string> GeneratePlayerShopOnMap() => UserShops.Select(shop => $"pflag 1 {shop.Value.OwnerId} {shop.Key + 1}").ToList();

        public string GenerateRsfn(bool isInit = false)
        {
            if (MapInstanceType == MapInstanceType.TimeSpaceInstance)
            {
                return $"rsfn {MapIndexX} {MapIndexY} {(isInit ? 1 : (Monsters.Where(s => s.IsAlive).ToList().Count == 0 ? 0 : 1))}";
            }
            return string.Empty;
        }

        public IEnumerable<string> GenerateUserShops() => UserShops.Select(shop => $"shop 1 {shop.Value.OwnerId} 1 3 0 {shop.Value.Name}").ToList();

        public List<MapMonster> GetListMonsterInRange(short mapX, short mapY, byte distance) => _monsters.Where(s => s.IsAlive && s.IsInRange(mapX, mapY, distance)).ToList();

        public List<string> GetMapItems()
        {
            List<string> packets = new List<string>();
            Sessions.Where(s => s.Character?.InvisibleGm == false).ToList().ForEach(s => s.Character.Mates.Where(m => m.IsTeamMember).ToList().ForEach(m => packets.Add(m.GenerateIn())));
            Portals.ForEach(s => packets.Add(s.GenerateGp()));
            ScriptedInstances.Where(s => s.Type == ScriptedInstanceType.TimeSpace).ToList().ForEach(s => packets.Add(s.GenerateWp()));
            Monsters.ForEach(s =>
            {
                packets.Add(s.GenerateIn());
                if (s.IsBoss)
                {
                    packets.Add(s.GenerateBoss());
                }
            });
            Npcs.ForEach(s => packets.Add(s.GenerateIn()));
            packets.AddRange(GenerateNPCShopOnMap());
            DroppedList.ForEach(s => packets.Add(s.GenerateIn()));
            Buttons.ForEach(s => packets.Add(s.GenerateIn()));
            packets.AddRange(GenerateUserShops());
            packets.AddRange(GeneratePlayerShopOnMap());
            return packets;
        }

        public MapMonster GetMonster(long mapMonsterId) => _monsters[mapMonsterId];

        public int GetNextMonsterId()
        {
            int nextId = _mapMonsterIds.Count > 0 ? _mapMonsterIds.Last() + 1 : 1;
            _mapMonsterIds[nextId] = nextId;
            return nextId;
        }

        public int GetNextNpcId()
        {
            int nextId = _mapNpcIds.Count > 0 ? _mapNpcIds.Last() + 1 : 1;
            _mapNpcIds[nextId] = nextId;
            return nextId;
        }

        public MapNpc GetNpc(long mapNpcId) => _npcs[mapNpcId];

        public void LoadMonsters()
        {
            Parallel.ForEach(DAOFactory.MapMonsterDAO.LoadFromMap(Map.MapId).ToList(), monster =>
            {
                MapMonster mapMonster = new MapMonster(monster);
                mapMonster.Initialize(this);
                int mapMonsterId = mapMonster.MapMonsterId;
                _monsters[mapMonsterId] = mapMonster;
                _mapMonsterIds[mapMonsterId] = mapMonsterId;
            });
        }

        public void LoadNpcs()
        {
            Parallel.ForEach(DAOFactory.MapNpcDAO.LoadFromMap(Map.MapId).ToList(), npc =>
            {
                MapNpc mapNpc = new MapNpc(npc);
                mapNpc.Initialize(this);
                int mapNpcId = mapNpc.MapNpcId;
                _npcs[mapNpcId] = mapNpc;
                _mapNpcIds[mapNpcId] = mapNpcId;
            });
        }

        public void LoadPortals()
        {
            foreach (PortalDTO portal in DAOFactory.PortalDAO.LoadByMap(Map.MapId))
            {
                Portal p = new Portal(portal)
                {
                    SourceMapInstanceId = MapInstanceId
                };
                Portals.Add(p);
            }
        }

        public void MapClear()
        {
            Broadcast("mapclear");
            Parallel.ForEach(GetMapItems(), s => Broadcast(s));
        }

        public MapItem PutItem(InventoryType type, short slot, short amount, ref ItemInstance inv, ClientSession session)
        {
            Logger.LogUserEventDebug("PUTITEM", session.GenerateIdentity(), $"type: {type} slot: {slot} amount: {amount}");
            Guid random2 = Guid.NewGuid();
            MapItem droppedItem = null;
            List<GridPos> possibilities = new List<GridPos>();

            for (short x = -2; x < 3; x++)
            {
                for (short y = -2; y < 3; y++)
                {
                    possibilities.Add(new GridPos { X = x, Y = y });
                }
            }

            short mapX = 0;
            short mapY = 0;
            bool niceSpot = false;
            foreach (GridPos possibility in possibilities.OrderBy(s => _random.Next()))
            {
                mapX = (short)(session.Character.PositionX + possibility.X);
                mapY = (short)(session.Character.PositionY + possibility.Y);
                if (!Map.IsBlockedZone(mapX, mapY))
                {
                    niceSpot = true;
                    break;
                }
            }

            if (niceSpot && amount > 0 && amount <= inv.Amount)
            {
                ItemInstance newItemInstance = inv.DeepCopy();
                newItemInstance.Id = random2;
                newItemInstance.Amount = amount;
                droppedItem = new CharacterMapItem(mapX, mapY, newItemInstance);

                DroppedList[droppedItem.TransportId] = droppedItem;
                inv.Amount -= amount;
            }
            return droppedItem;
        }

        public void RemoveMapItem()
        {
            // take the data from list to remove it without having enumeration problems (ToList)
            try
            {
                List<MapItem> dropsToRemove = DroppedList.Where(dl => dl.CreatedDate.AddMinutes(3) < DateTime.Now);
                Parallel.ForEach(dropsToRemove, drop =>
                {
                    Broadcast(StaticPacketHelper.Out(UserType.Object, drop.TransportId));
                    DroppedList.Remove(drop.TransportId);
                });
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public void RemoveMonster(MapMonster monsterToRemove) => _monsters.Remove(monsterToRemove.MapMonsterId);

        public void RemoveNpc(MapNpc npcToRemove) => _npcs.Remove(npcToRemove.MapNpcId);

        public void SpawnButton(MapButton parameter)
        {
            Buttons.Add(parameter);
            Broadcast(parameter.GenerateIn());
        }

        public void ThrowItems(Tuple<int, short, byte, int, int> parameter)
        {
            MapMonster mon = Monsters.Find(s => s.MapMonsterId == parameter.Item1) ?? Monsters.Find(s => s.MonsterVNum == parameter.Item1);
            if (mon == null)
            {
                return;
            }
            short originX = mon.MapX;
            short originY = mon.MapY;
            short destX;
            short destY;
            int amount = ServerManager.RandomNumber(parameter.Item4, parameter.Item5);
            for (int i = 0; i < parameter.Item3; i++)
            {
                destX = (short)(originX + ServerManager.RandomNumber(-10, 10));
                destY = (short)(originY + ServerManager.RandomNumber(-10, 10));
                MonsterMapItem droppedItem = new MonsterMapItem(destX, destY, parameter.Item2, amount);
                DroppedList[droppedItem.TransportId] = droppedItem;
                Broadcast($"throw {droppedItem.ItemVNum} {droppedItem.TransportId} {originX} {originY} {droppedItem.PositionX} {droppedItem.PositionY} {(droppedItem.GoldAmount > 1 ? droppedItem.GoldAmount : droppedItem.Amount)}");
            }
        }

        internal void CreatePortal(Portal portal, int timeInSeconds = 0, bool isTemporary = false)
        {
            portal.SourceMapInstanceId = MapInstanceId;
            Portals.Add(portal);
            Broadcast(portal.GenerateGp());
            if (isTemporary)
            {
                Observable.Timer(TimeSpan.FromSeconds(timeInSeconds)).Subscribe(o =>
                {
                    Portals.Remove(portal);
                    MapClear();
                });
            }
        }

        internal IEnumerable<Character> GetCharactersInRange(short mapX, short mapY, byte distance)
        {
            List<Character> characters = new List<Character>();
            IEnumerable<ClientSession> cl = Sessions.Where(s => s.HasSelectedCharacter && s.Character.Hp > 0);
            IEnumerable<ClientSession> clientSessions = cl as IList<ClientSession> ?? cl.ToList();
            for (int i = clientSessions.Count() - 1; i >= 0; i--)
            {
                if (Map.GetDistance(new MapCell { X = mapX, Y = mapY }, new MapCell { X = clientSessions.ElementAt(i).Character.PositionX, Y = clientSessions.ElementAt(i).Character.PositionY }) <= distance + 1)
                {
                    characters.Add(clientSessions.ElementAt(i).Character);
                }
            }
            return characters;
        }

        internal void RemoveMonstersTarget(long characterId) => Parallel.ForEach(Monsters.Where(m => m.Target == characterId), monster => monster.RemoveTarget());

        internal void StartLife()
        {
            Observable.Interval(TimeSpan.FromSeconds(5)).Subscribe(x =>
            {
                if (InstanceBag?.EndState != 1)
                {
                    WaveEvents.ForEach(s =>
                    {
                        if (s.LastStart.AddSeconds(s.Delay) <= DateTime.Now)
                        {
                            if (s.Offset == 0)
                            {
                                s.Events.ForEach(e => EventHelper.Instance.RunEvent(e));
                            }
                            s.Offset = s.Offset > 0 ? (byte)(s.Offset - 1) : (byte)0;
                            s.LastStart = DateTime.Now;
                        }
                    });
                    try
                    {
                        if (Monsters.Count(s => s.IsAlive) == 0)
                        {
                            OnMapClean.ForEach(e => EventHelper.Instance.RunEvent(e));
                            OnMapClean.RemoveAll(s => s != null);
                        }
                        if (!IsSleeping)
                        {
                            RemoveMapItem();
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }
                }
            });
        }

        internal int SummonMonster(MonsterToSummon summon)
        {
            NpcMonster npcmonster = ServerManager.GetNpc(summon.VNum);
            if (npcmonster != null)
            {
                MapMonster mapMonster = new MapMonster { MonsterVNum = npcmonster.NpcMonsterVNum, MapY = summon.SpawnCell.Y, MapX = summon.SpawnCell.X, MapId = Map.MapId, IsMoving = summon.IsMoving, MapMonsterId = GetNextMonsterId(), ShouldRespawn = false, Target = summon.Target, OnDeathEvents = summon.DeathEvents, OnNoticeEvents = summon.NoticingEvents, IsTarget = summon.IsTarget, IsBonus = summon.IsBonus, IsBoss = summon.IsBoss, NoticeRange = summon.NoticeRange };
                mapMonster.Initialize(this);
                mapMonster.IsHostile = summon.IsHostile;
                AddMonster(mapMonster);
                Broadcast(mapMonster.GenerateIn());
                return mapMonster.MapMonsterId;
            }
            return default;
        }

        internal ConcurrentBag<int> SummonMonsters(List<MonsterToSummon> summonParameters)
        {
            ConcurrentBag<int> ids = new ConcurrentBag<int>();
            Parallel.ForEach(summonParameters, npcMonster =>
            {
                NpcMonster npcmonster = ServerManager.GetNpc(npcMonster.VNum);
                if (npcmonster != null)
                {
                    MapMonster mapMonster = new MapMonster { MonsterVNum = npcmonster.NpcMonsterVNum, MapY = npcMonster.SpawnCell.Y, MapX = npcMonster.SpawnCell.X, MapId = Map.MapId, IsMoving = npcMonster.IsMoving, MapMonsterId = GetNextMonsterId(), ShouldRespawn = false, Target = npcMonster.Target, OnDeathEvents = npcMonster.DeathEvents, OnNoticeEvents = npcMonster.NoticingEvents, IsTarget = npcMonster.IsTarget, IsBonus = npcMonster.IsBonus, IsBoss = npcMonster.IsBoss, NoticeRange = npcMonster.NoticeRange };
                    mapMonster.Initialize(this);
                    mapMonster.IsHostile = npcMonster.IsHostile;
                    AddMonster(mapMonster);
                    Broadcast(mapMonster.GenerateIn());
                    ids.Add(mapMonster.MapMonsterId);
                }
            });
            return ids;
        }

        internal int SummonNpc(NpcToSummon summonParameters)
        {
            NpcMonster npcMonster = ServerManager.GetNpc(summonParameters.VNum);
            if (npcMonster != null)
            {
                MapNpc mapNpc = new MapNpc { NpcVNum = npcMonster.NpcMonsterVNum, MapY = summonParameters.SpawnCell.X, MapX = summonParameters.SpawnCell.Y, MapId = Map.MapId, IsHostile = true, IsMoving = true, MapNpcId = GetNextNpcId(), Target = summonParameters.Target, OnDeathEvents = summonParameters.DeathEvents, IsMate = summonParameters.IsMate, IsProtected = summonParameters.IsProtected };
                mapNpc.Initialize(this);
                AddNPC(mapNpc);
                Broadcast(mapNpc.GenerateIn());
                return mapNpc.MapNpcId;
            }
            return default;
        }

        internal ConcurrentBag<int> SummonNpcs(List<NpcToSummon> summonParameters)
        {
            ConcurrentBag<int> ids = new ConcurrentBag<int>();
            Parallel.ForEach(summonParameters, npcMonster =>
            {
                NpcMonster npcmonster = ServerManager.GetNpc(npcMonster.VNum);
                if (npcmonster != null)
                {
                    MapNpc mapNpc = new MapNpc { NpcVNum = npcmonster.NpcMonsterVNum, MapY = npcMonster.SpawnCell.X, MapX = npcMonster.SpawnCell.Y, MapId = Map.MapId, IsHostile = true, IsMoving = true, MapNpcId = GetNextNpcId(), Target = npcMonster.Target, OnDeathEvents = npcMonster.DeathEvents, IsMate = npcMonster.IsMate, IsProtected = npcMonster.IsProtected };
                    mapNpc.Initialize(this);
                    AddNPC(mapNpc);
                    Broadcast(mapNpc.GenerateIn());
                    ids.Add(mapNpc.MapNpcId);
                }
            });
            return ids;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _npcs.Dispose();
                _monsters.Dispose();
                _mapNpcIds.Dispose();
                _mapMonsterIds.Dispose();
                DroppedList.Dispose();
                foreach (ClientSession session in ServerManager.Instance.Sessions.Where(s => s.Character != null && s.Character.MapInstanceId == MapInstanceId))
                {
                    ServerManager.Instance.ChangeMap(session.Character.CharacterId, session.Character.MapId, session.Character.MapX, session.Character.MapY);
                }
            }
        }

        internal void SpawnMeteorsOnRadius(int v, ClientSession session, Skill sk)
        {
            throw new NotImplementedException();
        }

        internal int GetNextId()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}