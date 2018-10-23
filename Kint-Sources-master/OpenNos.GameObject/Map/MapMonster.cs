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
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Battle;
using OpenNos.GameObject.Helpers;
using OpenNos.PathFinder;
using System;
using OpenNos.GameObject.Networking;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using static OpenNos.Domain.BCardType;

namespace OpenNos.GameObject
{
    public class MapMonster : MapMonsterDTO
    {
        #region Members

        private int _movetime;

        private bool _noAttack;
        private bool _noMove;
        private Random _random;


        #endregion

        #region Instantiation

        public MapMonster()
        {
            Buff = new ThreadSafeSortedList<short, Buff>();
            HitQueue = new ConcurrentQueue<HitRequest>();
            OnDeathEvents = new List<EventContainer>();
            OnNoticeEvents = new List<EventContainer>();
        }

        public MapMonster(MapMonsterDTO input)
        {
            Buff = new ThreadSafeSortedList<short, Buff>();
            HitQueue = new ConcurrentQueue<HitRequest>();
            OnDeathEvents = new List<EventContainer>();
            OnNoticeEvents = new List<EventContainer>();
            IsDisabled = input.IsDisabled;
            IsMoving = input.IsMoving;
            MapId = input.MapId;
            MapMonsterId = input.MapMonsterId;
            MapX = input.MapX;
            MapY = input.MapY;
            MonsterVNum = input.MonsterVNum;
            Position = input.Position;
        }

        #endregion

        #region Properties

        public ThreadSafeSortedList<short, Buff> Buff { get; set; }

        public int CurrentHp { get; set; }

        public int CurrentMp { get; set; }

        public IDictionary<long, long> DamageList { get; private set; }

        public DateTime Death { get; set; }

        public ConcurrentQueue<HitRequest> HitQueue { get; }

        public BattleEntity BattleEntity { get; private set; }

        public bool IsAlive { get; set; }

        public bool IsBonus { get; set; }

        public bool IsBoss { get; set; }

        public bool IsHostile { get; set; }

        public bool IsTarget { get; set; }

        public DateTime LastEffect { get; set; }

        public DateTime LastMove { get; set; }

        public DateTime LastSkill { get; set; }

        public IDisposable LifeEvent { get; set; }

        public MapInstance MapInstance { get; set; }

        public int MaxHp { get; set; }

        public int MaxMp { get; set; }

        public NpcMonster Monster { get; private set; }

        public ZoneEvent MoveEvent { get; set; }

        public bool NoAggresiveIcon { get; internal set; }

        public byte NoticeRange { get; set; }

        public List<EventContainer> OnDeathEvents { get; set; }

        public List<EventContainer> OnNoticeEvents { get; set; }

        public List<Node> Path { get; set; }

        public bool? ShouldRespawn { get; set; }

        public List<NpcMonsterSkill> Skills { get; set; }

        public bool Started { get; internal set; }

        public long Target { get; set; }

        public bool Invisible { get; set; }

        private short FirstX { get; set; }

        private short FirstY { get; set; }

        #endregion

        #region Methods

        public void AddBuff(Buff indicator)
        {
            if (indicator?.Card != null)
            {
                Buff[indicator.Card.CardId] = indicator;
                indicator.RemainingTime = indicator.Card.Duration;
                indicator.Start = DateTime.Now;

                indicator.Card.BCards.ForEach(c => c.ApplyBCards(this));
                Observable.Timer(TimeSpan.FromMilliseconds(indicator.Card.Duration * 100)).Subscribe(o =>
                {
                    removeBuff(indicator.Card.CardId);
                    if (indicator.Card.TimeoutBuff != 0 &&
                        ServerManager.RandomNumber() < indicator.Card.TimeoutBuffChance)
                    {
                        AddBuff(new Buff(indicator.Card.TimeoutBuff, Monster.Level));
                    }
                });
                _noAttack |= indicator.Card.BCards.Any(s =>
                    s.Type == (byte)CardType.SpecialAttack &&
                    s.SubType.Equals((byte)AdditionalTypes.SpecialAttack.NoAttack / 10));
                _noMove |= indicator.Card.BCards.Any(s =>
                    s.Type == (byte)CardType.Move &&
                    s.SubType.Equals((byte)AdditionalTypes.Move.MovementImpossible / 10));
            }
        }
        public void ReflectiveBuffs()
            {}


    public string GenerateBoss() => $"rboss 3 {MapMonsterId} {CurrentHp} {MaxHp}";

        public string GenerateIn()
        {
            if (IsAlive && !IsDisabled)
            {
                return StaticPacketHelper.In(UserType.Monster, MonsterVNum, MapMonsterId, MapX, MapY, Position, (int)((float)CurrentHp / (float)MaxHp * 100), (int)((float)CurrentMp / (float)MaxMp * 100), 0, NoAggresiveIcon ? InRespawnType.NoEffect : InRespawnType.TeleportationEffect, false);
            }
            return string.Empty;
        }

        public void Initialize(MapInstance currentMapInstance)
        {
            MapInstance = currentMapInstance;
            Initialize();
        }

        public void Initialize()
        {
            FirstX = MapX;
            FirstY = MapY;
            LastSkill = LastMove = LastEffect = DateTime.Now;
            Target = -1;
            Path = new List<Node>();
            IsAlive = true;
            ShouldRespawn = ShouldRespawn ?? true;
            Monster = ServerManager.GetNpc(MonsterVNum);

            MaxHp = Monster.MaxHP;
            MaxMp = Monster.MaxMP;
            if (MapInstance?.MapInstanceType == MapInstanceType.RaidInstance)
            {
                if (IsBoss)
                {
                    MaxHp *= 7;
                    MaxMp *= 7;
                }
                else
                {
                    MaxHp *= 5;
                    MaxMp *= 5;

                    if (IsTarget)
                    {
                        MaxHp *= 6;
                        MaxMp *= 6;
                    }
                }
            }

            // Irrelevant for now(Act4)
            //if (MapInstance?.MapInstanceType == MapInstanceType.Act4Morcos || MapInstance?.MapInstanceType == MapInstanceType.Act4Hatus || MapInstance?.MapInstanceType == MapInstanceType.Act4Calvina || MapInstance?.MapInstanceType == MapInstanceType.Act4Berios)
            //{
            //    if (MonsterVNum == 563 || MonsterVNum == 577 || MonsterVNum == 629 || MonsterVNum == 624)
            //    {
            //        MaxHp *= 5;
            //        MaxMp *= 5;
            //    }
            //}

            NoAggresiveIcon = Monster.NoAggresiveIcon;

            IsHostile = Monster.IsHostile;
            CurrentHp = MaxHp;
            CurrentMp = MaxMp;
            Skills = Monster.Skills.ToList();
            DamageList = new Dictionary<long, long>();
            _random = new Random(MapMonsterId);
            _movetime = ServerManager.RandomNumber(400, 3200);
        }

