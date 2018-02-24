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
using OpenNos.GameObject.Helpers;
using OpenNos.PathFinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using OpenNos.GameObject.Networking;
using static OpenNos.Domain.BCardType;

namespace OpenNos.GameObject
{
    public class MapNpc : MapNpcDTO
    {
        public MapNpc()
        {
        }

        public MapNpc(MapNpcDTO input)
        {
            Dialog = input.Dialog;
            Effect = input.Effect;
            EffectDelay = input.EffectDelay;
            IsDisabled = input.IsDisabled;
            IsMoving = input.IsMoving;
            IsSitting = input.IsSitting;
            MapId = input.MapId;
            MapNpcId = input.MapNpcId;
            MapX = input.MapX;
            MapY = input.MapY;
            NpcVNum = input.NpcVNum;
            Position = input.Position;
        }

        #region Members

        public NpcMonster Npc;

        private int _movetime;

        private Random _random;

        #endregion

        #region Properties

        public bool EffectActivated { get; set; }

        public short FirstX { get; set; }

        public short FirstY { get; set; }

        public bool IsHostile { get; set; }

        public bool IsMate { get; set; }

        public bool IsProtected { get; set; }

        public DateTime LastEffect { get; private set; }

        public DateTime LastMove { get; private set; }

        public IDisposable LifeEvent { get; set; }

        public MapInstance MapInstance { get; set; }

        public List<EventContainer> OnDeathEvents { get; set; }

        public List<Node> Path { get; set; }

        public List<Recipe> Recipes { get; set; }

        public Shop Shop { get; set; }

        public bool Started { get; internal set; }

        public long Target { get; set; }

        public List<TeleporterDTO> Teleporters { get; set; }

        #endregion

        #region Methods

        public string GenerateIn(InRespawnType respawnType = InRespawnType.NoEffect)
        {
            if (!IsDisabled)
            {
                return StaticPacketHelper.In(UserType.Npc, NpcVNum, MapNpcId, MapX, MapY, Position, 100, 100, Dialog, respawnType, IsSitting);
            }
            return string.Empty;
        }

        public string GetNpcDialog() => $"npc_req 2 {MapNpcId} {Dialog}";

        public void Initialize(MapInstance currentMapInstance)
        {
            MapInstance = currentMapInstance;
            Initialize();
        }

        public void Initialize()
        {
            _random = new Random(MapNpcId);
            Npc = ServerManager.GetNpc(NpcVNum);
            LastEffect = DateTime.Now;
            LastMove = DateTime.Now;
            IsHostile = Npc.IsHostile;
            FirstX = MapX;
            FirstY = MapY;
            EffectActivated = true;
            EffectDelay = 4000;
            _movetime = ServerManager.RandomNumber(500, 3000);
            Path = new List<Node>();
            Recipes = ServerManager.Instance.GetRecipesByMapNpcId(MapNpcId);
            Target = -1;
            Teleporters = ServerManager.Instance.GetTeleportersByNpcVNum((short)MapNpcId);
            Shop shop = ServerManager.Instance.GetShopByMapNpcId(MapNpcId);
            if (shop != null)
            {
                shop.Initialize();
                Shop = shop;
            }
        }

        public void RunDeathEvent()
        {
            MapInstance.InstanceBag.NpcsKilled++;
            OnDeathEvents.ForEach(e =>
            {
                if (e.EventActionType == EventActionType.THROWITEMS)
                {
                    Tuple<int, short, byte, int, int> evt = (Tuple<int, short, byte, int, int>)e.Parameter;
                    e.Parameter = new Tuple<int, short, byte, int, int>(MapNpcId, evt.Item2, evt.Item3, evt.Item4, evt.Item5);
                }
                EventHelper.Instance.RunEvent(e);
            });
            OnDeathEvents.RemoveAll(s => s != null);
        }

        /// <summary>
        /// Remove the current Target from Npc.
        /// </summary>
        internal void RemoveTarget()
        {
            if (Target != -1)
            {
                Path.Clear();
                Target = -1;

                //return to origin
                Path = BestFirstSearch.FindPathJagged(new Node { X = MapX, Y = MapY }, new Node { X = FirstX, Y = FirstY }, MapInstance.Map.JaggedGrid);
            }
        }

