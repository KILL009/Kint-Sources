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

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OpenNos.Core;
using OpenNos.Data;
using OpenNos.DAL;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;
using OpenNos.XMLModel.Models.Quest;

namespace OpenNos.GameObject.Networking
{
    public class ServerManager : BroadcastableBase
    {
        #region Members

        public ThreadSafeSortedList<long, Group> GroupsThreadSafe;

        public bool InShutdown;

        public bool IsReboot { get; set; }

        public bool ShutdownStop;

        private static readonly ConcurrentBag<Card> _cards = new ConcurrentBag<Card>();

        private static readonly ConcurrentBag<Item> _items = new ConcurrentBag<Item>();

        private static readonly ConcurrentDictionary<Guid, MapInstance> _mapinstances = new ConcurrentDictionary<Guid, MapInstance>();

        private static readonly ConcurrentBag<Map> _maps = new ConcurrentBag<Map>();

        private static readonly ConcurrentBag<NpcMonster> _npcs = new ConcurrentBag<NpcMonster>();

        private static readonly CryptoRandom _random = new CryptoRandom();

        private static readonly ConcurrentBag<Skill> _skills = new ConcurrentBag<Skill>();

        private static ServerManager _instance;

        private List<DropDTO> _generalDrops;

        private bool _inRelationRefreshMode;

        private long _lastGroupId;

        private ThreadSafeSortedList<short, List<MapNpc>> _mapNpcs;

        private ThreadSafeSortedList<short, List<DropDTO>> _monsterDrops;

        private ThreadSafeSortedList<short, List<NpcMonsterSkill>> _monsterSkills;

        private ThreadSafeSortedList<int, RecipeListDTO> _recipeLists;

        private ThreadSafeSortedList<short, Recipe> _recipes;

        private ThreadSafeSortedList<int, List<ShopItemDTO>> _shopItems;

        private ThreadSafeSortedList<int, Shop> _shops;

        private ThreadSafeSortedList<int, List<ShopSkillDTO>> _shopSkills;

        private ThreadSafeSortedList<int, List<TeleporterDTO>> _teleporters;

        #endregion

        #region Instantiation

        private ServerManager()
        {
            // do nothing
        }

        #endregion

        #region Properties

        public static ServerManager Instance => _instance ?? (_instance = new ServerManager());

        public Act4Stat Act4AngelStat { get; set; }

        public Act4Stat Act4DemonStat { get; set; }

        public DateTime Act4RaidStart { get; set; }


        public MapInstance ArenaInstance { get; private set; }

        public ThreadSafeGenericList<BazaarItemLink> BazaarList { get; set; }

        public int ChannelId { get; set; }

        public List<CharacterRelationDTO> CharacterRelations { get; set; }

        public ConfigurationObject Configuration { get; set; }

        public bool EventInWaiting { get; set; }

        public MapInstance FamilyArenaInstance { get; private set; }

        public ThreadSafeSortedList<long, Family> FamilyList { get; set; }

        public List<Group> GroupList { get; set; } = new List<Group>();

        public List<Group> Groups => GroupsThreadSafe.GetAllItems();

        public bool InBazaarRefreshMode { get; set; }

        public MallAPIHelper MallApi { get; set; }

        public List<int> MateIds { get; internal set; } = new List<int>();

        public List<PenaltyLogDTO> PenaltyLogs { get; set; }

        public ThreadSafeSortedList<long, QuestModel> QuestList { get; set; }

        public ConcurrentBag<ScriptedInstance> Raids { get; set; }

        public List<Schedule> Schedules { get; set; }

        public string ServerGroup { get; set; }

        public List<EventType> StartedEvents { get; set; }

        public Task TaskShutdown { get; set; }

        public List<CharacterDTO> TopComplimented { get; set; }

        public List<CharacterDTO> TopPoints { get; set; }

        public List<CharacterDTO> TopReputation { get; set; }

        public Guid WorldId { get; private set; }

        public List<ArenaMember> ArenaMembers { get; set; } = new List<ArenaMember>();

        public ThreadSafeSortedList<long, ClientSession> CharacterScreenSessions { get; set; }
        public long MaxBankGold { get; set; }
        public long MaxGold { get; set; }
        public int RaidType { get; set; }

        #endregion

        #region Methods

        public void AddGroup(Group group) => GroupsThreadSafe[group.GroupId] = group;

        public void AskPvpRevive(long characterId)
        {
            ClientSession session = GetSessionByCharacterId(characterId);
            if (session?.HasSelectedCharacter == true)
            {
                if (session.Character.IsVehicled)
                {
                    session.Character.RemoveVehicle();
                }
                session.Character.DisableBuffs(BuffType.All);
                session.SendPacket(session.Character.GenerateStat());
                session.SendPacket(session.Character.GenerateCond());
                session.SendPackets(UserInterfaceHelper.GenerateVb());

                session.Character.LeaveTalentArena();
                session.SendPacket("eff_ob -1 -1 0 4269");
                session.SendPacket(UserInterfaceHelper.GenerateDialog($"#revival^2 #revival^1 {Language.Instance.GetMessageFromKey("ASK_REVIVE_PVP")}"));
                ReviveTask(session);
            }
        }

       

        // PacketHandler -> with Callback?
        public void AskRevive(long characterId)
        {
            ClientSession session = GetSessionByCharacterId(characterId);
            if (session?.HasSelectedCharacter == true && session.HasCurrentMapInstance)
            {
                if (session.Character.IsVehicled)
                {
                    session.Character.RemoveVehicle();
                }
                session.Character.DisableBuffs(BuffType.All);
                session.SendPacket(session.Character.GenerateStat());
                session.SendPacket(session.Character.GenerateCond());
                session.SendPackets(UserInterfaceHelper.GenerateVb());

                switch (session.CurrentMapInstance.MapInstanceType)
                {
                    case MapInstanceType.BaseMapInstance:
                        if (session.Character.Level > 20)
                        {
                            session.Character.Dignity -= (short)(session.Character.Level < 50 ? session.Character.Level : 50);
                            if (session.Character.Dignity < -1000)
                            {
                                session.Character.Dignity = -1000;
                            }
                            session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("LOSE_DIGNITY"), (short)(session.Character.Level < 50 ? session.Character.Level : 50)), 11));
                            session.SendPacket(session.Character.GenerateFd());
                            session.CurrentMapInstance.Broadcast(session, session.Character.GenerateIn(), ReceiverType.AllExceptMe);
                            session.CurrentMapInstance.Broadcast(session, session.Character.GenerateGidx(), ReceiverType.AllExceptMe);
                        }
                        session.SendPacket("eff_ob -1 -1 0 4269");
                        session.SendPacket(UserInterfaceHelper.GenerateDialog($"#revival^0 #revival^1 {(session.Character.Level > 20 ? Language.Instance.GetMessageFromKey("ASK_REVIVE") : Language.Instance.GetMessageFromKey("ASK_REVIVE_FREE"))}"));
                        ReviveTask(session);
                        break;