        /// <summary>
        /// Check if the Monster is in the given Range.
        /// </summary>
        /// <param name="mapX">The X coordinate on the Map of the object to check.</param>
        /// <param name="mapY">The Y coordinate on the Map of the object to check.</param>
        /// <param name="distance">The maximum distance of the object to check.</param>
        /// <returns>True if the Monster is in range, False if not.</returns>
        public bool IsInRange(short mapX, short mapY, byte distance)
        {
            return Map.GetDistance(
             new MapCell
             {
                 X = mapX,
                 Y = mapY
             }, new MapCell
             {
                 X = MapX,
                 Y = MapY
             }) <= distance + 1;
        }

        public void RunDeathEvent()
        {
            Buff.ClearAll();
            _noMove = false;
            _noAttack = false;
            if (IsBonus)
            {
                MapInstance.InstanceBag.Combo++;
                MapInstance.InstanceBag.Point += EventHelper.CalculateComboPoint(MapInstance.InstanceBag.Combo + 1);
            }
            else
            {
                MapInstance.InstanceBag.Combo = 0;
                MapInstance.InstanceBag.Point += EventHelper.CalculateComboPoint(MapInstance.InstanceBag.Combo);
            }
            MapInstance.InstanceBag.MonstersKilled++;
            OnDeathEvents.ForEach(e => EventHelper.Instance.RunEvent(e, monster: this));
        }

        public void SetDeathStatement()
        {
            IsAlive = false;
            CurrentHp = 0;
            CurrentMp = 0;
            Death = DateTime.Now;
            LastMove = DateTime.Now;
        }

        public void StartLife()
        {
            try
            {
                if (!MapInstance.IsSleeping)
                {
                    monsterLife();
                }
            }
            catch (Exception e)
            {
                //Logger.Error(e);
            }
        }

        internal void GetNearestOponent()
        {
            if (Target == -1)
            {
                const int maxDistance = 22;
                int distance = 100;
                List<ClientSession> sess = new List<ClientSession>();
                DamageList.Keys.ToList().ForEach(s => sess.Add(MapInstance.GetSessionByCharacterId(s)));
                //ClientSession session = sess.OrderBy(s => distance = Map.GetDistance(new MapCell { X = MapX, Y = MapY }, new MapCell { X = s.Character.PositionX, Y = s.Character.PositionY })).FirstOrDefault();
                Character character = sess.Where(s => s?.Character != null && (ServerManager.Instance.ChannelId != 51 || MonsterVNum - (byte)s.Character.Faction != 678) && s.Character.Hp > 0 && !s.Character.InvisibleGm && !s.Character.Invisible && s.Character.MapInstance == MapInstance && Map.GetDistance(new MapCell { X = MapX, Y = MapY }, new MapCell { X = s.Character.PositionX, Y = s.Character.PositionY }) < Monster.NoticeRange).OrderBy(s => distance = Map.GetDistance(new MapCell { X = MapX, Y = MapY }, new MapCell { X = s.Character.PositionX, Y = s.Character.PositionY })).FirstOrDefault()?.Character;
                if (distance < maxDistance && character != null)
                {
                    Target = character.CharacterId;
                }
            }
        }

        internal void HostilityTarget()
        {
            if (IsHostile && Target == -1)
            {
                Character character = ServerManager.Instance.Sessions.Where(s => s?.Character != null && (ServerManager.Instance.ChannelId != 51 || MonsterVNum - (byte)s.Character.Faction != 678) && s.Character.Hp > 0 && !s.Character.InvisibleGm && !s.Character.Invisible && s.Character.MapInstance == MapInstance && Map.GetDistance(new MapCell { X = MapX, Y = MapY }, new MapCell { X = s.Character.PositionX, Y = s.Character.PositionY }) < Monster.NoticeRange).OrderBy(s => ServerManager.RandomNumber(0, int.MaxValue)).FirstOrDefault()?.Character;
                if (character != null)
                {
                    if (OnNoticeEvents.Count == 0 && MoveEvent == null)
                    {
                        Target = character.CharacterId;
                        if (!NoAggresiveIcon)
                        {
                            character.Session.SendPacket(StaticPacketHelper.GenerateEff(UserType.Monster, MapMonsterId, 5000));
                        }
                    }
                    OnNoticeEvents.ForEach(e => EventHelper.Instance.RunEvent(e, monster: this));
                    OnNoticeEvents.RemoveAll(s => s != null);
                }
            }
        }

        /// <summary>
        /// Remove the current Target from Monster.
        /// </summary>
        internal void RemoveTarget()
        {
            if (Target != -1)
            {
                (Path ?? (Path = new List<Node>())).Clear();
                Target = -1;

                //return to origin
                Path = BestFirstSearch.FindPathJagged(new Node { X = MapX, Y = MapY }, new Node { X = FirstX, Y = FirstY },
                    MapInstance.Map.JaggedGrid);
            }
        }