        internal void StartLife()
        {
            try
            {
                if (!MapInstance.IsSleeping)
                {
                    npcLife();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void npcLife()
        {
            double time = (DateTime.Now - LastEffect).TotalMilliseconds;
            if (time > EffectDelay)
            {
                if (IsMate || IsProtected)
                {
                    MapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, MapNpcId, 825), MapX, MapY);
                }
                if (Effect > 0 && EffectActivated)
                {
                    MapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, MapNpcId, Effect), MapX, MapY);
                }
                LastEffect = DateTime.Now;
            }

            time = (DateTime.Now - LastMove).TotalMilliseconds;
            if (IsMoving && Npc.Speed > 0 && time > _movetime)
            {
                _movetime = ServerManager.RandomNumber(500, 3000);
                byte point = (byte)ServerManager.RandomNumber(2, 4);
                byte fpoint = (byte)ServerManager.RandomNumber(0, 2);

                byte xpoint = (byte)ServerManager.RandomNumber(fpoint, point);
                byte ypoint = (byte)(point - xpoint);

                short mapX = FirstX;
                short mapY = FirstY;

                if (MapInstance.Map.GetFreePosition(ref mapX, ref mapY, xpoint, ypoint))
                {
                    double value = (xpoint + ypoint) / (double)(2 * Npc.Speed);
                    Observable.Timer(TimeSpan.FromMilliseconds(1000 * value)).Subscribe(x =>
                    {
                        MapX = mapX;
                        MapY = mapY;
                    });
                    LastMove = DateTime.Now.AddSeconds(value);
                    MapInstance.Broadcast(new BroadcastPacket(null, PacketFactory.Serialize(StaticPacketHelper.Move(UserType.Npc, MapNpcId, MapX, MapY, Npc.Speed)), ReceiverType.All, xCoordinate: mapX, yCoordinate: mapY));
                }
            }
            if (Target == -1)
            {
                if (IsHostile && Shop == null)
                {
                    MapMonster monster = MapInstance.Monsters.Find(s => MapInstance == s.MapInstance && Map.GetDistance(new MapCell { X = MapX, Y = MapY }, new MapCell { X = s.MapX, Y = s.MapY }) < (Npc.NoticeRange > 5 ? Npc.NoticeRange / 2 : Npc.NoticeRange));
                    ClientSession session = MapInstance.Sessions.FirstOrDefault(s => MapInstance == s.Character.MapInstance && Map.GetDistance(new MapCell { X = MapX, Y = MapY }, new MapCell { X = s.Character.PositionX, Y = s.Character.PositionY }) < Npc.NoticeRange);

                    if (monster != null && session != null)
                    {
                        Target = monster.MapMonsterId;
                    }
                }
            }
            else if (Target != -1)
            {
                MapMonster monster = MapInstance.Monsters.Find(s => s.MapMonsterId == Target);
                if (monster == null || monster.CurrentHp < 1)
                {
                    Target = -1;
                    return;
                }
                NpcMonsterSkill npcMonsterSkill = null;
                if (ServerManager.RandomNumber(0, 10) > 8)
                {
                    npcMonsterSkill = Npc.Skills.Where(s => (DateTime.Now - s.LastSkillUse).TotalMilliseconds >= 100 * s.Skill.Cooldown).OrderBy(rnd => _random.Next()).FirstOrDefault();
                }
                int hitmode = 0;
                bool onyxWings = false;
                int damage = DamageHelper.Instance.CalculateDamage(new BattleEntity(this), new BattleEntity(monster), npcMonsterSkill?.Skill, ref hitmode, ref onyxWings);
                if (monster.Monster.BCards.Find(s => s.Type == (byte)CardType.LightAndShadow && s.SubType == (byte)AdditionalTypes.LightAndShadow.InflictDamageToMP) is BCard card)
                {
                    int reduce = damage / 100 * card.FirstData;
                    if (monster.CurrentMp < reduce)
                    {
                        monster.CurrentMp = 0;
                    }
                    else
                    {
                        monster.CurrentMp -= reduce;
                    }
                }
                int distance = Map.GetDistance(new MapCell { X = MapX, Y = MapY }, new MapCell { X = monster.MapX, Y = monster.MapY });
                if (monster.CurrentHp > 0 && ((npcMonsterSkill != null && distance < npcMonsterSkill.Skill.Range) || distance <= Npc.BasicRange))
                {
                    if (((DateTime.Now - LastEffect).TotalMilliseconds >= 1000 + (Npc.BasicCooldown * 200) && Npc.Skills.Count == 0) || npcMonsterSkill != null)
                    {
                        if (npcMonsterSkill != null)
                        {
                            npcMonsterSkill.LastSkillUse = DateTime.Now;
                            MapInstance.Broadcast(StaticPacketHelper.CastOnTarget(UserType.Npc, MapNpcId, 3, Target, npcMonsterSkill.Skill.CastAnimation, npcMonsterSkill.Skill.CastEffect, npcMonsterSkill.Skill.SkillVNum));
                        }

                        if (npcMonsterSkill != null && npcMonsterSkill.Skill.CastEffect != 0)
                        {
                            MapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, MapNpcId, Effect));
                        }
                        monster.CurrentHp -= damage;
                        MapInstance.Broadcast(npcMonsterSkill != null
                            ? StaticPacketHelper.SkillUsed(UserType.Npc, MapNpcId, 3, Target, npcMonsterSkill.SkillVNum, npcMonsterSkill.Skill.Cooldown, npcMonsterSkill.Skill.AttackAnimation, npcMonsterSkill.Skill.Effect, 0, 0, monster.CurrentHp > 0, (int)((float)monster.CurrentHp / (float)monster.Monster.MaxHP * 100), damage, hitmode, 0)
                            : StaticPacketHelper.SkillUsed(UserType.Npc, MapNpcId, 3, Target, 0, Npc.BasicCooldown, 11, Npc.BasicSkill, 0, 0, monster.CurrentHp > 0, (int)((float)monster.CurrentHp / (float)monster.Monster.MaxHP * 100), damage, hitmode, 0));
                        LastEffect = DateTime.Now;
                        if (monster.CurrentHp < 1)
                        {
                            RemoveTarget();
                            monster.IsAlive = false;
                            monster.LastMove = DateTime.Now;
                            monster.CurrentHp = 0;
                            monster.CurrentMp = 0;
                            monster.Death = DateTime.Now;
                            Target = -1;
                        }
                    }
                }
                else
                {
                    int maxdistance = Npc.NoticeRange > 5 ? Npc.NoticeRange / 2 : Npc.NoticeRange;
                    if (IsMoving)
                    {
                        const short maxDistance = 5;
                        int maxindex = Path.Count > Npc.Speed / 2 && Npc.Speed > 1 ? Npc.Speed / 2 : Path.Count;
                        if (maxindex < 1)
                        {
                            maxindex = 1;
                        }
                        if ((Path.Count == 0 && distance >= 1 && distance < maxDistance) || (Path.Count >= maxindex && maxindex > 0 && Path[maxindex - 1] == null))
                        {
                            short xoffset = (short)ServerManager.RandomNumber(-1, 1);
                            short yoffset = (short)ServerManager.RandomNumber(-1, 1);

                            //go to monster
                            Path = BestFirstSearch.FindPathJagged(new GridPos { X = MapX, Y = MapY }, new GridPos { X = (short)(monster.MapX + xoffset), Y = (short)(monster.MapY + yoffset) }, MapInstance.Map.JaggedGrid);
                            maxindex = Path.Count > Npc.Speed / 2 && Npc.Speed > 1 ? Npc.Speed / 2 : Path.Count;
                        }
                        if (DateTime.Now > LastMove && Npc.Speed > 0 && Path.Count > 0)
                        {
                            short mapX = Path[maxindex - 1].X;
                            short mapY = Path[maxindex - 1].Y;
                            double waitingtime = Map.GetDistance(new MapCell { X = mapX, Y = mapY }, new MapCell { X = MapX, Y = MapY }) / (double)Npc.Speed;
                            MapInstance.Broadcast(new BroadcastPacket(null, PacketFactory.Serialize(StaticPacketHelper.Move(UserType.Npc, MapNpcId, MapX, MapY, Npc.Speed)), ReceiverType.All, xCoordinate: mapX, yCoordinate: mapY));
                            LastMove = DateTime.Now.AddSeconds(waitingtime > 1 ? 1 : waitingtime);

                            Observable.Timer(TimeSpan.FromMilliseconds((int)((waitingtime > 1 ? 1 : waitingtime) * 1000))).Subscribe(x =>
                            {
                                MapX = mapX;
                                MapY = mapY;
                            });

                            Path.RemoveRange(0, maxindex);
                        }
                        if (Target != -1 && (MapId != monster.MapId || distance > maxDistance))
                        {
                            RemoveTarget();
                        }
                    }
                }
            }
        }

        #endregion
    }
}