                    case MapInstanceType.TimeSpaceInstance:
                        if (!(session.CurrentMapInstance.InstanceBag.Lives - session.CurrentMapInstance.InstanceBag.DeadList.Count <= 1))
                        {
                            session.Character.Hp = 1;
                            session.Character.Mp = 1;
                            return;
                        }
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("YOU_HAVE_LIFE"), session.CurrentMapInstance.InstanceBag.Lives - session.CurrentMapInstance.InstanceBag.DeadList.Count + 1), 0));
                        session.SendPacket(UserInterfaceHelper.GenerateDialog($"#revival^1 #revival^1 {(session.Character.Level > 10 ? Language.Instance.GetMessageFromKey("ASK_REVIVE_TS_LOW_LEVEL") : Language.Instance.GetMessageFromKey("ASK_REVIVE_TS"))}"));
                        session.CurrentMapInstance.InstanceBag.DeadList.Add(session.Character.CharacterId);
                        ReviveTask(session);
                        break;

                    case MapInstanceType.RaidInstance:
                        List<long> save = session.CurrentMapInstance.InstanceBag.DeadList.ToList();
                        if (session.CurrentMapInstance.InstanceBag.Lives - session.CurrentMapInstance.InstanceBag.DeadList.Count < 0)
                        {
                            session.Character.Hp = 1;
                            session.Character.Mp = 1;
                        }
                        else if (2 - save.Count(s => s == session.Character.CharacterId) > 0)
                        {
                            session.SendPacket(UserInterfaceHelper.GenerateInfo(string.Format(Language.Instance.GetMessageFromKey("YOU_HAVE_LIFE_RAID"), 2 - session.CurrentMapInstance.InstanceBag.DeadList.Count(s => s == session.Character.CharacterId))));
                            session.SendPacket(UserInterfaceHelper.GenerateInfo(string.Format(Language.Instance.GetMessageFromKey("RAID_MEMBER_DEAD"), session.Character.Name)));
                            try
                            {
                                session.Character.Group?.Raid?.InstanceBag.DeadList.Add(session.Character.CharacterId);
                            }
                            catch (IndexOutOfRangeException ex)
                            {
                                Logger.Error(ex);
                            }
                            session.Character.Group?.Characters.ForEach(grpSession =>
                            {
                                grpSession?.SendPacket(grpSession.Character.Group?.GeneraterRaidmbf(grpSession));
                                grpSession?.SendPacket(grpSession.Character.Group?.GenerateRdlst());
                            });
                            Task.Factory.StartNew(async () =>
                            {
                                await Task.Delay(20000).ConfigureAwait(false);
                                Instance.ReviveFirstPosition(session.Character.CharacterId);
                            });
                        }
                        else
                        {
                            session.Character.Group?.Characters.ForEach(grpSession =>
                            {
                                grpSession.SendPacket(grpSession.Character.Group?.GeneraterRaidmbf(grpSession));
                                grpSession.SendPacket(grpSession.Character.Group?.GenerateRdlst());
                            });
                            session.Character.Group?.LeaveGroup(session);
                            session.SendPacket(session.Character.GenerateRaid(1, true));
                            session.SendPacket(session.Character.GenerateRaid(2, true));
                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("KICKED_FROM_RAID"), 0));
                        }
                        break;

                    case MapInstanceType.LodInstance:
                        session.SendPacket(UserInterfaceHelper.GenerateDialog($"#revival^0 #revival^1 {Language.Instance.GetMessageFromKey("ASK_REVIVE_LOD")}"));
                        ReviveTask(session);
                        break;

                    case MapInstanceType.Act4Berios:
                    case MapInstanceType.Act4Calvina:
                    case MapInstanceType.Act4Hatus:
                    case MapInstanceType.Act4Morcos:
                        session.SendPacket(UserInterfaceHelper.GenerateDialog($"#revival^0 #revival^1 {Language.Instance.GetMessageFromKey("ASK_REVIVE_ACT4RAID")}"));
                        ReviveTask(session);
                        break;

                    default:
                        Instance.ReviveFirstPosition(session.Character.CharacterId);
                        break;
                }
            }
        }

        public void BazaarRefresh(long bazaarItemId)
        {
            InBazaarRefreshMode = true;
            CommunicationServiceClient.Instance.UpdateBazaar(ServerGroup, bazaarItemId);
            SpinWait.SpinUntil(() => !InBazaarRefreshMode);
        }

        public void ChangeMap(long id, short? mapId = null, short? mapX = null, short? mapY = null)
        {
            ClientSession session = GetSessionByCharacterId(id);
            if (session?.Character != null)
            {
                if (mapId != null)
                {
                    session.Character.MapInstanceId = GetBaseMapInstanceIdByMapId((short)mapId);
                }
                ChangeMapInstance(id, session.Character.MapInstanceId, mapX, mapY);
            }
        }

        // Both partly
        public void ChangeMapInstance(long characterId, Guid mapInstanceId, short? mapX = null, short? mapY = null, bool noAggroLoss = false)
        {
            ClientSession session = GetSessionByCharacterId(characterId);
            if (session?.Character != null && !session.Character.IsChangingMapInstance)
            {
                try
                {
                    if (session.Character.IsExchanging)
                    {
                        session.Character.CloseExchangeOrTrade();
                    }

                    if (session.Character.HasShopOpened)
                    {
                        session.Character.CloseShop();
                    }

                    session.Character.LeaveTalentArena();
                    if (!noAggroLoss)
                    {
                        session.CurrentMapInstance.RemoveMonstersTarget(session.Character.CharacterId);
                    }

                    session.CurrentMapInstance.UnregisterSession(session.Character.CharacterId);

                    LeaveMap(session.Character.CharacterId);

                    session.Character.IsChangingMapInstance = true;

                    // cleanup sending queue to avoid sending uneccessary packets to it
                    session.ClearLowPriorityQueue();

                    session.Character.IsSitting = false;
                    session.Character.MapInstanceId = mapInstanceId;
                    session.CurrentMapInstance = session.Character.MapInstance;

                    if (session.Character.MapInstance.MapInstanceType == MapInstanceType.BaseMapInstance)
                    {
                        session.Character.MapId = session.Character.MapInstance.Map.MapId;
                        if (mapX != null && mapY != null)
                        {
                            session.Character.MapX = mapX.Value;
                            session.Character.MapY = mapY.Value;
                        }
                    }

                    if (mapX != null && mapY != null)
                    {
                        session.Character.PositionX = mapX.Value;
                        session.Character.PositionY = mapY.Value;
                    }

                    foreach (Mate mate in session.Character.Mates.Where(m => m.IsTeamMember))
                    {
                        mate.PositionX =
                            (short)(session.Character.PositionX + (mate.MateType == MateType.Partner ? -1 : 1));
                        mate.PositionY = (short)(session.Character.PositionY + 1);
                        if (mate.PositionX >= session.CurrentMapInstance.Map.XLength)
                        {
                            mate.PositionX = (short)(session.CurrentMapInstance.Map.XLength - 1);
                        }
                        if (mate.PositionY >= session.CurrentMapInstance.Map.YLength)
                        {
                            mate.PositionY = (short)(session.CurrentMapInstance.Map.YLength - 1);
                        }
                        mate.UpdateBushFire();
                    }

                    session.Character.UpdateBushFire();

                    session.CurrentMapInstance.RegisterSession(session);

                    session.SendPacket(session.Character.GenerateCInfo());
                    session.SendPacket(session.Character.GenerateCMode());
                    session.SendPacket(session.Character.GenerateEq());
                    session.SendPacket(session.Character.GenerateEquipment());
                    session.SendPacket(session.Character.GenerateLev());
                    session.SendPacket(session.Character.GenerateStat());
                    session.SendPacket(session.Character.GenerateAt());
                    session.SendPacket(session.Character.GenerateCond());
                    session.SendPacket(session.Character.GenerateCMap());
                    session.SendPacket(session.Character.GenerateStatChar());
                    session.SendPacket(session.Character.GeneratePairy());
                    session.SendPacket(Character.GenerateAct());
                    session.SendPacket(session.Character.GenerateScpStc());
                    if (ChannelId == 51)
                    {
                        session.SendPacket(session.Character.GenerateFc());

                        if (mapInstanceId == session.Character.Family?.Act4Raid?.MapInstanceId ||
                            mapInstanceId == session.Character.Family?.Act4RaidBossMap?.MapInstanceId)
                        {
                            session.SendPacket(session.Character.GenerateDG());
                        }
                    }

                    if (session.Character.Group?.Raid != null && session.Character.Group.Raid.InstanceBag?.Lock == true)
                    {
                        session.SendPacket(session.Character.Group.GeneraterRaidmbf(session));
                    }

                    Parallel.ForEach(
                        session.CurrentMapInstance.Sessions.Where(s =>
                            s.Character?.InvisibleGm == false &&
                            s.Character.CharacterId != session.Character.CharacterId), visibleSession =>
                            {
                                if (ChannelId != 51 || session.Character.Faction == visibleSession.Character.Faction)
                                {
                                    session.SendPacket(visibleSession.Character.GenerateIn());
                                    session.SendPacket(visibleSession.Character.GenerateGidx());
                                    visibleSession.Character.Mates
                                        .Where(m => m.IsTeamMember && m.CharacterId != session.Character.CharacterId)
                                        .ToList().ForEach(m => session.SendPacket(m.GenerateIn()));
                                }
                                else
                                {
                                    session.SendPacket(visibleSession.Character.GenerateIn(true));
                                    visibleSession.Character.Mates
                                        .Where(m => m.IsTeamMember && m.CharacterId != session.Character.CharacterId)
                                        .ToList().ForEach(m => session.SendPacket(m.GenerateIn(true)));
                                }
                            });

                    session.SendPackets(session.CurrentMapInstance.GetMapItems());
                    MapInstancePortalHandler
                        .GenerateMinilandEntryPortals(session.CurrentMapInstance.Map.MapId,
                            session.Character.Miniland.MapInstanceId).ForEach(p => session.SendPacket(p.GenerateGp()));

                    if (session.CurrentMapInstance.InstanceBag.Clock.Enabled)
                    {
                        session.SendPacket(session.CurrentMapInstance.InstanceBag.Clock.GetClock());
                    }

                    if (session.CurrentMapInstance.Clock.Enabled)
                    {
                        session.SendPacket(session.CurrentMapInstance.Clock.GetClock());
                    }

                    // TODO: fix this
                    if (session.Character.MapInstance.Map.MapTypes.Any(m =>
                        m.MapTypeId == (short)MapTypeEnum.CleftOfDarkness))
                    {
                        session.SendPacket("bc 0 0 0");
                    }

                    if (!session.Character.InvisibleGm)
                    {
                        Parallel.ForEach(session.CurrentMapInstance.Sessions.Where(s => s.Character != null), s =>
                        {
                            if (ChannelId != 51 || session.Character.Faction == s.Character.Faction)
                            {
                                s.SendPacket(session.Character.GenerateIn());
                                s.SendPacket(session.Character.GenerateGidx());
                                session.Character.Mates.Where(m => m.IsTeamMember).ToList()
                                    .ForEach(m => s.SendPacket(m.GenerateIn(false, ChannelId == 51)));
                            }
                            else
                            {
                                s.SendPacket(session.Character.GenerateIn(true));
                                session.Character.Mates.Where(m => m.IsTeamMember).ToList()
                                    .ForEach(m => s.SendPacket(m.GenerateIn(true, ChannelId == 51)));
                            }
                        });
                    }

                    session.SendPacket(session.Character.GeneratePinit());
                    session.Character.Mates.ForEach(s => session.SendPacket(s.GenerateScPacket()));
                    session.SendPackets(session.Character.GeneratePst());

                    if (session.Character.Size != 10)
                    {
                        session.SendPacket(session.Character.GenerateScal());
                    }

                    if (session.CurrentMapInstance?.IsDancing == true && !session.Character.IsDancing)
                    {
                        session.CurrentMapInstance?.Broadcast("dance 2");
                    }
                    else if (session.CurrentMapInstance?.IsDancing == false && session.Character.IsDancing)
                    {
                        session.Character.IsDancing = false;
                        session.CurrentMapInstance?.Broadcast("dance");
                    }

                    if (Groups != null)
                    {
                        Parallel.ForEach(Groups, group =>
                        {
                            foreach (ClientSession groupSession in group.Characters.GetAllItems())
                            {
                                ClientSession groupCharacterSession = Sessions.FirstOrDefault(s =>
                                    s.Character != null &&
                                    s.Character.CharacterId == groupSession.Character.CharacterId &&
                                    s.CurrentMapInstance == groupSession.CurrentMapInstance);
                                if (groupCharacterSession == null)
                                {
                                    continue;
                                }

                                groupSession.SendPacket(groupSession.Character.GeneratePinit());
                                groupSession.SendPackets(groupSession.Character.GeneratePst());
                            }
                        });
                    }

                    if (session.Character.Group?.GroupType == GroupType.Group)
                    {
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GeneratePidx(),
                            ReceiverType.AllExceptMe);
                    }

                    session.Character.IsChangingMapInstance = false;
                    session.SendPacket(session.Character.GenerateMinimapPosition());
                    session.CurrentMapInstance.OnCharacterDiscoveringMapEvents.ForEach(e =>
                    {
                        if (!e.Item2.Contains(session.Character.CharacterId))
                        {
                            e.Item2.Add(session.Character.CharacterId);
                            EventHelper.Instance.RunEvent(e.Item1, session);
                        }
                    });
                    session.CurrentMapInstance.OnCharacterDiscoveringMapEvents.Clear();
                }
                catch (Exception ex)
                {
                    Logger.Warn("Character changed while changing map. Do not abuse Commands.", ex);
                    session.Character.IsChangingMapInstance = false;
                }
            }
        }

        public void FamilyRefresh(long familyId) => CommunicationServiceClient.Instance.UpdateFamily(ServerGroup, familyId);

        public static MapInstance GenerateMapInstance(short mapId, MapInstanceType type, InstanceBag mapclock)
        {
            Map map = _maps.FirstOrDefault(m => m.MapId.Equals(mapId));
            if (map != null)
            {
                Guid guid = Guid.NewGuid();
                MapInstance mapInstance = new MapInstance(map, guid, false, type, mapclock);
                mapInstance.LoadMonsters();
                mapInstance.LoadNpcs();
                mapInstance.LoadPortals();
                Parallel.ForEach(mapInstance.Monsters, mapMonster =>
                {
                    mapMonster.MapInstance = mapInstance;
                    mapInstance.AddMonster(mapMonster);
                });
                Parallel.ForEach(mapInstance.Npcs, mapNpc =>
                {
                    mapNpc.MapInstance = mapInstance;
                    mapInstance.AddNPC(mapNpc);
                });
                _mapinstances.TryAdd(guid, mapInstance);
                return mapInstance;
            }
            return null;
        }

        public static IEnumerable<Card> GetAllCard() => _cards;

        public static List<MapInstance> GetAllMapInstances() => _mapinstances.Values.ToList();

        public List<Recipe> GetAllRecipes() => _recipes.GetAllItems();

        public static IEnumerable<Skill> GetAllSkill() => _skills;

        public static Guid GetBaseMapInstanceIdByMapId(short mapId) => _mapinstances.FirstOrDefault(s => s.Value?.Map.MapId == mapId && s.Value.MapInstanceType == MapInstanceType.BaseMapInstance).Key;

        public static Card GetCard(short cardId) => _cards.FirstOrDefault(m => m.CardId.Equals(cardId));

        public List<DropDTO> GetDropsByMonsterVNum(short monsterVNum) => _monsterDrops.ContainsKey(monsterVNum) ? _generalDrops.Concat(_monsterDrops[monsterVNum]).ToList() : new List<DropDTO>();

        public Group GetGroupByCharacterId(long characterId) => Groups?.SingleOrDefault(g => g.IsMemberOfGroup(characterId));

        public static Item GetItem(short vnum) => _items.FirstOrDefault(m => m.VNum.Equals(vnum));

        public static MapInstance GetMapInstance(Guid id) => _mapinstances.ContainsKey(id) ? _mapinstances[id] : null;

        public static MapInstance GetMapInstanceByMapId(short mapId) => _mapinstances.Values.FirstOrDefault(s => s.Map.MapId == mapId);

        public static List<MapInstance> GetMapInstances(Func<MapInstance, bool> predicate) => _mapinstances.Values.Where(predicate).ToList();

        public long GetNextGroupId() => ++_lastGroupId;

        public static NpcMonster GetNpc(short npcVNum) => _npcs.FirstOrDefault(m => m.NpcMonsterVNum.Equals(npcVNum));

        public List<Recipe> GetRecipesByItemVNum(short itemVNum)
        {
            List<Recipe> recipes = new List<Recipe>();
            foreach (RecipeListDTO recipeList in _recipeLists.Where(r => r.ItemVNum == itemVNum))
            {
                recipes.Add(_recipes[recipeList.RecipeId]);
            }
            return recipes;
        }

        public List<Recipe> GetRecipesByMapNpcId(int mapNpcId)
        {
            List<Recipe> recipes = new List<Recipe>();
            foreach (RecipeListDTO recipeList in _recipeLists.Where(r => r.MapNpcId == mapNpcId))
            {
                recipes.Add(_recipes[recipeList.RecipeId]);
            }
            return recipes;
        }

        public void SetProperty(long charId, string property, object value)
        {
            var session = GetSessionByCharacterId(charId);
            if (session == null)
            {
                return;
            }

            var propertyinfo = session.Character.GetType().GetProperty(property);
            propertyinfo?.SetValue(session.Character, value, null);
        }

        public T GetProperty<T>(string charName, string property)
        {
            var session = Sessions.FirstOrDefault(s => s.Character != null && s.Character.Name.Equals(charName));
            if (session == null)
            {
                return default(T);
            }

            return (T)session.Character.GetType().GetProperties().Single(pi => pi.Name == property).GetValue(session.Character, null);
        }

        public T GetProperty<T>(long charId, string property)
        {
            var session = GetSessionByCharacterId(charId);
            if (session == null)
            {
                return default(T);
            }

            return (T)session.Character.GetType().GetProperties().Single(pi => pi.Name == property).GetValue(session.Character, null);
        }

        public ClientSession GetSessionByCharacterName(string name) => Sessions.SingleOrDefault(s => s.Character.Name == name);

        public ClientSession GetSessionBySessionId(int sessionId) => Sessions.SingleOrDefault(s => s.SessionId == sessionId);

        public static Skill GetSkill(short skillVNum) => _skills.FirstOrDefault(m => m.SkillVNum.Equals(skillVNum));

        public void GroupLeave(ClientSession session)
        {
            if (Groups != null)
            {
                Group grp = Instance.Groups.Find(s => s.IsMemberOfGroup(session.Character.CharacterId));
                if (grp != null)
                {
                    switch (grp.GroupType)
                    {
                        case GroupType.BigTeam:
                        case GroupType.Team:
                            if (grp.Characters.ElementAt(0) == session && grp.CharacterCount > 1)
                            {
                                Broadcast(session,
                                    UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("NEW_LEADER")),
                                    ReceiverType.OnlySomeone, string.Empty,
                                    grp.Characters.ElementAt(1)?.Character.CharacterId ?? 0);
                            }
                            grp.LeaveGroup(session);
                            session.SendPacket(session.Character.GenerateRaid(1, true));
                            session.SendPacket(session.Character.GenerateRaid(2, true));
                            foreach (ClientSession groupSession in grp.Characters.GetAllItems())
                            {
                                groupSession.SendPacket(grp.GenerateRdlst());
                                groupSession.SendPacket(groupSession.Character.GenerateRaid(0));
                            }
                            if (session.CurrentMapInstance?.MapInstanceType == MapInstanceType.RaidInstance)
                            {
                                ChangeMap(session.Character.CharacterId, session.Character.MapId, session.Character.MapX, session.Character.MapY);
                            }
                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("RAID_LEFT"), 0));
                            break;

                        case GroupType.GiantTeam:
                            ClientSession[] grpmembers = new ClientSession[40];
                            grp.Characters.CopyTo(grpmembers);
                            foreach (ClientSession targetSession in grpmembers)
                            {
                                if (targetSession != null)
                                {
                                    targetSession.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("GROUP_CLOSED"), 0));
                                    Broadcast(targetSession.Character.GeneratePidx(true));
                                    grp.LeaveGroup(targetSession);
                                    targetSession.SendPacket(targetSession.Character.GeneratePinit());
                                    targetSession.SendPackets(targetSession.Character.GeneratePst());
                                }
                            }
                            GroupList.RemoveAll(s => s.GroupId == grp.GroupId);
                            GroupsThreadSafe.Remove(grp.GroupId);
                            break;

                        case GroupType.Group:
                            if (grp.Characters.ElementAt(0) == session && grp.CharacterCount > 1)
                            {
                                Broadcast(session, UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("NEW_LEADER")), ReceiverType.OnlySomeone, string.Empty, grp.Characters.ElementAt(1).Character.CharacterId);
                            }
                            grp.LeaveGroup(session);
                            if (grp.CharacterCount == 1)
                            {
                                ClientSession targetSession = grp.Characters.ElementAt(0);
                                if (targetSession != null)
                                {
                                    targetSession.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("GROUP_CLOSED"), 0));
                                    Broadcast(targetSession.Character.GeneratePidx(true));
                                    grp.LeaveGroup(targetSession);
                                    targetSession.SendPacket(targetSession.Character.GeneratePinit());
                                    targetSession.SendPackets(targetSession.Character.GeneratePst());
                                }
                            }
                            else
                            {
                                foreach (ClientSession groupSession in grp.Characters.GetAllItems())
                                {
                                    groupSession.SendPacket(groupSession.Character.GeneratePinit());
                                    groupSession.SendPackets(session.Character.GeneratePst());
                                    groupSession.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("LEAVE_GROUP"), session.Character.Name), 0));
                                }
                            }
                            session.SendPacket(session.Character.GeneratePinit());
                            session.SendPackets(session.Character.GeneratePst());
                            Broadcast(session.Character.GeneratePidx(true));
                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("GROUP_LEFT"), 0));
                            break;

                        default:
                            return;
                    }
                    session.Character.Group = null;
                }
            }
        }

        public void Initialize()
        {
            Act4RaidStart = DateTime.Now;
            Act4AngelStat = new Act4Stat();
            Act4DemonStat = new Act4Stat();

            CharacterScreenSessions = new ThreadSafeSortedList<long, ClientSession>();

            // Load Configuration

            Schedules = ConfigurationManager.GetSection("eventScheduler") as List<Schedule>;

            OrderablePartitioner<ItemDTO> itemPartitioner = Partitioner.Create(DAOFactory.ItemDAO.LoadAll(), EnumerablePartitionerOptions.NoBuffering);
            Parallel.ForEach(itemPartitioner, new ParallelOptions { MaxDegreeOfParallelism = 4 }, itemDto =>
            {
                switch (itemDto.ItemType)
                {
                    case ItemType.Armor:
                    case ItemType.Jewelery:
                    case ItemType.Fashion:
                    case ItemType.Specialist:
                    case ItemType.Weapon:
                        _items.Add(new WearableItem(itemDto));
                        break;

                    case ItemType.Box:
                        _items.Add(new BoxItem(itemDto));
                        break;

                    case ItemType.Shell:
                    case ItemType.Magical:
                    case ItemType.Event:
                        _items.Add(new MagicalItem(itemDto));
                        break;

                    case ItemType.Food:
                        _items.Add(new FoodItem(itemDto));
                        break;

                    case ItemType.Potion:
                        _items.Add(new PotionItem(itemDto));
                        break;

                    case ItemType.Production:
                        _items.Add(new ProduceItem(itemDto));
                        break;

                    case ItemType.Snack:
                        _items.Add(new SnackItem(itemDto));
                        break;

                    case ItemType.Special:
                        _items.Add(new SpecialItem(itemDto));
                        break;

                    case ItemType.Teacher:
                        _items.Add(new TeacherItem(itemDto));
                        break;

                    case ItemType.Upgrade:
                        _items.Add(new UpgradeItem(itemDto));
                        break;

                    default:
                        _items.Add(new NoFunctionItem(itemDto));
                        break;
                }
            });
            Logger.Info(string.Format(Language.Instance.GetMessageFromKey("ITEMS_LOADED"), _items.Count));

            // intialize monsterdrops
            _monsterDrops = new ThreadSafeSortedList<short, List<DropDTO>>();
            Parallel.ForEach(DAOFactory.DropDAO.LoadAll().GroupBy(d => d.MonsterVNum), monsterDropGrouping =>
            {
                if (monsterDropGrouping.Key.HasValue)
                {
                    _monsterDrops[monsterDropGrouping.Key.Value] = monsterDropGrouping.OrderBy(d => d.DropChance).ToList();
                }
                else
                {
                    _generalDrops = monsterDropGrouping.ToList();
                }
            });
            Logger.Info(string.Format(Language.Instance.GetMessageFromKey("DROPS_LOADED"), _monsterDrops.Sum(i => i.Count)));

            // initialize monsterskills
            _monsterSkills = new ThreadSafeSortedList<short, List<NpcMonsterSkill>>();
            Parallel.ForEach(DAOFactory.NpcMonsterSkillDAO.LoadAll().GroupBy(n => n.NpcMonsterVNum), monsterSkillGrouping => _monsterSkills[monsterSkillGrouping.Key] = monsterSkillGrouping.Select(n => new NpcMonsterSkill(n)).ToList());
            Logger.Info(string.Format(Language.Instance.GetMessageFromKey("MONSTERSKILLS_LOADED"), _monsterSkills.Sum(i => i.Count)));

            // initialize bazaar
            BazaarList = new ThreadSafeGenericList<BazaarItemLink>();
            OrderablePartitioner<BazaarItemDTO> bazaarPartitioner = Partitioner.Create(DAOFactory.BazaarItemDAO.LoadAll(), EnumerablePartitionerOptions.NoBuffering);
            Parallel.ForEach(bazaarPartitioner, new ParallelOptions { MaxDegreeOfParallelism = 8 }, bazaarItem =>
            {
                BazaarItemLink item = new BazaarItemLink
                {
                    BazaarItem = bazaarItem
                };
                CharacterDTO chara = DAOFactory.CharacterDAO.LoadById(bazaarItem.SellerId);
                if (chara != null)
                {
                    item.Owner = chara.Name;
                    item.Item = new ItemInstance(DAOFactory.IteminstanceDAO.LoadById(bazaarItem.ItemInstanceId));
                }
                BazaarList.Add(item);
            });
            Logger.Info(string.Format(Language.Instance.GetMessageFromKey("BAZAAR_LOADED"), BazaarList.Count));

            // initialize npcmonsters
            Parallel.ForEach(DAOFactory.NpcMonsterDAO.LoadAll(), npcMonster =>
            {
                NpcMonster npcMonsterObj = new NpcMonster(npcMonster);
                npcMonsterObj.Initialize();
                npcMonsterObj.BCards = new List<BCard>();
                DAOFactory.BCardDAO.LoadByNpcMonsterVNum(npcMonster.NpcMonsterVNum).ToList().ForEach(s => npcMonsterObj.BCards.Add(new BCard(s)));
                _npcs.Add(npcMonsterObj);
            });
            Logger.Info(string.Format(Language.Instance.GetMessageFromKey("NPCMONSTERS_LOADED"), _npcs.Count));

            // intialize recipes
            _recipes = new ThreadSafeSortedList<short, Recipe>();
            Parallel.ForEach(DAOFactory.RecipeDAO.LoadAll(), recipeGrouping =>
            {
                Recipe recipe = new Recipe(recipeGrouping);
                _recipes[recipeGrouping.RecipeId] = recipe;
                recipe.Initialize();
            });
            Logger.Info(string.Format(Language.Instance.GetMessageFromKey("RECIPES_LOADED"), _recipes.Count));

            // initialize recipelist
            _recipeLists = new ThreadSafeSortedList<int, RecipeListDTO>();
            Parallel.ForEach(DAOFactory.RecipeListDAO.LoadAll(), recipeListGrouping => _recipeLists[recipeListGrouping.RecipeListId] = recipeListGrouping);
            Logger.Info(string.Format(Language.Instance.GetMessageFromKey("RECIPELISTS_LOADED"), _recipeLists.Count));

            // initialize shopitems
            _shopItems = new ThreadSafeSortedList<int, List<ShopItemDTO>>();
            Parallel.ForEach(DAOFactory.ShopItemDAO.LoadAll().GroupBy(s => s.ShopId), shopItemGrouping => _shopItems[shopItemGrouping.Key] = shopItemGrouping.ToList());
            Logger.Info(string.Format(Language.Instance.GetMessageFromKey("SHOPITEMS_LOADED"), _shopItems.Sum(i => i.Count)));

            // initialize shopskills
            _shopSkills = new ThreadSafeSortedList<int, List<ShopSkillDTO>>();
            Parallel.ForEach(DAOFactory.ShopSkillDAO.LoadAll().GroupBy(s => s.ShopId), shopSkillGrouping => _shopSkills[shopSkillGrouping.Key] = shopSkillGrouping.ToList());
            Logger.Info(string.Format(Language.Instance.GetMessageFromKey("SHOPSKILLS_LOADED"), _shopSkills.Sum(i => i.Count)));

            // initialize shops
            _shops = new ThreadSafeSortedList<int, Shop>();
            Parallel.ForEach(DAOFactory.ShopDAO.LoadAll(), shopGrouping =>
            {
                Shop shop = new Shop(shopGrouping);
                _shops[shopGrouping.MapNpcId] = shop;
                shop.Initialize();
            });
            Logger.Info(string.Format(Language.Instance.GetMessageFromKey("SHOPS_LOADED"), _shops.Count));

            // initialize teleporters
            _teleporters = new ThreadSafeSortedList<int, List<TeleporterDTO>>();
            Parallel.ForEach(DAOFactory.TeleporterDAO.LoadAll().GroupBy(t => t.MapNpcId), teleporterGrouping => _teleporters[teleporterGrouping.Key] = teleporterGrouping.Select(t => t).ToList());
            Logger.Info(string.Format(Language.Instance.GetMessageFromKey("TELEPORTERS_LOADED"), _teleporters.Sum(i => i.Count)));

            // initialize skills
            Parallel.ForEach(DAOFactory.SkillDAO.LoadAll(), skill =>
            {
                Skill skillObj = new Skill(skill);
                skillObj.Combos.AddRange(DAOFactory.ComboDAO.LoadBySkillVnum(skillObj.SkillVNum).ToList());
                skillObj.BCards = new List<BCard>();
                DAOFactory.BCardDAO.LoadBySkillVNum(skillObj.SkillVNum).ToList().ForEach(o => skillObj.BCards.Add(new BCard(o)));
                _skills.Add(skillObj);
            });
            Logger.Info(string.Format(Language.Instance.GetMessageFromKey("SKILLS_LOADED"), _skills.Count));

            // initialize cards
            Parallel.ForEach(DAOFactory.CardDAO.LoadAll(), card =>
            {
                Card cardObj = new Card(card)
                {
                    BCards = new List<BCard>()
                };
                DAOFactory.BCardDAO.LoadByCardId(cardObj.CardId).ToList().ForEach(o => cardObj.BCards.Add(new BCard(o)));
                _cards.Add(cardObj);
            });
            Logger.Info(string.Format(Language.Instance.GetMessageFromKey("CARDS_LOADED"), _cards.Count));

            // intialize mapnpcs
            _mapNpcs = new ThreadSafeSortedList<short, List<MapNpc>>();
            Parallel.ForEach(DAOFactory.MapNpcDAO.LoadAll().GroupBy(t => t.MapId), mapNpcGrouping => _mapNpcs[mapNpcGrouping.Key] = mapNpcGrouping.Select(t => t as MapNpc).ToList());
            Logger.Info(string.Format(Language.Instance.GetMessageFromKey("MAPNPCS_LOADED"), _mapNpcs.Sum(i => i.Count)));

            try
            {
                int i = 0;
                int monstercount = 0;
                OrderablePartitioner<MapDTO> mapPartitioner = Partitioner.Create(DAOFactory.MapDAO.LoadAll(), EnumerablePartitionerOptions.NoBuffering);
                Parallel.ForEach(mapPartitioner, new ParallelOptions { MaxDegreeOfParallelism = 8 }, map =>
                {
                    Guid guid = Guid.NewGuid();
                    Map mapinfo = new Map(map.MapId, map.Data)
                    {
                        Music = map.Music,
                        Name = map.Name,
                        ShopAllowed = map.ShopAllowed
                    };
                    _maps.Add(mapinfo);
                    MapInstance newMap = new MapInstance(mapinfo, guid, map.ShopAllowed, MapInstanceType.BaseMapInstance, new InstanceBag());
                    _mapinstances.TryAdd(guid, newMap);

                    Task.Run((Action)newMap.LoadPortals);
                    newMap.LoadNpcs();
                    newMap.LoadMonsters();

                    Parallel.ForEach(newMap.Npcs, mapNpc =>
                    {
                        mapNpc.MapInstance = newMap;
                        newMap.AddNPC(mapNpc);
                    });
                    Parallel.ForEach(newMap.Monsters, mapMonster =>
                    {
                        mapMonster.MapInstance = newMap;
                        newMap.AddMonster(mapMonster);
                    });
                    monstercount += newMap.Monsters.Count;
                    i++;
                });
                if (i != 0)
                {
                    Logger.Info(string.Format(Language.Instance.GetMessageFromKey("MAPS_LOADED"), i));
                }
                else
                {
                    Logger.Error(Language.Instance.GetMessageFromKey("NO_MAP"));
                }
                Logger.Info(string.Format(Language.Instance.GetMessageFromKey("MAPMONSTERS_LOADED"), monstercount));
                StartedEvents = new List<EventType>();

               

                // initialize families
                LoadFamilies();
                LaunchEvents();
                RefreshRanking();
                CharacterRelations = DAOFactory.CharacterRelationDAO.LoadAll().ToList();
                PenaltyLogs = DAOFactory.PenaltyLogDAO.LoadAll().ToList();
                if (DAOFactory.MapDAO.LoadById(2006) != null)
                {
                    ArenaInstance = GenerateMapInstance(2006, MapInstanceType.NormalInstance, new InstanceBag());
                    ArenaInstance.IsPVP = true;
                }
                if (DAOFactory.MapDAO.LoadById(2106) != null)
                {
                    FamilyArenaInstance = GenerateMapInstance(2106, MapInstanceType.NormalInstance, new InstanceBag());
                    FamilyArenaInstance.IsPVP = true;
                }
                LoadScriptedInstances();

                XmlSerializer serializer = new XmlSerializer(typeof(QuestModel));
                QuestList = new ThreadSafeSortedList<long, QuestModel>();
                Parallel.ForEach(DAOFactory.QuestDAO.LoadAll(), s =>
                {
                    if (s.QuestData != null)
                    {
                        using (TextReader reader = new StringReader(s.QuestData))
                        {
                            QuestList[s.QuestId] = (QuestModel)serializer.Deserialize(reader);
                        }
                    }
                });

                Logger.Info(string.Format(Language.Instance.GetMessageFromKey("QUESTS_LOADED"), QuestList.Count));
            }
            catch (Exception ex)
            {
                Logger.Error("General Error", ex);
            }
            WorldId = Guid.NewGuid();
        }

        public bool IsCharacterMemberOfGroup(long characterId) => Groups?.Any(g => g.IsMemberOfGroup(characterId)) == true;

        public bool IsCharactersGroupFull(long characterId) => Groups?.Any(g => g.IsMemberOfGroup(characterId) && (g.CharacterCount == (byte)g.GroupType || g.GroupType == GroupType.TalentArena)) == true;

        public bool ItemHasRecipe(short itemVNum) => _recipeLists.Any(r => r.ItemVNum == itemVNum);

        public void JoinMiniland(ClientSession session, ClientSession minilandOwner)
        {
            if (session.Character.Miniland.MapInstanceId == minilandOwner.Character.Miniland.MapInstanceId)
            {
                foreach (Mate mate in session.Character.Mates)
                {
                    mate.Hp = mate.MaxHp;
                    mate.Mp = mate.MaxMp;
                }
            }

            ChangeMapInstance(session.Character.CharacterId, minilandOwner.Character.Miniland.MapInstanceId, 5, 8);
            if (session.Character.Miniland.MapInstanceId != minilandOwner.Character.Miniland.MapInstanceId)
            {
                session.SendPacket(UserInterfaceHelper.GenerateMsg(session.Character.MinilandMessage.Replace(' ', '^'), 0));
                session.SendPacket(session.Character.GenerateMlinfobr());
                minilandOwner.Character.GeneralLogs.Add(new GeneralLogDTO { AccountId = session.Account.AccountId, CharacterId = session.Character.CharacterId, IpAddress = session.IpAddress, LogData = "Miniland", LogType = "World", Timestamp = DateTime.Now });
                session.SendPacket(minilandOwner.Character.GenerateMinilandObjectForFriends());
            }
            else
            {
                session.SendPacket(session.Character.GenerateMlinfo());
                session.SendPacket(minilandOwner.Character.GetMinilandObjectList());
            }
            minilandOwner.Character.Mates.Where(s => !s.IsTeamMember).ToList().ForEach(s => session.SendPacket(s.GenerateIn()));
            session.SendPackets(minilandOwner.Character.GetMinilandEffects());
            session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MINILAND_VISITOR"), session.Character.GeneralLogs.CountLinq(s => s.LogData == "Miniland" && s.Timestamp.Day == DateTime.Now.Day), session.Character.GeneralLogs.CountLinq(s => s.LogData == "Miniland")), 10));
        }

        // Server
        public void Kick(string characterName)
        {
            ClientSession session = Sessions.FirstOrDefault(s => s.Character?.Name.Equals(characterName) == true);
            session?.Disconnect();
        }

        // Map
        public void LeaveMap(long id)
        {
            ClientSession session = GetSessionByCharacterId(id);
            if (session == null)
            {
                return;
            }
            session.SendPacket(UserInterfaceHelper.GenerateMapOut());
            if (!session.Character.InvisibleGm)
            {
                session.Character.Mates.Where(s => s.IsTeamMember).ToList().ForEach(s => session.CurrentMapInstance?.Broadcast(session, StaticPacketHelper.Out(UserType.Npc, s.MateTransportId), ReceiverType.AllExceptMe));
                session.CurrentMapInstance?.Broadcast(session, StaticPacketHelper.Out(UserType.Player, session.Character.CharacterId), ReceiverType.AllExceptMe);
            }
        }

        public bool MapNpcHasRecipe(int mapNpcId) => _recipeLists.Any(r => r.MapNpcId == mapNpcId);

        public static int RandomNumber(int min = 0, int max = 100) => _random.Next(min, max);

        public void RefreshRanking()
        {
            TopComplimented = DAOFactory.CharacterDAO.GetTopCompliment();
            TopPoints = DAOFactory.CharacterDAO.GetTopPoints();
            TopReputation = DAOFactory.CharacterDAO.GetTopReputation();
        }

        public void RelationRefresh(long relationId)
        {
            _inRelationRefreshMode = true;
            CommunicationServiceClient.Instance.UpdateRelation(ServerGroup, relationId);
            SpinWait.SpinUntil(() => !_inRelationRefreshMode);
        }

        public static void RemoveMapInstance(Guid mapId)
        {
            KeyValuePair<Guid, MapInstance> map = _mapinstances.FirstOrDefault(s => s.Key == mapId);
            if (!map.Equals(default))
            {
                map.Value.Dispose();
                ((IDictionary)_mapinstances).Remove(map.Key);
            }
        }

        // Map
        public void ReviveFirstPosition(long characterId)
        {
            ClientSession session = GetSessionByCharacterId(characterId);
            if (session?.Character.Hp <= 0)
            {
                if (session.CurrentMapInstance.MapInstanceType == MapInstanceType.TimeSpaceInstance || session.CurrentMapInstance.MapInstanceType == MapInstanceType.RaidInstance)
                {
                    session.Character.Hp = (int)session.Character.HPLoad();
                    session.Character.Mp = (int)session.Character.MPLoad();
                    session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateRevive());
                    session.SendPacket(session.Character.GenerateStat());
                }
                else
                {
                    if (ChannelId == 51)
                    {
                        session.Character.Hp = (int)session.Character.HPLoad();
                        session.Character.Mp = (int)session.Character.MPLoad();
                        short x = (short)(39 + RandomNumber(-2, 3));
                        short y = (short)(42 + RandomNumber(-2, 3));
                        if (session.Character.Faction == FactionType.Angel)
                        {
                            ChangeMap(session.Character.CharacterId, 130, x, y);
                        }
                        else if (session.Character.Faction == FactionType.Demon)
                        {
                            ChangeMap(session.Character.CharacterId, 131, x, y);
                        }
                    }
                    else
                    {
                        session.Character.Hp = 1;
                        session.Character.Mp = 1;
                        if (session.CurrentMapInstance.MapInstanceType == MapInstanceType.BaseMapInstance)
                        {
                            RespawnMapTypeDTO resp = session.Character.Respawn;
                            short x = (short)(resp.DefaultX + RandomNumber(-3, 3));
                            short y = (short)(resp.DefaultY + RandomNumber(-3, 3));
                            ChangeMap(session.Character.CharacterId, resp.DefaultMapId, x, y);
                        }
                        else
                        {
                            Instance.ChangeMap(session.Character.CharacterId, session.Character.MapId, session.Character.MapX, session.Character.MapY);
                        }
                    }
                    session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateTp());
                    session.CurrentMapInstance?.Broadcast(session.Character.GenerateRevive());
                    session.SendPacket(session.Character.GenerateStat());
                }
            }
        }

        public void SaveAll()
        {
            CommunicationServiceClient.Instance.CleanupOutdatedSession();
            foreach (ClientSession sess in Sessions)
            {
                sess.Character?.Save();
            }
            DAOFactory.BazaarItemDAO.RemoveOutDated();
        }

        public static void Shout(string message, bool noAdminTag = false)
        {
            Instance.Broadcast(UserInterfaceHelper.GenerateSay((noAdminTag ? string.Empty : $"({Language.Instance.GetMessageFromKey("ADMINISTRATOR")})") + message, 10));
            Instance.Broadcast(UserInterfaceHelper.GenerateMsg(message, 2));
        }

        public async Task ShutdownTaskAsync()
        {
            Shout(string.Format(Language.Instance.GetMessageFromKey("SHUTDOWN_MIN"), 5));
            for (int i = 0; i < 60 * 4; i++)
            {
                await Task.Delay(1000).ConfigureAwait(false);
                if (Instance.ShutdownStop)
                {
                    Instance.ShutdownStop = false;
                    return;
                }
            }
            Shout(string.Format(Language.Instance.GetMessageFromKey("SHUTDOWN_MIN"), 1));
            for (int i = 0; i < 30; i++)
            {
                await Task.Delay(1000).ConfigureAwait(false);
                if (Instance.ShutdownStop)
                {
                    Instance.ShutdownStop = false;
                    return;
                }
            }
            Shout(string.Format(Language.Instance.GetMessageFromKey("SHUTDOWN_SEC"), 30));
            for (int i = 0; i < 30; i++)
            {
                await Task.Delay(1000).ConfigureAwait(false);
                if (Instance.ShutdownStop)
                {
                    Instance.ShutdownStop = false;
                    return;
                }
            }
            Shout(string.Format(Language.Instance.GetMessageFromKey("SHUTDOWN_SEC"), 10));
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(1000).ConfigureAwait(false);
                if (Instance.ShutdownStop)
                {
                    Instance.ShutdownStop = false;
                    return;
                }
            }
            InShutdown = true;
            Instance.SaveAll();
            CommunicationServiceClient.Instance.UnregisterWorldServer(WorldId);
            if (IsReboot)
            {
                if (ChannelId == 51)
                {
                    await Task.Delay(16000).ConfigureAwait(false);
                }
                else
                {
                    await Task.Delay((ChannelId - 1) * 2000).ConfigureAwait(false);
                }
                Process.Start("OpenNos.World.exe", "--nomsg");
            }
            Environment.Exit(0);
        }

        public void TeleportOnRandomPlaceInMap(ClientSession session, Guid guid)
        {
            MapInstance map = GetMapInstance(guid);
            if (guid != default)
            {
                MapCell pos = map.Map.GetRandomPosition();
                ChangeMapInstance(session.Character.CharacterId, guid, pos.X, pos.Y);
            }
        }

        // Server
        public void UpdateGroup(long charId)
        {
            try
            {
                if (Groups != null)
                {
                    Group myGroup = Groups.Find(s => s.IsMemberOfGroup(charId));
                    if (myGroup == null)
                    {
                        return;
                    }
                    ThreadSafeGenericList<ClientSession> groupMembers = Groups.Find(s => s.IsMemberOfGroup(charId))?.Characters;
                    if (groupMembers != null)
                    {
                        foreach (ClientSession session in groupMembers.GetAllItems())
                        {
                            session.SendPacket(session.Character.GeneratePinit());
                            session.SendPackets(session.Character.GeneratePst());
                            session.SendPacket(session.Character.GenerateStat());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        internal List<NpcMonsterSkill> GetNpcMonsterSkillsByMonsterVNum(short npcMonsterVNum) => _monsterSkills.ContainsKey(npcMonsterVNum) ? _monsterSkills[npcMonsterVNum] : new List<NpcMonsterSkill>();

        internal Shop GetShopByMapNpcId(int mapNpcId) => _shops.ContainsKey(mapNpcId) ? _shops[mapNpcId] : null;

        internal List<ShopItemDTO> GetShopItemsByShopId(int shopId) => _shopItems.ContainsKey(shopId) ? _shopItems[shopId] : new List<ShopItemDTO>();

        internal List<ShopSkillDTO> GetShopSkillsByShopId(int shopId) => _shopSkills.ContainsKey(shopId) ? _shopSkills[shopId] : new List<ShopSkillDTO>();

        internal List<TeleporterDTO> GetTeleportersByNpcVNum(short npcMonsterVNum)
        {
            if (_teleporters?.ContainsKey(npcMonsterVNum) == true)
            {
                return _teleporters[npcMonsterVNum];
            }
            return new List<TeleporterDTO>();
        }

        internal static void StopServer()
        {
            Instance.ShutdownStop = true;
            Instance.TaskShutdown = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _monsterDrops.Dispose();
                GroupsThreadSafe.Dispose();
                _monsterSkills.Dispose();
                _shopSkills.Dispose();
                _shopItems.Dispose();
                _shops.Dispose();
                _recipes.Dispose();
                _mapNpcs.Dispose();
                _teleporters.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        private void Act4Process()
        {
            if (ChannelId != 51)
            {
                return;
            }

            MapInstance angelMapInstance = GetMapInstance(GetBaseMapInstanceIdByMapId(132));
            MapInstance demonMapInstance = GetMapInstance(GetBaseMapInstanceIdByMapId(133));

            void SummonMukraju(MapInstance instance, byte faction)
            {
                MapMonster monster = new MapMonster
                {
                    MonsterVNum = 556,
                    MapY = (faction == 1 ? (short)92 : (short)95),
                    MapX = (faction == 1 ? (short)114 : (short)20),
                    MapId = (short)(131 + faction),
                    IsMoving = true,
                    MapMonsterId = instance.GetNextMonsterId(),
                    ShouldRespawn = false
                };
                monster.Initialize(instance);
                instance.AddMonster(monster);
                instance.Broadcast(monster.GenerateIn());
            }

            int CreateRaid(byte faction)
            {
                MapInstanceType raidType = MapInstanceType.Act4Morcos;
                int rng = RandomNumber(1, 5);
                switch (rng)
                {
                    case 2:
                        raidType = MapInstanceType.Act4Hatus;
                        break;

                    case 3:
                        raidType = MapInstanceType.Act4Calvina;
                        break;

                    case 4:
                        raidType = MapInstanceType.Act4Berios;
                        break;
                }
                Event.Act4Raid.GenerateRaid(raidType, faction);
                return rng;
            }

            if (Act4AngelStat.Percentage > 10000)
            {
                Act4AngelStat.Mode = 1;
                Act4AngelStat.Percentage = 0;
                Act4AngelStat.TotalTime = 300;
                SummonMukraju(angelMapInstance, 1);
            }

            if (Act4AngelStat.Mode == 1 && !angelMapInstance.Monsters.Any(s => s.MonsterVNum == 556))
            {
                Act4AngelStat.Mode = 3;
                Act4AngelStat.TotalTime = 3600;

                switch (CreateRaid(1))
                {
                    case 1:
                        Act4AngelStat.IsMorcos = true;
                        break;

                    case 2:
                        Act4AngelStat.IsHatus = true;
                        break;

                    case 3:
                        Act4AngelStat.IsCalvina = true;
                        break;

                    case 4:
                        Act4AngelStat.IsBerios = true;
                        break;
                }
            }

            if (Act4DemonStat.Percentage > 10000)
            {
                Act4DemonStat.Mode = 1;
                Act4DemonStat.Percentage = 0;
                Act4DemonStat.TotalTime = 300;
                SummonMukraju(demonMapInstance, 2);
            }

            if (Act4DemonStat.Mode == 1 && !demonMapInstance.Monsters.Any(s => s.MonsterVNum == 556))
            {
                Act4DemonStat.Mode = 3;
                Act4DemonStat.TotalTime = 3600;

                switch (CreateRaid(2))
                {
                    case 1:
                        Act4DemonStat.IsMorcos = true;
                        break;

                    case 2:
                        Act4DemonStat.IsHatus = true;
                        break;

                    case 3:
                        Act4DemonStat.IsCalvina = true;
                        break;

                    case 4:
                        Act4DemonStat.IsBerios = true;
                        break;
                }
            }

            Parallel.ForEach(Sessions, sess => sess.SendPacket(sess.Character.GenerateFc()));
        }

        // Server
        private static void BotProcess()
        {
            try
            {
                Shout(Language.Instance.GetMessageFromKey($"BOT_MESSAGE_{RandomNumber(0, 5)}"));
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void GroupProcess()
        {
            try
            {
                if (Groups != null)
                {
                    Parallel.ForEach(Groups, grp =>
                    {
                        foreach (ClientSession session in grp.Characters.GetAllItems())
                        {
                            session.SendPackets(grp.GeneratePst(session));
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void LaunchEvents()
        {
            GroupsThreadSafe = new ThreadSafeSortedList<long, Group>();

            Observable.Interval(TimeSpan.FromMinutes(5)).Subscribe(x => SaveAllProcess());
            Observable.Interval(TimeSpan.FromMinutes(1)).Subscribe(x => Act4Process());
            Observable.Interval(TimeSpan.FromSeconds(2)).Subscribe(x => GroupProcess());
            Observable.Interval(TimeSpan.FromHours(3)).Subscribe(x => BotProcess());
            Observable.Interval(TimeSpan.FromMinutes(1)).Subscribe(x => MaintenanceProcess());

            EventHelper.Instance.RunEvent(new EventContainer(GetMapInstance(GetBaseMapInstanceIdByMapId(98)), EventActionType.NPCSEFFECTCHANGESTATE, true));
            Parallel.ForEach(Schedules, schedule => Observable.Timer(TimeSpan.FromSeconds(EventHelper.GetMilisecondsBeforeTime(schedule.Time).TotalSeconds), TimeSpan.FromDays(1)).Subscribe(e => EventHelper.GenerateEvent(schedule.Event)));
            EventHelper.GenerateEvent(EventType.ACT4SHIP);

            Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(x => RemoveItemProcess());
            Observable.Interval(TimeSpan.FromMilliseconds(400)).Subscribe(x =>
            {
                Parallel.ForEach(_mapinstances, map =>
                {
                    Parallel.ForEach(map.Value.Npcs, npc => npc.StartLife());
                    Parallel.ForEach(map.Value.Monsters, monster => monster.StartLife());
                });
            });

            CommunicationServiceClient.Instance.SessionKickedEvent += OnSessionKicked;
            CommunicationServiceClient.Instance.MessageSentToCharacter += OnMessageSentToCharacter;
            CommunicationServiceClient.Instance.FamilyRefresh += OnFamilyRefresh;
            CommunicationServiceClient.Instance.RelationRefresh += OnRelationRefresh;
            CommunicationServiceClient.Instance.StaticBonusRefresh += OnStaticBonusRefresh;
            CommunicationServiceClient.Instance.BazaarRefresh += OnBazaarRefresh;
            CommunicationServiceClient.Instance.PenaltyLogRefresh += OnPenaltyLogRefresh;
            CommunicationServiceClient.Instance.GlobalEvent += OnGlobalEvent;
            CommunicationServiceClient.Instance.ShutdownEvent += OnShutdown;
            CommunicationServiceClient.Instance.RestartEvent += OnRestart;
            ConfigurationServiceClient.Instance.ConfigurationUpdate += OnConfiguratinEvent;
            MailServiceClient.Instance.MailSent += OnMailSent;
            _lastGroupId = 1;
        }

        private void OnStaticBonusRefresh(object sender, EventArgs e)
        {
            long characterId = (long)sender;

            ClientSession sess = GetSessionByCharacterId(characterId);
            if (sess != null)
            {
                sess.Character.StaticBonusList = DAOFactory.StaticBonusDAO.LoadByCharacterId(characterId).ToList();
            }
        }

        private void OnMailSent(object sender, EventArgs e)
        {
            MailDTO mail = (MailDTO)sender;

            ClientSession session = GetSessionByCharacterId(mail.IsSenderCopy ? mail.SenderId : mail.ReceiverId);
            if (session != null)
            {
                if (mail.AttachmentVNum != null)
                {
                    session.Character.MailList.Add((session.Character.MailList.Count > 0 ? session.Character.MailList.OrderBy(s => s.Key).Last().Key : 0) + 1, mail);
                    session.SendPacket(session.Character.GenerateParcel(mail));
                    session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("ITEM_GIFTED"), GetItem(mail.AttachmentVNum.Value)?.Name, mail.AttachmentAmount), 12));
                }
                else
                {
                    session.Character.MailList.Add((session.Character.MailList.Count > 0 ? session.Character.MailList.OrderBy(s => s.Key).Last().Key : 0) + 1, mail);
                    session.SendPacket(session.Character.GeneratePost(mail, mail.IsSenderCopy ? (byte)2 : (byte)1));
                }
            }
        }

        private void OnConfiguratinEvent(object sender, EventArgs e) => Configuration = (ConfigurationObject)sender;

        private void LoadFamilies()
        {
            FamilyList = new ThreadSafeSortedList<long, Family>();
            Parallel.ForEach(DAOFactory.FamilyDAO.LoadAll(), familyDto =>
            {
                Family family = new Family(familyDto)
                {
                    FamilyCharacters = new List<FamilyCharacter>()
                };
                foreach (FamilyCharacterDTO famchar in DAOFactory.FamilyCharacterDAO.LoadByFamilyId(family.FamilyId).ToList())
                {
                    family.FamilyCharacters.Add(new FamilyCharacter(famchar));
                }
                FamilyCharacter familyCharacter = family.FamilyCharacters.Find(s => s.Authority == FamilyAuthority.Head);
                if (familyCharacter != null)
                {
                    family.Warehouse = new Inventory(new Character(familyCharacter.Character));
                    foreach (ItemInstanceDTO inventory in DAOFactory.IteminstanceDAO.LoadByCharacterId(familyCharacter.CharacterId).Where(s => s.Type == InventoryType.FamilyWareHouse).ToList())
                    {
                        inventory.CharacterId = familyCharacter.CharacterId;
                        family.Warehouse[inventory.Id] = new ItemInstance(inventory);
                    }
                }
                family.FamilyLogs = DAOFactory.FamilyLogDAO.LoadByFamilyId(family.FamilyId).ToList();
                FamilyList[family.FamilyId] = family;
            });
        }

        private void LoadScriptedInstances()
        {
            Raids = new ConcurrentBag<ScriptedInstance>();
            Parallel.ForEach(_mapinstances, map =>
            {
                foreach (ScriptedInstanceDTO si in DAOFactory.ScriptedInstanceDAO.LoadByMap(map.Value.Map.MapId).ToList())
                {
                    ScriptedInstance siObj = new ScriptedInstance(si);
                    if (siObj.Type == ScriptedInstanceType.TimeSpace)
                    {
                        siObj.LoadGlobals();
                        map.Value.ScriptedInstances.Add(siObj);
                    }
                    else if (siObj.Type == ScriptedInstanceType.Raid)
                    {
                        siObj.LoadGlobals();
                        Raids.Add(siObj);
                        Portal port = new Portal
                        {
                            Type = (byte)PortalType.Raid,
                            SourceMapId = siObj.MapId,
                            SourceX = siObj.PositionX,
                            SourceY = siObj.PositionY
                        };
                        map.Value.Portals.Add(port);
                    }
                }
            });
        }

        private void MaintenanceProcess()
        {
            List<ClientSession> sessions = Sessions.Where(c => c.IsConnected).ToList();
            MaintenanceLogDTO maintenanceLog = DAOFactory.MaintenanceLogDAO.LoadFirst();
            if (maintenanceLog != null)
            {
                if (maintenanceLog.DateStart <= DateTime.Now)
                {
                    Logger.LogUserEvent("MAINTENANCE_STATE", "Caller: ServerManager", $"[Maintenance]{Language.Instance.GetMessageFromKey("MAINTENANCE_PLANNED")}");
                    sessions.Where(s => s.Account.Authority < AuthorityType.Moderator).ToList().ForEach(session => session.Disconnect());
                }
                else if (maintenanceLog.DateStart <= DateTime.Now.AddMinutes(5))
                {
                    int min = (maintenanceLog.DateStart - DateTime.Now).Minutes;
                    if (min != 0)
                    {
                        Shout($"Maintenance will begin in {min} minutes");
                    }
                }
            }
        }

        private void OnBazaarRefresh(object sender, EventArgs e)
        {
            long bazaarId = (long)sender;
            BazaarItemDTO bzdto = DAOFactory.BazaarItemDAO.LoadById(bazaarId);
            BazaarItemLink bzlink = BazaarList.Find(s => s.BazaarItem.BazaarItemId == bazaarId);
            lock (BazaarList)
            {
                if (bzdto != null)
                {
                    CharacterDTO chara = DAOFactory.CharacterDAO.LoadById(bzdto.SellerId);
                    if (bzlink != null)
                    {
                        BazaarList.Remove(bzlink);
                        bzlink.BazaarItem = bzdto;
                        bzlink.Owner = chara.Name;
                        bzlink.Item = new ItemInstance(DAOFactory.IteminstanceDAO.LoadById(bzdto.ItemInstanceId));
                        BazaarList.Add(bzlink);
                    }
                    else
                    {
                        BazaarItemLink item = new BazaarItemLink
                        {
                            BazaarItem = bzdto
                        };
                        if (chara != null)
                        {
                            item.Owner = chara.Name;
                            item.Item = new ItemInstance(DAOFactory.IteminstanceDAO.LoadById(bzdto.ItemInstanceId));
                        }
                        BazaarList.Add(item);
                    }
                }
                else if (bzlink != null)
                {
                    BazaarList.Remove(bzlink);
                }
            }
            InBazaarRefreshMode = false;
        }

        private void OnFamilyRefresh(object sender, EventArgs e)
        {
            long familyId = (long)sender;
            FamilyDTO famdto = DAOFactory.FamilyDAO.LoadById(familyId);
            Family fam = FamilyList[familyId];
            lock (FamilyList)
            {
                if (famdto != null)
                {
                    Family newFam = new Family(famdto);
                    if (fam != null)
                    {
                        newFam.LandOfDeath = fam.LandOfDeath;
                        newFam.Act4Raid = fam.Act4Raid;
                        newFam.Act4RaidBossMap = fam.Act4RaidBossMap;
                    }

                    newFam.FamilyCharacters = new List<FamilyCharacter>();
                    foreach (FamilyCharacterDTO famchar in DAOFactory.FamilyCharacterDAO.LoadByFamilyId(famdto.FamilyId).ToList())
                    {
                        newFam.FamilyCharacters.Add(new FamilyCharacter(famchar));
                    }
                    FamilyCharacter familyCharacter = newFam.FamilyCharacters.Find(s => s.Authority == FamilyAuthority.Head);
                    if (familyCharacter != null)
                    {
                        newFam.Warehouse = new Inventory(new Character(familyCharacter.Character));
                        foreach (ItemInstanceDTO inventory in DAOFactory.IteminstanceDAO.LoadByCharacterId(familyCharacter.CharacterId).Where(s => s.Type == InventoryType.FamilyWareHouse).ToList())
                        {
                            inventory.CharacterId = familyCharacter.CharacterId;
                            newFam.Warehouse[inventory.Id] = new ItemInstance(inventory);
                        }
                    }
                    newFam.FamilyLogs = DAOFactory.FamilyLogDAO.LoadByFamilyId(famdto.FamilyId).ToList();
                    FamilyList[familyId] = newFam;

                    foreach (ClientSession sess in Sessions.Where(s => newFam.FamilyCharacters.Any(f => f.CharacterId.Equals(s.Character.CharacterId))))
                    {
                        sess.Character.Family = newFam;
                    }
                }
                else if (fam != null)
                {
                    lock (FamilyList)
                    {
                        FamilyList.Remove(fam.FamilyId);
                    }
                    foreach (ClientSession sess in Sessions.Where(s => fam.FamilyCharacters.Any(f => f.CharacterId.Equals(s.Character.CharacterId))))
                    {
                        sess.Character.Family = null;
                        sess.SendPacket(sess.Character.GenerateGidx());
                    }
                }
            }
        }

        private static void OnGlobalEvent(object sender, EventArgs e) => EventHelper.GenerateEvent((EventType)sender);

        private void OnMessageSentToCharacter(object sender, EventArgs e)
        {
            if (sender != null)
            {
                SCSCharacterMessage message = (SCSCharacterMessage)sender;

                ClientSession targetSession = Sessions.SingleOrDefault(s => s.Character.CharacterId == message.DestinationCharacterId);
                switch (message.Type)
                {
                    case MessageType.WhisperGM:
                    case MessageType.Whisper:
                        if (targetSession == null || (message.Type == MessageType.WhisperGM && targetSession.Account.Authority != AuthorityType.GameMaster))
                        {
                            return;
                        }

                        if (targetSession.Character.GmPvtBlock)
                        {
                            if (message.DestinationCharacterId != null)
                            {
                                CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                                {
                                    DestinationCharacterId = message.SourceCharacterId,
                                    SourceCharacterId = message.DestinationCharacterId.Value,
                                    SourceWorldId = WorldId,
                                    Message = targetSession.Character.GenerateSay(
                                        Language.Instance.GetMessageFromKey("GM_CHAT_BLOCKED"), 10),
                                    Type = MessageType.PrivateChat
                                });
                            }
                        }
                        else if (targetSession.Character.WhisperBlocked)
                        {
                            if (message.DestinationCharacterId != null)
                            {
                                CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                                {
                                    DestinationCharacterId = message.SourceCharacterId,
                                    SourceCharacterId = message.DestinationCharacterId.Value,
                                    SourceWorldId = WorldId,
                                    Message = UserInterfaceHelper.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("USER_WHISPER_BLOCKED"), 0),
                                    Type = MessageType.PrivateChat
                                });
                            }
                        }
                        else
                        {
                            if (message.SourceWorldId != WorldId)
                            {
                                if (message.DestinationCharacterId != null)
                                {
                                    CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                                    {
                                        DestinationCharacterId = message.SourceCharacterId,
                                        SourceCharacterId = message.DestinationCharacterId.Value,
                                        SourceWorldId = WorldId,
                                        Message = targetSession.Character.GenerateSay(
                                            string.Format(
                                                Language.Instance.GetMessageFromKey("MESSAGE_SENT_TO_CHARACTER"),
                                                targetSession.Character.Name, ChannelId), 11),
                                        Type = MessageType.PrivateChat
                                    });
                                }
                                targetSession.SendPacket($"{message.Message} <{Language.Instance.GetMessageFromKey("CHANNEL")}: {CommunicationServiceClient.Instance.GetChannelIdByWorldId(message.SourceWorldId)}>");
                            }
                            else
                            {
                                targetSession.SendPacket(message.Message);
                            }
                        }
                        break;

                    case MessageType.Shout:
                        Shout(message.Message);
                        break;

                    case MessageType.PrivateChat:
                        targetSession?.SendPacket(message.Message);
                        break;

                    case MessageType.FamilyChat:
                        if (message.DestinationCharacterId.HasValue && message.SourceWorldId != WorldId)
                        {
                            Parallel.ForEach(Instance.Sessions, session =>
                            {
                                if (session.HasSelectedCharacter && session.Character.Family != null && session.Character.Family.FamilyId == message.DestinationCharacterId)
                                {
                                    session.SendPacket($"say 1 0 6 <{Language.Instance.GetMessageFromKey("CHANNEL")}: {CommunicationServiceClient.Instance.GetChannelIdByWorldId(message.SourceWorldId)}>{message.Message}");
                                }
                            });
                        }
                        break;

                    case MessageType.Family:
                        if (message.DestinationCharacterId.HasValue)
                        {
                            Parallel.ForEach(Instance.Sessions, session =>
                            {
                                if (session.HasSelectedCharacter && session.Character.Family != null && session.Character.Family.FamilyId == message.DestinationCharacterId)
                                {
                                    session.SendPacket(message.Message);
                                }
                            });
                        }
                        break;
                }
            }
        }

        private void OnPenaltyLogRefresh(object sender, EventArgs e)
        {
            int relId = (int)sender;
            PenaltyLogDTO reldto = DAOFactory.PenaltyLogDAO.LoadById(relId);
            PenaltyLogDTO rel = PenaltyLogs.Find(s => s.PenaltyLogId == relId);
            if (reldto != null)
            {
                if (rel != null)
                {
                }
                else
                {
                    PenaltyLogs.Add(reldto);
                }
            }
            else if (rel != null)
            {
                PenaltyLogs.Remove(rel);
            }
        }

        private void OnRelationRefresh(object sender, EventArgs e)
        {
            _inRelationRefreshMode = true;
            long relId = (long)sender;
            lock (CharacterRelations)
            {
                CharacterRelationDTO reldto = DAOFactory.CharacterRelationDAO.LoadById(relId);
                CharacterRelationDTO rel = CharacterRelations.Find(s => s.CharacterRelationId == relId);
                if (reldto != null)
                {
                    if (rel != null)
                    {
                    }
                    else
                    {
                        CharacterRelations.Add(reldto);
                    }
                }
                else if (rel != null)
                {
                    CharacterRelations.Remove(rel);
                }
            }
            _inRelationRefreshMode = false;
        }

        private void OnSessionKicked(object sender, EventArgs e)
        {
            if (sender != null)
            {
                Tuple<long?, long?> kickedSession = (Tuple<long?, long?>)sender;
                if (!kickedSession.Item1.HasValue && !kickedSession.Item2.HasValue)
                {
                    return;
                }
                long? accId = kickedSession.Item1;
                long? sessId = kickedSession.Item2;

                ClientSession targetSession = CharacterScreenSessions.FirstOrDefault(s => s.SessionId == sessId || s.Account.AccountId == accId);
                targetSession?.Disconnect();
                targetSession = Sessions.FirstOrDefault(s => s.SessionId == sessId || s.Account.AccountId == accId);
                targetSession?.Disconnect();
            }
        }

        private static void OnShutdown(object sender, EventArgs e)
        {
            if (Instance.TaskShutdown != null)
            {
                Instance.ShutdownStop = true;
                Instance.TaskShutdown = null;
            }
            else
            {
                Instance.TaskShutdown = Instance.ShutdownTaskAsync();
                Instance.TaskShutdown.Start();
            }
        }

        private static void OnRestart(object sender, EventArgs e)
        {
            if (Instance.TaskShutdown != null)
            {
                Instance.IsReboot = false;
                Instance.ShutdownStop = true;
                Instance.TaskShutdown = null;
            }
            else
            {
                Instance.IsReboot = true;
                Instance.TaskShutdown = Instance.ShutdownTaskAsync();
                Instance.TaskShutdown.Start();
            }
        }

        private void RemoveItemProcess()
        {
            try
            {
                Parallel.ForEach(Sessions.Where(c => c.IsConnected), session => session.Character?.RefreshValidity());
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private static void ReviveTask(ClientSession session)
        {
            Task.Factory.StartNew(async () =>
            {
                bool revive = true;
                for (int i = 1; i <= 30; i++)
                {
                    await Task.Delay(1000).ConfigureAwait(false);
                    if (session.Character.Hp > 0)
                    {
                        revive = false;
                        break;
                    }
                }
                if (revive)
                {
                    Instance.ReviveFirstPosition(session.Character.CharacterId);
                }
            });
        }

        // Server
        private void SaveAllProcess()
        {
            try
            {
                Logger.Info(Language.Instance.GetMessageFromKey("SAVING_ALL"));
                SaveAll();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        #endregion
    }
}