        /// <summary>
        /// Follow the Monsters target to it's position.
        /// </summary>
        /// <param name="targetSession">The TargetSession to follow</param>
        private void followTarget(ClientSession targetSession)
        {
            if (IsMoving && !_noMove)
            {
                const short maxDistance = 22;
                int distance = Map.GetDistance(new MapCell() { X = targetSession.Character.PositionX, Y = targetSession.Character.PositionY }, new MapCell() { X = MapX, Y = MapY });
                if (targetSession != null)
                {
                    if (targetSession.Character.LastMonsterAggro.AddSeconds(5) < DateTime.Now ||
                    targetSession.Character.BrushFireJagged == null)
                    {
                        targetSession.Character.UpdateBushFire();
                    }
                    targetSession.Character.LastMonsterAggro = DateTime.Now;
                }
                if (Path.Count == 0 && targetSession != null)
                {
                    short xoffset = (short)ServerManager.RandomNumber(-1, 1);
                    short yoffset = (short)ServerManager.RandomNumber(-1, 1);
                    try
                    {
                        Path = BestFirstSearch.TracePathJagged(new Node { X = MapX, Y = MapY },
                            targetSession.Character.BrushFireJagged,
                            targetSession.Character.MapInstance.Map.JaggedGrid);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Pathfinding using Pathfinder failed. Map: {MapId} StartX: {MapX} StartY: {MapY} TargetX: {(short)(targetSession.Character.PositionX + xoffset)} TargetY: {(short)(targetSession.Character.PositionY + yoffset)}", ex);
                        RemoveTarget();
                    }
                }
                if (Monster != null && DateTime.Now > LastMove && Monster.Speed > 0 && Path.Count > 0)
                {
                    int maxindex = Path.Count > Monster.Speed / 2 ? Monster.Speed / 2 : Path.Count;
                    short mapX = Path[maxindex - 1].X;
                    short mapY = Path[maxindex - 1].Y;
                    double waitingtime = Map.GetDistance(new MapCell { X = mapX, Y = mapY }, new MapCell { X = MapX, Y = MapY }) / (double)Monster.Speed;
                    MapInstance.Broadcast(new BroadcastPacket(null, PacketFactory.Serialize(StaticPacketHelper.Move(UserType.Monster, MapMonsterId, mapX, mapY, Monster.Speed)), ReceiverType.All, xCoordinate: mapX, yCoordinate: mapY));
                    LastMove = DateTime.Now.AddSeconds(waitingtime > 1 ? 1 : waitingtime);

                    Observable.Timer(TimeSpan.FromMilliseconds((int)((waitingtime > 1 ? 1 : waitingtime) * 1000))).Subscribe(x =>
                    {
                        MapX = mapX;
                        MapY = mapY;
                    });
                    distance = (int)Path[0].F;
                    Path.RemoveRange(0, maxindex > Path.Count ? Path.Count : maxindex);
                }

                if (targetSession == null || MapId != targetSession.Character.MapInstance.Map.MapId || distance > (maxDistance) + 3)
                {
                    RemoveTarget();
                }
            }
        }

