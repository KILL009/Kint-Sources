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
using OpenNos.GameObject.Battle;
using OpenNos.GameObject.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Packets.ServerPackets;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;
using OpenNos.PathFinder;
using OpenNos.XMLModel.Models.Quest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using static OpenNos.Domain.BCardType;
using OpenNos.Core.ConcurrencyExtensions;
using OpenNos.GameObject.Networking;
using OpenNos.Core.Extensions;
using System.Diagnostics.CodeAnalysis;


namespace OpenNos.GameObject
{
    public class Character : CharacterDTO
    {
        #region Members

        private readonly object _syncObj = new object();

        private bool _isStaticBuffListInitial;

        private Random _random;
      
        private int _slhpbonus;

        private byte _speed;

        #endregion

        #region Instantiation

        public Character()
        {
            GroupSentRequestCharacterIds = new ThreadSafeGenericList<long>();
            FamilyInviteCharacters = new ThreadSafeGenericList<long>();
            TradeRequests = new ThreadSafeGenericList<long>();
            FriendRequestCharacters = new ThreadSafeGenericList<long>();
            StaticBonusList = new List<StaticBonusDTO>();
            MinilandObjects = new List<MinilandObject>();
            Mates = new List<Mate>();
            LastMonsterAggro = DateTime.Now;
            LastPulse = DateTime.Now;
            MTListTargetQueue = new ConcurrentStack<MTListHitTarget>();
            EquipmentBCards = new ThreadSafeGenericList<BCard>();
            MeditationDictionary = new Dictionary<short, DateTime>();
            Buff = new ThreadSafeSortedList<short, Buff>();
            BuffObservables = new ThreadSafeSortedList<short, IDisposable>();
            CellonOptions = new ThreadSafeGenericList<CellonOptionDTO>();
            PVELockObject = new object();
            ShellEffectArmor = new ConcurrentBag<ShellEffectDTO>();
            ShellEffectMain = new ConcurrentBag<ShellEffectDTO>();
            ShellEffectSecondary = new ConcurrentBag<ShellEffectDTO>();
            ObservableBag = new Dictionary<short, IDisposable>();
        }
        
        public Character(CharacterDTO input)
        {
            AccountId = input.AccountId;
            Act4Dead = input.Act4Dead;
            Act4Kill = input.Act4Kill;
            Act4Points = input.Act4Points;
            ArenaWinner = input.ArenaWinner;
            Biography = input.Biography;
            BuffBlocked = input.BuffBlocked;
            CharacterId = input.CharacterId;
            Class = input.Class;
            Compliment = input.Compliment;
            Dignity = input.Dignity;
            EmoticonsBlocked = input.EmoticonsBlocked;
            ExchangeBlocked = input.ExchangeBlocked;
            Faction = input.Faction;
            FamilyRequestBlocked = input.FamilyRequestBlocked;
            FriendRequestBlocked = input.FriendRequestBlocked;
            Gender = input.Gender;
            Gold = input.Gold;
            GoldBank = input.GoldBank;
            GroupRequestBlocked = input.GroupRequestBlocked;
            HairColor = input.HairColor;
            HairStyle = input.HairStyle;
            HeroChatBlocked = input.HeroChatBlocked;
            HeroLevel = input.HeroLevel;
            HeroXp = input.HeroXp;
            Hp = input.Hp;
            HpBlocked = input.HpBlocked;
            JobLevel = input.JobLevel;
            JobLevelXp = input.JobLevelXp;
            LastFamilyLeave = input.LastFamilyLeave;
            Level = input.Level;
            LevelXp = input.LevelXp;
            MapId = input.MapId;
            MapX = input.MapX;
            MapY = input.MapY;
            MasterPoints = input.MasterPoints;
            MasterTicket = input.MasterTicket;
            MaxMateCount = input.MaxMateCount;
            MinilandInviteBlocked = input.MinilandInviteBlocked;
            MinilandMessage = input.MinilandMessage;
            MinilandPoint = input.MinilandPoint;
            MinilandState = input.MinilandState;
            MouseAimLock = input.MouseAimLock;
            Mp = input.Mp;
            Name = input.Name;
            QuickGetUp = input.QuickGetUp;
            RagePoint = input.RagePoint;
            Reputation = input.Reputation;
            Slot = input.Slot;
            SpAdditionPoint = input.SpAdditionPoint;
            SpPoint = input.SpPoint;
            State = input.State;
            TalentLose = input.TalentLose;
            TalentSurrender = input.TalentSurrender;
            TalentWin = input.TalentWin;
            WhisperBlocked = input.WhisperBlocked;
            GroupSentRequestCharacterIds = new ThreadSafeGenericList<long>();
            FamilyInviteCharacters = new ThreadSafeGenericList<long>();
            TradeRequests = new ThreadSafeGenericList<long>();
            FriendRequestCharacters = new ThreadSafeGenericList<long>();
            StaticBonusList = new List<StaticBonusDTO>();
            MinilandObjects = new List<MinilandObject>();
            Mates = new List<Mate>();
            LastMonsterAggro = DateTime.Now;
            LastPulse = DateTime.Now;
            MTListTargetQueue = new ConcurrentStack<MTListHitTarget>();
            EquipmentBCards = new ThreadSafeGenericList<BCard>();
            MeditationDictionary = new Dictionary<short, DateTime>();
            Buff = new ThreadSafeSortedList<short, Buff>();
            BuffObservables = new ThreadSafeSortedList<short, IDisposable>();
            CellonOptions = new ThreadSafeGenericList<CellonOptionDTO>();
            PVELockObject = new object();
            ShellEffectArmor = new ConcurrentBag<ShellEffectDTO>();
            ShellEffectMain = new ConcurrentBag<ShellEffectDTO>();
            ShellEffectSecondary = new ConcurrentBag<ShellEffectDTO>();
        }

        public void ChangeClassPrestige(ClassType adventurer)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Properties
        public byte ArenaWinnerTemp { get; set; }

        public AuthorityType Authority { get; set; }

        public Node[][] BrushFireJagged { get; set; }

        public ThreadSafeSortedList<short, Buff> Buff { get; internal set; }

        public ThreadSafeSortedList<short, IDisposable> BuffObservables { get; internal set; }

        public bool CanFight => !IsSitting && ExchangeInfo == null;

        public ThreadSafeGenericList<CellonOptionDTO> CellonOptions { get; set; }

        public List<CharacterRelationDTO> CharacterRelations
        {
            get
            {
                lock (ServerManager.Instance.CharacterRelations)
                {
                    return ServerManager.Instance.CharacterRelations == null ? new List<CharacterRelationDTO>() : ServerManager.Instance.CharacterRelations.Where(s => s.CharacterId == CharacterId || s.RelatedCharacterId == CharacterId).ToList();
                }
            }
        }

        public short CurrentMinigame { get; set; }

        public int DarkResistance { get; set; }

        public int Defence { get; set; }

        public int DefenceRate { get; set; }

        public int Direction { get; set; }

        public int DistanceCritical { get; set; }

        public int DistanceCriticalRate { get; set; }

        public int DistanceDefence { get; set; }

        public int DistanceDefenceRate { get; set; }

        public int DistanceRate { get; set; }

        public byte Element { get; set; }

        public int ElementRate { get; set; }

        public int ElementRateSP { get; private set; }

        public ThreadSafeGenericList<BCard> EquipmentBCards { get; set; }

        public ExchangeInfo ExchangeInfo { get; set; }

        public Family Family { get; set; }

        public FamilyCharacterDTO FamilyCharacter => Family?.FamilyCharacters.Find(s => s.CharacterId == CharacterId);

        public ThreadSafeGenericList<long> FamilyInviteCharacters { get; set; }

        public int FireResistance { get; set; }

        public int FoodAmount { get; set; }

        public int FoodHp { get; set; }

        public int FoodMp { get; set; }

        public ThreadSafeGenericList<long> FriendRequestCharacters { get; set; }

        public ThreadSafeGenericList<GeneralLogDTO> GeneralLogs { get; set; }

        public bool GmPvtBlock { get; set; }

        public Group Group { get; set; }

        public ThreadSafeGenericList<long> GroupSentRequestCharacterIds { get; set; }

        public bool HasGodMode { get; set; }

        public bool HasShopOpened { get; set; }

        public int HitCritical { get; set; }

        public int HitCriticalRate { get; set; }

        public int HitRate { get; set; }

        public int HPMax { get; set; }

        public List<BCard> StaticBcards { get; set; }

        public bool InExchangeOrTrade => ExchangeInfo != null || Speed == 0;

        public Inventory Inventory { get; set; }

        public bool Invisible { get; set; }

        public void DisableBuffs(List<BuffType> types, int level = 100) => BattleEntity.DisableBuffs(types, level);

        public bool InvisibleGm { get; set; }

        public bool IsChangingMapInstance { get; set; }

        public bool IsCustomSpeed { get; set; }

        public bool IsDancing { get; set; }

        public bool isAbsorbing { get; set; }

        public int damageAb { get; set; }

        /// <summary>
        /// Defines if the Character Is currently sending or getting items thru exchange.
        /// </summary>
        public bool IsExchanging { get; set; }

        public bool IsShopping { get; set; }

        public bool IsSitting { get; set; }

        public bool IsUsingFairyBooster => _isStaticBuffListInitial ? Buff.ContainsKey(131) : DAOFactory.StaticBuffDAO.LoadByCharacterId(CharacterId).Any(s => s.CardId.Equals(131));

        public bool IsVehicled { get; set; }

        public bool IsWaitingForEvent { get; set; }

        public bool IsWaitingForEventSheep { get; set; }

        public DateTime LastDefence { get; set; }

        public DateTime LastDelay { get; set; }

        public DateTime LastEffect { get; set; }

        public DateTime LastHealth { get; set; }

        public short LastItemVNum { get; set; }

        public DateTime LastMapObject { get; set; }

        public DateTime LastMonsterAggro { get; set; }

        public DateTime LastMove { get; set; }

        public int LastNpcMonsterId { get; set; }

        public int LastNRunId { get; set; }

        public DateTime LastPermBuffRefresh { get; set; }

        public double LastPortal { get; set; }

        public DateTime LastPotion { get; set; }

        public DateTime LastPulse { get; set; }

        public int ChargeValue { get; set; }

        public DateTime LastPVPRevive { get; set; }

        public DateTime LastSkillComboUse { get; set; }

        public DateTime LastSkillUse { get; set; }

        public double LastSp { get; set; }

        public DateTime LastSpeedChange { get; set; }

        public int BuffRandomTime { get; set; }

        public DateTime LastSpGaugeRemove { get; set; }

        public DateTime LastTransform { get; set; }

        public DateTime LastVessel { get; set; }

        public int LightResistance { get; set; }

        public int MagicalDefence { get; set; }

        public IDictionary<int, MailDTO> MailList { get; set; }
        public BattleEntity BattleEntity { get; private set; }

        public MapInstance MapInstance => ServerManager.GetMapInstance(MapInstanceId);

        public Guid MapInstanceId { get; set; }

        public List<Mate> Mates { get; set; }

        public int MaxDistance { get; set; }

        public int MaxFood { get; set; }

        public int MaxHit { get; set; }

        public int MaxSnack { get; set; }

        public Dictionary<short, DateTime> MeditationDictionary { get; set; }

        public int MessageCounter { get; set; }

        public int MinDistance { get; set; }

        public int MinHit { get; set; }

        public MinigameLogDTO MinigameLog { get; set; }

        public MapInstance Miniland { get; private set; }

        public List<MinilandObject> MinilandObjects { get; set; }

        public int Morph { get; set; }

        public int MorphUpgrade { get; set; }

        public int MorphUpgrade2 { get; set; }

        public int MPMax { get; set; }

        public ConcurrentStack<MTListHitTarget> MTListTargetQueue { get; set; }

        public bool NoAttack { get; set; }

        public bool NoMove { get; set; }

        public short PositionX { get; set; }

        public short PositionY { get; set; }

        public object PVELockObject { get; set; }

        public ThreadSafeSortedList<long, QuestModel> Quests { get; internal set; }

        public List<QuicklistEntryDTO> QuicklistEntries { get; private set; }

        public RespawnMapTypeDTO Respawn
        {
            get
            {
                RespawnMapTypeDTO respawn = new RespawnMapTypeDTO
                {
                    DefaultX = 79,
                    DefaultY = 116,
                    DefaultMapId = 1,
                    RespawnMapTypeId = -1
                };

                if (Session.HasCurrentMapInstance && Session.CurrentMapInstance.Map.MapTypes.Count > 0)
                {
                    long? respawnmaptype = Session.CurrentMapInstance.Map.MapTypes[0].RespawnMapTypeId;
                    if (respawnmaptype != null)
                    {
                        RespawnDTO resp = Respawns.Find(s => s.RespawnMapTypeId == respawnmaptype);
                        if (resp == null)
                        {
                            RespawnMapTypeDTO defaultresp = Session.CurrentMapInstance.Map.DefaultRespawn;
                            if (defaultresp != null)
                            {
                                respawn.DefaultX = defaultresp.DefaultX;
                                respawn.DefaultY = defaultresp.DefaultY;
                                respawn.DefaultMapId = defaultresp.DefaultMapId;
                                respawn.RespawnMapTypeId = (long)respawnmaptype;
                            }
                        }
                        else
                        {
                            respawn.DefaultX = resp.X;
                            respawn.DefaultY = resp.Y;
                            respawn.DefaultMapId = resp.MapId;
                            respawn.RespawnMapTypeId = (long)respawnmaptype;
                        }
                    }
                }
                return respawn;
            }
        }

        public List<RespawnDTO> Respawns { get; set; }

        public RespawnMapTypeDTO Return
        {
            get
            {
                RespawnMapTypeDTO respawn = new RespawnMapTypeDTO();
                if (Session.HasCurrentMapInstance && Session.CurrentMapInstance.Map.MapTypes.Count > 0)
                {
                    long? respawnmaptype = Session.CurrentMapInstance.Map.MapTypes[0].ReturnMapTypeId;
                    if (respawnmaptype != null)
                    {
                        RespawnDTO resp = Respawns.Find(s => s.RespawnMapTypeId == respawnmaptype);
                        if (resp == null)
                        {
                            RespawnMapTypeDTO defaultresp = Session.CurrentMapInstance.Map.DefaultReturn;
                            if (defaultresp != null)
                            {
                                respawn.DefaultX = defaultresp.DefaultX;
                                respawn.DefaultY = defaultresp.DefaultY;
                                respawn.DefaultMapId = defaultresp.DefaultMapId;
                                respawn.RespawnMapTypeId = (long)respawnmaptype;
                            }
                        }
                        else
                        {
                            respawn.DefaultX = resp.X;
                            respawn.DefaultY = resp.Y;
                            respawn.DefaultMapId = resp.MapId;
                            respawn.RespawnMapTypeId = (long)respawnmaptype;
                        }
                    }
                }
                return respawn;
            }
        }

        public short SaveX { get; set; }

        public short SaveY { get; set; }

        public int ScPage { get; set; }

        public ClientSession Session { get; private set; }

        public ConcurrentBag<ShellEffectDTO> ShellEffectArmor { get; set; }

        public ConcurrentBag<ShellEffectDTO> ShellEffectMain { get; set; }

        public ConcurrentBag<ShellEffectDTO> ShellEffectSecondary { get; set; }

        public Dictionary<short, IDisposable> ObservableBag { get; set; }

        public int Size { get; set; } = 10;

        public byte SkillComboCount { get; set; }

        public ThreadSafeSortedList<int, CharacterSkill> Skills { get; private set; }

        public ThreadSafeSortedList<int, CharacterSkill> SkillsSp { get; set; }

        public int SnackAmount { get; set; }

        public int SnackHp { get; set; }

        public int SnackMp { get; set; }

        public int SpCooldown { get; set; }

        public byte Speed
        {
            get
            {
                //    if (HasBuff(CardType.Move, (byte)AdditionalTypes.Move.MovementImpossible))
                //    {
                //        return 0;
                //    }

                byte bonusSpeed = 0;/*(byte)GetBuff(CardType.Move, (byte)AdditionalTypes.Move.SetMovementNegated)[0];*/
                if (_speed + bonusSpeed > 59)
                {
                    return 59;
                }
                return (byte)(_speed + bonusSpeed);
            }

            set
            {
                LastSpeedChange = DateTime.Now;
                _speed = value > 59 ? (byte)59 : value;
            }
        }

        public List<StaticBonusDTO> StaticBonusList { get; set; }

        public ScriptedInstance Timespace { get; set; }

        public int TimesUsed { get; set; }

        public ThreadSafeGenericList<long> TradeRequests { get; set; }

        public bool Undercover { get; set; }

        public bool UseSp { get; set; }

        public byte VehicleSpeed { private get; set; }

        public int WareHouseSize { get; set; }

        public int WaterResistance { get; set; }

        public long LastTargetType { get; set; }

        public long LastTargetId { get; set; }

        public bool IsWaitingForGift { get; set; }

        public int Point { get; set; }

        public int Point2 { get; set; }

        public int Point3 { get; set; }

        public bool CanAttack { get; set; }
        public int Prestige { get; set; }
        public bool isCommand { get; private set; }
        public IDisposable DragonModeObservable { get;  set; }
        public int RetainedHp { get; internal set; }
        public int AccumulatedDamage { get;  set; }
        public IDisposable DotDebuff { get;  set; }


        #endregion

        #region Methods

        public void AddBuff(Buff indicator, bool noMessage = false)
        {
            int buffTime = 0;

            if (indicator.Card != null && (!noMessage || !Buff.Any(s => s.Card.CardId == indicator.Card.CardId)))
            {
                Buff.Remove(indicator.Card.CardId);
                Buff[indicator.Card.CardId] = indicator;
                //TODO: Find a better way to do this                             
                indicator.RemainingTime = indicator.Card.Duration == 0 ? buffTime : indicator.Card.Duration;
                indicator.Start = DateTime.Now;

                Session.SendPacket($"bf 1 {CharacterId} 0.{indicator.Card.CardId}.{indicator.RemainingTime} {Level}");
                Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("UNDER_EFFECT"), indicator.Card.Name), 20));

                indicator.Card.BCards.ForEach(c => c.ApplyBCards(this));
                if (BuffObservables.ContainsKey(indicator.Card.CardId))
                {
                    BuffObservables[indicator.Card.CardId]?.Dispose();
                    BuffObservables.Remove(indicator.Card.CardId);
                }
                BuffObservables[indicator.Card.CardId] = Observable.Timer(TimeSpan.FromMilliseconds(indicator.Card.Duration * 100)).Subscribe(o =>
              {
                  RemoveBuff(indicator.Card.CardId);
                  if (indicator.Card.TimeoutBuff != 0 && ServerManager.RandomNumber() < indicator.Card.TimeoutBuffChance)
                  {
                      AddBuff(new Buff(indicator.Card.TimeoutBuff, Level));
                  }
              });
                if (indicator.Card.BCards.Any(s => s.Type == (byte)CardType.Move && !s.SubType.Equals((byte)AdditionalTypes.Move.MovementImpossible / 10)))
                {
                    LastSpeedChange = DateTime.Now;
                    LoadSpeed();
                    Session.SendPacket(GenerateCond());
                }
                if (indicator.Card.BCards.Any(s => s.Type == (byte)CardType.SpecialAttack && s.SubType.Equals((byte)AdditionalTypes.SpecialAttack.NoAttack / 10)))
                {
                    NoAttack = true;
                    Session.SendPacket(GenerateCond());
                }
                if (indicator.Card.BCards.Any(s => s.Type == (byte)CardType.Move && s.SubType.Equals((byte)AdditionalTypes.Move.MovementImpossible / 10)))
                {
                    NoMove = true;
                    Session.SendPacket(GenerateCond());
                }

