using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.PathFinder;
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

        public ConcurrentBag<MapDesignObject> MapDesignObjects = new ConcurrentBag<MapDesignObject>();

        private readonly List<int> mapMonsterIds;

        private readonly List<int> mapNpcIds;

        private readonly ConcurrentDictionary<long, MapMonster> monsters;

        private readonly ConcurrentDictionary<long, MapNpc> npcs;

        private readonly Random random;

        private bool disposed;

        private bool isSleeping;

        private bool isSleepingRequest;

        private static List<int> mapMonsterGeneratedIds = new List<int>();

        #endregion

        #region Instantiation

        public MapInstance(Map map, Guid guid, bool shopAllowed, MapInstanceType type, InstanceBag instanceBag)
        {
            Buttons = new List<MapButton>();
            XpRate = 1;
            DropRate = 1;
            ShopAllowed = shopAllowed;
            MapInstanceType = type;
            isSleeping = true;
            LastUserShopId = 0;
            InstanceBag = instanceBag;
            Clock = new Clock(3);
            random = new Random();
            Map = map;
            MapInstanceId = guid;
            ScriptedInstances = new List<ScriptedInstance>();
            OnCharacterDiscoveringMapEvents = new List<Tuple<EventContainer, List<long>>>();
            OnMoveOnMapEvents = new List<EventContainer>();
            OnAreaEntryEvents = new List<ZoneEvent>();
            WaveEvents = new List<EventWave>();
            OnMapClean = new List<EventContainer>();
            monsters = new ConcurrentDictionary<long, MapMonster>();
            npcs = new ConcurrentDictionary<long, MapNpc>();
            mapMonsterIds = new List<int>();
            mapNpcIds = new List<int>();
            DroppedList = new ConcurrentDictionary<long, MapItem>();
            Portals = new List<Portal>();
            UserShops = new Dictionary<long, MapShop>();
            StartLife();
        }

        #endregion

        #region Properties

        public List<MapButton> Buttons { get; set; }

        public Clock Clock { get; set; }

        public ConcurrentDictionary<long, MapItem> DroppedList { get; }

        public int DropRate { get; set; }

        public InstanceBag InstanceBag { get; set; }

        public bool IsDancing { get; set; }

        public bool IsPVP { get; set; }

        public bool IsSleeping
        {
            get
            {
                if (!isSleepingRequest || isSleeping || LastUnregister.AddSeconds(30) >= DateTime.Now)
                {
                    return isSleeping;
                }

                isSleeping = true;
                isSleepingRequest = false;
                Monsters.Where(s => s.Life != null).ToList().ForEach(s => s.StopLife());
                Npcs.Where(s => s.Life != null).ToList().ForEach(s => s.StopLife());
                return true;
            }
            set
            {
                if (value)
                {
                    isSleepingRequest = true;
                }
                else
                {
                    isSleeping = false;
                    isSleepingRequest = false;
                }
            }
        }

        public long LastUserShopId { get; set; }

        public Map Map { get; set; }

        public byte MapIndexX { get; set; }

        public byte MapIndexY { get; set; }

        public Guid MapInstanceId { get; set; }

        public MapInstanceType MapInstanceType { get; set; }

        public List<MapMonster> Monsters => monsters.Select(s => s.Value).ToList();

        public List<MapNpc> Npcs => npcs.Select(s => s.Value).ToList();

        public List<ZoneEvent> OnAreaEntryEvents { get; }

        public List<Tuple<EventContainer, List<long>>> OnCharacterDiscoveringMapEvents { get; }

        public List<EventContainer> OnMapClean { get; }

        public List<EventContainer> OnMoveOnMapEvents { get; }

        public List<Portal> Portals { get; }

        public List<ScriptedInstance> ScriptedInstances { get; }

        public bool ShopAllowed { get; }

        public Dictionary<long, MapShop> UserShops { get; }

        public List<EventWave> WaveEvents { get; }

        public int XpRate { get; set; }

        private IDisposable Life { get; set; }

        #endregion

        #region Methods

        public void AddMonster(MapMonster monster) => monsters[monster.MapMonsterId] = monster;

        public void AddNPC(MapNpc monster) => npcs[monster.MapNpcId] = monster;

        public void DespawnMonster(int monsterVnum)
        {
            Parallel.ForEach(monsters.Select(s => s.Value).Where(s => s.MonsterVNum == monsterVnum), monster =>
            {
                monster.IsAlive = false;
                monster.LastMove = DateTime.Now;
                monster.CurrentHp = 0;
                monster.CurrentMp = 0;
                monster.Death = DateTime.Now;
                Broadcast(monster.GenerateOut());
            });
        }

        public void DespawnMonster(MapMonster monster)
        {
            monster.IsAlive = false;
            monster.LastMove = DateTime.Now;
            monster.CurrentHp = 0;
            monster.CurrentMp = 0;
            monster.Death = DateTime.Now;
            Broadcast(monster.GenerateOut());
        }

        public sealed override void Dispose()
        {
            if (disposed)
            {
                return;
            }

            Dispose(true);
            GC.SuppressFinalize(this);
            disposed = true;
        }

        public void DropItemByMonster(long? owner, DropDTO drop, short mapX, short mapY)
        {
            // TODO: Parallelize, if possible.
            try
            {
                var localMapX = mapX;
                var localMapY = mapY;
                List<MapCell> possibilities = new List<MapCell>();

                for (short x = -1; x < 2; x++)
                {
                    for (short y = -1; y < 2; y++)
                        possibilities.Add(new MapCell { X = x, Y = y });
                }

                foreach (MapCell possibilitie in possibilities.OrderBy(s => ServerManager.Instance.RandomNumber()))
                {
                    localMapX = (short)(mapX + possibilitie.X);
                    localMapY = (short)(mapY + possibilitie.Y);
                    if (!Map.IsBlockedZone(localMapX, localMapY))
                    {
                        break;
                    }
                }

                var droppedItem = new MonsterMapItem(localMapX, localMapY, drop.ItemVNum, drop.Amount, owner ?? -1);
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
                var droppedItem = new MonsterMapItem(drop.Item3, drop.Item4, drop.Item1, drop.Item2);
                DroppedList[droppedItem.TransportId] = droppedItem;
                Broadcast($"drop {droppedItem.ItemVNum} {droppedItem.TransportId} {droppedItem.PositionX} {droppedItem.PositionY} {(droppedItem.GoldAmount > 1 ? droppedItem.GoldAmount : droppedItem.Amount)} 0 0 -1");
            }
        }

        public string GenerateMapDesignObjects()
        {
            var mlobjstring = "mltobj";
            var i = 0;
            foreach (MapDesignObject mp in MapDesignObjects)
            {
                mlobjstring += $" {mp.ItemInstance.ItemVNum}.{i}.{mp.MapX}.{mp.MapY}";
                i++;
            }

            return mlobjstring;
        }

        public string GenerateRsfn(bool isInit = false)
        {
            return MapInstanceType == MapInstanceType.TimeSpaceInstance ? $"rsfn {MapIndexX} {MapIndexY} {(isInit ? 1 : (Monsters.Where(s => s.IsAlive).ToList().Count == 0 ? 0 : 1))}" : string.Empty;
        }

        public List<MapMonster> GetListMonsterInRange(short mapX, short mapY, byte distance)
        {
            return monsters.Select(s => s.Value).Where(s => s.IsAlive && s.IsInRange(mapX, mapY, distance)).ToList();
        }

        public IEnumerable<string> GetMapDesignObjectEffects()
        {
            return MapDesignObjects.Select(mp => mp.GenerateEffect(false)).ToList();
        }

        public List<string> GetMapItems()
        {
            List<string> packets = new List<string>();

            // TODO: Parallelize getting of items of mapinstance
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
            Parallel.ForEach(DroppedList.Select(s => s.Value), session => packets.Add(session.GenerateIn()));
            Buttons.ForEach(s => packets.Add(s.GenerateIn()));
            packets.AddRange(GenerateUserShops());
            packets.AddRange(GeneratePlayerShopOnMap());
            return packets;
        }

        public MapMonster GetMonster(long mapMonsterId)
        {
            return !monsters.ContainsKey(mapMonsterId) ? null : monsters[mapMonsterId];
        }

        public int GetNextMonsterId()
        {
            int nextId = 0;

            lock (mapMonsterGeneratedIds)
            {
                if (mapMonsterGeneratedIds.Any())
                {
                    nextId = mapMonsterGeneratedIds.Last();
                }

                foreach (MapMonsterDTO monsterDTO in DAOFactory.MapMonsterDAO.LoadAll())
                {
                    if (monsterDTO.MapMonsterId > nextId)
                    {
                        nextId = monsterDTO.MapMonsterId;
                    }
                }

                ++nextId;

                mapMonsterGeneratedIds.Add(nextId);
            }

            return nextId;
        }

        public void LoadMonsters()
        {
            OrderablePartitioner<MapMonsterDTO> partitioner = Partitioner.Create(DAOFactory.MapMonsterDAO.Where(s => s.MapId == Map.MapId), EnumerablePartitionerOptions.None);
            Parallel.ForEach(partitioner, monster =>
            {
                if (!(monster is MapMonster mapMonster))
                {
                    return;
                }

                mapMonster.Initialize(this);
                var mapMonsterId = mapMonster.MapMonsterId;
                monsters[mapMonsterId] = mapMonster;
                mapMonsterIds.Add(mapMonsterId);
            });
        }

        public void LoadNpcs()
        {
            OrderablePartitioner<MapNpcDTO> partitioner = Partitioner.Create(DAOFactory.MapNpcDAO.Where(s => s.MapId == Map.MapId), EnumerablePartitionerOptions.None);
            Parallel.ForEach(partitioner, npc =>
            {
                if (!(npc is MapNpc mapNpc))
                {
                    return;
                }

                mapNpc.Initialize(this);
                var mapNpcId = mapNpc.MapNpcId;
                npcs[mapNpcId] = mapNpc;
                mapNpcIds.Add(mapNpcId);
            });
        }

        public void LoadPortals()
        {
            OrderablePartitioner<PortalDTO> partitioner = Partitioner.Create(DAOFactory.PortalDAO.Where(s => s.SourceMapId == Map.MapId), EnumerablePartitionerOptions.None);
            ConcurrentDictionary<int, Portal> portalList = new ConcurrentDictionary<int, Portal>();
            Parallel.ForEach(partitioner, portalDTO =>
            {
                if (!(portalDTO is Portal portal))
                {
                    return;
                }

                portal.SourceMapInstanceId = MapInstanceId;
                portalList[portal.PortalId] = portal;
            });
            Portals.AddRange(portalList.Select(s => s.Value));
        }

        public void MapClear()
        {
            Broadcast("mapclear");
            GetMapItems().ForEach(Broadcast);
        }

        public MapItem PutItem(InventoryType type, short slot, byte amount, ref ItemInstance inv, ClientSession session)
        {
            var random2 = Guid.NewGuid();
            MapItem droppedItem = null;
            List<GridPos> possibilities = new List<GridPos>();

            for (short x = -2; x < 3; x++)
            {
                for (short y = -2; y < 3; y++)
                    possibilities.Add(new GridPos { X = x, Y = y });
            }

            short mapX = 0;
            short mapY = 0;
            var niceSpot = false;
            foreach (GridPos possibility in possibilities.OrderBy(s => random.Next()))
            {
                mapX = (short)(session.Character.PositionX + possibility.X);
                mapY = (short)(session.Character.PositionY + possibility.Y);
                if (Map.IsBlockedZone(mapX, mapY))
                {
                    continue;
                }

                niceSpot = true;
                break;
            }

            if (!niceSpot)
            {
                return null;
            }

            if (amount <= 0 || amount > inv.Amount)
            {
                return null;
            }

            var newItemInstance = inv.DeepCopy();
            newItemInstance.Id = random2;
            newItemInstance.Amount = amount;
            droppedItem = new CharacterMapItem(mapX, mapY, newItemInstance);
            DroppedList[droppedItem.TransportId] = droppedItem;
            inv.Amount -= amount;
            return droppedItem;
        }

        public void RemoveMonster(MapMonster monsterToRemove)
        {
            monsters.TryRemove(monsterToRemove.MapMonsterId, out MapMonster value);
        }

        public void SpawnButton(MapButton parameter)
        {
            Buttons.Add(parameter);
            Broadcast(parameter.GenerateIn());
        }

        public void ThrowItems(Tuple<int, short, byte, int, int> parameter)
        {
            var mon = Monsters.FirstOrDefault(s => s.MapMonsterId == parameter.Item1);

            if (mon == null)
            {
                return;
            }

            var originX = mon.MapX;
            var originY = mon.MapY;
            var amount = ServerManager.Instance.RandomNumber(parameter.Item4, parameter.Item5);
            if (parameter.Item2 == 1024)
            {
                amount *= ServerManager.Instance.GoldRate;
            }

            for (int i = 0; i < parameter.Item3; i++)
            {
                var destX = (short)(originX + ServerManager.Instance.RandomNumber(-10, 10));
                var destY = (short)(originY + ServerManager.Instance.RandomNumber(-10, 10));
                var droppedItem = new MonsterMapItem(destX, destY, parameter.Item2, amount);
                DroppedList[droppedItem.TransportId] = droppedItem;
                Broadcast(
                    $"throw {droppedItem.ItemVNum} {droppedItem.TransportId} {originX} {originY} {droppedItem.PositionX} {droppedItem.PositionY} {(droppedItem.GoldAmount > 1 ? droppedItem.GoldAmount : droppedItem.Amount)}");
            }
        }

        internal void CreatePortal(Portal portal)
        {
            portal.SourceMapInstanceId = MapInstanceId;
            Portals.Add(portal);
            Broadcast(portal.GenerateGp());
        }

        internal IEnumerable<Character> GetCharactersInRange(short mapX, short mapY, byte distance)
        {
            List<Character> characters = new List<Character>();
            IEnumerable<ClientSession> cl = Sessions.Where(s => s.HasSelectedCharacter && s.Character.Hp > 0);
            IEnumerable<ClientSession> clientSessions = cl as IList<ClientSession> ?? cl.ToList();
            for (int i = clientSessions.Count() - 1; i >= 0; i--)
            {
                if (Map.GetDistance(new MapCell { X = mapX, Y = mapY }, new MapCell { X = clientSessions.ElementAt(i).Character.PositionX, Y = clientSessions.ElementAt(i).Character.PositionY }) <=
                    distance + 1)
                {
                    characters.Add(clientSessions.ElementAt(i).Character);
                }
            }

            return characters;
        }

        internal void RemoveMonstersTarget(long characterId)
        {
            Parallel.ForEach(Monsters.Where(m => m.Target == characterId), monster => { monster.RemoveTarget(); });
        }

        internal void SummonMonster(MonsterToSummon monsterToSummon)
        {
            var npcmonster = ServerManager.Instance.GetNpc(monsterToSummon.VNum);

            if (npcmonster != null)
            {
                var monster = new MapMonster
                {
                    MonsterVNum = npcmonster.NpcMonsterVNum,
                    MapY = monsterToSummon.SpawnCell.Y,
                    MapX = monsterToSummon.SpawnCell.X,
                    MapId = Map.MapId,
                    IsMoving = monsterToSummon.IsMoving,
                    MapMonsterId = GetNextMonsterId(),
                    ShouldRespawn = false,
                    Target = monsterToSummon.Target,
                    OnDeathEvents = monsterToSummon.DeathEvents,
                    OnNoticeEvents = monsterToSummon.NoticingEvents,
                    IsTarget = monsterToSummon.IsTarget,
                    IsBonus = monsterToSummon.IsBonus,
                    IsBoss = monsterToSummon.IsBoss,
                    NoticeRange = monsterToSummon.NoticeRange
                };

                monster.Initialize(this);
                monster.IsHostile = monsterToSummon.IsHostile;
                AddMonster(monster);
                Broadcast(monster.GenerateIn());
            }
        }

        internal void SummonMonsters(IEnumerable<MonsterToSummon> summonParameters)
        {
            Parallel.ForEach(summonParameters, monsterToSummon => SummonMonster(monsterToSummon));
        }

        internal void SummonNpc(NpcToSummon npcToSummon)
        {
            var npcmonster = ServerManager.Instance.GetNpc(npcToSummon.VNum);

            if (npcmonster != null)
            {
                var npc = new MapNpc
                {
                    NpcVNum = npcmonster.NpcMonsterVNum,
                    MapY = npcToSummon.SpawnCell.X,
                    MapX = npcToSummon.SpawnCell.Y,
                    MapId = Map.MapId,
                    IsHostile = true,
                    IsMoving = true,
                    MapNpcId = GetNextNpcId(),
                    Target = npcToSummon.Target,
                    OnDeathEvents = npcToSummon.DeathEvents,
                    IsMate = npcToSummon.IsMate,
                    IsProtected = npcToSummon.IsProtected
                };

                npc.Initialize(this);
                AddNPC(npc);
                Broadcast(npc.GenerateIn());
            }
        }

        internal void SummonNpcs(IEnumerable<NpcToSummon> summonParameters)
        {
            Parallel.ForEach(summonParameters, npcToSummon => SummonNpc(npcToSummon));
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            Clock?.Dispose();
            Life?.Dispose();
            monsters.Select(s => s.Value).ToList().ForEach(monster => monster.Life?.Dispose());
            npcs.Select(s => s.Value).ToList().ForEach(npc => npc?.Life?.Dispose());

            foreach (ClientSession session in ServerManager.Instance.Sessions.Where(s => s.Character != null && s.Character.MapInstanceId == MapInstanceId))
            {
                ServerManager.Instance.ChangeMap(session.Character.CharacterId, session.Character.MapId, session.Character.MapX, session.Character.MapY);
            }
        }

        private IEnumerable<string> GenerateNPCShopOnMap()
        {
            return (from npc in Npcs where npc.Shop != null select $"shop 2 {npc.MapNpcId} {npc.Shop.ShopId} {npc.Shop.MenuType} {npc.Shop.ShopType} {npc.Shop.Name}").ToList();
        }

        private IEnumerable<string> GeneratePlayerShopOnMap()
        {
            return UserShops.Select(shop => $"pflag 1 {shop.Value.OwnerId} {shop.Key + 1}").ToList();
        }

        private IEnumerable<string> GenerateUserShops()
        {
            return UserShops.Select(shop => $"shop 1 {shop.Value.OwnerId} 1 3 0 {shop.Value.Name}").ToList();
        }

        // TODO: Fix, Seems glitchy.
        private int GetNextNpcId()
        {
            var nextId = mapNpcIds.Any() ? mapNpcIds.Last() + 1 : 1;
            mapNpcIds.Add(nextId);
            return nextId;
        }

        private void RemoveMapItem()
        {
            // take the data from list to remove it without having enumeration problems (ToList)
            try
            {
                List<MapItem> dropsToRemove = DroppedList.Select(s => s.Value).Where(dl => dl.CreatedDate.AddMinutes(3) < DateTime.Now).ToList();

                Parallel.ForEach(dropsToRemove, drop =>
                {
                    Broadcast(drop.GenerateOut(drop.TransportId));
                    DroppedList.TryRemove(drop.TransportId, out MapItem value);
                });
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void StartLife()
        {
            Life = Observable.Interval(TimeSpan.FromMilliseconds(400)).Subscribe(x =>
            {
                WaveEvents.ForEach(s =>
                {
                    if (s.LastStart.AddSeconds(s.Delay) > DateTime.Now)
                    {
                        return;
                    }

                    if (s.Offset == 0)
                    {
                        s.Events.ToList().ForEach(e => EventHelper.Instance.RunEvent(e));
                    }

                    s.Offset = s.Offset > 0 ? (byte)(s.Offset - 1) : (byte)0;
                    s.LastStart = DateTime.Now;
                });
                try
                {
                    if (!IsSleeping)
                    {
                        Monsters.Where(s => s.Life == null).ToList().ForEach(s => s.StartLife());
                        Npcs.Where(s => s.Life == null).ToList().ForEach(s => s.StartLife());

                        if (Monsters.Count(s => s.IsAlive) == 0)
                        {
                            OnMapClean.ForEach(e => { EventHelper.Instance.RunEvent(e); });
                            OnMapClean.RemoveAll(s => s != null);
                        }

                        RemoveMapItem();
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            });
        }

        #endregion
    }
}