        /// <summary>
        /// Handle any kind of Monster interaction
        /// </summary>
        private void monsterLife()
        {
            if (Monster == null)
            {
                return;
            }
            if ((DateTime.Now - LastEffect).TotalSeconds >= 5)
            {
                LastEffect = DateTime.Now;
                if (IsTarget)
                {
                    MapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Monster, MapMonsterId, 823));
                }
                if (IsBonus)
                {
                    MapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Monster, MapMonsterId, 827));
                }
            }

            if (IsBoss && IsAlive)
            {
                MapInstance.Broadcast(GenerateBoss());
            }

            // handle hit queue
            while (HitQueue.TryDequeue(out HitRequest hitRequest))
            {
                if (IsAlive && hitRequest.Session.Character.Hp > 0 && (ServerManager.Instance.ChannelId != 51 || MonsterVNum - (byte)hitRequest.Session.Character.Faction != 678))
                {
                    int hitmode = 0;
                    bool isCaptureSkill = hitRequest.Skill.BCards.Any(s => s.Type.Equals((byte)CardType.Capture));

                    // calculate damage
                    bool onyxWings = false;
                    BattleEntity battleEntity = new BattleEntity(hitRequest.Session.Character, hitRequest.Skill);
                    int damage = DamageHelper.Instance.CalculateDamage(battleEntity, new BattleEntity(this), hitRequest.Skill, ref hitmode, ref onyxWings, hitRequest.Session.Character.damageAb);
                    if (onyxWings)
                    {
                        short onyxX = (short)(hitRequest.Session.Character.PositionX + 2);
                        short onyxY = (short)(hitRequest.Session.Character.PositionY + 2);
                        int onyxId = MapInstance.GetNextMonsterId();
                        MapMonster onyx = new MapMonster() { MonsterVNum = 2371, MapX = onyxX, MapY = onyxY, MapMonsterId = onyxId, IsHostile = false, IsMoving = false, ShouldRespawn = false };
                        MapInstance.Broadcast(UserInterfaceHelper.GenerateGuri(31, 1, hitRequest.Session.Character.CharacterId, onyxX, onyxY));
                        onyx.Initialize(MapInstance);
                        MapInstance.AddMonster(onyx);
                        MapInstance.Broadcast(onyx.GenerateIn());
                        CurrentHp -= damage / 2;
                        Observable.Timer(TimeSpan.FromMilliseconds(350)).Subscribe(o =>
                        {
                            MapInstance.Broadcast(StaticPacketHelper.SkillUsed(UserType.Monster, onyxId, 3, MapMonsterId, -1, 0, -1, hitRequest.Skill.Effect, -1, -1, true, 92, damage / 2, 0, 0));
                            MapInstance.RemoveMonster(onyx);
                            MapInstance.Broadcast(StaticPacketHelper.Out(UserType.Monster, onyx.MapMonsterId));
                        });
                    }
                    if (hitmode != 1)
                    {
                        hitRequest.Skill.BCards.Where(s => s.Type.Equals((byte)CardType.Buff)).ToList().ForEach(s => s.ApplyBCards(this, hitRequest.Session));
                        hitRequest.Skill.BCards.Where(s => s.Type.Equals((byte)CardType.Capture)).ToList().ForEach(s => s.ApplyBCards(this, hitRequest.Session));
                        if (battleEntity?.ShellWeaponEffects != null)
                        {
                            foreach (ShellEffectDTO shell in battleEntity.ShellWeaponEffects)
                            {
                                switch (shell.Effect)
                                {
                                    case (byte)ShellWeaponEffectType.Blackout:
                                        {
                                            Buff buff = new Buff(7, battleEntity.Level);
                                            if (ServerManager.RandomNumber() < shell.Value)
                                            {
                                                AddBuff(buff);
                                            }
                                            break;
                                        }
                                    case (byte)ShellWeaponEffectType.DeadlyBlackout:
                                        {
                                            Buff buff = new Buff(66, battleEntity.Level);
                                            if (ServerManager.RandomNumber() < shell.Value)
                                            {
                                                AddBuff(buff);
                                            }
                                            break;
                                        }
                                    case (byte)ShellWeaponEffectType.MinorBleeding:
                                        {
                                            Buff buff = new Buff(1, battleEntity.Level);
                                            if (ServerManager.RandomNumber() < shell.Value)
                                            {
                                                AddBuff(buff);
                                            }
                                            break;
                                        }
                                    case (byte)ShellWeaponEffectType.Bleeding:
                                        {
                                            Buff buff = new Buff(21, battleEntity.Level);
                                            if (ServerManager.RandomNumber() < shell.Value)
                                            {
                                                AddBuff(buff);
                                            }
                                            break;
                                        }
                                    case (byte)ShellWeaponEffectType.HeavyBleeding:
                                        {
                                            Buff buff = new Buff(42, battleEntity.Level);
                                            if (ServerManager.RandomNumber() < shell.Value)
                                            {
                                                AddBuff(buff);
                                            }
                                            break;
                                        }
                                    case (byte)ShellWeaponEffectType.Freeze:
                                        {
                                            Buff buff = new Buff(27, battleEntity.Level);
                                            if (ServerManager.RandomNumber() < shell.Value)
                                            {
                                                AddBuff(buff);
                                            }
                                            break;
                                        }
                                }
                            }
                        }
                    }
                    if (DamageList.ContainsKey(hitRequest.Session.Character.CharacterId))
                    {
                        DamageList[hitRequest.Session.Character.CharacterId] += damage;
                    }
                    else
                    {
                        DamageList.Add(hitRequest.Session.Character.CharacterId, damage);
                    }
                    if (isCaptureSkill)
                    {
                        damage = 0;
                    }
                    if (CurrentHp <= damage)
                    {
                        SetDeathStatement();
                    }
                    else
                    {
                        CurrentHp -= damage;
                    }

                    // only set the hit delay if we become the monsters target with this hit
                    if (Target == -1)
                    {
                        LastSkill = DateTime.Now;
                    }

                    int nearestDistance = 100;
                    foreach (KeyValuePair<long, long> kvp in DamageList)
                    {
                        ClientSession session = MapInstance.GetSessionByCharacterId(kvp.Key);
                        if (session != null)
                        {
                            int distance = Map.GetDistance(new MapCell
                            {
                                X = MapX,
                                Y = MapY
                            }, new MapCell
                            {
                                X = session.Character.PositionX,
                                Y = session.Character.PositionY
                            });
                            if (distance < nearestDistance)
                            {
                                nearestDistance = distance;
                                Target = session.Character.CharacterId;
                            }
                        }
                    }

                    switch (hitRequest.TargetHitType)
                    {
                        case TargetHitType.SingleTargetHit:
                            if (!isCaptureSkill)
                            {
                                MapInstance?.Broadcast(StaticPacketHelper.SkillUsed(UserType.Player, hitRequest.Session.Character.CharacterId, 3, MapMonsterId, hitRequest.Skill.SkillVNum, hitRequest.Skill.Cooldown, hitRequest.Skill.AttackAnimation, hitRequest.SkillEffect, hitRequest.Session.Character.PositionX, hitRequest.Session.Character.PositionY, IsAlive, (int)((float)CurrentHp / (float)MaxHp * 100), damage, hitmode, (byte)(hitRequest.Skill.SkillType - 1)));
                            }
                            break;

                        case TargetHitType.SingleTargetHitCombo:
                            MapInstance?.Broadcast(StaticPacketHelper.SkillUsed(UserType.Player, hitRequest.Session.Character.CharacterId, 3, MapMonsterId, hitRequest.Skill.SkillVNum, hitRequest.Skill.Cooldown, hitRequest.SkillCombo.Animation, hitRequest.SkillCombo.Effect, hitRequest.Session.Character.PositionX, hitRequest.Session.Character.PositionY, IsAlive, (int)((float)CurrentHp / (float)MaxHp * 100), damage, hitmode, (byte)(hitRequest.Skill.SkillType - 1)));
                            break;

                        case TargetHitType.SingleAOETargetHit:
                            switch (hitmode)
                            {
                                case 1:
                                    hitmode = 4;
                                    break;

                                case 3:
                                    hitmode = 6;
                                    break;

                                default:
                                    hitmode = 5;
                                    break;
                            }
                            if (hitRequest.ShowTargetHitAnimation)
                            {
                                MapInstance?.Broadcast(StaticPacketHelper.SkillUsed(UserType.Player, hitRequest.Session.Character.CharacterId, 3, MapMonsterId, hitRequest.Skill.SkillVNum, hitRequest.Skill.Cooldown, hitRequest.Skill.AttackAnimation, hitRequest.SkillEffect, hitRequest.Session.Character.PositionX, hitRequest.Session.Character.PositionY, IsAlive, (int)((float)CurrentHp / (float)MaxHp * 100), damage, hitmode, (byte)(hitRequest.Skill.SkillType - 1)));
                            }
                            else
                            {
                                MapInstance?.Broadcast(StaticPacketHelper.SkillUsed(UserType.Player, hitRequest.Session.Character.CharacterId, 3, MapMonsterId, 0, 0, 0, 0, 0, 0, IsAlive, (int)((float)CurrentHp / (float)MaxHp * 100), damage, hitmode, (byte)(hitRequest.Skill.SkillType - 1)));
                            }
                            break;

                        case TargetHitType.AOETargetHit:
                            switch (hitmode)
                            {
                                case 1:
                                    hitmode = 4;
                                    break;

                                case 3:
                                    hitmode = 6;
                                    break;

                                default:
                                    hitmode = 5;
                                    break;
                            }
                            MapInstance?.Broadcast(StaticPacketHelper.SkillUsed(UserType.Player, hitRequest.Session.Character.CharacterId, 3, MapMonsterId, hitRequest.Skill.SkillVNum, hitRequest.Skill.Cooldown, hitRequest.Skill.AttackAnimation, hitRequest.SkillEffect, hitRequest.Session.Character.PositionX, hitRequest.Session.Character.PositionY, IsAlive, (int)((float)CurrentHp / (float)MaxHp * 100), damage, hitmode, (byte)(hitRequest.Skill.SkillType - 1)));
                            break;

                        case TargetHitType.ZoneHit:
                            MapInstance?.Broadcast(StaticPacketHelper.SkillUsed(UserType.Player, hitRequest.Session.Character.CharacterId, 3, MapMonsterId, hitRequest.Skill.SkillVNum, hitRequest.Skill.Cooldown, hitRequest.Skill.AttackAnimation, hitRequest.SkillEffect, hitRequest.MapX, hitRequest.MapY, IsAlive, (int)((float)CurrentHp / (float)MaxHp * 100), damage, 5, (byte)(hitRequest.Skill.SkillType - 1)));
                            break;

                        case TargetHitType.SpecialZoneHit:
                            MapInstance?.Broadcast(StaticPacketHelper.SkillUsed(UserType.Player, hitRequest.Session.Character.CharacterId, 3, MapMonsterId, hitRequest.Skill.SkillVNum, hitRequest.Skill.Cooldown, hitRequest.Skill.AttackAnimation, hitRequest.SkillEffect, hitRequest.Session.Character.PositionX, hitRequest.Session.Character.PositionY, IsAlive, (int)((float)CurrentHp / (float)MaxHp * 100), damage, hitmode, (byte)(hitRequest.Skill.SkillType - 1)));
                            break;
                    }
                
                    if (hitmode != 1)
                    {
                        hitRequest.Session.Character.RemoveBuff(85);                       
                        hitRequest.Session.Character.RemoveBuff(559);
                        hitRequest.Session.Character.Invisible = false;
                    }

                    if (CurrentHp <= 0 && !isCaptureSkill)
                    {
                        // generate the kill bonus
                        hitRequest.Session.Character.GenerateKillBonus(this);
                    }
                }
                else
                {
                    // monster already has been killed, send cancel
                    hitRequest.Session.SendPacket(StaticPacketHelper.Cancel(2, MapMonsterId));
                }
                if (IsBoss)
                {
                    MapInstance.Broadcast(GenerateBoss());
                }
            }

            // Respawn
            if (!IsAlive && ShouldRespawn != null && !ShouldRespawn.Value)
            {
                MapInstance.RemoveMonster(this);
            }
            if (!IsAlive && ShouldRespawn != null && ShouldRespawn.Value)
            {
                double timeDeath = (DateTime.Now - Death).TotalSeconds;
                if (timeDeath >= Monster.RespawnTime / 10d)
                {
                    respawn();
                }
            }

            // normal movement
            else if (Target == -1)
            {
                move();
            }

            // target following
            else if (MapInstance != null)
            {
                GetNearestOponent();
                HostilityTarget();

                ClientSession targetSession = MapInstance.GetSessionByCharacterId(Target);

                // remove target in some situations
                if (targetSession == null || targetSession.Character.Invisible || targetSession.Character.Hp <= 0 || CurrentHp <= 0)
                {
                    RemoveTarget();
                    return;
                }

                NpcMonsterSkill npcMonsterSkill = null;
                if (Skills != null && ServerManager.RandomNumber(0, 10) > 8)
                {
                    npcMonsterSkill = Skills.Where(s => (DateTime.Now - s.LastSkillUse).TotalMilliseconds >= 100 * s.Skill?.Cooldown)?.OrderBy(rnd => _random.Next())?.FirstOrDefault();
                }

                if (npcMonsterSkill?.Skill.TargetType == 1 && npcMonsterSkill?.Skill.HitType == 0)
                {
                    targetHit(targetSession, npcMonsterSkill);
                }

                // check if target is in range
                if (!targetSession.Character.InvisibleGm && !targetSession.Character.Invisible && targetSession.Character.Hp > 0)
                {
                    if (npcMonsterSkill != null && CurrentMp >= npcMonsterSkill.Skill.MpCost
                         && Map.GetDistance(new MapCell
                         {
                             X = MapX,
                             Y = MapY
                         },
                             new MapCell
                             {
                                 X = targetSession.Character.PositionX,
                                 Y = targetSession.Character.PositionY
                             }) < npcMonsterSkill.Skill.Range)
                    {
                        targetHit(targetSession, npcMonsterSkill);
                    }
                    else if (Map.GetDistance(new MapCell
                    {
                        X = MapX,
                        Y = MapY
                    },
                                new MapCell
                                {
                                    X = targetSession.Character.PositionX,
                                    Y = targetSession.Character.PositionY
                                }) <= Monster.BasicRange)
                    {
                        targetHit(targetSession, npcMonsterSkill);
                    }
                    else
                    {
                        followTarget(targetSession);
                    }
                }
                else
                {
                    followTarget(targetSession);
                }
            }
        }

        private void move()
        {
            // Normal Move Mode
            if (Monster == null || !IsAlive || _noMove)
            {
                return;
            }

            if (IsMoving && Monster.Speed > 0)
            {
                double time = (DateTime.Now - LastMove).TotalMilliseconds;
                if (Path == null)
                {
                    Path = new List<Node>();
                }
                if (Path.Count > 0) // move back to initial position after following target
                {
                    int timetowalk = 2000 / Monster.Speed;
                    if (time > timetowalk)
                    {
                        int maxindex = Path.Count > Monster.Speed / 2 ? Monster.Speed / 2 : Path.Count;
                        if (Path[maxindex - 1] == null)
                        {
                            return;
                        }
                        short mapX = Path[maxindex - 1].X;
                        short mapY = Path[maxindex - 1].Y;
                        double waitingtime = Map.GetDistance(new MapCell { X = mapX, Y = mapY }, new MapCell { X = MapX, Y = MapY }) / (double)Monster.Speed;
                        LastMove = DateTime.Now.AddSeconds(waitingtime > 1 ? 1 : waitingtime);

                        Observable.Timer(TimeSpan.FromMilliseconds(timetowalk)).Subscribe(x =>
                        {
                            MapX = mapX;
                            MapY = mapY;
                            MoveEvent?.Events.ForEach(e => EventHelper.Instance.RunEvent(e, monster: this));
                        });
                        Path.RemoveRange(0, maxindex > Path.Count ? Path.Count : maxindex);
                        MapInstance.Broadcast(new BroadcastPacket(null, PacketFactory.Serialize(StaticPacketHelper.Move(UserType.Monster, MapMonsterId, MapX, MapY, Monster.Speed)), ReceiverType.All, xCoordinate: mapX, yCoordinate: mapY));
                        return;
                    }
                }
                else if (time > _movetime)
                {
                    short mapX = FirstX, mapY = FirstY;
                    if (MapInstance.Map?.GetFreePosition(ref mapX, ref mapY, (byte)ServerManager.RandomNumber(0, 2), (byte)_random.Next(0, 2)) ?? false)
                    {
                        int distance = Map.GetDistance(new MapCell
                        {
                            X = mapX,
                            Y = mapY
                        }, new MapCell
                        {
                            X = MapX,
                            Y = MapY
                        });

                        double value = 1000d * distance / (2 * Monster.Speed);
                        Observable.Timer(TimeSpan.FromMilliseconds(value)).Subscribe(x =>
                        {
                            MapX = mapX;
                            MapY = mapY;
                        });

                        LastMove = DateTime.Now.AddMilliseconds(value);
                        MapInstance.Broadcast(new BroadcastPacket(null, PacketFactory.Serialize(StaticPacketHelper.Move(UserType.Monster, MapMonsterId, MapX, MapY, Monster.Speed)), ReceiverType.All));
                    }
                }
            }
            HostilityTarget();
        }

        private void removeBuff(short id)
        {
            Buff indicator = Buff[id];

            if (indicator != null)
            {
                Buff.Remove(id);
                if (indicator.Card.BCards.Any(s => s.Type == (byte)CardType.SpecialAttack && s.SubType.Equals((byte)AdditionalTypes.SpecialAttack.NoAttack / 10)))
                {
                    _noAttack = false;
                }
                if (indicator.Card.BCards.Any(s => s.Type == (byte)CardType.Move && s.SubType.Equals((byte)AdditionalTypes.Move.MovementImpossible / 10)))
                {
                    _noMove = false;
                }

                if (indicator.Card.BCards.Any(s => s.Type == (byte)CardType.SpecialActions && s.SubType.Equals((byte)AdditionalTypes.SpecialActions.Hide/ 10)))
                {
                    Invisible = false;
                }
            }
        }

        private void respawn()
        {
            if (Monster != null)
            {
                DamageList = new Dictionary<long, long>();
                IsAlive = true;
                Target = -1;
                CurrentHp = MaxHp;
                CurrentMp = MaxMp;
                MapX = FirstX;
                MapY = FirstY;
                Path = new List<Node>();
                MapInstance.Broadcast(GenerateIn());
            }
        }

        /// <summary>
        /// Hit the Target Character.
        /// </summary>
        /// <param name="targetSession"></param>
        /// <param name="npcMonsterSkill"></param>
        private void targetHit(ClientSession targetSession, NpcMonsterSkill npcMonsterSkill)
        {
            if (Monster != null && targetSession?.Character != null && ((DateTime.Now - LastSkill).TotalMilliseconds >= 1000 + (Monster.BasicCooldown * 200) || npcMonsterSkill != null) && !_noAttack)
            {
                int hitmode = 0;
                bool onyxWings = false;
                int damage = DamageHelper.Instance.CalculateDamage(new BattleEntity(this), new BattleEntity(targetSession.Character, null), npcMonsterSkill?.Skill, ref hitmode, ref onyxWings,0);

                if (npcMonsterSkill != null)
                {
                    if (CurrentMp < npcMonsterSkill.Skill.MpCost)
                    {
                        followTarget(targetSession);
                        return;
                    }
                    npcMonsterSkill.LastSkillUse = DateTime.Now;
                    CurrentMp -= npcMonsterSkill.Skill.MpCost;
                    MapInstance.Broadcast(StaticPacketHelper.CastOnTarget(UserType.Monster, MapMonsterId, 1, Target, npcMonsterSkill.Skill.CastAnimation, npcMonsterSkill.Skill.CastEffect, npcMonsterSkill.Skill.SkillVNum));
                }
                LastMove = DateTime.Now;


                // deal 0 damage to GM with GodMode
                if (targetSession.Character.HasGodMode)
                {
                    damage = 0;
                }

                if (hitmode != 1)
                {
                    targetSession.Character.RemoveBuff(85);
                    targetSession.Character.Invisible = false;
                }
                if (targetSession.Character.Invisible)
                {
                    targetSession.Character.Invisible = false;
                    targetSession.CurrentMapInstance?.Broadcast(targetSession.Character.GenerateInvisible());
                    targetSession.SendPacket(targetSession.Character.GenerateEq());

                    targetSession.Character.Mates.Where(m => m.IsTeamMember).ToList().ForEach(m =>
                        targetSession.CurrentMapInstance?.Broadcast(m.GenerateIn(), ReceiverType.AllExceptMe));
                    targetSession.CurrentMapInstance?.Broadcast(targetSession, targetSession.Character.GenerateIn(),
                        ReceiverType.AllExceptMe);
                    targetSession.CurrentMapInstance?.Broadcast(targetSession, targetSession.Character.GenerateGidx(),
                        ReceiverType.AllExceptMe);
                }
                if (targetSession.Character.IsSitting)
                {
                    targetSession.Character.IsSitting = false;
                    MapInstance.Broadcast(targetSession.Character.GenerateRest());
                }
                int castTime = 0;
                if (npcMonsterSkill != null && npcMonsterSkill.Skill.CastEffect != 0)
                {
                    MapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Monster, MapMonsterId, npcMonsterSkill.Skill.CastEffect), MapX, MapY);
                    castTime = npcMonsterSkill.Skill.CastTime * 100;
                }
                Observable.Timer(TimeSpan.FromMilliseconds(castTime)).Subscribe(o =>
                {
                    if (targetSession?.Character != null && targetSession.Character.Hp > 0)
                    {
                        TargetHit2(targetSession, npcMonsterSkill, damage, hitmode);
                    }
                });
            }
        }

        private void TargetHit2(ClientSession targetSession, NpcMonsterSkill npcMonsterSkill, int damage, int hitmode)
        {
            lock (targetSession.Character.PVELockObject)
            {
                if (targetSession.Character.Hp > 0)
                {
                    if (damage >= targetSession.Character.Hp &&
                        Monster.BCards.Any(s => s.Type == 39 && s.SubType == 0 && s.ThirdData == 1))
                    {
                        damage = targetSession.Character.Hp - 1;
                    }

                    targetSession.Character.GetDamage(damage);
                    MapInstance.Broadcast(null, targetSession.Character.GenerateStat(), ReceiverType.OnlySomeone,
                        string.Empty, Target);
                    MapInstance.Broadcast(npcMonsterSkill != null
                        ? StaticPacketHelper.SkillUsed(UserType.Monster, MapMonsterId, 1, Target,
                            npcMonsterSkill.SkillVNum, npcMonsterSkill.Skill.Cooldown,
                            npcMonsterSkill.Skill.AttackAnimation, npcMonsterSkill.Skill.Effect, MapX, MapY,
                            targetSession.Character.Hp > 0,
                            (int)(targetSession.Character.Hp / targetSession.Character.HPLoad() * 100), damage,
                            hitmode, 0)
                        : StaticPacketHelper.SkillUsed(UserType.Monster, MapMonsterId, 1, Target, 0,
                            Monster.BasicCooldown, 11, Monster.BasicSkill, 0, 0, targetSession.Character.Hp > 0,
                            (int)(targetSession.Character.Hp / targetSession.Character.HPLoad() * 100), damage,
                            hitmode, 0));
                    npcMonsterSkill?.Skill.BCards.ForEach(s => s.ApplyBCards(this));
                    LastSkill = DateTime.Now;
                    if (targetSession.Character.Hp <= 0)
                    {
                        RemoveTarget();
                        Observable.Timer(TimeSpan.FromMilliseconds(1000)).Subscribe(o =>
                            ServerManager.Instance.AskRevive((long)targetSession.Character?.CharacterId));
                    }
                }
            }

            if (npcMonsterSkill != null && (npcMonsterSkill.Skill.Range > 0 || npcMonsterSkill.Skill.TargetRange > 0))
            {
                foreach (Character characterInRange in MapInstance
                    .GetCharactersInRange(
                        npcMonsterSkill.Skill.TargetRange == 0 ? MapX : targetSession.Character.PositionX,
                        npcMonsterSkill.Skill.TargetRange == 0 ? MapY : targetSession.Character.PositionY,
                        npcMonsterSkill.Skill.TargetRange).Where(s =>
                        s.CharacterId != Target &&
                        (ServerManager.Instance.ChannelId != 51 ||
                         (MonsterVNum - (byte)s.Faction != 678 && MonsterVNum - (byte)s.Faction != 971)) &&
                        s.Hp > 0 && !s.InvisibleGm))
                {
                    if (characterInRange.IsSitting)
                    {
                        characterInRange.IsSitting = false;
                        MapInstance.Broadcast(characterInRange.GenerateRest());
                    }

                    if (characterInRange.HasGodMode)
                    {
                        damage = 0;
                        hitmode = 1;
                    }

                    if (characterInRange.Hp > 0)
                    {
                        characterInRange.GetDamage(damage);
                        MapInstance.Broadcast(null, characterInRange.GenerateStat(), ReceiverType.OnlySomeone,
                            string.Empty, characterInRange.CharacterId);
                        MapInstance.Broadcast(StaticPacketHelper.SkillUsed(UserType.Monster, MapMonsterId, 1,
                            characterInRange.CharacterId, 0, Monster.BasicCooldown, 11, Monster.BasicSkill, 0, 0,
                            characterInRange.Hp > 0, (int)(characterInRange.Hp / characterInRange.HPLoad() * 100),
                            damage, hitmode, 0));
                        if (characterInRange.Hp <= 0)
                        {
                            Observable.Timer(TimeSpan.FromMilliseconds(1000)).Subscribe(o =>
                            {
                                ServerManager.Instance.AskRevive((long)characterInRange?.CharacterId);
                            });
                        }
                    }
                }

                foreach (Mate mateInRange in MapInstance.Sessions.SelectMany(x => x.Character.Mates).Where(s =>
                    s.IsTeamMember && s.MateTransportId != Target &&
                    (ServerManager.Instance.ChannelId != 51 ||
                     (MonsterVNum - (byte)s.Owner.Faction != 678 && MonsterVNum - (byte)s.Owner.Faction != 971)) &&
                    s.Hp > 0 && !s.Owner.InvisibleGm && Map.GetDistance(
                        new MapCell()
                        {
                            X = npcMonsterSkill.Skill.TargetRange == 0 ? MapX : targetSession.Character.PositionX,
                            Y = npcMonsterSkill.Skill.TargetRange == 0 ? MapY : targetSession.Character.PositionY
                        }, new MapCell() { X = s.PositionX, Y = s.PositionY }) <= npcMonsterSkill.Skill.TargetRange))
                {
                    if (mateInRange.IsSitting)
                    {
                        mateInRange.IsSitting = false;
                        MapInstance.Broadcast(mateInRange.GenerateRest());
                    }

                    if (mateInRange.Owner.HasGodMode)
                    {
                        damage = 0;
                        hitmode = 1;
                    }

                    if (mateInRange.Hp > 0)
                    {
                        mateInRange.GetDamage(damage);
                        mateInRange.Owner.Session.SendPacket(mateInRange.GenerateStatInfo());
                        MapInstance.Broadcast(StaticPacketHelper.SkillUsed(UserType.Monster, MapMonsterId, 2,
                            mateInRange.MateTransportId, 0, Monster.BasicCooldown, 11, Monster.BasicSkill, 0, 0,
                            mateInRange.Hp > 0, mateInRange.Hp / (mateInRange.MaxHp * 100), damage, hitmode, 0));
                        if (mateInRange.Hp <= 0)
                        {
                            mateInRange.IsTeamMember = false;
                            mateInRange.Owner.Session.CurrentMapInstance.Broadcast(mateInRange.GenerateOut());
                            mateInRange.Owner.Session.SendPacket(mateInRange.Owner.GenerateSay(
                                string.Format(Language.Instance.GetMessageFromKey("PET_DIED"), mateInRange.Name), 11));
                            mateInRange.Owner.Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                string.Format(Language.Instance.GetMessageFromKey("PET_DIED"), mateInRange.Name), 0));

                        }
                    }
                }
            }
        }

     
        private void TargetHit2(Mate mate, NpcMonsterSkill npcMonsterSkill, int damage, int hitmode)
        {
            lock (mate.PveLockObject)
            {
                if (mate.Hp > 0)
                {
                    if (damage >= mate.Hp &&
                        Monster.BCards.Any(s => s.Type == 39 && s.SubType == 0 && s.ThirdData == 1))
                    {
                        damage = mate.Hp - 1;
                    }

                    mate.GetDamage(damage);
                    mate.Owner.Session.SendPacket(mate.GenerateStatInfo());
                    MapInstance.Broadcast(npcMonsterSkill != null
                        ? StaticPacketHelper.SkillUsed(UserType.Monster, MapMonsterId, 2, Target,
                            npcMonsterSkill.SkillVNum, npcMonsterSkill.Skill.Cooldown,
                            npcMonsterSkill.Skill.AttackAnimation, npcMonsterSkill.Skill.Effect, MapX, MapY,
                            mate.Hp > 0, mate.Hp / (mate.MaxHp * 100), damage, hitmode, 0)
                        : StaticPacketHelper.SkillUsed(UserType.Monster, MapMonsterId, 2, Target, 0,
                            Monster.BasicCooldown, 11, Monster.BasicSkill, 0, 0, mate.Hp > 0,
                            mate.Hp / (mate.MaxHp * 100), damage, hitmode, 0));
                    npcMonsterSkill?.Skill.BCards.ForEach(s => s.ApplyBCards(this));
                    LastSkill = DateTime.Now;
                    if (mate.Hp <= 0)
                    {
                        RemoveTarget();
                        mate.IsTeamMember = false;
                        mate.Owner.Session.CurrentMapInstance.Broadcast(mate.GenerateOut());
                        mate.Owner.Session.SendPacket(mate.Owner.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("PET_DIED"), mate.Name), 11));
                        mate.Owner.Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                            string.Format(Language.Instance.GetMessageFromKey("PET_DIED"), mate.Name), 0));
                    }
                }
            }

            if (npcMonsterSkill != null && (npcMonsterSkill.Skill.Range > 0 || npcMonsterSkill.Skill.TargetRange > 0))
            {
                foreach (Character characterInRange in MapInstance
                    .GetCharactersInRange(npcMonsterSkill.Skill.TargetRange == 0 ? MapX : mate.PositionX,
                        npcMonsterSkill.Skill.TargetRange == 0 ? MapY : mate.PositionY,
                        npcMonsterSkill.Skill.TargetRange).Where(s =>
                        s.CharacterId != Target &&
                        (ServerManager.Instance.ChannelId != 51 ||
                         (MonsterVNum - (byte)s.Faction != 678 && MonsterVNum - (byte)s.Faction != 971)) &&
                        s.Hp > 0 && !s.InvisibleGm))
                {
                    if (characterInRange.IsSitting)
                    {
                        characterInRange.IsSitting = false;
                        MapInstance.Broadcast(characterInRange.GenerateRest());
                    }

                    if (characterInRange.HasGodMode)
                    {
                        damage = 0;
                        hitmode = 1;
                    }

                    if (characterInRange.Hp > 0)
                    {
                        characterInRange.GetDamage(damage);
                        MapInstance.Broadcast(null, characterInRange.GenerateStat(), ReceiverType.OnlySomeone,
                            string.Empty, characterInRange.CharacterId);
                        MapInstance.Broadcast(StaticPacketHelper.SkillUsed(UserType.Monster, MapMonsterId, 1,
                            characterInRange.CharacterId, 0, Monster.BasicCooldown, 11, Monster.BasicSkill, 0, 0,
                            characterInRange.Hp > 0, (int)(characterInRange.Hp / characterInRange.HPLoad() * 100),
                            damage, hitmode, 0));
                        if (characterInRange.Hp <= 0)
                        {
                            Observable.Timer(TimeSpan.FromMilliseconds(1000)).Subscribe(o =>
                                ServerManager.Instance.AskRevive((long)characterInRange?.CharacterId));
                        }
                    }
                }

                foreach (Mate mateInRange in MapInstance.Sessions.SelectMany(x => x.Character.Mates).Where(s =>
                    s.IsTeamMember && s.MateTransportId != Target &&
                    (ServerManager.Instance.ChannelId != 51 ||
                     (MonsterVNum - (byte)s.Owner.Faction != 678 && MonsterVNum - (byte)s.Owner.Faction != 971)) &&
                    s.Hp > 0 && !s.Owner.InvisibleGm && Map.GetDistance(
                        new MapCell()
                        {
                            X = npcMonsterSkill.Skill.TargetRange == 0 ? MapX : mate.PositionX,
                            Y = npcMonsterSkill.Skill.TargetRange == 0 ? MapY : mate.PositionY
                        }, new MapCell() { X = s.PositionX, Y = s.PositionY }) <= npcMonsterSkill.Skill.TargetRange))
                {
                    if (mateInRange.IsSitting)
                    {
                        mateInRange.IsSitting = false;
                        MapInstance.Broadcast(mateInRange.GenerateRest());
                    }

                    if (mateInRange.Owner.HasGodMode)
                    {
                        damage = 0;
                        hitmode = 1;
                    }

                    if (mateInRange.Hp > 0)
                    {
                        mateInRange.GetDamage(damage);
                        mateInRange.Owner.Session.SendPacket(mateInRange.GenerateStatInfo());
                        MapInstance.Broadcast(StaticPacketHelper.SkillUsed(UserType.Monster, MapMonsterId, 2,
                            mateInRange.MateTransportId, 0, Monster.BasicCooldown, 11, Monster.BasicSkill, 0, 0,
                            mateInRange.Hp > 0, mateInRange.Hp / (mateInRange.MaxHp * 100), damage, hitmode, 0));
                        if (mateInRange.Hp <= 0)
                        {
                            mateInRange.IsTeamMember = false;
                            mateInRange.Owner.Session.CurrentMapInstance.Broadcast(mateInRange.GenerateOut());
                            mateInRange.Owner.Session.SendPacket(mateInRange.Owner.GenerateSay(
                                string.Format(Language.Instance.GetMessageFromKey("PET_DIED"), mateInRange.Name), 11));
                            mateInRange.Owner.Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                string.Format(Language.Instance.GetMessageFromKey("PET_DIED"), mateInRange.Name), 0));

                        }
                    }
                }
            }
        }
     
        #endregion
    }
}