                if (indicator.Card.BCards.Any(s => s.Type == (byte)CardType.SpecialActions && s.SubType.Equals((byte)AdditionalTypes.SpecialActions.Hide / 10)))
                {
                    Invisible = true;
                    Session.CurrentMapInstance?.Broadcast(GenerateInvisible());
                    Session.SendPacket(GenerateEq());
                    Mates.Where(m => m.IsTeamMember).ToList().ForEach(m =>
                    Session.CurrentMapInstance?.Broadcast(m.GenerateIn(), ReceiverType.AllExceptMe));
                    Session.CurrentMapInstance?.Broadcast(Session, GenerateIn(),
                    ReceiverType.AllExceptMe);
                    Session.CurrentMapInstance?.Broadcast(Session, GenerateGidx(),
                        ReceiverType.AllExceptMe);

                    if (indicator.Card.BCards.Any(s => s.Type == (byte)CardType.FalconSkill && s.SubType == ((byte)AdditionalTypes.FalconSkill.Ambush / 10)))
                    {
                        Session.Character.Invisible = true; Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateInvisible());
                        Session.SendPacket(Session.Character.GenerateEq());

                        Session.Character.Mates.Where(m => m.IsTeamMember).ToList().ForEach(m =>
                        Session.CurrentMapInstance?.Broadcast(m.GenerateIn(), ReceiverType.AllExceptMe));
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateIn(),
                            ReceiverType.AllExceptMe);
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateGidx(),
                            ReceiverType.AllExceptMe);

                    }
                }
            }
        }

        public bool AddPet(Mate mate)
        {
            if (mate.MateType == MateType.Pet ? MaxMateCount > Mates.Count : 3 > Mates.Count(s => s.MateType == MateType.Partner))
            {
                Mates.Add(mate);
                MapInstance.Broadcast(mate.GenerateIn());
                Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("YOU_GET_PET"), mate.Name), 12));
                Session.SendPacket(UserInterfaceHelper.GeneratePClear());
                Session.SendPackets(GenerateScP());
                Session.SendPackets(GenerateScN());
                return true;
            }
            return false;
        }

        public void AddPetWithSkill(Mate mate)
        {
            bool isUsingMate = true;
            if (!Mates.Any(s => s.IsTeamMember && s.MateType == mate.MateType))
            {
                isUsingMate = false;
                mate.IsTeamMember = true;
            }
            else
            {
                //set position to mate because mate will send to miniland
                mate.MapX = 5;
                mate.MapY = 8;
            }
            Session.SendPacket($"ctl 2 {mate.MateTransportId} 3");
            Mates.Add(mate);
            Session.SendPacket(UserInterfaceHelper.GeneratePClear());
            Session.SendPackets(GenerateScP());
            Session.SendPackets(GenerateScN());
            if (!isUsingMate)
            {
                MapInstance.Broadcast(mate.GenerateIn());
                Session.SendPacket(GeneratePinit());
                Session.SendPacket(UserInterfaceHelper.GeneratePClear());
                Session.SendPackets(GenerateScP());
                Session.SendPackets(GenerateScN());
                Session.SendPackets(GeneratePst());
            }
        }

        public void AddRelation(long characterId, CharacterRelationType Relation)
        {
            CharacterRelationDTO addRelation = new CharacterRelationDTO
            {
                CharacterId = CharacterId,
                RelatedCharacterId = characterId,
                RelationType = Relation
            };

            DAOFactory.CharacterRelationDAO.InsertOrUpdate(ref addRelation);
            ServerManager.Instance.RelationRefresh(addRelation.CharacterRelationId);
            Session.SendPacket(GenerateFinit());
            ClientSession target = ServerManager.Instance.Sessions.FirstOrDefault(s => s.Character?.CharacterId == characterId);
            target?.SendPacket(target?.Character.GenerateFinit());
        }

        internal void PushBackToDirection(int v)
        {
            throw new NotImplementedException();
        }

        public void AddStaticBuff(StaticBuffDTO staticBuff)
        {
            Buff bf = new Buff(staticBuff.CardId, Level)
            {
                Start = DateTime.Now,
                StaticBuff = true
            };
            Buff oldbuff = Buff[staticBuff.CardId];
            if (staticBuff.RemainingTime > 0)
            {
                bf.RemainingTime = staticBuff.RemainingTime;
                Buff[staticBuff.CardId] = bf;
            }
            else if (oldbuff != null)
            {
                Buff.Remove(bf.Card.CardId);
                int time = (int)((oldbuff.Start.AddSeconds(oldbuff.Card.Duration * 6 / 10) - DateTime.Now).TotalSeconds / 10 * 6);
                bf.RemainingTime = (bf.Card.Duration * 6 / 10) + (time > 0 ? time : 0);
                Buff[bf.Card.CardId] = bf;
            }
            else
            {
                bf.RemainingTime = bf.Card.Duration * 6 / 10;
                Buff[bf.Card.CardId] = bf;
            }
            bf.Card.BCards.ForEach(c => c.ApplyBCards(this));
            if (BuffObservables.ContainsKey(bf.Card.CardId))
            {
                BuffObservables[bf.Card.CardId].Dispose();
                BuffObservables.Remove(bf.Card.CardId);
            }
            BuffObservables[bf.Card.CardId] = Observable.Timer(TimeSpan.FromSeconds(bf.RemainingTime)).Subscribe(o =>
            {
                RemoveBuff(bf.Card.CardId);
                if (bf.Card.TimeoutBuff != 0 && ServerManager.RandomNumber() < bf.Card.TimeoutBuffChance)
                {
                    AddBuff(new Buff(bf.Card.TimeoutBuff, Level));
                }
            });
            if (!_isStaticBuffListInitial)
            {
                _isStaticBuffListInitial = true;
            }

            Session.SendPacket($"vb {bf.Card.CardId} 1 {bf.RemainingTime * 10}");
            Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("UNDER_EFFECT"), bf.Card.Name), 12));
        }

        internal void TeleportInRadius(int firstData)
        {
            throw new NotImplementedException();
        }

        public bool CanAddMate(Mate mate) => mate.MateType == MateType.Pet ? MaxMateCount > Mates.Count : 3 > Mates.Count(s => s.MateType == MateType.Partner);

        public void ChangeChannel(string ip, int port, byte mode)
        {
            Session.SendPacket($"mz {ip} {port} {Slot}");
            Session.SendPacket($"it {mode}");
            Session.IsDisposing = true;
            CommunicationServiceClient.Instance.RegisterCrossServerAccountLogin(Session.Account.AccountId, Session.SessionId);

            //explictly save data before disconnecting to prevent data loss
            Save();

            Session.Disconnect();
        }

        public void ChangeClass(ClassType characterClass)
        {
            JobLevel = (byte)(this.isCommand && characterClass != ClassType.Adventurer ? JobLevel : isCommand && characterClass == ClassType.Adventurer ? 20 : 1);
            JobLevelXp = 0;
            Session.SendPacket("npinfo 0");
            Session.SendPacket(UserInterfaceHelper.GeneratePClear());

            if (characterClass == (byte)ClassType.Adventurer)
            {
                HairStyle = (byte)HairStyle > 1 ? 0 : HairStyle;
            }
            LoadSpeed();
            Class = characterClass;
            Hp = (int)HPLoad();
            Mp = (int)MPLoad();
            Session.SendPacket(GenerateTit());
            Session.SendPacket(GenerateStat());
            Session.CurrentMapInstance?.Broadcast(Session, GenerateEq());
            Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 8), PositionX, PositionY);
            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CLASS_CHANGED"), 0));
            Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 4794), PositionX, PositionY);
            int faction = 1 + ServerManager.RandomNumber(0, 2);
            Faction = (FactionType)faction;
            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey($"GET_PROTECTION_POWER_{faction}"), 0));
            Session.SendPacket("scr 0 0 0 0 0 0");
            Session.SendPacket(GenerateFaction());
            Session.SendPacket(GenerateStatChar());
            Session.SendPacket(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 4793 + faction));
            Session.SendPacket(GenerateCond());
            Session.SendPacket(GenerateLev());
            Session.CurrentMapInstance?.Broadcast(Session, GenerateCMode());
            Session.CurrentMapInstance?.Broadcast(Session, GenerateIn(), ReceiverType.AllExceptMe);
            Session.CurrentMapInstance?.Broadcast(Session, GenerateGidx(), ReceiverType.AllExceptMe);
            Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 6), PositionX, PositionY);
            Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 23), PositionX, PositionY);
            foreach (CharacterSkill skill in Skills.GetAllItems())
            {
                if (skill.SkillVNum >= 200)
                {
                    Skills.Remove(skill.SkillVNum);
                }
            }

            Skills[(short)(200 + (20 * (byte)Class))] = new CharacterSkill { SkillVNum = (short)(200 + (20 * (byte)Class)), CharacterId = CharacterId };
            Skills[(short)(201 + (20 * (byte)Class))] = new CharacterSkill { SkillVNum = (short)(201 + (20 * (byte)Class)), CharacterId = CharacterId };
            Skills[236] = new CharacterSkill { SkillVNum = 236, CharacterId = CharacterId };

            Session.SendPacket(GenerateSki());

            foreach (QuicklistEntryDTO quicklists in DAOFactory.QuicklistEntryDAO.LoadByCharacterId(CharacterId).Where(quicklists => QuicklistEntries.Any(qle => qle.Id == quicklists.Id)))
            {
                DAOFactory.QuicklistEntryDAO.Delete(quicklists.Id);
            }

            QuicklistEntries = new List<QuicklistEntryDTO>
            {
                new QuicklistEntryDTO
                {
                    CharacterId = CharacterId,
                    Q1 = 0,
                    Q2 = 9,
                    Type = 1,
                    Slot = 3,
                    Pos = 1
                }
            };
            if (ServerManager.Instance.Groups.Any(s => s.IsMemberOfGroup(Session) && s.GroupType == GroupType.Group))
            {
                Session.CurrentMapInstance?.Broadcast(Session, $"pidx 1 1.{CharacterId}", ReceiverType.AllExceptMe);
            }
        }
       
        public void ChangeSex()
        {
            Gender = Gender == GenderType.Female ? GenderType.Male : GenderType.Female;
            if (IsVehicled)
            {
                Morph = Gender == GenderType.Female ? Morph + 1 : Morph - 1;
            }
            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SEX_CHANGED"), 0));
            Session.SendPacket(GenerateEq());
            Session.SendPacket(GenerateGender());
            Session.CurrentMapInstance?.Broadcast(Session, GenerateIn(), ReceiverType.AllExceptMe);
            Session.CurrentMapInstance?.Broadcast(Session, GenerateGidx(), ReceiverType.AllExceptMe);
            Session.CurrentMapInstance?.Broadcast(GenerateCMode());
            Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 4790), PositionX, PositionY);
        }

        public static void UpdateQuests()
        {

        }

        
        public void GetReput(long val)
        {
            Reputation += val * ServerManager.Instance.ReputRate;
            Session.SendPacket(GenerateFd());
            Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("REPUT_INCREASE"), val), 11));
        }

        public void CharacterLife()
        {
            //foreach (QuestModel quest in Quests.GetAllItems().Where(q => q.WalkObjective != null))
            //{
            //    if (Session.CurrentMapInstance.Map.MapId == quest.WalkObjective.MapId && IsInRange(quest.WalkObjective.MapX, quest.WalkObjective.MapY, 3))
            //    {
            //        Quests.Remove(quest);
            //        if (quest.Reward.QuestId != -1)
            //        {
            //            Quests[quest.Reward.QuestId] = ServerManager.Instance.QuestList[quest.Reward.QuestId].Copy();
            //            UpdateQuests();
            //        }
            //    }
            //}

            int x = 1;
            bool change = false;
            if (Hp == 0 && LastHealth.AddSeconds(2) <= DateTime.Now)
            {
                Mp = 0;
                Session.SendPacket(GenerateStat());
                LastHealth = DateTime.Now;
            }
            else
            {
                if (CurrentMinigame != 0 && LastEffect.AddSeconds(3) <= DateTime.Now)
                {
                    Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, CurrentMinigame));
                    LastEffect = DateTime.Now;
                }
                if (LastEffect.AddMilliseconds(400) <= DateTime.Now && MessageCounter > 0)
                {
                    MessageCounter--;
                }

                if (LastEffect.AddSeconds(5) <= DateTime.Now)
                {
                    if (Session.CurrentMapInstance?.MapInstanceType == MapInstanceType.RaidInstance)
                    {
                        Session.SendPacket(GenerateRaid(3));
                    }

                    ItemInstance ring = Inventory.LoadBySlotAndType((byte)EquipmentType.Ring, InventoryType.Wear);
                    ItemInstance bracelet = Inventory.LoadBySlotAndType((byte)EquipmentType.Bracelet, InventoryType.Wear);
                    ItemInstance necklace = Inventory.LoadBySlotAndType((byte)EquipmentType.Necklace, InventoryType.Wear);
                    CellonOptions.Clear();
                    if (ring != null)
                    {
                        CellonOptions.AddRange(ring.CellonOptions);
                    }
                    if (bracelet != null)
                    {
                        CellonOptions.AddRange(bracelet.CellonOptions);
                    }
                    if (necklace != null)
                    {
                        CellonOptions.AddRange(necklace.CellonOptions);
                    }

                    ItemInstance amulet = Inventory.LoadBySlotAndType((byte)EquipmentType.Amulet, InventoryType.Wear);
                    if (amulet != null)
                    {
                        if (amulet.ItemVNum == 4503 || amulet.ItemVNum == 4504)
                        {
                            Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, amulet.Item.EffectValue + (Class == ClassType.Adventurer ? 0 : (byte)Class - 1)), PositionX, PositionY);
                        }
                        else
                        {
                            Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, amulet.Item.EffectValue), PositionX, PositionY);
                        }
                    }
                    if (Group != null && (Group.GroupType == GroupType.Team || Group.GroupType == GroupType.BigTeam || Group.GroupType == GroupType.GiantTeam))
                    {
                        try
                        {
                            Session.CurrentMapInstance?.Broadcast(Session, StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 801 + (Group.IsLeader(Session) ? 1 : 0)), ReceiverType.AllExceptGroup);
                            Session.CurrentMapInstance?.Broadcast(Session, StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 832 + (Group.IsLeader(Session) ? 1 : 0)), ReceiverType.Group);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }
                    }
                    Mates.Where(s => s.CanPickUp).ToList().ForEach(s => Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, s.MateTransportId, 4725)));
                    LastEffect = DateTime.Now;
                }

                if (LastHealth.AddSeconds(2) <= DateTime.Now || (IsSitting && LastHealth.AddSeconds(1.5) <= DateTime.Now))
                {
                    LastHealth = DateTime.Now;
                    if (Session.HealthStop)
                    {
                        Session.HealthStop = false;
                        return;
                    }

                    // HEAL
                    if (LastHealth.AddSeconds(2) <= DateTime.Now)
                    {
                        var heal = GetBuff(CardType.HealingBurningAndCasting, (byte)AdditionalTypes.HealingBurningAndCasting.RestoreHP)[0];
                        Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateRc(heal));
                        if (Hp + heal < HPLoad())
                        {
                            Hp += heal;
                            change = true;
                        }
                        else
                        {
                            if (Hp != (int)HPLoad())
                            {
                                change = true;
                            }

                            Hp = (int)HPLoad();
                        }

                        if (change)
                        {
                            Session.SendPacket(GenerateStat());
                        }
                    }

                    // DEBUFF HP LOSS
                    if (LastHealth.AddSeconds(2) <= DateTime.Now)
                    {
                        var debuff = (int)(GetBuff(CardType.RecoveryAndDamagePercent, (byte)AdditionalTypes.RecoveryAndDamagePercent.HPReduced)[0] * (HPLoad() / 100));
                        if (Hp - debuff > 1)
                        {
                            Hp -= debuff;
                            change = true;
                        }
                        else
                        {
                            if (Hp != 1)
                            {
                                change = true;
                            }

                            Hp = 1;
                        }

                        if (change)
                        {
                            Session.SendPacket(GenerateStat());
                        }
                    }

                    if (LastDefence.AddSeconds(4) <= DateTime.Now && LastSkillUse.AddSeconds(2) <= DateTime.Now && Hp > 0)
                    {
                        if (x == 0)
                        {
                            x = 1;
                        }
                        if (Hp + HealthHPLoad() < HPLoad())
                        {
                            change = true;
                            Hp += HealthHPLoad();
                        }
                        else
                        {
                            change |= Hp != (int)HPLoad();
                            Hp = (int)HPLoad();
                        }
                        if (x == 1)
                        {
                            if (Mp + HealthMPLoad() < MPLoad())
                            {
                                Mp += HealthMPLoad();
                                change = true;
                            }
                            else
                            {
                                change |= Mp != (int)MPLoad();
                                Mp = (int)MPLoad();
                            }
                        }
                        if (change)
                        {
                            //if (Group != null)
                            //{
                            //    if (Group.Raid == null)
                            //    {
                            //        Group.Characters.ForEach(s => s?.SendPacket(GenerateStat()));
                            //    }
                            //}
                            //else
                            //{
                            Session.SendPacket(GenerateStat());
                           // }
                        }
                    }
                }
                if (MeditationDictionary.Count != 0)
                {
                    if (MeditationDictionary.ContainsKey(534) && MeditationDictionary[534] < DateTime.Now)
                    {
                        Session.SendPacket(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 3465));
                        AddBuff(new Buff(534, Level));
                        if (BuffObservables.ContainsKey(533))
                        {
                            BuffObservables[533].Dispose();
                            BuffObservables.Remove(533);
                        }
                        RemoveBuff(533);
                        MeditationDictionary.Remove(534);
                    }
                    else if (MeditationDictionary.ContainsKey(533) && MeditationDictionary[533] < DateTime.Now)
                    {
                        Session.SendPacket(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 3466));
                        AddBuff(new Buff(533, Level));
                        if (BuffObservables.ContainsKey(532))
                        {
                            BuffObservables[532].Dispose();
                            BuffObservables.Remove(532);
                        }
                        RemoveBuff(532);
                        MeditationDictionary.Remove(533);
                    }
                    else if (MeditationDictionary.ContainsKey(532) && MeditationDictionary[532] < DateTime.Now)
                    {
                        Session.SendPacket(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 3469));
                        AddBuff(new Buff(532, Level));
                        if (BuffObservables.ContainsKey(534))
                        {
                            BuffObservables[534].Dispose();
                            BuffObservables.Remove(534);
                        }
                        RemoveBuff(534);
                        MeditationDictionary.Remove(532);
                    }
                }

                if (SkillComboCount > 0 && LastSkillComboUse.AddSeconds(10) < DateTime.Now)
                {
                    SkillComboCount = 0;
                    Session.SendPackets(GenerateQuicklist());
                    Session.SendPacket("mslot 0 -1");
                }

                if (UseSp)
                {
                    ItemInstance specialist = Inventory.LoadBySlotAndType((byte)EquipmentType.Sp, InventoryType.Wear);
                    if (specialist == null)
                    {
                        return;
                    }
                    if (LastPermBuffRefresh.AddSeconds(2) <= DateTime.Now)
                    {
                        LastPermBuffRefresh = DateTime.Now;
                        switch (specialist.Design)
                        {
                            case 6:
                                AddBuff(new Buff(387, Level), true);
                                break;

                            case 7:
                                AddBuff(new Buff(395, Level), true);
                                break;

                            case 8:
                                AddBuff(new Buff(396, Level), true);
                                break;

                            case 9:
                                AddBuff(new Buff(397, Level), true);
                                break;

                            case 10:
                                AddBuff(new Buff(398, Level), true);
                                break;

                            case 11:
                                AddBuff(new Buff(410, Level), true);
                                break;

                            case 12:
                                AddBuff(new Buff(411, Level), true);
                                break;

                            case 13:
                                AddBuff(new Buff(444, Level), true);
                                break;
                            case 14:
                                AddBuff(new Buff(663, Level), true);
                                break;
                            case 15:
                                AddBuff(new Buff(109, Level), true);
                                break;
                            case 16:
                                AddBuff(new Buff(664, Level), true);
                                break;

                        }
                        Mate m = Mates.Find(s => s.IsTeamMember && s.MateType == MateType.Pet);
                        if (m != null)
                        {
                            switch (m.NpcMonsterVNum)
                            {
                                case 2105:
                                    // Inferno
                                    AddBuff(new Buff(383, m.Level), true);
                                    RemoveBuff(374);
                                    RemoveBuff(381);
                                    RemoveBuff(377);
                                    RemoveBuff(162);
                                    RemoveBuff(385);
                                    RemoveBuff(391);
                                    RemoveBuff(399);
                                    RemoveBuff(442);
                                    break;
                                case 670:
                                    // Fibi Frosty
                                    AddBuff(new Buff(374, m.Level), true);
                                    RemoveBuff(383);
                                    RemoveBuff(381);
                                    RemoveBuff(377);
                                    RemoveBuff(162);
                                    RemoveBuff(385);
                                    RemoveBuff(391);
                                    RemoveBuff(399);
                                    RemoveBuff(442);
                                    break;
                                case 836:
                                    // Fluffy Bally
                                    AddBuff(new Buff(381, m.Level), true);
                                    RemoveBuff(383);
                                    RemoveBuff(374);
                                    RemoveBuff(377);
                                    RemoveBuff(162);
                                    RemoveBuff(385);
                                    RemoveBuff(391);
                                    RemoveBuff(399);
                                    RemoveBuff(442);
                                    break;
                                case 829:
                                    // Rudi Rowdy
                                    AddBuff(new Buff(377, m.Level), true);
                                    RemoveBuff(383);
                                    RemoveBuff(374);
                                    RemoveBuff(381);
                                    RemoveBuff(162);
                                    RemoveBuff(385);
                                    RemoveBuff(391);
                                    RemoveBuff(399);
                                    RemoveBuff(442);
                                    break;
                                case 178:
                                    // New Year Lucky Pig
                                    AddBuff(new Buff(162, m.Level), true);
                                    RemoveBuff(383);
                                    RemoveBuff(374);
                                    RemoveBuff(381);
                                    RemoveBuff(377);
                                    RemoveBuff(385);
                                    RemoveBuff(391);
                                    RemoveBuff(399);
                                    RemoveBuff(442);
                                    break;
                                case 838:
                                    // Navy Bushtail
                                    AddBuff(new Buff(385, m.Level), true);
                                    RemoveBuff(383);
                                    RemoveBuff(374);
                                    RemoveBuff(381);
                                    RemoveBuff(377);
                                    RemoveBuff(162);
                                    RemoveBuff(391);
                                    RemoveBuff(399);
                                    RemoveBuff(442);
                                    break;
                                case 844:
                                    // Cowboy Bushtail
                                    AddBuff(new Buff(391, m.Level), true);
                                    RemoveBuff(383);
                                    RemoveBuff(374);
                                    RemoveBuff(381);
                                    RemoveBuff(377);
                                    RemoveBuff(162);
                                    RemoveBuff(385);
                                    RemoveBuff(399);
                                    RemoveBuff(442);
                                    break;
                                case 842:
                                    // Indian Bushtail
                                    AddBuff(new Buff(399, m.Level), true);
                                    RemoveBuff(383);
                                    RemoveBuff(374);
                                    RemoveBuff(381);
                                    RemoveBuff(377);
                                    RemoveBuff(162);
                                    RemoveBuff(385);
                                    RemoveBuff(391);
                                    RemoveBuff(442);
                                    break;
                                case 840:
                                    // Leo the Coward
                                    AddBuff(new Buff(442, m.Level), true);
                                    RemoveBuff(383);
                                    RemoveBuff(374);
                                    RemoveBuff(381);
                                    RemoveBuff(377);
                                    RemoveBuff(162);
                                    RemoveBuff(385);
                                    RemoveBuff(391);
                                    RemoveBuff(399);
                                    break;
                            }
                        }
                        else
                        {
                            RemoveBuff(383);
                            RemoveBuff(374);
                            RemoveBuff(381);
                            RemoveBuff(377);
                            RemoveBuff(162);
                            RemoveBuff(385);
                            RemoveBuff(391);
                            RemoveBuff(399);
                            RemoveBuff(442);
                        }
                    }
                    if (LastSpGaugeRemove <= new DateTime(0001, 01, 01, 00, 00, 00))
                    {
                        LastSpGaugeRemove = DateTime.Now;
                    }
                    if (LastSkillUse.AddSeconds(15) >= DateTime.Now && LastSpGaugeRemove.AddSeconds(1) <= DateTime.Now)
                    {
                        byte spType = 0;

                        if ((specialist.Item.Morph > 1 && specialist.Item.Morph < 8) || (specialist.Item.Morph > 9 && specialist.Item.Morph < 16))
                        {
                            spType = 3;
                        }
                        else if (specialist.Item.Morph > 16 && specialist.Item.Morph < 29)
                        {
                            spType = 2;
                        }
                        else if (specialist.Item.Morph == 9)
                        {
                            spType = 1;
                        }
                        if (SpPoint >= spType)
                        {
                            SpPoint -= spType;
                        }
                        else if (SpPoint < spType && SpPoint != 0)
                        {
                            spType -= (byte)SpPoint;
                            SpPoint = 0;
                            SpAdditionPoint -= spType;
                        }
                        else if (SpPoint == 0 && SpAdditionPoint >= spType)
                        {
                            SpAdditionPoint -= spType;
                        }
                        else if (SpPoint == 0 && SpAdditionPoint < spType)
                        {
                            SpAdditionPoint = 0;

                            double currentRunningSeconds = (DateTime.Now - Process.GetCurrentProcess().StartTime.AddSeconds(-50)).TotalSeconds;

                            if (UseSp)
                            {
                                LastSp = currentRunningSeconds;
                                if (Session?.HasSession == true)
                                {
                                    if (IsVehicled)
                                    {
                                        return;
                                    }
                                    UseSp = false;
                                    LoadSpeed();
                                    Session.SendPacket(GenerateCond());
                                    Session.SendPacket(GenerateLev());
                                    SpCooldown = 30;
                                    if (SkillsSp != null)
                                    {
                                        foreach (CharacterSkill ski in SkillsSp.Where(s => !s.CanBeUsed()))
                                        {
                                            short time = ski.Skill.Cooldown;
                                            double temp = (ski.LastUse - DateTime.Now).TotalMilliseconds + (time * 100);
                                            temp /= 1000;
                                            SpCooldown = temp > SpCooldown ? (int)temp : SpCooldown;
                                        }
                                    }
                                    Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("STAY_TIME"), SpCooldown), 11));
                                    Session.SendPacket($"sd {SpCooldown}");
                                    Session.CurrentMapInstance?.Broadcast(GenerateCMode());
                                    Session.CurrentMapInstance?.Broadcast(UserInterfaceHelper.GenerateGuri(6, 1, CharacterId), PositionX, PositionY);

                                    // ms_c
                                    Session.SendPacket(GenerateSki());
                                    Session.SendPackets(GenerateQuicklist());
                                    Session.SendPacket(GenerateStat());
                                    Session.SendPacket(GenerateStatChar());

                                    Logger.LogUserEvent("CHARACTER_SPECIALIST_RETURN", Session.GenerateIdentity(), $"SpCooldown: {SpCooldown}");

                                    Observable.Timer(TimeSpan.FromMilliseconds(SpCooldown * 1000)).Subscribe(o =>
                                    {
                                        Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("TRANSFORM_DISAPPEAR"), 11));
                                        Session.SendPacket("sd 0");
                                    });
                                }
                            }
                        }
                        Session.SendPacket(GenerateSpPoint());
                        LastSpGaugeRemove = DateTime.Now;
                    }
                }
            }
        }

        internal string GenerateDm(ushort drain)
        {
            throw new NotImplementedException();
        }

        public string GenerateEff(int v)
        {
            throw new NotImplementedException();
        }

        public void CloseExchangeOrTrade()
        {
            if (InExchangeOrTrade)
            {
                long? targetSessionId = ExchangeInfo?.TargetCharacterId;

                if (targetSessionId.HasValue && Session.HasCurrentMapInstance)
                {
                    ClientSession targetSession = Session.CurrentMapInstance.GetSessionByCharacterId(targetSessionId.Value);

                    if (targetSession == null)
                    {
                        return;
                    }

                    Session.SendPacket("exc_close 0");
                    targetSession.SendPacket("exc_close 0");
                    ExchangeInfo = null;
                    targetSession.Character.ExchangeInfo = null;
                }
            }
        }

        public void CloseShop()
        {
            if (HasShopOpened && Session.HasCurrentMapInstance)
            {
                KeyValuePair<long, MapShop> shop = Session.CurrentMapInstance.UserShops.FirstOrDefault(mapshop => mapshop.Value.OwnerId.Equals(CharacterId));
                if (!shop.Equals(default))
                {
                    Session.CurrentMapInstance.UserShops.Remove(shop.Key);

                    // declare that the shop cannot be closed
                    HasShopOpened = false;

                    Session.CurrentMapInstance?.Broadcast(GenerateShopEnd());
                    Session.CurrentMapInstance?.Broadcast(Session, GeneratePlayerFlag(0), ReceiverType.AllExceptMe);
                    IsSitting = false;
                    IsShopping = false; // close shop by character will always completely close the shop

                    LoadSpeed();
                    Session.SendPacket(GenerateCond());
                    Session.CurrentMapInstance?.Broadcast(GenerateRest());
                }
            }
        }

        public void Dance() => IsDancing = !IsDancing;

        public Character DeepCopy() => (Character)MemberwiseClone();

        public void DeleteBlackList(long characterId)
        {
            CharacterRelationDTO chara = CharacterRelations.Find(s => s.RelatedCharacterId == characterId);
            if (chara != null)
            {
                long id = chara.CharacterRelationId;
                DAOFactory.CharacterRelationDAO.Delete(id);
                ServerManager.Instance.RelationRefresh(id);
                Session.SendPacket(GenerateBlinit());
            }
        }

        public void DeleteItem(InventoryType type, short slot)
        {
            if (Inventory != null)
            {
                Inventory.DeleteFromSlotAndType(slot, type);
                Session.SendPacket(UserInterfaceHelper.Instance.GenerateInventoryRemove(type, slot));
            }
        }

        public void DeleteItemByItemInstanceId(Guid id)
        {
            if (Inventory != null)
            {
                Tuple<short, InventoryType> result = Inventory.DeleteById(id);
                Session.SendPacket(UserInterfaceHelper.Instance.GenerateInventoryRemove(result.Item2, result.Item1));
            }
        }

        public void DeleteRelation(long characterId)
        {
            CharacterRelationDTO chara = CharacterRelations.Find(s => s.RelatedCharacterId == characterId || s.CharacterId == characterId);
            if (chara != null)
            {
                long id = chara.CharacterRelationId;
                CharacterDTO charac = DAOFactory.CharacterDAO.LoadById(characterId);
                DAOFactory.CharacterRelationDAO.Delete(id);
                ServerManager.Instance.RelationRefresh(id);

                Session.SendPacket(GenerateFinit());
                if (charac != null)
                {
                    List<CharacterRelationDTO> lst = ServerManager.Instance.CharacterRelations.Where(s => s.CharacterId == id || s.RelatedCharacterId == id).ToList();
                    string result = "finit";
                    foreach (CharacterRelationDTO relation in lst.Where(c => c.RelationType == CharacterRelationType.Friend))
                    {
                        long id2 = relation.RelatedCharacterId == CharacterId ? relation.CharacterId : relation.RelatedCharacterId;
                        bool isOnline = CommunicationServiceClient.Instance.IsCharacterConnected(ServerManager.Instance.ServerGroup, id2);
                        result += $" {id2}|{(short)relation.RelationType}|{(isOnline ? 1 : 0)}|{DAOFactory.CharacterDAO.LoadById(id2).Name}";
                    }
                    int? sentChannelId = CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                    {
                        DestinationCharacterId = charac.CharacterId,
                        SourceCharacterId = CharacterId,
                        SourceWorldId = ServerManager.Instance.WorldId,
                        Message = result,
                        Type = MessageType.PrivateChat
                    });
                }
            }
        }

        public void DeleteTimeout()
        {
            if (Inventory == null)
            {
                return;
            }

            foreach (ItemInstance item in Inventory.GetAllItems())
            {
                if (item.IsBound && item.ItemDeleteTime != null && item.ItemDeleteTime < DateTime.Now)
                {
                    Inventory.DeleteById(item.Id);
                    EquipmentBCards.RemoveAll(o => o.ItemVNum == item.ItemVNum);
                    if (item.Type == InventoryType.Wear)
                    {
                        Session.SendPacket(GenerateEquipment());
                    }
                    else
                    {
                        Session.SendPacket(UserInterfaceHelper.Instance.GenerateInventoryRemove(item.Type, item.Slot));
                    }
                    Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("ITEM_TIMEOUT"), 10));
                }
            }
        }

        public void DisableBuffs(BuffType types, int level = 100)
        {
            lock (Buff)
            {
                List<Buff> buff = Buff.Where(s => (types & s.Card.BuffType) == s.Card.BuffType && !s.StaticBuff && s.Card.Level < level);
                buff.ForEach(s =>
                {
                    if (BuffObservables.ContainsKey(s.Card.CardId))
                    {
                        BuffObservables[s.Card.CardId].Dispose();
                        BuffObservables.Remove(s.Card.CardId);
                    }
                    RemoveBuff(s.Card.CardId);
                });
            }
        }

        /// <summary>
        /// Make the character moveable also from Teleport, ..
        /// </summary>
        public void Dispose()
        {
            CloseShop();
            CloseExchangeOrTrade();
            GroupSentRequestCharacterIds.Clear();
            FamilyInviteCharacters.Clear();
            FriendRequestCharacters.Clear();
        }

        public static string GenerateAct() => "act 6";

        public string GenerateAt()
        {
            MapInstance mapForMusic = MapInstance;
            return $"at {CharacterId} {MapInstance.Map.MapId} {PositionX} {PositionY} {Direction} 0 {mapForMusic?.InstanceMusic ?? 0} 2 -1";
        }

        public string GenerateBlinit()
        {
            string result = "blinit";
            foreach (CharacterRelationDTO relation in CharacterRelations.Where(s => s.CharacterId == CharacterId && s.RelationType == CharacterRelationType.Blocked))
            {
                result += $" {relation.RelatedCharacterId}|{DAOFactory.CharacterDAO.LoadById(relation.RelatedCharacterId).Name}";
            }
            return result;
        }



        public string GenerateCInfo() => $"c_info {(Authority == AuthorityType.Moderator && !Undercover ? "[Support]" + Name : Authority == AuthorityType.Donador ? Name + "[DT]" : Name)} - -1 {(Family != null && !Undercover ? $"{Family.FamilyId} {Family.Name}({Language.Instance.GetMessageFromKey(FamilyCharacter.Authority.ToString().ToUpper())})" : "-1 -")} {CharacterId} {(Invisible ? 6 : Undercover ? (byte)AuthorityType.User : Authority < AuthorityType.User ? (byte)AuthorityType.User : (byte)Authority)} {(byte)Gender} {(byte)HairStyle} {(byte)HairColor} {(byte)Class} {(GetDignityIco() == 1 ? GetReputationIco() : -GetDignityIco())} {(Authority == AuthorityType.Moderator ? 500 : Compliment)} {(UseSp || IsVehicled ? Morph : 0)} {(Invisible ? 1 : 0)} {Family?.FamilyLevel ?? 0} {(UseSp ? MorphUpgrade : 0)} {ArenaWinner}";

        public string GenerateCMap() => $"c_map 0 {MapInstance.Map.MapId} {(MapInstance.MapInstanceType != MapInstanceType.BaseMapInstance ? 1 : 0)}";

        public string GenerateCMode() => $"c_mode 1 {CharacterId} {(UseSp || IsVehicled ? Morph : 0)} {(UseSp ? MorphUpgrade : 0)} {(UseSp ? MorphUpgrade2 : 0)} {ArenaWinner}";

        public string GenerateCond() => $"cond 1 {CharacterId} {(NoAttack ? 1 : 0)} {(NoMove ? 1 : 0)} {Speed}";

        public string GenerateDG()
        {
            byte raidType = 0;
            if (ServerManager.Instance.Act4RaidStart.AddMinutes(60) < DateTime.Now)
            {
                ServerManager.Instance.Act4RaidStart = DateTime.Now;
            }
            double seconds = (ServerManager.Instance.Act4RaidStart.AddMinutes(60) - DateTime.Now).TotalSeconds;
            switch (Family?.Act4Raid?.MapInstanceType)
            {
                case MapInstanceType.Act4Morcos:
                    raidType = 1;
                    break;

                case MapInstanceType.Act4Hatus:
                    raidType = 2;
                    break;

                case MapInstanceType.Act4Calvina:
                    raidType = 3;
                    break;

                case MapInstanceType.Act4Berios:
                    raidType = 4;
                    break;
            }
            return $"dg {raidType} {(seconds > 1800 ? 1 : 2)} {(int)seconds} 0";
        }

        public void GenerateDignity(NpcMonster monsterinfo)
        {
            if (Level < monsterinfo.Level && Dignity < 100 && Level > 20)
            {
                Dignity += (float)0.5;
                if (Dignity == (int)Dignity)
                {
                    Session.SendPacket(GenerateFd());
                    Session.CurrentMapInstance?.Broadcast(Session, GenerateIn(), ReceiverType.AllExceptMe);
                    Session.CurrentMapInstance?.Broadcast(Session, GenerateGidx(), ReceiverType.AllExceptMe);
                    Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("RESTORE_DIGNITY"), 11));
                }
            }
        }

        public string GenerateDir() => $"dir 1 {CharacterId} {Direction}";

        public string GenerateEq()
        {
            int color = (byte)HairColor;
            ItemInstance head = Inventory?.LoadBySlotAndType((byte)EquipmentType.Hat, InventoryType.Wear);

            if (head?.Item.IsColored == true)
            {
                color = head.Design;
            }
            return $"eq {CharacterId} {(Invisible ? 6 : (Undercover ? (byte)AuthorityType.User : Authority < AuthorityType.GameMaster ? 0 : 2))} {(byte)Gender} {(byte)HairStyle} {color} {(byte)Class} {GenerateEqListForPacket()} {(!InvisibleGm ? GenerateEqRareUpgradeForPacket() : null)}";
        }

        public string GenerateEqListForPacket()
        {
            string[] invarray = new string[16];
            if (Inventory != null)
            {
                for (short i = 0; i < 16; i++)
                {
                    ItemInstance item = Inventory.LoadBySlotAndType(i, InventoryType.Wear);
                    if (item != null)
                    {
                        invarray[i] = item.ItemVNum.ToString();
                    }
                    else
                    {
                        invarray[i] = "-1";
                    }
                }
            }
            return $"{invarray[(byte)EquipmentType.Hat]}.{invarray[(byte)EquipmentType.Armor]}.{invarray[(byte)EquipmentType.MainWeapon]}.{invarray[(byte)EquipmentType.SecondaryWeapon]}.{invarray[(byte)EquipmentType.Mask]}.{invarray[(byte)EquipmentType.Fairy]}.{invarray[(byte)EquipmentType.CostumeSuit]}.{invarray[(byte)EquipmentType.CostumeHat]}.{invarray[(byte)EquipmentType.WeaponSkin]}";
        }

        public string GenerateEqRareUpgradeForPacket()
        {
            sbyte weaponRare = 0;
            short weaponUpgrade = 0;
            sbyte armorRare = 0;
            short armorUpgrade = 0;
            if (Inventory != null)
            {
                for (short i = 0; i < 20; i++)
                {
                    ItemInstance wearable = Inventory.LoadBySlotAndType(i, InventoryType.Wear);
                    if (wearable != null)
                    {
                        switch (wearable.Item.EquipmentSlot)
                        {
                            case EquipmentType.Armor:
                                armorRare = wearable.Rare;
                                armorUpgrade = wearable.Upgrade;
                                break;

                            case EquipmentType.MainWeapon:
                                weaponRare = wearable.Rare;
                                weaponUpgrade = wearable.Upgrade;
                                break;
                        }
                    }
                }
            }
            return $"{weaponUpgrade}{weaponRare} {armorUpgrade}{armorRare}";
        }

        public string GenerateEquipment()
        {
            string eqlist = string.Empty;
            EquipmentBCards.Clear();
            if (Inventory != null)
            {
                for (short i = 0; i < 20; i++)
                {
                    ItemInstance item = Inventory.LoadBySlotAndType(i, InventoryType.Wear);
                    if (item != null)
                    {
                        if (item.Item.EquipmentSlot != EquipmentType.Sp)
                        {
                            EquipmentBCards.AddRange(item.Item.BCards);
                            switch (item.Item.ItemType)
                            {
                                case ItemType.Armor:
                                    ShellEffectArmor.Clear();

                                    foreach (ShellEffectDTO dto in item.ShellEffects)
                                    {
                                        ShellEffectArmor.Add(dto);
                                    }
                                    break;
                                case ItemType.Weapon:
                                    switch (item.Item.EquipmentSlot)
                                    {
                                        case EquipmentType.MainWeapon:
                                            ShellEffectMain.Clear();

                                            foreach (ShellEffectDTO dto in item.ShellEffects)
                                            {
                                                ShellEffectMain.Add(dto);
                                            }
                                            break;

                                        case EquipmentType.SecondaryWeapon:
                                            ShellEffectSecondary.Clear();

                                            foreach (ShellEffectDTO dto in item.ShellEffects)
                                            {
                                                ShellEffectSecondary.Add(dto);
                                            }
                                            break;
                                    }
                                    break;
                            }
                        }
                        eqlist += $" {i}.{item.Item.VNum}.{item.Rare}.{(item.Item.IsColored ? item.Design : item.Upgrade)}.0";
                    }
                }
            }
            return $"equip {GenerateEqRareUpgradeForPacket()}{eqlist}";
        }

        public string GenerateExts() => $"exts 0 {48 + ((HaveBackpack() ? 1 : 0) * 12)} {48 + ((HaveBackpack() ? 1 : 0) * 12)} {48 + ((HaveBackpack() ? 1 : 0) * 12)}";

        public string GenerateFaction() => $"fs {(byte)Faction}";

        public string GenerateFamilyMember()
        {
            string str = "gmbr 0";
            try
            {
                if (Family?.FamilyCharacters != null)
                {
                    foreach (FamilyCharacter TargetCharacter in Family?.FamilyCharacters)
                    {
                        bool isOnline = CommunicationServiceClient.Instance.IsCharacterConnected(ServerManager.Instance.ServerGroup, TargetCharacter.CharacterId);
                        str += $" {TargetCharacter.Character.CharacterId}|{Family.FamilyId}|{TargetCharacter.Character.Name}|{TargetCharacter.Character.Level}|{(byte)TargetCharacter.Character.Class}|{(byte)TargetCharacter.Authority}|{(byte)TargetCharacter.Rank}|{(isOnline ? 1 : 0)}|{TargetCharacter.Character.HeroLevel}";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return str;
        }

        public string GenerateFamilyMemberExp()
        {
            string str = "gexp";
            try
            {
                if (Family?.FamilyCharacters != null)
                {
                    foreach (FamilyCharacter TargetCharacter in Family?.FamilyCharacters)
                    {
                        str += $" {TargetCharacter.CharacterId}|{TargetCharacter.Experience}";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return str;
        }

        public string GenerateFamilyMemberMessage()
        {
            string str = "gmsg";
            try
            {
                if (Family?.FamilyCharacters != null)
                {
                    foreach (FamilyCharacter TargetCharacter in Family?.FamilyCharacters)
                    {
                        str += $" {TargetCharacter.CharacterId}|{TargetCharacter.DailyMessage}";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return str;
        }

        public List<string> GenerateFamilyWarehouseHist()
        {
            if (Family != null)
            {
                List<string> packetList = new List<string>();
                string packet = string.Empty;
                int i = 0;
                int amount = -1;
                foreach (FamilyLogDTO log in Family.FamilyLogs.Where(s => s.FamilyLogType == FamilyLogType.WareHouseAdded || s.FamilyLogType == FamilyLogType.WareHouseRemoved).OrderByDescending(s => s.Timestamp).Take(100))
                {
                    packet += $" {(log.FamilyLogType == FamilyLogType.WareHouseAdded ? 0 : 1)}|{log.FamilyLogData}|{(int)(DateTime.Now - log.Timestamp).TotalHours}";
                    i++;
                    if (i == 50)
                    {
                        i = 0;
                        packetList.Add($"fslog_stc {amount}{packet}");
                        amount++;
                    }
                    else if (i == Family.FamilyLogs.Count)
                    {
                        packetList.Add($"fslog_stc {amount}{packet}");
                    }
                }

                return packetList;
            }
            return new List<string>();
        }

        public void GenerateFamilyXp(int FXP)
        {
            if (!Session.Account.PenaltyLogs.Any(s => s.Penalty == PenaltyType.BlockFExp && s.DateEnd > DateTime.Now) && Family != null && FamilyCharacter != null)
            {
                FamilyCharacterDTO famchar = FamilyCharacter;
                FamilyDTO fam = Family;
                fam.FamilyExperience += FXP;
                famchar.Experience += FXP;
                if (CharacterHelper.LoadFamilyXPData(Family.FamilyLevel) <= fam.FamilyExperience)
                {
                    fam.FamilyExperience -= CharacterHelper.LoadFamilyXPData(Family.FamilyLevel);
                    fam.FamilyLevel++;
                    Family.InsertFamilyLog(FamilyLogType.FamilyLevelUp, level: fam.FamilyLevel);
                    CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                    {
                        DestinationCharacterId = Family.FamilyId,
                        SourceCharacterId = CharacterId,
                        SourceWorldId = ServerManager.Instance.WorldId,
                        Message = UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("FAMILY_UP"), 0),
                        Type = MessageType.Family
                    });
                }
                DAOFactory.FamilyCharacterDAO.InsertOrUpdate(ref famchar);
                DAOFactory.FamilyDAO.InsertOrUpdate(ref fam);
                ServerManager.Instance.FamilyRefresh(Family.FamilyId);
                CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                {
                    DestinationCharacterId = Family.FamilyId,
                    SourceCharacterId = CharacterId,
                    SourceWorldId = ServerManager.Instance.WorldId,
                    Message = "fhis_stc",
                    Type = MessageType.Family
                });
            }
        }

        public string GenerateFc()
        {
            return $"fc {(byte)Faction} {ServerManager.Instance.Act4AngelStat.MinutesUntilReset} {ServerManager.Instance.Act4AngelStat.Percentage / 100} {ServerManager.Instance.Act4AngelStat.Mode}" +
                $" {ServerManager.Instance.Act4AngelStat.CurrentTime} {ServerManager.Instance.Act4AngelStat.TotalTime} {Convert.ToByte(ServerManager.Instance.Act4AngelStat.IsMorcos)}" +
                $" {Convert.ToByte(ServerManager.Instance.Act4AngelStat.IsHatus)} {Convert.ToByte(ServerManager.Instance.Act4AngelStat.IsCalvina)} {Convert.ToByte(ServerManager.Instance.Act4AngelStat.IsBerios)}" +
                $" 0 {ServerManager.Instance.Act4DemonStat.Percentage / 100} {ServerManager.Instance.Act4DemonStat.Mode} {ServerManager.Instance.Act4DemonStat.CurrentTime} {ServerManager.Instance.Act4DemonStat.TotalTime}" +
                $" {Convert.ToByte(ServerManager.Instance.Act4DemonStat.IsMorcos)} {Convert.ToByte(ServerManager.Instance.Act4DemonStat.IsHatus)} {Convert.ToByte(ServerManager.Instance.Act4DemonStat.IsCalvina)} " +
                $"{Convert.ToByte(ServerManager.Instance.Act4DemonStat.IsBerios)} 0";

            //return $"fc {Faction} 0 69 0 0 0 1 1 1 1 0 34 0 0 0 1 1 1 1 0";
        }

        public string GenerateFd() => $"fd {Reputation} {GetReputationIco()} {(int)Dignity} {Math.Abs(GetDignityIco())}";

        public string GenerateFinfo(long? relatedCharacterLoggedId, bool isConnected)
        {
            string result = "finfo";
            foreach (CharacterRelationDTO relation in CharacterRelations.Where(c => c.RelationType == CharacterRelationType.Friend))
            {
                if (relatedCharacterLoggedId.HasValue && (relatedCharacterLoggedId.Value == relation.RelatedCharacterId || relatedCharacterLoggedId.Value == relation.CharacterId))
                {
                    result += $" {relation.RelatedCharacterId}.{(isConnected ? 1 : 0)}";
                }
            }
            return result;
        }

        public string GenerateFinit()
        {
            string result = "finit";
            foreach (CharacterRelationDTO relation in CharacterRelations.Where(c => c.RelationType == CharacterRelationType.Friend))
            {
                long id = relation.RelatedCharacterId == CharacterId ? relation.CharacterId : relation.RelatedCharacterId;
                if (DAOFactory.CharacterDAO.LoadById(id) is CharacterDTO character)
                {
                    bool isOnline = CommunicationServiceClient.Instance.IsCharacterConnected(ServerManager.Instance.ServerGroup, id);
                    result += $" {id}|{(short)relation.RelationType}|{(isOnline ? 1 : 0)}|{character.Name}";
                }
            }
            return result;
        }

        public string GenerateFStashAll()
        {
            string stash = $"f_stash_all {Family.WarehouseSize}";
            foreach (ItemInstance item in Family.Warehouse.GetAllItems())
            {
                stash += $" {item.GenerateStashPacket()}";
            }
            return stash;
        }

        public string GenerateGender() => $"p_sex {(byte)Gender}";

        public string GenerateGet(long id) => $"get 1 {CharacterId} {id} 0";

        public string GenerateGExp()
        {
            string str = "gexp";
            foreach (FamilyCharacter familyCharacter in Family.FamilyCharacters)
            {
                str += $" {familyCharacter.CharacterId}|{familyCharacter.Experience}";
            }
            return str;
        }

        public string GenerateGidx() => Family != null ? $"gidx 1 {CharacterId} {Family.FamilyId} {Family.Name}({Language.Instance.GetMessageFromKey(Family.FamilyCharacters.Find(s => s.CharacterId == CharacterId)?.Authority.ToString().ToUpper())}) {Family.FamilyLevel}" : $"gidx 1 {CharacterId} -1 - 0";

        public string GenerateGInfo()
        {
            if (Family != null)
            {
                try
                {
                    FamilyCharacter familyCharacter = Family.FamilyCharacters.Find(s => s.Authority == FamilyAuthority.Head);
                    if (familyCharacter != null)
                    {
                        return $"ginfo {Family.Name} {familyCharacter.Character.Name} {(byte)Family.FamilyHeadGender} {Family.FamilyLevel} {Family.FamilyExperience} {CharacterHelper.LoadFamilyXPData(Family.FamilyLevel)} {Family.FamilyCharacters.Count} {Family.MaxSize} {(byte)FamilyCharacter.Authority} {(Family.ManagerCanInvite ? 1 : 0)} {(Family.ManagerCanNotice ? 1 : 0)} {(Family.ManagerCanShout ? 1 : 0)} {(Family.ManagerCanGetHistory ? 1 : 0)} {(byte)Family.ManagerAuthorityType} {(Family.MemberCanGetHistory ? 1 : 0)} {(byte)Family.MemberAuthorityType} {Family.FamilyMessage.Replace(' ', '^')}";
                    }
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        public string GenerateGold() => $"gold {Gold} 0";

        public string GenerateIcon(int type, int value, short itemVNum) => $"icon {type} {CharacterId} {value} {itemVNum}";

        public string GenerateIdentity() => $"Character: {Name}";

        public string GenerateIn(bool foe = false)
        {
            // string chars = "!§$%&/()=?*+~#";
            string _name = Name;
            if (foe)
            {
                _name = "!§$%&/()=?*+~#";
            }
            int _faction = 0;
            if (ServerManager.Instance.ChannelId == 51)
            {
                _faction = (byte)Faction + 2;
            }
            int color = HairStyle == HairStyleType.Hair8 ? 0 : (byte)HairColor;
            ItemInstance fairy = null;
            if (Inventory != null)
            {
                ItemInstance headWearable = Inventory.LoadBySlotAndType((byte)EquipmentType.Hat, InventoryType.Wear);
                if (headWearable?.Item.IsColored == true)
                {
                    color = headWearable.Design;
                }
                fairy = Inventory.LoadBySlotAndType((byte)EquipmentType.Fairy, InventoryType.Wear);
            }
            return $"in 1 {(Authority == AuthorityType.Moderator && !Undercover ? "[Support]" + _name : Authority == AuthorityType.Donador ? _name + "[DT]" : _name)} - {CharacterId} {PositionX} {PositionY} {Direction} {(Undercover ? (byte)AuthorityType.User : Authority < AuthorityType.User ? (byte)AuthorityType.User : (byte)Authority)} {(byte)Gender} {(byte)HairStyle} {color} {(byte)Class} {GenerateEqListForPacket()} {Math.Ceiling(Hp / HPLoad() * 100)} {Math.Ceiling(Mp / MPLoad() * 100)} {(IsSitting ? 1 : 0)} {(Group?.GroupType == GroupType.Group ? (Group?.GroupId ?? -1) : -1)} {(fairy != null && !Undercover ? 4 : 0)} {fairy?.Item.Element ?? 0} 0 {fairy?.Item.Morph ?? 0} 0 {(UseSp || IsVehicled ? Morph : 0)} {GenerateEqRareUpgradeForPacket()} {(!Undercover ? (foe ? -1 : Family?.FamilyId ?? -1) : -1)} {(!Undercover ? (foe ? _name : Family?.Name ?? "-") : "-")} {(GetDignityIco() == 1 ? GetReputationIco() : -GetDignityIco())} {(Invisible ? 1 : 0)} {(UseSp ? MorphUpgrade : 0)} {_faction} {(UseSp ? MorphUpgrade2 : 0)} {Level} {Family?.FamilyLevel ?? 0} {ArenaWinner} {(Authority == AuthorityType.Moderator && !Undercover ? 500 : Compliment)} {Size} {HeroLevel}";
        }

        public string GenerateInvisible() => $"cl {CharacterId} {(Invisible ? 1 : 0)} {(InvisibleGm ? 1 : 0)}";

        public void GenerateKillBonus(MapMonster monsterToAttack)
        {
            lock (_syncObj)
            {
                if (monsterToAttack == null || monsterToAttack.IsAlive)
                {
                    return;
                }
                monsterToAttack.RunDeathEvent();

                Random random = new Random(DateTime.Now.Millisecond & monsterToAttack.MapMonsterId);

                // owner set
                long? dropOwner = monsterToAttack.DamageList.Count > 0 ? monsterToAttack.DamageList.First().Key : (long?)null;
                Group group = null;
                if (dropOwner != null)
                {
                    group = ServerManager.Instance.Groups.Find(g => g.IsMemberOfGroup((long)dropOwner));
                }

                if (ServerManager.Instance.ChannelId == 51 && ServerManager.Instance.Act4DemonStat.Mode == 0 && ServerManager.Instance.Act4AngelStat.Mode == 0 && !CaligorRaid.IsRunning)
                {
                    if (Faction == FactionType.Angel)
                    {
                        ServerManager.Instance.Act4AngelStat.Percentage++;
                    }
                    else if (Faction == FactionType.Demon)
                    {
                        ServerManager.Instance.Act4DemonStat.Percentage++;
                    }
                }

                // end owner set
                if (Session.HasCurrentMapInstance && (Group == null || Group.GroupType == GroupType.Group))
                {
                    List<DropDTO> droplist = monsterToAttack.Monster.Drops.Where(s => Session.CurrentMapInstance.Map.MapTypes.Any(m => m.MapTypeId == s.MapTypeId) || s.MapTypeId == null).ToList();
                    if (monsterToAttack.Monster.MonsterType != MonsterType.Special)
                    {
                        #region item drop

                        int levelDifference = Session.Character.Level - monsterToAttack.Monster.Level;
                        int dropRate = ServerManager.Instance.Configuration.RateDrop * MapInstance.DropRate;
                        int x = 0;
                        foreach (DropDTO drop in droplist.OrderBy(s => random.Next()))
                        {
                            if (x < 4)
                            {
                                double rndamount = ServerManager.RandomNumber() * random.NextDouble();
                                double divider = levelDifference >= 10 ? (levelDifference - 9) * 1.2D : levelDifference <= -10 ? (levelDifference + 9) * 1.2D : 1D;
                                if (rndamount <= (double)drop.DropChance * dropRate / 5000.000 / divider)
                                {
                                    x++;
                                    if (Session.CurrentMapInstance != null)
                                    {
                                        if (Session.CurrentMapInstance.Map.MapTypes.Any(s => s.MapTypeId == (short)MapTypeEnum.Act4) || monsterToAttack.Monster.MonsterType == MonsterType.Elite)
                                        {
                                            List<long> alreadyGifted = new List<long>();
                                            foreach (long charId in monsterToAttack.DamageList.Keys)
                                            {
                                                if (!alreadyGifted.Contains(charId))
                                                {
                                                    ClientSession giftsession = ServerManager.Instance.GetSessionByCharacterId(charId);
                                                    giftsession?.Character.GiftAdd(drop.ItemVNum, (byte)drop.Amount);
                                                    alreadyGifted.Add(charId);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (group?.GroupType == GroupType.Group)
                                            {
                                                if (group.SharingMode == (byte)GroupSharingType.ByOrder)
                                                {
                                                    dropOwner = group.GetNextOrderedCharacterId(this);
                                                    if (dropOwner.HasValue)
                                                    {
                                                        group.Characters.ForEach(s => s.SendPacket(s.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("ITEM_BOUND_TO"), ServerManager.GetItem(drop.ItemVNum).Name, group.Characters.Single(c => c.Character.CharacterId == (long)dropOwner).Character.Name, drop.Amount), 10)));
                                                    }
                                                }
                                                else
                                                {
                                                    group.Characters.ForEach(s => s.SendPacket(s.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("DROPPED_ITEM"), ServerManager.GetItem(drop.ItemVNum).Name, drop.Amount), 10)));
                                                }
                                            }

                                            long? owner = dropOwner;
                                            Observable.Timer(TimeSpan.FromMilliseconds(500)).Subscribe(o =>
                                            {
                                                if (Session.HasCurrentMapInstance)
                                                {
                                                    if (CharacterId == owner && (StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.AutoLoot) || DateTime.Now <= new DateTime(2019, 12, 31, 12, 00, 00)))
                                                    {
                                                        GiftAdd(drop.ItemVNum, (byte)drop.Amount);
                                                    }
                                                    else
                                                    {
                                                        Session.CurrentMapInstance.DropItemByMonster(owner, drop, monsterToAttack.MapX, monsterToAttack.MapY);
                                                    }
                                                }
                                            });
                                        }
                                    }
                                }
                            }
                        }

                        #endregion

                        #region gold drop

                        // gold calculation
                        int gold = GetGold(monsterToAttack);
                        long maxGold = ServerManager.Instance.Configuration.MaxGold;
                        gold = gold > maxGold ? (int)maxGold : gold;
                        double randChance = ServerManager.RandomNumber() * random.NextDouble();

                        if (gold > 0 && randChance <= (int)(ServerManager.Instance.Configuration.RateGoldDrop * 10 * CharacterHelper.GoldPenalty(Level, monsterToAttack.Monster.Level)))
                        {
                            DropDTO drop2 = new DropDTO
                            {
                                Amount = gold,
                                ItemVNum = 1046
                            };
                            if (Session.CurrentMapInstance != null)
                            {
                                if (Session.CurrentMapInstance.Map.MapTypes.Any(s => s.MapTypeId == (short)MapTypeEnum.Act4) || monsterToAttack.Monster.MonsterType == MonsterType.Elite)
                                {
                                    List<long> alreadyGifted = new List<long>();
                                    foreach (long charId in monsterToAttack.DamageList.Keys)
                                    {
                                        if (!alreadyGifted.Contains(charId))
                                        {
                                            ClientSession session = ServerManager.Instance.GetSessionByCharacterId(charId);
                                            if (session != null)
                                            {
                                                session.Character.Gold += drop2.Amount;
                                                if (session.Character.Gold > maxGold)
                                                {
                                                    session.Character.Gold = maxGold;
                                                    session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("MAX_GOLD"), 0));
                                                }
                                                session.SendPacket(session.Character.GenerateSay($"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {ServerManager.GetItem(drop2.ItemVNum).Name} x {drop2.Amount}", 10));
                                                session.SendPacket(session.Character.GenerateGold());
                                            }
                                            alreadyGifted.Add(charId);
                                        }
                                    }
                                }
                                else
                                {
                                    if (group != null && MapInstance.MapInstanceType != MapInstanceType.LodInstance)
                                    {
                                        if (group.SharingMode == (byte)GroupSharingType.ByOrder)
                                        {
                                            dropOwner = group.GetNextOrderedCharacterId(this);

                                            if (dropOwner.HasValue)
                                            {
                                                group.Characters.ForEach(s => s.SendPacket(s.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("ITEM_BOUND_TO"), ServerManager.GetItem(drop2.ItemVNum).Name, group.Characters.Single(c => c.Character.CharacterId == (long)dropOwner).Character.Name, drop2.Amount), 10)));
                                            }
                                        }
                                        else
                                        {
                                            group.Characters.ForEach(s => s.SendPacket(s.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("DROPPED_ITEM"), ServerManager.GetItem(drop2.ItemVNum).Name, drop2.Amount), 10)));
                                        }
                                    }

                                    // delayed Drop
                                    Observable.Timer(TimeSpan.FromMilliseconds(500)).Subscribe(o =>
                                    {
                                        if (Session.HasCurrentMapInstance)
                                        {
                                            if (CharacterId == dropOwner && (StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.AutoLoot) || DateTime.Now <= new DateTime(2019, 12, 31, 12, 00, 00)))
                                            {
                                                Gold += drop2.Amount;
                                                if (Gold > maxGold)
                                                {

                                                    Gold = maxGold;
                                                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("MAX_GOLD"), 0));

                                                }
                                                Session.SendPacket(GenerateSay($"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {ServerManager.GetItem(drop2.ItemVNum).Name} x {drop2.Amount}", 10));
                                                Session.SendPacket(GenerateGold());
                                            }

                                            else

                                            {
                                                Session.CurrentMapInstance.DropItemByMonster(dropOwner, drop2, monsterToAttack.MapX, monsterToAttack.MapY);
                                            }
                                        }
                                    });
                                }
                            }
                        }

                        #endregion

                        #region exp

                        if (Hp > 0)
                        {
                            Group grp = ServerManager.Instance.Groups.Find(g => g.IsMemberOfGroup(CharacterId));
                            if (grp != null)
                            {
                                foreach (ClientSession targetSession in grp.Characters.Where(g => g.Character.MapInstanceId == MapInstanceId))
                                {
                                    if (grp.IsMemberOfGroup(monsterToAttack.DamageList.FirstOrDefault().Key))
                                    {
                                        targetSession.Character.GenerateXp(monsterToAttack, true);
                                    }
                                    else
                                    {
                                        targetSession.SendPacket(targetSession.Character.GenerateSay(Language.Instance.GetMessageFromKey("XP_NOTFIRSTHIT"), 10));
                                        targetSession.Character.GenerateXp(monsterToAttack, false);
                                    }
                                    targetSession.Character.SetReputation(monsterToAttack.Monster.Level);
                                }
                            }
                            else
                            {
                                if (monsterToAttack.DamageList.FirstOrDefault().Key == CharacterId)
                                {
                                    GenerateXp(monsterToAttack, true);
                                }
                               
                                else
                                {
                                    Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("XP_NOTFIRSTHIT"), 10));
                                    GenerateXp(monsterToAttack, false);
                                }
                                SetReputation(monsterToAttack.Monster.Level);
                            }
                            GenerateDignity(monsterToAttack.Monster);
                        }

                        #endregion
                    }
                }
            }
        }

        public string GenerateLev()
        {
            ItemInstance specialist = null;
            if (Inventory != null)
            {
                specialist = Inventory.LoadBySlotAndType((byte)EquipmentType.Sp, InventoryType.Wear);
            }
            return $"lev {Level} {(int)(Level < 100 ? LevelXp : LevelXp / 100)} {(!UseSp || specialist == null ? JobLevel : specialist.SpLevel)} {(!UseSp || specialist == null ? JobLevelXp : specialist.XP)} {(int)(Level < 100 ? XpLoad() : XpLoad() / 100)} {(!UseSp || specialist == null ? JobXPLoad() : SpXpLoad())} {Reputation} {GetCP()} {(int)(HeroLevel < 100 ? HeroXp : HeroXp / 100)} {HeroLevel} {(int)(HeroLevel < 100 ? HeroXPLoad() : HeroXPLoad() / 100)} 0";
        }

        public string GenerateLevelUp()
        {
            Logger.LogUserEvent("LEVELUP", Session.GenerateIdentity(), $"Level: {Level} JobLevel: {JobLevel} SPLevel: {Inventory.LoadBySlotAndType((byte)EquipmentType.Sp, InventoryType.Wear)?.SpLevel} HeroLevel: {HeroLevel} MapId: {Session.CurrentMapInstance?.Map.MapId} MapX: {PositionX} MapY: {PositionY}");
            return $"levelup {CharacterId}";
        }

        public void GenerateMiniland()
        {
            if (Miniland == null)
            {
                Miniland = ServerManager.GenerateMapInstance(20001, MapInstanceType.NormalInstance, new InstanceBag());
                foreach (MinilandObjectDTO obj in DAOFactory.MinilandObjectDAO.LoadByCharacterId(CharacterId))
                {
                    MinilandObject mapobj = new MinilandObject(obj);
                    if (mapobj.ItemInstanceId != null)
                    {
                        ItemInstance item = Inventory.GetItemInstanceById((Guid)mapobj.ItemInstanceId);
                        if (item != null)
                        {
                            mapobj.ItemInstance = item;
                            MinilandObjects.Add(mapobj);
                        }
                    }
                }
            }
        }

        public string GenerateMinilandObjectForFriends()
        {
            string mlobjstring = "mltobj";
            int i = 0;
            foreach (MinilandObject mp in MinilandObjects)
            {
                mlobjstring += $" {mp.ItemInstance.ItemVNum}.{i}.{mp.MapX}.{mp.MapY}";
                i++;
            }
            return mlobjstring;
        }

        public string GenerateMinilandPoint() => $"mlpt {MinilandPoint} 100";

        public string GenerateMinimapPosition() => MapInstance.MapInstanceType == MapInstanceType.TimeSpaceInstance
            || MapInstance.MapInstanceType == MapInstanceType.RaidInstance
            ? $"rsfp {MapInstance.MapIndexX} {MapInstance.MapIndexY}" : "rsfp 0 -1";

        public string GenerateMlinfo() => $"mlinfo 3800 {MinilandPoint} 100 {GeneralLogs.CountLinq(s => s.LogData == nameof(Miniland) && s.Timestamp.Day == DateTime.Now.Day)} {GeneralLogs.CountLinq(s => s.LogData == nameof(Miniland))} 10 {(byte)MinilandState} {Language.Instance.GetMessageFromKey("WELCOME_MUSIC_INFO")} {Language.Instance.GetMessageFromKey("MINILAND_WELCOME_MESSAGE")}";

        public string GenerateMlinfobr() => $"mlinfobr 3800 {Name} {GeneralLogs.CountLinq(s => s.LogData == nameof(Miniland) && s.Timestamp.Day == DateTime.Now.Day)} {GeneralLogs.CountLinq(s => s.LogData == nameof(Miniland))} 25 {MinilandMessage.Replace(' ', '^')}";

        public string GenerateMloMg(MinilandObject mlobj, MinigamePacket packet) => $"mlo_mg {packet.MinigameVNum} {MinilandPoint} 0 0 {mlobj.ItemInstance.DurabilityPoint} {mlobj.ItemInstance.Item.MinilandObjectPoint}";

        public string GenerateNpcDialog(int value) => $"npc_req 1 {CharacterId} {value}";

        public string GeneratePairy()
        {
            ItemInstance fairy = null;
            if (Inventory != null)
            {
                fairy = Inventory.LoadBySlotAndType((byte)EquipmentType.Fairy, InventoryType.Wear);
            }
            ElementRate = 0;
            Element = 0;
            bool shouldChangeMorph = false;

            if (fairy != null)
            {
                //exclude magical fairy
                shouldChangeMorph = IsUsingFairyBooster && (fairy.Item.Morph > 4 && fairy.Item.Morph != 9 && fairy.Item.Morph != 14);
                ElementRate += fairy.ElementRate + fairy.Item.ElementRate + (IsUsingFairyBooster ? 30 : 0);
                Element = fairy.Item.Element;
            }

            return fairy != null
                ? $"pairy 1 {CharacterId} 4 {fairy.Item.Element} {fairy.ElementRate + fairy.Item.ElementRate} {fairy.Item.Morph + (shouldChangeMorph ? 5 : 0)}"
                : $"pairy 1 {CharacterId} 0 0 0 0";
        }

        public string GenerateParcel(MailDTO mail) => mail.AttachmentVNum != null ? $"parcel 1 1 {MailList.First(s => s.Value.MailId == mail.MailId).Key} {(mail.Title == "NOSMALL" ? 1 : 4)} 0 {mail.Date.ToString("yyMMddHHmm")} {mail.Title} {mail.AttachmentVNum} {mail.AttachmentAmount} {(byte)ServerManager.GetItem((short)mail.AttachmentVNum).Type}" : string.Empty;

        public string GeneratePidx(bool isLeaveGroup = false)
        {
            if (!isLeaveGroup && Group != null)
            {
                string result = $"pidx {Group.GroupId}";
                foreach (ClientSession session in Group.Characters.GetAllItems())
                {
                    if (session.Character != null)
                    {
                        result += $" {(Group.IsMemberOfGroup(CharacterId) ? 1 : 0)}.{session.Character.CharacterId} ";
                    }
                }
                return result;
            }
            return $"pidx -1 1.{CharacterId}";
        }

        public string GeneratePinit()
        {
            Group grp = ServerManager.Instance.Groups.Find(s => s.IsMemberOfGroup(CharacterId) && s.GroupType == GroupType.Group);
            List<Mate> mates = Mates;
            int i = 0;
            string str = string.Empty;
            if (mates != null)
            {
                foreach (Mate mate in mates.Where(s => s.IsTeamMember).OrderByDescending(s => s.MateType))
                {
                    i++;
                    str += $" 2|{mate.MateTransportId}|{(mate.MateType == MateType.Pet ? "0" : "1")}|{mate.Level}|{(mate.IsUsingSp && mate.SpInstance != null ? mate.SpInstance.Item.Name.Replace(' ', '^') : mate.Name.Replace(' ', '^'))}|-1|{(mate.IsUsingSp && mate.SpInstance != null ? mate.SpInstance.Item.Morph : mate.Monster.NpcMonsterVNum)}|0";
                }
            }
            if (grp != null)
            {
                foreach (ClientSession groupSessionForId in grp.Characters.GetAllItems())
                {
                    i++;
                    str += $" 1|{groupSessionForId.Character.CharacterId}|{i}|{groupSessionForId.Character.Level}|{groupSessionForId.Character.Name}|0|{(byte)groupSessionForId.Character.Gender}|{(byte)groupSessionForId.Character.Class}|{(groupSessionForId.Character.UseSp ? groupSessionForId.Character.Morph : 0)}|{groupSessionForId.Character.HeroLevel}";
                }
            }
            return $"pinit {i} {str}";
        }

        public string GeneratePlayerFlag(long pflag) => $"pflag 1 {CharacterId} {pflag}";

        public string GeneratePost(MailDTO mail, byte type)
        {
            if (mail != null)
            {
                return $"post 1 {type} {(MailList?.FirstOrDefault(s => s.Value?.MailId == mail?.MailId))?.Key} 0 {(mail.IsOpened ? 1 : 0)} {mail.Date.ToString("yyMMddHHmm")} {(type == 2 ? DAOFactory.CharacterDAO.LoadById(mail.ReceiverId).Name : DAOFactory.CharacterDAO.LoadById(mail.SenderId).Name)} {mail.Title}";
            }
            return string.Empty;
        }

        public string GeneratePostMessage(MailDTO mailDTO, byte type)
        {
            CharacterDTO sender = DAOFactory.CharacterDAO.LoadById(mailDTO.SenderId);

            return $"post 5 {type} {MailList.First(s => s.Value == mailDTO).Key} 0 0 {(byte)mailDTO.SenderClass} {(byte)mailDTO.SenderGender} {mailDTO.SenderMorphId} {(byte)mailDTO.SenderHairStyle} {(byte)mailDTO.SenderHairColor} {mailDTO.EqPacket} {sender.Name} {mailDTO.Title} {mailDTO.Message}";
        }

        public List<string> GeneratePst() => Mates.Where(s => s.IsTeamMember).OrderByDescending(s => s.MateType).Select(mate => $"pst 2 {mate.MateTransportId} {(mate.MateType == MateType.Partner ? "0" : "1")} {mate.Hp / mate.MaxHp * 100} {mate.Mp / mate.MaxMp * 100} {mate.Hp} {mate.Mp} 0 0 0").ToList();

        public string GeneratePStashAll()
        {
            string stash = $"pstash_all {(StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.PetBackPack) ? 50 : 0)}";
            return Inventory.Where(s => s.Type == InventoryType.PetWarehouse).Aggregate(stash, (current, item) => current + $" {item.GenerateStashPacket()}");
        }

        public IEnumerable<string> GenerateQuicklist()
        {
            string[] pktQs = { "qslot 0", "qslot 1", "qslot 2" };

            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    QuicklistEntryDTO qi = QuicklistEntries.FirstOrDefault(n => n.Q1 == j && n.Q2 == i && n.Morph == (UseSp ? Morph : 0));
                    pktQs[j] += $" {qi?.Type ?? 7}.{qi?.Slot ?? 7}.{qi?.Pos.ToString() ?? "-1"}";
                }
            }

            return pktQs;
        }

        public string GenerateRaid(int Type, bool exit = false)
        {
            string result = string.Empty;
            switch (Type)
            {
                case 0:
                    result = "raid 0";
                    Group?.Characters?.ForEach(s => result += $" {s.Character?.CharacterId}");
                    break;

                case 2:
                    result = $"raid 2 {(exit ? "-1" : $"{CharacterId}")}";
                    break;

                case 1:
                    result = $"raid 1 {(exit ? 0 : 1)}";
                    break;

                case 3:
                    result = "raid 3";
                    Group?.Characters?.ForEach(s => result += $" {s.Character?.CharacterId}.{Math.Ceiling(s.Character.Hp / s.Character.HPLoad() * 100)}.{Math.Ceiling(s.Character.Mp / s.Character.MPLoad() * 100)}");
                    break;

                case 4:
                    result = "raid 4";
                    break;

                case 5:
                    result = "raid 5 1";
                    break;
            }
            return result;
        }

        public static string GenerateRaidBf(byte type) => $"raidbf 0 {type} 25 ";

        public string GenerateRc(int characterHealth) => $"rc 1 {CharacterId} {characterHealth} 0";

        public string GenerateRCSList(CSListPacket packet)
        {
            string list = string.Empty;
            BazaarItemLink[] billist = new BazaarItemLink[ServerManager.Instance.BazaarList.Count + 20];
            ServerManager.Instance.BazaarList.CopyTo(billist);
            foreach (BazaarItemLink bz in billist.Where(s => s != null && s.BazaarItem.SellerId == CharacterId).Skip(packet.Index * 50).Take(50))
            {
                if (bz.Item != null)
                {
                    int soldedAmount = bz.BazaarItem.Amount - bz.Item.Amount;
                    int amount = bz.BazaarItem.Amount;
                    bool package = bz.BazaarItem.IsPackage;
                    bool isNosbazar = bz.BazaarItem.MedalUsed;
                    long price = bz.BazaarItem.Price;
                    long minutesLeft = (long)(bz.BazaarItem.DateStart.AddHours(bz.BazaarItem.Duration) - DateTime.Now).TotalMinutes;
                    byte Status = minutesLeft >= 0 ? (soldedAmount < amount ? (byte)BazaarType.OnSale : (byte)BazaarType.Solded) : (byte)BazaarType.DelayExpired;
                    if (Status == (byte)BazaarType.DelayExpired)
                    {
                        minutesLeft = (long)(bz.BazaarItem.DateStart.AddHours(bz.BazaarItem.Duration).AddDays(isNosbazar ? 30 : 7) - DateTime.Now).TotalMinutes;
                    }
                    string info = string.Empty;
                    if (bz.Item.Item.Type == InventoryType.Equipment)
                    {
                        info = bz.Item?.GenerateEInfo().Replace(' ', '^').Replace("e_info^", string.Empty);
                    }
                    if (packet.Filter == 0 || packet.Filter == Status)
                    {
                        list += $"{bz.BazaarItem.BazaarItemId}|{bz.BazaarItem.SellerId}|{bz.Item.ItemVNum}|{soldedAmount}|{amount}|{(package ? 1 : 0)}|{price}|{Status}|{minutesLeft}|{(isNosbazar ? 1 : 0)}|0|{bz.Item.Rare}|{bz.Item.Upgrade}|{info} ";
                    }
                }
            }

            return $"rc_slist {packet.Index} {list}";
        }

        public string GenerateReqInfo()
        {
            ItemInstance fairy = null;
            ItemInstance armor = null;
            ItemInstance weapon2 = null;
            ItemInstance weapon = null;
            if (Inventory != null)
            {
                fairy = Inventory.LoadBySlotAndType((byte)EquipmentType.Fairy, InventoryType.Wear);
                armor = Inventory.LoadBySlotAndType((byte)EquipmentType.Armor, InventoryType.Wear);
                weapon2 = Inventory.LoadBySlotAndType((byte)EquipmentType.SecondaryWeapon, InventoryType.Wear);
                weapon = Inventory.LoadBySlotAndType((byte)EquipmentType.MainWeapon, InventoryType.Wear);
            }

            bool isPvpPrimary = false;
            bool isPvpSecondary = false;
            bool isPvpArmor = false;

            if (weapon?.Item.Name.Contains(": ") == true)
            {
                isPvpPrimary = true;
            }
            isPvpSecondary |= weapon2?.Item.Name.Contains(": ") == true;
            isPvpArmor |= armor?.Item.Name.Contains(": ") == true;

            // tc_info 0 name 0 0 0 0 -1 - 0 0 0 0 0 0 0 0 0 0 0 wins deaths reput 0 0 0 morph
            // talentwin talentlose capitul rankingpoints arenapoints 0 0 ispvpprimary ispvpsecondary
            // ispvparmor herolvl desc
            return $"tc_info {Level} {Name} {fairy?.Item.Element ?? 0} {ElementRate} {(byte)Class} {(byte)Gender} {(Family != null ? $"{Family.FamilyId} {Family.Name}({Language.Instance.GetMessageFromKey(FamilyCharacter.Authority.ToString().ToUpper())})" : "-1 -")} {GetReputationIco()} {GetDignityIco()} {(weapon != null ? 1 : 0)} {weapon?.Rare ?? 0} {weapon?.Upgrade ?? 0} {(weapon2 != null ? 1 : 0)} {weapon2?.Rare ?? 0} {weapon2?.Upgrade ?? 0} {(armor != null ? 1 : 0)} {armor?.Rare ?? 0} {armor?.Upgrade ?? 0} {Act4Kill} {Act4Dead} {Reputation} 0 0 0 {(UseSp ? Morph : 0)} {TalentWin} {TalentLose} {TalentSurrender} 0 {MasterPoints} {Compliment} {Act4Points} {(isPvpPrimary ? 1 : 0)} {(isPvpSecondary ? 1 : 0)} {(isPvpArmor ? 1 : 0)} {HeroLevel} {(string.IsNullOrEmpty(Biography) ? Language.Instance.GetMessageFromKey("NO_PREZ_MESSAGE") : Biography)}";
        }

        public string GenerateRest() => $"rest 1 {CharacterId} {(IsSitting ? 1 : 0)}";

        public string GenerateRevive()
        {
            int lives = MapInstance.InstanceBag.Lives - MapInstance.InstanceBag.DeadList.Count + 1;
            return $"revive 1 {CharacterId} {(lives > 0 ? lives : 0)}";
        }

        public string GenerateSay(string message, int type, bool ignoreNickname = false) => $"say {(ignoreNickname ? 2 : 1)} {CharacterId} {type} {message}";

        public string GenerateScal() => $"char_sc 1 {CharacterId} {Size}";

        public List<string> GenerateScN()
        {
            List<string> list = new List<string>();
            byte i = 0;
            Mates.Where(s => s.MateType == MateType.Partner).ToList().ForEach(s =>
            {
                s.PetId = i;
                s.LoadInventory();
                list.Add(s.GenerateScPacket());
                i++;
            });
            return list;
        }

        public List<string> GenerateScP(byte Page = 0)
        {
            List<string> list = new List<string>();
            byte i = 0;
            Mates.Where(s => s.MateType == MateType.Pet).Skip(Page * 10).Take(10).ToList().ForEach(s =>
            {
                s.PetId = i;
                list.Add(s.GenerateScPacket());
                i++;
            });
            return list;
        }

        public string GenerateScpStc() => $"sc_p_stc {MaxMateCount / 10}";

        public string GenerateShop(string shopname) => $"shop 1 {CharacterId} 1 3 0 {shopname}";

        public string GenerateShopEnd() => $"shop 1 {CharacterId} 0 0";

        public string GenerateSki()
        {
            List<CharacterSkill> characterSkills = UseSp ? SkillsSp.GetAllItems() : Skills.GetAllItems();
            string skibase = string.Empty;
            if (!UseSp)
            {
                skibase = $"{200 + (20 * (byte)Class)} {201 + (20 * (byte)Class)}";
            }
            else if (characterSkills.Count > 0)
            {
                skibase = $"{characterSkills[0].SkillVNum} {characterSkills[0].SkillVNum}";
            }
            string generatedSkills = string.Empty;
            foreach (CharacterSkill ski in characterSkills)
            {
                generatedSkills += $" {ski.SkillVNum}";
            }

            return $"ski {skibase}{generatedSkills}";
        }

        public string GenerateSpk(object message, int type) => $"spk 1 {CharacterId} {type} {Name} {message}";

        public string GenerateSpPoint() => $"sp {SpAdditionPoint} 1000000 {SpPoint} 10000";

        [Obsolete("GenerateStartupInventory should be used only on startup, for refreshing an inventory slot please use GenerateInventoryAdd instead.")]
        public void GenerateStartupInventory()
        {
            string inv0 = "inv 0", inv1 = "inv 1", inv2 = "inv 2", inv3 = "inv 3", inv6 = "inv 6", inv7 = "inv 7"; // inv 3 used for miniland objects
            if (Inventory != null)
            {
                foreach (ItemInstance inv in Inventory.GetAllItems())
                {
                    switch (inv.Type)
                    {
                        case InventoryType.Equipment:
                            if (inv.Item.EquipmentSlot == EquipmentType.Sp)
                            {
                                inv0 += $" {inv.Slot}.{inv.ItemVNum}.{inv.Rare}.{inv.Upgrade}.{inv.SpStoneUpgrade}";
                            }
                            else
                            {
                                inv0 += $" {inv.Slot}.{inv.ItemVNum}.{inv.Rare}.{(inv.Item.IsColored ? inv.Design : inv.Upgrade)}.0";
                            }
                            break;

                        case InventoryType.Main:
                            inv1 += $" {inv.Slot}.{inv.ItemVNum}.{inv.Amount}.0";
                            break;

                        case InventoryType.Etc:
                            inv2 += $" {inv.Slot}.{inv.ItemVNum}.{inv.Amount}.0";
                            break;

                        case InventoryType.Miniland:
                            inv3 += $" {inv.Slot}.{inv.ItemVNum}.{inv.Amount}";
                            break;

                        case InventoryType.Specialist:
                            inv6 += $" {inv.Slot}.{inv.ItemVNum}.{inv.Rare}.{inv.Upgrade}.{inv.SpStoneUpgrade}";
                            break;

                        case InventoryType.Costume:
                            inv7 += $" {inv.Slot}.{inv.ItemVNum}.{inv.Rare}.{inv.Upgrade}.0";
                            break;
                    }
                }
            }
            Session.SendPacket(inv0);
            Session.SendPacket(inv1);
            Session.SendPacket(inv2);
            Session.SendPacket(inv3);
            Session.SendPacket(inv6);
            Session.SendPacket(inv7);
            Session.SendPacket(GetMinilandObjectList());
        }

        public string GenerateStashAll()
        {
            string stash = $"stash_all {WareHouseSize}";
            foreach (ItemInstance item in Inventory.Where(s => s.Type == InventoryType.Warehouse))
            {
                stash += $" {item.GenerateStashPacket()}";
            }
            return stash;
        }

        public string GenerateStat()
        {
            double option =
                (WhisperBlocked ? Math.Pow(2, (int)CharacterOption.WhisperBlocked - 1) : 0)
                + (FamilyRequestBlocked ? Math.Pow(2, (int)CharacterOption.FamilyRequestBlocked - 1) : 0)
                + (!MouseAimLock ? Math.Pow(2, (int)CharacterOption.MouseAimLock - 1) : 0)
                + (MinilandInviteBlocked ? Math.Pow(2, (int)CharacterOption.MinilandInviteBlocked - 1) : 0)
                + (ExchangeBlocked ? Math.Pow(2, (int)CharacterOption.ExchangeBlocked - 1) : 0)
                + (FriendRequestBlocked ? Math.Pow(2, (int)CharacterOption.FriendRequestBlocked - 1) : 0)
                + (EmoticonsBlocked ? Math.Pow(2, (int)CharacterOption.EmoticonsBlocked - 1) : 0)
                + (HpBlocked ? Math.Pow(2, (int)CharacterOption.HpBlocked - 1) : 0)
                + (BuffBlocked ? Math.Pow(2, (int)CharacterOption.BuffBlocked - 1) : 0)
                + (GroupRequestBlocked ? Math.Pow(2, (int)CharacterOption.GroupRequestBlocked - 1) : 0)
                + (HeroChatBlocked ? Math.Pow(2, (int)CharacterOption.HeroChatBlocked - 1) : 0)
                + (QuickGetUp ? Math.Pow(2, (int)CharacterOption.QuickGetUp - 1) : 0);
            return $"stat {Hp} {HPLoad()} {Mp} {MPLoad()} 0 {option}";
        }

        public string GenerateStatChar()
        {
            int weaponUpgrade = 0;
            int secondaryUpgrade = 0;
            int armorUpgrade = 0;
            MinHit = CharacterHelper.MinHit(Class, Level);
            MaxHit = CharacterHelper.MaxHit(Class, Level);
            HitRate = CharacterHelper.HitRate(Class, Level);
            HitCriticalRate = CharacterHelper.HitCriticalRate(Class, Level);
            HitCritical = CharacterHelper.HitCritical(Class, Level);
            MinDistance = CharacterHelper.MinDistance(Class, Level);
            MaxDistance = CharacterHelper.MaxDistance(Class, Level);
            DistanceRate = CharacterHelper.DistanceRate(Class, Level);
            DistanceCriticalRate = CharacterHelper.DistCriticalRate(Class, Level);
            DistanceCritical = CharacterHelper.DistCritical(Class, Level);
            FireResistance = GetStuffBuff(CardType.ElementResistance, (int)AdditionalTypes.ElementResistance.FireIncreased)[0] + GetStuffBuff(CardType.ElementResistance, (int)AdditionalTypes.ElementResistance.AllIncreased)[0];
            LightResistance = GetStuffBuff(CardType.ElementResistance, (int)AdditionalTypes.ElementResistance.LightIncreased)[0] + GetStuffBuff(CardType.ElementResistance, (int)AdditionalTypes.ElementResistance.AllIncreased)[0];
            WaterResistance = GetStuffBuff(CardType.ElementResistance, (int)AdditionalTypes.ElementResistance.WaterIncreased)[0] + GetStuffBuff(CardType.ElementResistance, (int)AdditionalTypes.ElementResistance.AllIncreased)[0];
            DarkResistance = GetStuffBuff(CardType.ElementResistance, (int)AdditionalTypes.ElementResistance.DarkIncreased)[0] + GetStuffBuff(CardType.ElementResistance, (int)AdditionalTypes.ElementResistance.AllIncreased)[0];
            Defence = CharacterHelper.Defence(Class, Level);
            DefenceRate = CharacterHelper.DefenceRate(Class, Level);
            ElementRate = 0;
            ElementRateSP = 0;
            DistanceDefence = CharacterHelper.DistanceDefence(Class, Level);
            DistanceDefenceRate = CharacterHelper.DistanceDefenceRate(Class, Level);
            MagicalDefence = CharacterHelper.MagicalDefence(Class, Level);
            if (UseSp)
            {
                // handle specialist
                ItemInstance specialist = Inventory?.LoadBySlotAndType((byte)EquipmentType.Sp, InventoryType.Wear);
                if (specialist != null)
                {
                    MinHit += specialist.DamageMinimum + (specialist.SpDamage * 10);
                    MaxHit += specialist.DamageMaximum + (specialist.SpDamage * 10);
                    MinDistance += specialist.DamageMinimum + (specialist.SpDamage * 10);
                    MaxDistance += specialist.DamageMaximum + (specialist.SpDamage * 10);
                    HitCriticalRate += specialist.CriticalLuckRate;
                    HitCritical += specialist.CriticalRate;
                    DistanceCriticalRate += specialist.CriticalLuckRate;
                    DistanceCritical += specialist.CriticalRate;
                    HitRate += specialist.HitRate;
                    DistanceRate += specialist.HitRate;
                    DefenceRate += specialist.DefenceDodge;
                    DistanceDefenceRate += specialist.DistanceDefenceDodge;
                    FireResistance += specialist.Item.FireResistance + specialist.SpFire;
                    WaterResistance += specialist.Item.WaterResistance + specialist.SpWater;
                    LightResistance += specialist.Item.LightResistance + specialist.SpLight;
                    DarkResistance += specialist.Item.DarkResistance + specialist.SpDark;
                    ElementRateSP += specialist.ElementRate + specialist.SpElement;
                    Defence += specialist.CloseDefence + (specialist.SpDefence * 10);
                    DistanceDefence += specialist.DistanceDefence + (specialist.SpDefence * 10);
                    MagicalDefence += specialist.MagicDefence + (specialist.SpDefence * 10);

                    ItemInstance mainWeapon = Inventory.LoadBySlotAndType((byte)EquipmentType.MainWeapon, InventoryType.Wear);
                    ItemInstance secondaryWeapon = Inventory.LoadBySlotAndType((byte)EquipmentType.SecondaryWeapon, InventoryType.Wear);
                    List<ShellEffectDTO> effects = new List<ShellEffectDTO>();
                    if (mainWeapon?.ShellEffects != null)
                    {
                        effects.AddRange(mainWeapon.ShellEffects);
                    }
                    if (secondaryWeapon?.ShellEffects != null)
                    {
                        effects.AddRange(secondaryWeapon.ShellEffects);
                    }

                    int GetShellWeaponEffectValue(ShellWeaponEffectType effectType)
                    {
                        return effects?.Where(s => s.Effect == (byte)effectType)?.OrderByDescending(s => s.Value)?.FirstOrDefault()?.Value ?? 0;
                    }

                    int point = CharacterHelper.SlPoint(specialist.SlDamage, 0) + GetShellWeaponEffectValue(ShellWeaponEffectType.SLDamage) + GetShellWeaponEffectValue(ShellWeaponEffectType.SLGlobal);

                    int p = 0;
                    if (point <= 10)
                    {
                        p = point * 5;
                    }
                    else if (point <= 20)
                    {
                        p = 50 + ((point - 10) * 6);
                    }
                    else if (point <= 30)
                    {
                        p = 110 + ((point - 20) * 7);
                    }
                    else if (point <= 40)
                    {
                        p = 180 + ((point - 30) * 8);
                    }
                    else if (point <= 50)
                    {
                        p = 260 + ((point - 40) * 9);
                    }
                    else if (point <= 60)
                    {
                        p = 350 + ((point - 50) * 10);
                    }
                    else if (point <= 70)
                    {
                        p = 450 + ((point - 60) * 11);
                    }
                    else if (point <= 80)
                    {
                        p = 560 + ((point - 70) * 13);
                    }
                    else if (point <= 90)
                    {
                        p = 690 + ((point - 80) * 14);
                    }
                    else if (point <= 94)
                    {
                        p = 830 + ((point - 90) * 15);
                    }
                    else if (point <= 95)
                    {
                        p = 890 + 16;
                    }
                    else if (point <= 97)
                    {
                        p = 906 + ((point - 95) * 17);
                    }
                    else if (point > 97)
                    {
                        p = 940 + ((point - 97) * 20);
                    }

                    MaxHit += p;
                    MinHit += p;
                    MaxDistance += p;
                    MinDistance += p;

                    point = CharacterHelper.SlPoint(specialist.SlDefence, 1) + GetShellWeaponEffectValue(ShellWeaponEffectType.SLDefence) + GetShellWeaponEffectValue(ShellWeaponEffectType.SLGlobal);
                    p = 0;
                    if (point <= 10)
                    {
                        p = point;
                    }
                    else if (point <= 20)
                    {
                        p = 10 + ((point - 10) * 2);
                    }
                    else if (point <= 30)
                    {
                        p = 30 + ((point - 20) * 3);
                    }
                    else if (point <= 40)
                    {
                        p = 60 + ((point - 30) * 4);
                    }
                    else if (point <= 50)
                    {
                        p = 100 + ((point - 40) * 5);
                    }
                    else if (point <= 60)
                    {
                        p = 150 + ((point - 50) * 6);
                    }
                    else if (point <= 70)
                    {
                        p = 210 + ((point - 60) * 7);
                    }
                    else if (point <= 80)
                    {
                        p = 280 + ((point - 70) * 8);
                    }
                    else if (point <= 90)
                    {
                        p = 360 + ((point - 80) * 9);
                    }
                    else if (point > 90)
                    {
                        p = 450 + ((point - 90) * 10);
                    }

                    Defence += p;
                    MagicalDefence += p;
                    DistanceDefence += p;

                    point = CharacterHelper.SlPoint(specialist.SlElement, 2) + GetShellWeaponEffectValue(ShellWeaponEffectType.SLElement) + GetShellWeaponEffectValue(ShellWeaponEffectType.SLGlobal);
                    p = point <= 50 ? point : 50 + ((point - 50) * 2);
                    ElementRateSP += p;

                    _slhpbonus = GetShellWeaponEffectValue(ShellWeaponEffectType.SLHP) + GetShellWeaponEffectValue(ShellWeaponEffectType.SLGlobal);
                }
            }

            // TODO: add base stats
            ItemInstance weapon = Inventory?.LoadBySlotAndType((byte)EquipmentType.MainWeapon, InventoryType.Wear);
            if (weapon != null)
            {
                weaponUpgrade = weapon.Upgrade;
                MinHit += weapon.DamageMinimum + weapon.Item.DamageMinimum;
                MaxHit += weapon.DamageMaximum + weapon.Item.DamageMaximum;
                HitRate += weapon.HitRate + weapon.Item.HitRate;
                HitCriticalRate += weapon.CriticalLuckRate + weapon.Item.CriticalLuckRate;
                HitCritical += weapon.CriticalRate + weapon.Item.CriticalRate;

                // maxhp-mp
            }

            ItemInstance weapon2 = Inventory?.LoadBySlotAndType((byte)EquipmentType.SecondaryWeapon, InventoryType.Wear);
            if (weapon2 != null)
            {
                secondaryUpgrade = weapon2.Upgrade;
                MinDistance += weapon2.DamageMinimum + weapon2.Item.DamageMinimum;
                MaxDistance += weapon2.DamageMaximum + weapon2.Item.DamageMaximum;
                DistanceRate += weapon2.HitRate + weapon2.Item.HitRate;
                DistanceCriticalRate += weapon2.CriticalLuckRate + weapon2.Item.CriticalLuckRate;
                DistanceCritical += weapon2.CriticalRate + weapon2.Item.CriticalRate;

                // maxhp-mp
            }

            ItemInstance armor = Inventory?.LoadBySlotAndType((byte)EquipmentType.Armor, InventoryType.Wear);
            if (armor != null)
            {
                armorUpgrade = armor.Upgrade;
                Defence += armor.CloseDefence + armor.Item.CloseDefence;
                DefenceRate += armor.DefenceDodge + armor.Item.DefenceDodge;
                MagicalDefence += armor.MagicDefence + armor.Item.MagicDefence;
                DistanceDefence += armor.DistanceDefence + armor.Item.DistanceDefence;
                DistanceDefenceRate += armor.DistanceDefenceDodge + armor.Item.DistanceDefenceDodge;
            }

            ItemInstance fairy = Inventory?.LoadBySlotAndType((byte)EquipmentType.Fairy, InventoryType.Wear);
            if (fairy != null)
            {
                ElementRate += fairy.ElementRate + fairy.Item.ElementRate + (IsUsingFairyBooster ? 30 : 0);
            }

            for (short i = 1; i < 14; i++)
            {
                ItemInstance item = Inventory?.LoadBySlotAndType(i, InventoryType.Wear);
                if (item != null && item.Item.EquipmentSlot != EquipmentType.MainWeapon
                        && item.Item.EquipmentSlot != EquipmentType.SecondaryWeapon
                        && item.Item.EquipmentSlot != EquipmentType.Armor
                        && item.Item.EquipmentSlot != EquipmentType.Sp)
                {
                    FireResistance += item.FireResistance + item.Item.FireResistance;
                    LightResistance += item.LightResistance + item.Item.LightResistance;
                    WaterResistance += item.WaterResistance + item.Item.WaterResistance;
                    DarkResistance += item.DarkResistance + item.Item.DarkResistance;
                    Defence += item.CloseDefence + item.Item.CloseDefence;
                    DefenceRate += item.DefenceDodge + item.Item.DefenceDodge;
                    MagicalDefence += item.MagicDefence + item.Item.MagicDefence;
                    DistanceDefence += item.DistanceDefence + item.Item.DistanceDefence;
                    DistanceDefenceRate += item.DistanceDefenceDodge + item.Item.DistanceDefenceDodge;
                }
            }
            byte type = Class == ClassType.Adventurer ? (byte)0 : (byte)(Class - 1);
            return $"sc {type} {weaponUpgrade} {MinHit} {MaxHit} {HitRate} {HitCriticalRate} {HitCritical} {(Class == ClassType.Archer ? 1 : 0)} {secondaryUpgrade} {MinDistance} {MaxDistance} {DistanceRate} {DistanceCriticalRate} {DistanceCritical} {armorUpgrade} {Defence} {DefenceRate} {DistanceDefence} {DistanceDefenceRate} {MagicalDefence} {FireResistance} {WaterResistance} {LightResistance} {DarkResistance}";
        }

        public string GenerateStatInfo() => $"st 1 {CharacterId} {Level} {HeroLevel} {(int)(Hp / (float)HPLoad() * 100)} {(int)(Mp / (float)MPLoad() * 100)} {Hp} {Mp}{Buff.GetAllItems().Aggregate(string.Empty, (current, buff) => current + $" {buff.Card.CardId}")}";

        public TalkPacket GenerateTalk(string message)
        {
            return new TalkPacket
            {
                CharacterId = CharacterId,
                Message = message
            };
        }

        public string GenerateTit() => $"tit {Language.Instance.GetMessageFromKey(Class == (byte)ClassType.Adventurer ? nameof(ClassType.Adventurer).ToUpper() : Class == ClassType.Swordman ? nameof(ClassType.Swordman).ToUpper() : Class == ClassType.Archer ? nameof(ClassType.Archer).ToUpper() : Class == ClassType.Magician ? nameof(ClassType.Magician).ToUpper()  : nameof(ClassType.Fighter).ToUpper())} {Name}";

        public string GenerateTp() => $"tp 1 {CharacterId} {PositionX} {PositionY} 0";

        public void GetAct4Points(int point)
        {
            //RefreshComplimentRankingIfNeeded();
            Act4Points += point;
        }

        public int[] GetBuff(CardType type, byte subtype)
        {
            int value1 = 0;
            int value2 = 0;

            foreach (BCard entry in EquipmentBCards.Where(s => s?.Type.Equals((byte)type) == true && s.SubType.Equals((byte)(subtype / 10))))
            {
                if (entry.IsLevelScaled)
                {
                    if (entry.IsLevelDivided)
                    {
                        value1 += Level / entry.FirstData;
                    }
                    else
                    {
                        value1 += entry.FirstData * Level;
                    }
                }
                else
                {
                    value1 += entry.FirstData;
                }
                value2 += entry.SecondData;
            }

            lock (Buff)
            {
                foreach (Buff buff in Buff.GetAllItems())
                {
                    // THIS ONE DOES NOT FOR STUFFS

                    foreach (BCard entry in buff.Card.BCards
                        .Where(s => s.Type.Equals((byte)type) && s.SubType.Equals((byte)(subtype / 10)) && (s.CastType != 1 || (s.CastType == 1 && buff.Start.AddMilliseconds(buff.Card.Delay * 100) < DateTime.Now))))
                    {
                        if (entry.IsLevelScaled)
                        {
                            if (entry.IsLevelDivided)
                            {
                                value1 += buff.Level / entry.FirstData;
                            }
                            else
                            {
                                value1 += entry.FirstData * buff.Level;
                            }
                        }
                        else
                        {
                            value1 += entry.FirstData;
                        }
                        value2 += entry.SecondData;
                    }
                }
            }

            return new[] { value1, value2 };
        }

        public int GetCP()
        {
            int cpmax = (Class > 0 ? 40 : 0) + (JobLevel * 2);
            int cpused = 0;
            foreach (CharacterSkill ski in Skills.GetAllItems())
            {
                cpused += ski.Skill.CPCost;
            }
            return cpmax - cpused;
        }

        public void GetDamage(int damage)
        {
            LastDefence = DateTime.Now;
            Dispose();

            Hp -= damage;
            if (Hp < 0)
            {
                Hp = 0;
            }
        }

        public int GetDignityIco()
        {
            int icoDignity = 1;

            if (Dignity <= -100)
            {
                icoDignity = 2;
            }
            if (Dignity <= -200)
            {
                icoDignity = 3;
            }
            if (Dignity <= -400)
            {
                icoDignity = 4;
            }
            if (Dignity <= -600)
            {
                icoDignity = 5;
            }
            if (Dignity <= -800)
            {
                icoDignity = 6;
            }

            return icoDignity;
        }

        public List<Portal> GetExtraPortal() => MapInstancePortalHandler.GenerateMinilandEntryPortals(MapInstance.Map.MapId, Miniland.MapInstanceId);

        public List<string> GetFamilyHistory()
        {
            //TODO: Fix some bugs(missing history etc)
            if (Family != null)
            {
                const string packetheader = "ghis";
                List<string> packetList = new List<string>();
                string packet = string.Empty;
                int i = 0;
                int amount = 0;
                foreach (FamilyLogDTO log in Family.FamilyLogs.Where(s => s.FamilyLogType != FamilyLogType.WareHouseAdded && s.FamilyLogType != FamilyLogType.WareHouseRemoved).OrderByDescending(s => s.Timestamp).Take(100))
                {
                    packet += $" {(byte)log.FamilyLogType}|{log.FamilyLogData}|{(int)(DateTime.Now - log.Timestamp).TotalHours}";
                    i++;
                    if (i == 50)
                    {
                        i = 0;
                        packetList.Add(packetheader + (amount == 0 ? " 0 " : string.Empty) + packet);
                        amount++;
                    }
                    else if (i + (50 * amount) == Family.FamilyLogs.Count)
                    {
                        packetList.Add(packetheader + (amount == 0 ? " 0 " : string.Empty) + packet);
                    }
                }

                return packetList;
            }
            return new List<string>();
        }

        public IEnumerable<string> GetMinilandEffects() => MinilandObjects.Select(mp => mp.GenerateMinilandEffect(false)).ToList();

        public string GetMinilandObjectList()
        {
            string mlobjstring = "mlobjlst";
            foreach (ItemInstance item in Inventory.Where(s => s.Type == InventoryType.Miniland).OrderBy(s => s.Slot))
            {
                if (item.Item.IsMinilandObject)
                {
                    WareHouseSize = item.Item.MinilandObjectPoint;
                }
                MinilandObject mp = MinilandObjects.Find(s => s.ItemInstanceId == item.Id);
                bool used = mp != null;
                mlobjstring += $" {item.Slot}.{(used ? 1 : 0)}.{(used ? mp.MapX : 0)}.{(used ? mp.MapY : 0)}.{(item.Item.Width != 0 ? item.Item.Width : 1) }.{(item.Item.Height != 0 ? item.Item.Height : 1) }.{(used ? mp.ItemInstance.DurabilityPoint : 0)}.100.0.1";
            }

            return mlobjstring;
        }

        public void GetReferrerReward()
        {
            long referrerId = Session.Account.ReferrerId;
            if (Level >= 70 && referrerId != 0 && !CharacterId.Equals(referrerId))
            {
                List<GeneralLogDTO> logs = DAOFactory.GeneralLogDAO.LoadByLogType("ReferralProgram", null).Where(g => g.IpAddress.Equals(Session.Account.RegistrationIP.Split(':')[1].Replace("//", ""))).ToList();
                if (logs.Count <= 5)
                {
                    CharacterDTO character = DAOFactory.CharacterDAO.LoadById(referrerId);
                    if (character == null || character.Level < 70)
                    {
                        return;
                    }
                    AccountDTO referrer = DAOFactory.AccountDAO.LoadById(character.AccountId);
                    if (referrer != null && !AccountId.Equals(character.AccountId))
                    {
                        Logger.LogUserEvent("REFERRERREWARD", Session.GenerateIdentity(), $"AccountId: {AccountId} ReferrerId: {referrerId}");
                        DAOFactory.AccountDAO.WriteGeneralLog(AccountId, Session.Account.RegistrationIP, CharacterId, GeneralLogType.ReferralProgram, $"ReferrerId: {referrerId}");

                        // send gifts like you want
                        //SendGift(CharacterId, 5910, 1, 0, 0, false);
                        //SendGift(referrerId, 5910, 1, 0, 0, false);
                    }
                }
            }
        }

        public int GetReputationIco()
        {
            if (Reputation >= 5000001)
            {
                switch (IsReputationHero())
                {
                    case 1:
                        return 28;

                    case 2:
                        return 29;

                    case 3:
                        return 30;

                    case 4:
                        return 31;

                    case 5:
                        return 32;
                }
            }
            if (Reputation <= 50)
            {
                return 1;
            }

            if (Reputation <= 150)
            {
                return 2;
            }

            if (Reputation <= 250)
            {
                return 3;
            }

            if (Reputation <= 500)
            {
                return 4;
            }

            if (Reputation <= 750)
            {
                return 5;
            }

            if (Reputation <= 1000)
            {
                return 6;
            }

            if (Reputation <= 2250)
            {
                return 7;
            }

            if (Reputation <= 3500)
            {
                return 8;
            }

            if (Reputation <= 5000)
            {
                return 9;
            }

            if (Reputation <= 9500)
            {
                return 10;
            }

            if (Reputation <= 19000)
            {
                return 11;
            }

            if (Reputation <= 25000)
            {
                return 12;
            }

            if (Reputation <= 40000)
            {
                return 13;
            }

            if (Reputation <= 60000)
            {
                return 14;
            }

            if (Reputation <= 85000)
            {
                return 15;
            }

            if (Reputation <= 115000)
            {
                return 16;
            }

            if (Reputation <= 150000)
            {
                return 17;
            }

            if (Reputation <= 190000)
            {
                return 18;
            }

            if (Reputation <= 235000)
            {
                return 19;
            }

            if (Reputation <= 285000)
            {
                return 20;
            }

            if (Reputation <= 350000)
            {
                return 21;
            }

            if (Reputation <= 500000)
            {
                return 22;
            }

            if (Reputation <= 1500000)
            {
                return 23;
            }

            if (Reputation <= 2500000)
            {
                return 24;
            }

            if (Reputation <= 3750000)
            {
                return 25;
            }

            return Reputation <= 5000000 ? 26 : 27;
        }

        /// <summary>
        /// Get Stuff Buffs Useful for Stats for example
        /// </summary>
        /// <param name="type"></param>
        /// <param name="subtype"></param>
        /// <returns></returns>
        public int[] GetStuffBuff(CardType type, byte subtype)
        {
            int value1 = 0;
            int value2 = 0;
            foreach (BCard entry in EquipmentBCards.Where(s => s.Type.Equals((byte)type) && s.SubType.Equals((byte)(subtype / 10))))
            {
                if (entry.IsLevelScaled)
                {
                    value1 += entry.FirstData * Level;
                }
                else
                {
                    value1 += entry.FirstData;
                }
                value2 += entry.SecondData;
            }

            return new[] { value1, value2 };
        }

        public void GiftAdd(short itemVNum, byte amount, byte rare = 0, byte upgrade = 0, short design = 0, bool forceRandom = false, byte minRare = 0)
        {
            //TODO add the rare support
            if (Inventory != null)
            {
                lock (Inventory)
                {
                    ItemInstance newItem = Inventory.InstantiateItemInstance(itemVNum, CharacterId, amount);
                    if (newItem != null)
                    {
                        newItem.Design = design;

                        if (newItem.Item.ItemType == ItemType.Armor || newItem.Item.ItemType == ItemType.Weapon || newItem.Item.ItemType == ItemType.Shell || forceRandom)
                        {
                            if (rare != 0 && !forceRandom)
                            {
                                try
                                {
                                    newItem.RarifyItem(Session, RarifyMode.Drop, RarifyProtection.None, forceRare: rare);
                                    newItem.Upgrade = (byte)(newItem.Item.BasicUpgrade + upgrade);
                                    if (newItem.Upgrade > 10)
                                    {
                                        newItem.Upgrade = 10;
                                    }
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                            }
                            else if (rare == 0 || forceRandom)
                            {
                                do
                                {
                                    try
                                    {
                                        newItem.RarifyItem(Session, RarifyMode.Drop, RarifyProtection.None);
                                        newItem.Upgrade = newItem.Item.BasicUpgrade;
                                        if (newItem.Rare >= minRare)
                                        {
                                            break;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        break;
                                    }
                                } while (forceRandom);
                            }
                        }

                        if (newItem.Item.Type.Equals(InventoryType.Equipment) && rare != 0 && !forceRandom)
                        {
                            newItem.Rare = (sbyte)rare;
                            newItem.SetRarityPoint();
                        }

                        if (newItem.Item.ItemType == ItemType.Shell)
                        {
                            newItem.Upgrade = (byte)ServerManager.RandomNumber(50, 81);
                        }

                        List<ItemInstance> newInv = Inventory.AddToInventory(newItem);
                        if (newInv.Count > 0)
                        {
                            Session.SendPacket(GenerateSay($"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {newItem.Item.Name} x {amount}", 10));
                        }
                        else if (MailList.Count <= 40 && design == 0)
                        {
                            SendGift(CharacterId, itemVNum, amount, newItem.Rare, newItem.Upgrade, false);
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_ACQUIRED_BY_THE_GIANT_MONSTER"), 0));
                        }
                    }
                }
            }
        }

        public bool HaveBackpack() => StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.BackPack);

        public double HPLoad()
        {
            double multiplicator = 5.0;
            int hp = 0;
            if (UseSp)
            {
                ItemInstance specialist = Inventory?.LoadBySlotAndType((byte)EquipmentType.Sp, InventoryType.Wear);
                if (specialist != null)
                {
                    int point = CharacterHelper.SlPoint(specialist.SlHP, 3) + _slhpbonus;

                    if (point <= 50)
                    {
                        multiplicator += point / 100.0;
                    }
                    else
                    {
                        multiplicator += 0.5 + ((point - 50.00) / 50.00);
                    }
                    hp = specialist.HP + (specialist.SpHP * 100);
                }
            }
            hp += CellonOptions.Where(s => s.Type == CellonOptionType.HPMax).Sum(s => s.Value);
            multiplicator += GetBuff(CardType.BearSpirit, (int)AdditionalTypes.BearSpirit.IncreaseMaximumHP)[0] / 100D;
            multiplicator += GetBuff(CardType.MaxHPMP, (int)AdditionalTypes.MaxHPMP.IncreasesMaximumHP)[0] / 100D;

            HPMax = (int)((CharacterHelper.HPData[(int)Class, Level] + hp + GetBuff(CardType.MaxHPMP, (byte)AdditionalTypes.MaxHPMP.MaximumHPIncreased)[0] + GetBuff(CardType.MaxHPMP, (byte)AdditionalTypes.MaxHPMP.MaximumHPMPIncreased)[0]) * multiplicator);
            return HPMax;
        }

        public void IncreaseDollars(int amount)
        {
            try
            {
                if (!ServerManager.Instance.MallApi.SendCurrencyAsync(ServerManager.Instance.Configuration.MallAPIKey, Session.Account.AccountId, amount).Result)
                {
                    Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("MALL_ACCOUNT_NOT_EXISTING"), 10));
                    return;
                }
            }
            catch
            {
                Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("MALL_UNKNOWN_ERROR"), 10));
                return;
            }

            Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MALL_CURRENCY_RECEIVE"), amount), 10));
        }

        public void Initialize()
        {
            _random = new Random();
            ExchangeInfo = null;
            SpCooldown = 30;
            SaveX = 0;
            SaveY = 0;
            LastDefence = DateTime.Now.AddSeconds(-21);
            LastDelay = DateTime.Now.AddSeconds(-5);
            LastHealth = DateTime.Now;
            LastEffect = DateTime.Now;
            Session = null;
            MailList = new Dictionary<int, MailDTO>();
            BattleEntity = new BattleEntity(this);
            Group = null;
            GmPvtBlock = false;
        }

        public static void InsertOrUpdatePenalty(PenaltyLogDTO log)
        {
            DAOFactory.PenaltyLogDAO.InsertOrUpdate(ref log);
            CommunicationServiceClient.Instance.RefreshPenalty(log.PenaltyLogId);
        }

        public bool IsBlockedByCharacter(long characterId) => CharacterRelations.Any(b => b.RelationType == CharacterRelationType.Blocked && b.CharacterId.Equals(characterId) && characterId != CharacterId);

        public bool IsBlockingCharacter(long characterId) => CharacterRelations.Any(c => c.RelationType == CharacterRelationType.Blocked && c.RelatedCharacterId.Equals(characterId));

        public bool IsFriendlistFull() => CharacterRelations.Where(s => s.RelationType == CharacterRelationType.Friend).ToList().Count >= 80;

        public bool IsFriendOfCharacter(long characterId) => CharacterRelations.Any(c => c.RelationType == CharacterRelationType.Friend && (c.RelatedCharacterId.Equals(characterId) || c.CharacterId.Equals(characterId)));

        /// <summary>
        /// Checks if the current character is in range of the given position
        /// </summary>
        /// <param name="xCoordinate">The x coordinate of the object to check.</param>
        /// <param name="yCoordinate">The y coordinate of the object to check.</param>
        /// <param name="range">The range of the coordinates to be maximal distanced.</param>
        /// <returns>True if the object is in Range, False if not.</returns>
        public bool IsInRange(int xCoordinate, int yCoordinate, int range = 50) => Math.Abs(PositionX - xCoordinate) <= range && Math.Abs(PositionY - yCoordinate) <= range;

        public bool IsMuted() => Session.Account.PenaltyLogs.Any(s => s.Penalty == PenaltyType.Muted && s.DateEnd > DateTime.Now);

        public int IsReputationHero()
        {
            int i = 0;
            foreach (CharacterDTO character in ServerManager.Instance.TopReputation)
            {
                i++;
                if (character.CharacterId == CharacterId)
                {
                    if (i == 1)
                    {
                        return 5;
                    }
                    if (i == 2)
                    {
                        return 4;
                    }
                    if (i == 3)
                    {
                        return 3;
                    }
                    if (i <= 13)
                    {
                        return 2;
                    }
                    if (i <= 43)
                    {
                        return 1;
                    }
                }
            }
            return 0;
        }

        public void LearnAdventurerSkill()
        {
            if (Class == 0)
            {
                byte NewSkill = 0;
                for (int i = 200; i <= 210; i++)
                {
                    if (i == 209)
                    {
                        i++;
                    }

                    Skill skinfo = ServerManager.GetSkill((short)i);
                    if (skinfo.Class == 0 && JobLevel >= skinfo.LevelMinimum && Skills.All(s => s.SkillVNum != i))
                    {
                        NewSkill = 1;
                        Skills[i] = new CharacterSkill { SkillVNum = (short)i, CharacterId = CharacterId };
                    }
                }
                if (NewSkill > 0)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SKILL_LEARNED"), 0));
                    Session.SendPacket(GenerateSki());
                    Session.SendPackets(GenerateQuicklist());
                }
            }
        }

        public void LearnSPSkill()
        {
            ItemInstance specialist = null;
            if (Inventory != null)
            {
                specialist = Inventory.LoadBySlotAndType((byte)EquipmentType.Sp, InventoryType.Wear);
            }
            byte SkillSpCount = (byte)SkillsSp.Count;
            SkillsSp = new ThreadSafeSortedList<int, CharacterSkill>();
            foreach (Skill ski in ServerManager.GetAllSkill())
            {
                if (specialist != null && ski.Class == Morph + 31 && specialist.SpLevel >= ski.LevelMinimum)
                {
                    SkillsSp[ski.SkillVNum] = new CharacterSkill { SkillVNum = ski.SkillVNum, CharacterId = CharacterId };
                }
            }
            if (SkillsSp.Count != SkillSpCount)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SKILL_LEARNED"), 0));
            }
        }

        public void LoadInventory()
        {
            IEnumerable<ItemInstanceDTO> inventories = DAOFactory.IteminstanceDAO.LoadByCharacterId(CharacterId).Where(s => s.Type != InventoryType.FamilyWareHouse).ToList();
            IEnumerable<CharacterDTO> characters = DAOFactory.CharacterDAO.LoadAllByAccount(Session.Account.AccountId);
            foreach (CharacterDTO character in characters.Where(s => s.CharacterId != CharacterId))
            {
                inventories = inventories.Concat(DAOFactory.IteminstanceDAO.LoadByCharacterId(character.CharacterId).Where(s => s.Type == InventoryType.Warehouse).ToList());
            }
            Inventory = new Inventory(this);
            foreach (ItemInstanceDTO inventory in inventories)
            {
                inventory.CharacterId = CharacterId;
                Inventory[inventory.Id] = new ItemInstance(inventory);
            }
        }

        public void LoadQuicklists()
        {
            QuicklistEntries = new List<QuicklistEntryDTO>();
            IEnumerable<QuicklistEntryDTO> quicklistDTO = DAOFactory.QuicklistEntryDAO.LoadByCharacterId(CharacterId).ToList();
            foreach (QuicklistEntryDTO qle in quicklistDTO)
            {
                QuicklistEntries.Add(qle);
            }
        }

        public void LoadSentMail()
        {
            foreach (MailDTO mail in DAOFactory.MailDAO.LoadSentByCharacter(CharacterId))
            {
                MailList.Add((MailList.Count > 0 ? MailList.OrderBy(s => s.Key).Last().Key : 0) + 1, mail);

                Session.SendPacket(GeneratePost(mail, 2));
            }
        }

        public void LoadSkills()
        {
            Skills = new ThreadSafeSortedList<int, CharacterSkill>();
            IEnumerable<CharacterSkillDTO> characterskillDTO = DAOFactory.CharacterSkillDAO.LoadByCharacterId(CharacterId).ToList();
            foreach (CharacterSkillDTO characterskill in characterskillDTO.OrderBy(s => s.SkillVNum))
            {
                if (!Skills.ContainsKey(characterskill.SkillVNum))
                {
                    Skills[characterskill.SkillVNum] = new CharacterSkill(characterskill);
                }
            }
        }

        public void LoadSpeed()
        {
            // only load speed if you dont use custom speed
            if (!IsVehicled && !IsCustomSpeed)
            {
                Speed = CharacterHelper.SpeedData[(byte)Class];

                if (UseSp)
                {
                    ItemInstance specialist = Inventory?.LoadBySlotAndType((byte)EquipmentType.Sp, InventoryType.Wear);
                    if (specialist != null)
                    {
                        Speed += specialist.Item.Speed;
                    }
                }

                byte fixSpeed = (byte)GetBuff(CardType.Move, (byte)AdditionalTypes.Move.SetMovement)[0];
                if (fixSpeed != 0)
                {
                    Speed = fixSpeed;
                }
                else
                {
                    Speed += (byte)GetBuff(CardType.Move, (byte)AdditionalTypes.Move.MovementSpeedIncreased)[0];
                    Speed *= (byte)(1 + (GetBuff(CardType.Move, (byte)AdditionalTypes.Move.MoveSpeedIncreased)[0] / 100D));
                }
            }

            if (IsShopping)
            {
                Speed = 0;
                IsCustomSpeed = false;
                return;
            }

            // reload vehicle speed after opening an shop for instance
            if (IsVehicled && !IsCustomSpeed)
            {
                Speed = VehicleSpeed;
            }
        }

        public double MPLoad()
        {
            int mp = 0;
            double multiplicator = 5.0;
            if (UseSp)
            {
                ItemInstance specialist = Inventory?.LoadBySlotAndType((byte)EquipmentType.Sp, InventoryType.Wear);
                if (specialist != null)
                {
                    int point = CharacterHelper.SlPoint(specialist.SlHP, 3);

                    if (point <= 50)
                    {
                        multiplicator += point / 100.0;
                    }
                    else
                    {
                        multiplicator += 0.5 + ((point - 50.00) / 50.00);
                    }
                    mp = specialist.MP + (specialist.SpHP * 100);
                }
            }
            mp += CellonOptions.Where(s => s.Type == CellonOptionType.MPMax).Sum(s => s.Value);

            multiplicator += GetBuff(CardType.BearSpirit, (int)AdditionalTypes.BearSpirit.IncreaseMaximumMP)[0] / 100D;
            multiplicator += GetBuff(CardType.MaxHPMP, (int)AdditionalTypes.MaxHPMP.IncreasesMaximumMP)[0] / 100D;

            MPMax = (int)((CharacterHelper.MPData[(int)Class, Level] + mp + GetBuff(CardType.MaxHPMP, (int)AdditionalTypes.MaxHPMP.MaximumMPIncreased)[0] + GetBuff(CardType.MaxHPMP, (int)AdditionalTypes.MaxHPMP.MaximumHPMPIncreased)[0]) * multiplicator);
            return MPMax;
        }

        public bool MuteMessage()
        {
            PenaltyLogDTO penalty = Session.Account.PenaltyLogs.OrderByDescending(s => s.DateEnd).FirstOrDefault();
            if (IsMuted() && penalty != null)
            {
                Session.CurrentMapInstance?.Broadcast(Gender == GenderType.Female ? GenerateSay(Language.Instance.GetMessageFromKey("MUTED_FEMALE"), 1) : GenerateSay(Language.Instance.GetMessageFromKey("MUTED_MALE"), 1));
                Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"), (penalty.DateEnd - DateTime.Now).ToString(@"hh\:mm\:ss")), 11));
                Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"), (penalty.DateEnd - DateTime.Now).ToString(@"hh\:mm\:ss")), 12));
                return true;
            }
            return false;
        }

        public string OpenFamilyWarehouse()
        {
            if (Family == null || Family.WarehouseSize == 0)
            {
                return UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("NO_FAMILY_WAREHOUSE"));
            }
            return GenerateFStashAll();
        }

        public List<string> OpenFamilyWarehouseHist()
        {
            List<string> packetList = new List<string>();
            if (Family == null || !(FamilyCharacter.Authority == FamilyAuthority.Head
                || FamilyCharacter.Authority == FamilyAuthority.Assistant
                || (FamilyCharacter.Authority == FamilyAuthority.Member && Family.MemberCanGetHistory)
                || (FamilyCharacter.Authority == FamilyAuthority.Manager && Family.ManagerCanGetHistory)))
            {
                packetList.Add(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("NO_FAMILY_RIGHT")));
                return packetList;
            }
            return GenerateFamilyWarehouseHist();
        }

        public void LoadMail()
        {
            int parcel = 0, letter = 0;
            foreach (MailDTO mail in DAOFactory.MailDAO.LoadSentToCharacter(CharacterId))
            {
                MailList.Add((MailList.Count > 0 ? MailList.OrderBy(s => s.Key).Last().Key : 0) + 1, mail);

                if (mail.AttachmentVNum != null)
                {
                    parcel++;
                    Session.SendPacket(GenerateParcel(mail));
                }
                else
                {
                    if (!mail.IsOpened)
                    {
                        letter++;
                    }
                    Session.SendPacket(GeneratePost(mail, 1));
                }
            }
            if (parcel > 0)
            {
                Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("GIFTED"), parcel), 11));
            }
            if (letter > 0)
            {
                Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("NEW_MAIL"), letter), 10));
            }
        }

        public void RemoveBuff(short id)
        {
            Buff indicator = Buff[id];
            if (indicator != null)
            {
                if (indicator.StaticBuff)
                {
                    Session.SendPacket($"vb {indicator.Card.CardId} 0 {indicator.Card.Duration}");
                    Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("EFFECT_TERMINATED"), indicator.Card.Name), 11));
                }
                else
                {
                    Session.SendPacket($"bf 1 {CharacterId} 0.{indicator.Card.CardId}.0 {Level}");
                    Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("EFFECT_TERMINATED"), indicator.Card.Name), 20));
                }

                if (Buff[indicator.Card.CardId] != null)
                {
                    Buff.Remove(id);
                }
                if (indicator.Card.BCards.Any(s => s.Type == (byte)CardType.Move && !s.SubType.Equals((byte)AdditionalTypes.Move.MovementImpossible / 10)))
                {
                    LastSpeedChange = DateTime.Now;
                    LoadSpeed();
                    Session.SendPacket(GenerateCond());
                }
                if (indicator.Card.BCards.Any(s => s.Type == (byte)CardType.SpecialAttack && s.SubType.Equals((byte)AdditionalTypes.SpecialAttack.NoAttack / 10)))
                {
                    NoAttack = false;
                    Session.SendPacket(GenerateCond());
                }             
                if (indicator.Card.BCards.Any(s => s.Type == (byte)CardType.Move && s.SubType.Equals((byte)AdditionalTypes.Move.MovementImpossible / 10)))
                {
                    NoMove = false;
                    Session.SendPacket(GenerateCond());
                }                           
                    // TODO : Find another way because it is hardcode
                    if (indicator.Card.CardId == 131)
                {
                    Session.SendPacket(GeneratePairy());
                }
            }
        }

        public void RemoveVehicle()
        {
            ItemInstance sp = null;
            if (Inventory != null)
            {
                sp = Inventory.LoadBySlotAndType((byte)EquipmentType.Sp, InventoryType.Wear);
            }
            IsVehicled = false;
            LoadSpeed();
            if (UseSp)
            {
                if (sp != null)
                {
                    Morph = sp.Item.Morph;
                    MorphUpgrade = sp.Upgrade;
                    MorphUpgrade2 = sp.Design;
                }
            }
            else
            {
                Morph = 0;
            }
            Session.CurrentMapInstance?.Broadcast(GenerateCMode());
            Session.SendPacket(GenerateCond());
            LastSpeedChange = DateTime.Now;
        }

        public void Rest()
        {
            if (LastSkillUse.AddSeconds(4) > DateTime.Now || LastDefence.AddSeconds(4) > DateTime.Now)
            {
                return;
            }
            if (!IsVehicled)
            {
                IsSitting = !IsSitting;
                Session.CurrentMapInstance?.Broadcast(GenerateRest());
            }
            else
            {
                Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("IMPOSSIBLE_TO_USE"), 10));
            }
        }

        public void Save()
        {
            Logger.LogUserEvent("CHARACTER_DB_SAVE", Session.GenerateIdentity(), "START");
            try
            {
                AccountDTO account = Session.Account;
                DAOFactory.AccountDAO.InsertOrUpdate(ref account);

                CharacterDTO character = DeepCopy();
                DAOFactory.CharacterDAO.InsertOrUpdate(ref character);

                if (Inventory != null)
                {
                    // be sure that noone tries to edit while saving is currently editing
                    lock (Inventory)
                    {
                        // load and concat inventory with equipment
                        List<ItemInstance> inventories = Inventory.GetAllItems();
                        IEnumerable<Guid> currentlySavedInventoryIds = DAOFactory.IteminstanceDAO.LoadSlotAndTypeByCharacterId(CharacterId);
                        IEnumerable<CharacterDTO> characters = DAOFactory.CharacterDAO.LoadAllCharactersByAccount(Session.Account.AccountId);
                        foreach (CharacterDTO characteraccount in characters.Where(s => s.CharacterId != CharacterId))
                        {
                            currentlySavedInventoryIds = currentlySavedInventoryIds.Concat(DAOFactory.IteminstanceDAO.LoadByCharacterId(characteraccount.CharacterId).Where(s => s.Type == InventoryType.Warehouse).Select(i => i.Id).ToList());
                        }

                        IEnumerable<MinilandObjectDTO> currentlySavedMinilandObjectEntries = DAOFactory.MinilandObjectDAO.LoadByCharacterId(CharacterId).ToList();
                        foreach (MinilandObjectDTO mobjToDelete in currentlySavedMinilandObjectEntries.Except(MinilandObjects))
                        {
                            DAOFactory.MinilandObjectDAO.DeleteById(mobjToDelete.MinilandObjectId);
                        }

                        DAOFactory.IteminstanceDAO.DeleteGuidList(currentlySavedInventoryIds.Except(inventories.Select(i => i.Id)));

                        // create or update all which are new or do still exist
                        List<ItemInstance> saveInventory = inventories.Where(s => s.Type != InventoryType.Bazaar && s.Type != InventoryType.FamilyWareHouse).ToList();

                        DAOFactory.IteminstanceDAO.InsertOrUpdateFromList(saveInventory);

                        foreach (ItemInstance itemInstance in saveInventory)
                        
                        {
                            DAOFactory.ShellEffectDAO.InsertOrUpdateFromList(itemInstance.ShellEffects, itemInstance.EquipmentSerialId);
                            DAOFactory.CellonOptionDAO.InsertOrUpdateFromList(itemInstance.CellonOptions, itemInstance.EquipmentSerialId);
                            //foreach (ShellEffectDTO effect in wearInstance.ShellEffects)
                            //{
                            //    effect.EquipmentSerialId = wearInstance.EquipmentSerialId;
                            //    effect.ShellEffectId = DAOFactory.ShellEffectDAO.InsertOrUpdate(effect).ShellEffectId;
                            //}
                            //foreach (CellonOptionDTO effect in wearInstance.CellonOptions)
                            //{
                            //    effect.EquipmentSerialId = wearInstance.EquipmentSerialId;
                            //    effect.CellonOptionId = DAOFactory.CellonOptionDAO.InsertOrUpdate(effect).CellonOptionId;
                            //}
                        }
                    }
                }

                

                if (Skills != null)
                {
                    IEnumerable<Guid> currentlySavedCharacterSkills = DAOFactory.CharacterSkillDAO.LoadKeysByCharacterId(CharacterId).ToList();

                    foreach (Guid characterSkillToDeleteId in currentlySavedCharacterSkills.Except(Skills.Select(s => s.Id)))
                    {
                        DAOFactory.CharacterSkillDAO.Delete(characterSkillToDeleteId);
                    }

                    foreach (CharacterSkill characterSkill in Skills.GetAllItems())
                    {
                        DAOFactory.CharacterSkillDAO.InsertOrUpdate(characterSkill);
                    }
                }

                IEnumerable<long> currentlySavedMates = DAOFactory.MateDAO.LoadByCharacterId(CharacterId).Select(s => s.MateId);

                foreach (long matesToDeleteId in currentlySavedMates.Except(Mates.Select(s => s.MateId)))
                {
                    DAOFactory.MateDAO.Delete(matesToDeleteId);
                }

                foreach (Mate mate in Mates)
                {
                    MateDTO matesave = mate;
                    DAOFactory.MateDAO.InsertOrUpdate(ref matesave);
                }

                IEnumerable<QuicklistEntryDTO> quickListEntriesToInsertOrUpdate = QuicklistEntries.ToList();

                IEnumerable<Guid> currentlySavedQuicklistEntries = DAOFactory.QuicklistEntryDAO.LoadKeysByCharacterId(CharacterId).ToList();
                foreach (Guid quicklistEntryToDelete in currentlySavedQuicklistEntries.Except(QuicklistEntries.Select(s => s.Id)))
                {
                    DAOFactory.QuicklistEntryDAO.Delete(quicklistEntryToDelete);
                }
                foreach (QuicklistEntryDTO quicklistEntry in quickListEntriesToInsertOrUpdate)
                {
                    DAOFactory.QuicklistEntryDAO.InsertOrUpdate(quicklistEntry);
                }

                foreach (MinilandObjectDTO mobjEntry in (IEnumerable<MinilandObjectDTO>)MinilandObjects.ToList())
                {
                    MinilandObjectDTO mobj = mobjEntry;
                    DAOFactory.MinilandObjectDAO.InsertOrUpdate(ref mobj);
                }

                IEnumerable<short> currentlySavedBuff = DAOFactory.StaticBuffDAO.LoadByTypeCharacterId(CharacterId);
                foreach (short bonusToDelete in currentlySavedBuff.Except(Buff.Select(s => s.Card.CardId)))
                {
                    DAOFactory.StaticBuffDAO.Delete(bonusToDelete, CharacterId);
                }
                if (_isStaticBuffListInitial)
                {
                    foreach (Buff buff in Buff.Where(s => s.StaticBuff).ToArray())
                    {
                        StaticBuffDTO bf = new StaticBuffDTO
                        {
                            CharacterId = CharacterId,
                            RemainingTime = (int)(buff.RemainingTime - (DateTime.Now - buff.Start).TotalSeconds),
                            CardId = buff.Card.CardId
                        };
                        DAOFactory.StaticBuffDAO.InsertOrUpdate(ref bf);
                    }
                }

                foreach (StaticBonusDTO bonus in StaticBonusList.ToArray())
                {
                    StaticBonusDTO bonus2 = bonus;
                    DAOFactory.StaticBonusDAO.InsertOrUpdate(ref bonus2);
                }

                foreach (GeneralLogDTO general in GeneralLogs.GetAllItems())
                {
                    if (!DAOFactory.GeneralLogDAO.IdAlreadySet(general.LogId))
                    {
                        DAOFactory.GeneralLogDAO.Insert(general);
                    }
                }
                foreach (RespawnDTO Resp in Respawns)
                {
                    RespawnDTO res = Resp;
                    if (Resp.MapId != 0 && Resp.X != 0 && Resp.Y != 0)
                    {
                        DAOFactory.RespawnDAO.InsertOrUpdate(ref res);
                    }
                }
                Logger.LogUserEvent("CHARACTER_DB_SAVE", Session.GenerateIdentity(), "FINISH");
            }
            catch (Exception e)
            {
                Logger.LogUserEventError("CHARACTER_DB_SAVE", Session.GenerateIdentity(), "ERROR", e);
            }
        }

        public void SendGift(long id, short vnum, short amount, sbyte rare, byte upgrade, bool isNosmall)
        {
            Item it = ServerManager.GetItem(vnum);

            if (it != null)
            {
                if (it.ItemType != ItemType.Weapon && it.ItemType != ItemType.Armor && it.ItemType != ItemType.Specialist)
                {
                    upgrade = 0;
                }
                else if (it.ItemType != ItemType.Weapon && it.ItemType != ItemType.Armor)
                {
                    rare = 0;
                }
                if (rare > 8 || rare < -2)
                {
                    rare = 0;
                }
                if (upgrade > 10 && it.ItemType != ItemType.Specialist)
                {
                    upgrade = 0;
                }
                else if (it.ItemType == ItemType.Specialist && upgrade > 15)
                {
                    upgrade = 0;
                }

                // maximum size of the amount is 999
                if (amount > 999)
                {
                    amount = 999;
                }

                MailDTO mail = new MailDTO
                {
                    AttachmentAmount = it.Type == InventoryType.Etc || it.Type == InventoryType.Main ? amount : (short)1,
                    IsOpened = false,
                    Date = DateTime.Now,
                    ReceiverId = id,
                    SenderId = CharacterId,
                    AttachmentRarity = (byte)rare,
                    AttachmentUpgrade = upgrade,
                    IsSenderCopy = false,
                    Title = isNosmall ? "NosHeat" : Name,
                    AttachmentVNum = vnum,
                    SenderClass = Class,
                    SenderGender = Gender,
                    SenderHairColor = HairColor,
                    SenderHairStyle = HairStyle,
                    EqPacket = GenerateEqListForPacket(),
                    SenderMorphId = Morph == 0 ? (short)-1 : (short)(Morph > short.MaxValue ? 0 : Morph)
                };
                MailServiceClient.Instance.SendMail(mail);
            }
        }

        public void SetReputation(int val)
        {
            Reputation += val;
            Session.SendPacket(GenerateFd());
            if (val > 0)
            {
                Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("REPUT_INCREASE"), val), 11));
            }
            else
            {
                Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("REPUT_DECREASE"), val), 12));
            }
        }

        public void SetRespawnPoint(short mapId, short mapX, short mapY)
        {
            if (Session.HasCurrentMapInstance && Session.CurrentMapInstance.Map.MapTypes.Count > 0)
            {
                long? respawnmaptype = Session.CurrentMapInstance.Map.MapTypes[0].RespawnMapTypeId;
                if (respawnmaptype != null)
                {
                    RespawnDTO resp = Respawns.Find(s => s.RespawnMapTypeId == respawnmaptype);
                    if (resp == null)
                    {
                        resp = new RespawnDTO { CharacterId = CharacterId, MapId = mapId, X = mapX, Y = mapY, RespawnMapTypeId = (long)respawnmaptype };
                        Respawns.Add(resp);
                    }
                    else
                    {
                        resp.X = mapX;
                        resp.Y = mapY;
                        resp.MapId = mapId;
                    }
                }
            }
        }

        public void SetReturnPoint(short mapId, short mapX, short mapY)
        {
            if (Session.HasCurrentMapInstance && Session.CurrentMapInstance.Map.MapTypes.Count > 0)
            {
                long? respawnmaptype = Session.CurrentMapInstance.Map.MapTypes[0].ReturnMapTypeId;
                if (respawnmaptype != null)
                {
                    RespawnDTO resp = Respawns.Find(s => s.RespawnMapTypeId == respawnmaptype);
                    if (resp == null)
                    {
                        resp = new RespawnDTO { CharacterId = CharacterId, MapId = mapId, X = mapX, Y = mapY, RespawnMapTypeId = (long)respawnmaptype };
                        Respawns.Add(resp);
                    }
                    else
                    {
                        resp.X = mapX;
                        resp.Y = mapY;
                        resp.MapId = mapId;
                    }
                }
            }
        }

        public void UpdateBushFire()
        {
            BrushFireJagged = BestFirstSearch.LoadBrushFireJagged(new GridPos
            {
                X = PositionX,
                Y = PositionY
            }, Session.CurrentMapInstance.Map.JaggedGrid);
        }

        public bool WeaponLoaded(CharacterSkill ski)
        {
            if (ski != null)
            {
                switch (Class)
                {
                    default:
                        return false;

                    case ClassType.Adventurer:
                        if (ski.Skill.Type == 1 && Inventory != null)
                        {
                            ItemInstance wearable = Inventory.LoadBySlotAndType((byte)EquipmentType.SecondaryWeapon, InventoryType.Wear);
                            if (wearable != null)
                            {
                                if (wearable.Ammo > 0)
                                {
                                    wearable.Ammo--;
                                    return true;
                                }
                                if (Inventory.CountItem(2081) < 1)
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_AMMO_ADVENTURER"), 10));
                                    return false;
                                }
                                Inventory.RemoveItemAmount(2081);
                                wearable.Ammo = 100;
                                Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("AMMO_LOADED_ADVENTURER"), 10));
                                return true;
                            }
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_WEAPON"), 10));
                            return false;
                        }
                        return true;

                    case ClassType.Swordman:
                        if (ski.Skill.Type == 1 && Inventory != null)
                        {
                            ItemInstance inv = Inventory.LoadBySlotAndType((byte)EquipmentType.SecondaryWeapon, InventoryType.Wear);
                            if (inv != null)
                            {
                                if (inv.Ammo > 0)
                                {
                                    inv.Ammo--;
                                    return true;
                                }
                                if (Inventory.CountItem(2082) < 1)
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_AMMO_SWORDSMAN"), 10));
                                    return false;
                                }

                                Inventory.RemoveItemAmount(2082);
                                inv.Ammo = 100;
                                Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("AMMO_LOADED_SWORDSMAN"), 10));
                                return true;
                            }
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_WEAPON"), 10));
                            return false;
                        }
                        return true;

                    case ClassType.Archer:
                        if (ski.Skill.Type == 1 && Inventory != null)
                        {
                            ItemInstance inv = Inventory.LoadBySlotAndType((byte)EquipmentType.MainWeapon, InventoryType.Wear);
                            if (inv != null)
                            {
                                if (inv.Ammo > 0)
                                {
                                    inv.Ammo--;
                                    return true;
                                }
                                if (Inventory.CountItem(2083) < 1)
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_AMMO_ARCHER"), 10));
                                    return false;
                                }

                                Inventory.RemoveItemAmount(2083);
                                inv.Ammo = 100;
                                Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("AMMO_LOADED_ARCHER"), 10));
                                return true;
                            }
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_WEAPON"), 10));
                            return false;
                        }
                        return true;

                    case ClassType.Magician:
                        return true;

                    case ClassType.Fighter:
                        return true;
                        
                        
                }
            }

            return false;
        }

        internal void RefreshValidity()
        {
            if (StaticBonusList.RemoveAll(s => s.StaticBonusType == StaticBonusType.BackPack && s.DateEnd < DateTime.Now) > 0)
            {
                Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("ITEM_TIMEOUT"), 10));
                Session.SendPacket(GenerateExts());
            }

            if (StaticBonusList.RemoveAll(s => s.DateEnd < DateTime.Now) > 0)
            {
                Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("ITEM_TIMEOUT"), 10));
            }

            if (Inventory != null)
            {
                foreach (object suit in Enum.GetValues(typeof(EquipmentType)))
                {
                    ItemInstance item = Inventory.LoadBySlotAndType((byte)suit, InventoryType.Wear);
                    if (item?.DurabilityPoint > 0)
                    {
                        item.DurabilityPoint--;
                        if (item.DurabilityPoint == 0)
                        {
                            Inventory.DeleteById(item.Id);
                            Session.SendPacket(GenerateStatChar());
                            Session.CurrentMapInstance?.Broadcast(GenerateEq());
                            Session.SendPacket(GenerateEquipment());
                            Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("ITEM_TIMEOUT"), 10));
                        }
                    }
                }
            }
        }

        internal void SetSession(ClientSession clientSession) => Session = clientSession;

        private void GenerateXp(MapMonster monster, bool isMonsterOwner)
        {
            NpcMonster monsterinfo = monster.Monster;
            if (!Session.Account.PenaltyLogs.Any(s => s.Penalty == PenaltyType.BlockExp && s.DateEnd > DateTime.Now))
            {
                Group grp = ServerManager.Instance.Groups.Find(g => g.IsMemberOfGroup(CharacterId));
                ItemInstance specialist = null;
                if (Hp <= 0)
                {
                    return;
                }
                if ((int)(LevelXp / (XpLoad() / 10)) < (int)((LevelXp + monsterinfo.XP) / (XpLoad() / 10)))
                {
                    Hp = (int)HPLoad();
                    Mp = (int)MPLoad();
                    Session.SendPacket(GenerateStat());
                    Session.SendPacket(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 5));
                }

                if (Inventory != null)
                {
                    specialist = Inventory.LoadBySlotAndType((byte)EquipmentType.Sp, InventoryType.Wear);
                }

                int xp;
                if (isMonsterOwner)
                {
                    xp = (int)(GetXP(monsterinfo, grp) * (1 + (GetBuff(CardType.Item, (byte)AdditionalTypes.Item.EXPIncreased)[0] / 100D)));
                }
                else
                {
                    xp = (int)(GetXP(monsterinfo, grp) / 3D * (1 + (GetBuff(CardType.Item, (byte)AdditionalTypes.Item.EXPIncreased)[0] / 100D)));
                }
                if (Level < ServerManager.Instance.Configuration.MaxLevel)
                {
                    LevelXp += xp;
                }

                foreach (Mate mate in Mates.Where(x => x.IsTeamMember))
                {
                    mate.GenerateXp(xp);
                }

                if ((Class == 0 && JobLevel < 20) || (Class != 0 && JobLevel < ServerManager.Instance.Configuration.MaxJobLevel))
                {
                    if (specialist != null && UseSp && specialist.SpLevel < ServerManager.Instance.Configuration.MaxSPLevel && specialist.SpLevel > 19)
                    {
                        JobLevelXp += (int)(GetJXP(monsterinfo, grp) / 2D * (1 + (GetBuff(CardType.Item, (byte)AdditionalTypes.Item.EXPIncreased)[0] / 100D)));
                    }
                    else
                    {
                        JobLevelXp += (int)(GetJXP(monsterinfo, grp) * (1 + (GetBuff(CardType.Item, (byte)AdditionalTypes.Item.EXPIncreased)[0] / 100D)));
                    }
                }
                if (specialist != null && UseSp && specialist.SpLevel < ServerManager.Instance.Configuration.MaxSPLevel)
                {
                    int multiplier = specialist.SpLevel < 10 ? 10 : specialist.SpLevel < 19 ? 5 : 1;
                    specialist.XP += (int)(GetJXP(monsterinfo, grp) * (multiplier + (GetBuff(CardType.Item, (byte)AdditionalTypes.Item.EXPIncreased)[0] / 100D)));
                }
                if (HeroLevel > 0 && HeroLevel < ServerManager.Instance.Configuration.MaxHeroLevel)
                {
                    if (isMonsterOwner)
                    {
                        HeroXp += (int)((GetHXP(monsterinfo, grp) / 50) * (1 + (GetBuff(CardType.Item, (byte)AdditionalTypes.Item.EXPIncreased)[0] / 100D)));
                    }
                    else
                    {
                        HeroXp += (int)((GetHXP(monsterinfo, grp) / 50) / 3D * (1 + (GetBuff(CardType.Item, (byte)AdditionalTypes.Item.EXPIncreased)[0] / 100D)));
                    }
                }
                double experience = XpLoad();
                while (LevelXp >= experience)
                {
                    LevelXp -= (long)experience;
                    Level++;
                    experience = XpLoad();
                    if (Level >= ServerManager.Instance.Configuration.MaxLevel)
                    {
                        Level = ServerManager.Instance.Configuration.MaxLevel;
                        LevelXp = 0;
                    }
                    else if (Level == ServerManager.Instance.Configuration.HeroicStartLevel)
                    {
                        HeroLevel = 1;
                        HeroXp = 0;
                    }
                    Hp = (int)HPLoad();
                    Mp = (int)MPLoad();
                    Session.SendPacket(GenerateStat());
                    if (Family != null)
                    {
                        if (Level > 20 && Level % 10 == 0)
                        {
                            Family.InsertFamilyLog(FamilyLogType.LevelUp, Name, level: Level);
                            Family.InsertFamilyLog(FamilyLogType.FamilyXP, Name, experience: 20 * Level);
                            GenerateFamilyXp(20 * Level);
                        }
                        else if (Level > 80)
                        {
                            Family.InsertFamilyLog(FamilyLogType.LevelUp, Name, level: Level);
                        }
                        else
                        {
                            ServerManager.Instance.FamilyRefresh(Family.FamilyId);
                            CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                            {
                                DestinationCharacterId = Family.FamilyId,
                                SourceCharacterId = CharacterId,
                                SourceWorldId = ServerManager.Instance.WorldId,
                                Message = "fhis_stc",
                                Type = MessageType.Family
                            });
                        }
                    }
                    Session.SendPacket(GenerateLevelUp());
                    GetReferrerReward();
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("LEVELUP"), 0));
                    Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 100), PositionX, PositionY);
                    Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 23), PositionX, PositionY);
                    ServerManager.Instance.UpdateGroup(CharacterId);
                }

                ItemInstance fairy = Inventory?.LoadBySlotAndType((byte)EquipmentType.Fairy, InventoryType.Wear);
                if (fairy != null)
                {
                    if (fairy.ElementRate + fairy.Item.ElementRate < fairy.Item.MaxElementRate
                        && Level <= monsterinfo.Level + 15 && Level >= monsterinfo.Level - 15)
                    {
                        fairy.XP += ServerManager.Instance.Configuration.RateFairyXP;
                    }
                    experience = CharacterHelper.LoadFairyXPData(fairy.ElementRate + fairy.Item.ElementRate);
                    while (fairy.XP >= experience)
                    {
                        fairy.XP -= (int)experience;
                        fairy.ElementRate++;
                        if (fairy.ElementRate + fairy.Item.ElementRate == fairy.Item.MaxElementRate)
                        {
                            fairy.XP = 0;
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("FAIRYMAX"), fairy.Item.Name), 10));
                        }
                        else
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("FAIRY_LEVELUP"), fairy.Item.Name), 10));
                        }
                        Session.SendPacket(GeneratePairy());
                    }
                }

                experience = JobXPLoad();
                while (JobLevelXp >= experience)
                {
                    JobLevelXp -= (long)experience;
                    JobLevel++;
                    experience = JobXPLoad();
                    if (JobLevel >= 20 && Class == 0)
                    {
                        JobLevel = 20;
                        JobLevelXp = 0;
                    }
                    else if (JobLevel >= ServerManager.Instance.Configuration.MaxJobLevel)
                    {
                        JobLevel = ServerManager.Instance.Configuration.MaxJobLevel;
                        JobLevelXp = 0;
                    }
                    Hp = (int)HPLoad();
                    Mp = (int)MPLoad();
                    Session.SendPacket(GenerateStat());
                    Session.SendPacket(GenerateLevelUp());
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("JOB_LEVELUP"), 0));
                    RewardsHelper.Instance.GetJobRewards(Session);
                    LearnAdventurerSkill();
                    Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 8), PositionX, PositionY);
                    Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 24), PositionX, PositionY);
                }
                if (specialist != null)
                {
                    experience = SpXpLoad();

                    while (UseSp && specialist.XP >= experience)
                    {
                        specialist.XP -= (long)experience;
                        specialist.SpLevel++;
                        experience = SpXpLoad();
                        Session.SendPacket(GenerateStat());
                        Session.SendPacket(GenerateLevelUp());
                        if (specialist.SpLevel >= ServerManager.Instance.Configuration.MaxSPLevel)
                        {
                            specialist.SpLevel = ServerManager.Instance.Configuration.MaxSPLevel;
                            specialist.XP = 0;
                        }
                        LearnSPSkill();
                        Skills.ForEach(s => s.LastUse = DateTime.Now.AddDays(-1));
                        Session.SendPacket(GenerateSki());
                        Session.SendPackets(GenerateQuicklist());

                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SP_LEVELUP"), 0));
                        Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 8), PositionX, PositionY);
                        Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 26), PositionX, PositionY);
                    }
                }
                experience = HeroXPLoad();
                while (HeroXp >= experience)
                {
                    HeroXp -= (long)experience;
                    HeroLevel++;
                    experience = HeroXPLoad();
                    if (HeroLevel >= ServerManager.Instance.Configuration.MaxHeroLevel)
                    {
                        HeroLevel = ServerManager.Instance.Configuration.MaxHeroLevel;
                        HeroXp = 0;
                    }
                    Hp = (int)HPLoad();
                    Mp = (int)MPLoad();
                    Session.SendPacket(GenerateStat());
                    Session.SendPacket(GenerateLevelUp());
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("HERO_LEVELUP"), 0));
                    Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 8), PositionX, PositionY);
                    Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 3468), PositionX, PositionY);
                }
                Session.SendPacket(GenerateLev());
            }
        }

        private int GetGold(MapMonster mapMonster)
        {
            if (MapId == 2006 || MapId == 150)
            {
                return 0;
            }
            double eqMultiplier = 1 + (GetBuff(CardType.Item, (byte)AdditionalTypes.Item.IncreaseEarnedGold)[0] / 100D);
            int lowBaseGold = ServerManager.RandomNumber(6 * mapMonster.Monster?.Level ?? 1, 12 * mapMonster.Monster?.Level ?? 1);
            int actMultiplier = Session?.CurrentMapInstance?.Map.MapTypes?.Any(s => s.MapTypeId == (short)MapTypeEnum.Act52) ?? false ? 10 : 1;
            if (Session?.CurrentMapInstance?.Map.MapTypes?.Any(s => s.MapTypeId == (short)MapTypeEnum.Act61 || s.MapTypeId == (short)MapTypeEnum.Act61a || s.MapTypeId == (short)MapTypeEnum.Act61d) == true)
            {
                actMultiplier = 5;
            }
            return (int)(lowBaseGold * ServerManager.Instance.Configuration.RateGold * actMultiplier * eqMultiplier);
        }

        private int GetHXP(NpcMonsterDTO monster, Group group)
        {
            int partySize = 1;
            float partyPenalty = 1f;

            if (group != null)
            {
                int levelSum = group.Characters.Sum(g => g.Character.Level);
                partySize = group.CharacterCount;
                partyPenalty = (6f / partySize) / levelSum;
            }

            int heroXp = (int)Math.Round(monster.HeroXp * CharacterHelper.ExperiencePenalty(Level, monster.Level) * ServerManager.Instance.Configuration.RateHeroicXP * MapInstance.XpRate);

            // divide jobexp by multiplication of partyPenalty with level e.g. 57 * 0,014...
            if (partySize > 1 && group != null)
            {
                heroXp = (int)Math.Round(heroXp / (HeroLevel * partyPenalty));
            }

            return heroXp;
        }

        private int GetJXP(NpcMonsterDTO monster, Group group)
        {
            int partySize = 1;
            double partyPenalty = 1d;

            if (group != null)
            {
                int levelSum = group.Characters.Sum(g => g.Character.JobLevel);
                partySize = group.CharacterCount;
                partyPenalty = (6f / partySize) / levelSum;
            }

            int jobxp = (int)Math.Round(monster.JobXP * CharacterHelper.ExperiencePenalty(JobLevel, monster.Level) * ServerManager.Instance.Configuration.RateXP * MapInstance.XpRate);

            if (partySize > 1 && group != null)
            {
                jobxp = (int)Math.Round(jobxp / (JobLevel * partyPenalty));
            }

            return jobxp;
        }

        private int GetXP(NpcMonsterDTO monster, Group group)
        {
            int partySize = 1;
            double partyPenalty = 1d;
            int levelDifference = Level - monster.Level;

            if (group != null)
            {
                int levelSum = group.Characters.Sum(g => g.Character.Level);
                partySize = group.CharacterCount;
                partyPenalty = (6f / partySize) / levelSum;
            }

            int xpcalculation = levelDifference < 5 ? monster.XP : monster.XP / 3 * 2;

            int xp = (int)Math.Round(xpcalculation * CharacterHelper.ExperiencePenalty(Level, monster.Level) * ServerManager.Instance.Configuration.RateXP * MapInstance.XpRate);

            if (Level <= 5 && levelDifference < -4)
            {
                xp += xp / 2;
            }
            if (monster.Level >= 75)
            {
                xp *= 2;
            }
            if (monster.Level >= 100)
            {
                xp *= 2;
                if (Level < 96)
                {
                    xp = 1;
                }
            }

            if (partySize > 1 && group != null)
            {
                xp /= this.Group.CharacterCount;
            }

            return xp;
        }

        private int HealthHPLoad()
        {
            if (IsSitting)
            {
                return CharacterHelper.HPHealth[(byte)Class];
            }
            return (DateTime.Now - LastDefence).TotalSeconds > 4 ? CharacterHelper.HPHealthStand[(byte)Class] : 0;
        }

        public void OpenBank()
        {
            Session.SendPacket($"gb 3 {Session.Account.BankGold / 1000} {Session.Character.Gold} 0 0");
            Session.SendPacket("s_memo 6 Welcome to the Cuarry Bank. You can deposit or withdraw 1,000 to 100 billion gold.");
        }

        private int HealthMPLoad()
        {
            if (IsSitting)
            {
                return CharacterHelper.MPHealth[(byte)Class];
            }
            return (DateTime.Now - LastDefence).TotalSeconds > 4 ? CharacterHelper.MPHealthStand[(byte)Class] : 0;
        }
        public void LoadPassive()
        {
            // TODO IMPROVE PERFORMANCES
            // ACCESSING A DICTIONARY LIKE THIS IS CPU CYCLE KILLER
            PassiveSkillHelper.Instance.PassiveSkillToBcards(Skills.Where(s => s.Skill.SkillType == 0).ToList()).ForEach(s =>EquipmentBCards.Add(s));
        }

        public string GenerateAct6()
        {
            return
                $"act6 1 0 {ServerManager.Instance.Act6Zenas.Percentage / 10} {Convert.ToByte(ServerManager.Instance.Act6Zenas.Mode)} {ServerManager.Instance.Act6Zenas.CurrentTime} {ServerManager.Instance.Act6Zenas.TotalTime} {ServerManager.Instance.Act6Erenia.Percentage / 10} {Convert.ToByte(ServerManager.Instance.Act6Erenia.Mode)} {ServerManager.Instance.Act6Erenia.CurrentTime} {ServerManager.Instance.Act6Erenia.TotalTime}";
        }

        public int[] GetWeaponSoftDamage()
        {
            int increase = 0;
            int increaseChance = 0;

            ItemInstance prim = Inventory.PrimaryWeapon;
            ItemInstance sec = Inventory.SecondaryWeapon;

            if (prim != null)
            {
                foreach (BCard bcard in prim.Item.BCards.Where(
                    s => s != null && s.Type.Equals((byte)CardType.IncreaseDamage) &&
                         s.SubType.Equals((byte)AdditionalTypes.IncreaseDamage.IncreasingPropability)))
                {
                    increaseChance += bcard.FirstData;
                    increase += bcard.SecondData;
                }
            }
            if (sec != null)
            {
                foreach (BCard bcard in sec.Item.BCards.Where(
                    s => s != null && s.Type.Equals((byte)CardType.IncreaseDamage) &&
                         s.SubType.Equals((byte)AdditionalTypes.IncreaseDamage.IncreasingPropability)))
                {
                    increaseChance += bcard.FirstData;
                    increase += bcard.SecondData;
                }
            }
            return new[] { increaseChance, increase };
        }

        private double HeroXPLoad() => HeroLevel == 0 ? 1 : CharacterHelper.HeroXpData[HeroLevel - 1];

        private double JobXPLoad() => Class == (byte)ClassType.Adventurer ? CharacterHelper.FirstJobXPData[JobLevel - 1] : CharacterHelper.SecondJobXPData[JobLevel - 1];

        private double SpXpLoad()
        {
            ItemInstance specialist = null;
            if (Inventory != null)
            {
                specialist = Inventory.LoadBySlotAndType((byte)EquipmentType.Sp, InventoryType.Wear);
            }
            return specialist != null ? CharacterHelper.SPXPData[specialist.SpLevel == 0 ? 0 : specialist.SpLevel - 1] : 0;
        }

        public void TeleportOnMap(short x, short y)
        {
            Session.Character.PositionX = x;
            Session.Character.PositionY = y;
            Session.SendPacket($"tp {1} {CharacterId} {x} {y} 0");
            Session.SendPacket(GenerateCond());
        }

       

        public double XpLoad() => CharacterHelper.XPData[Level - 1];

        #endregion
    }
}