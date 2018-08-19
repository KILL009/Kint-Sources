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
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;


namespace OpenNos.GameObject
{


    public class BCard : BCardDTO
    {
        public BCard()
        {

        }


        public BCard(BCardDTO input)
        {
            BCardId = input.BCardId;
            CardId = input.CardId;
            CastType = input.CastType;
            FirstData = input.FirstData;
            IsLevelDivided = input.IsLevelDivided;
            IsLevelScaled = input.IsLevelScaled;
            ItemVNum = input.ItemVNum;
            NpcMonsterVNum = input.NpcMonsterVNum;
            SecondData = input.SecondData;
            SkillVNum = input.SkillVNum;
            SubType = input.SubType;
            ThirdData = input.ThirdData;
            Type = input.Type;
        }

        #region Methods

        public void ApplyBCards(object session, object sender = null)
        {
            Type type = session.GetType();

            // int counterBuff = 0;
            if (type == null)
            {
                return;
            }
            switch ((BCardType.CardType)Type)
            {
                case BCardType.CardType.Buff:
                    {
                        if (type == typeof(Character) && session is Character character)
                        {
                            Buff buff = null;
                            if (sender != null)
                            {
                                Type sType = sender.GetType();
                                if (sType != null)
                                {
                                    if (sType == typeof(Character) && sender is Character sendingCharacter)
                                    {
                                        buff = new Buff((short)SecondData, sendingCharacter.Level);

                                        //Todo: Get anti stats from BCard
                                    }
                                }
                            }
                            else
                            {
                                buff = new Buff((short)SecondData, character.Level);
                            }
                            if (ServerManager.RandomNumber() < FirstData)
                            {
                                character.AddBuff(buff);
                            }
                        }
                        else if (type == typeof(MapMonster))
                        {
                            if (ServerManager.RandomNumber() < FirstData && session is MapMonster mapMonster)
                            {
                                mapMonster.AddBuff(new Buff((short)SecondData, mapMonster.Monster.Level));
                            }
                        }
                        else if (type == typeof(MapNpc))
                        {
                        }
                        else if (type == typeof(Mate))
                        {

                            if (ServerManager.RandomNumber() < FirstData && session is Mate mapMonster)
                            {
                                mapMonster.AddBuff(new Buff((short)SecondData, mapMonster.Monster.Level));
                            }
                        }
                        break;
                    }
                case BCardType.CardType.Move:
                    {
                        if (type == typeof(Character) && session is Character character)
                        {
                            character.LastSpeedChange = DateTime.Now;
                            character.Session.SendPacket(character.GenerateCond());
                        }
                    }
                    break;

                case BCardType.CardType.Summons:
                    if (type == typeof(Character))
                    {
                    }
                    else if (type == typeof(MapMonster))
                    {
                        if (session is MapMonster mapMonster)
                        {
                            List<MonsterToSummon> summonParameters = new List<MonsterToSummon>();
                            for (int i = 0; i < FirstData; i++)
                            {
                                short x = (short)(ServerManager.RandomNumber(-3, 3) + mapMonster.MapX);
                                short y = (short)(ServerManager.RandomNumber(-3, 3) + mapMonster.MapY);
                                summonParameters.Add(new MonsterToSummon((short)SecondData, new MapCell() { X = x, Y = y }, -1, true));
                            }
                            if (ServerManager.RandomNumber() <= Math.Abs(ThirdData) || ThirdData == 0)
                            {
                                switch (SubType)
                                {
                                    case 2:
                                        EventHelper.Instance.RunEvent(new EventContainer(mapMonster.MapInstance, EventActionType.SPAWNMONSTERS, summonParameters));
                                        break;

                                    default:
                                        if (!mapMonster.OnDeathEvents.Any(s => s.EventActionType == EventActionType.SPAWNMONSTERS))
                                        {
                                            mapMonster.OnDeathEvents.Add(new EventContainer(mapMonster.MapInstance, EventActionType.SPAWNMONSTERS, summonParameters));
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    else if (type == typeof(MapNpc))
                    {
                    }
                    else if (type == typeof(Mate))
                    {
                    }
                    break;

                case BCardType.CardType.SpecialAttack:
                    break;

                case BCardType.CardType.SpecialDefence:
                    break;

                case BCardType.CardType.AttackPower:
                    break;

                case BCardType.CardType.Target:
                    break;

                case BCardType.CardType.Critical:
                    break;

                case BCardType.CardType.SpecialCritical:
                    break;

                case BCardType.CardType.Element:
                    break;

                case BCardType.CardType.IncreaseDamage:
                    break;

                case BCardType.CardType.Defence:
                    break;

                case BCardType.CardType.DodgeAndDefencePercent:
                    break;

                case BCardType.CardType.Block:
                    break;

                case BCardType.CardType.Absorption:
                    break;

                case BCardType.CardType.ElementResistance:
                    break;

                case BCardType.CardType.EnemyElementResistance:
                    break;

                case BCardType.CardType.Damage:
                    break;

                case BCardType.CardType.GuarantedDodgeRangedAttack:
                    break;

                case BCardType.CardType.Morale:
                    break;

                case BCardType.CardType.Casting:
                    break;

                case BCardType.CardType.Reflection:
                    break;

                case BCardType.CardType.DrainAndSteal:
                    break;

                case BCardType.CardType.HealingBurningAndCasting:
                    if (type == typeof(Character))
                    {
                        if (session is Character character)
                        {
                            int bonus = 0;
                            if (SubType == (byte)AdditionalTypes.HealingBurningAndCasting.RestoreHP / 10)
                            {
                                if (IsLevelScaled)
                                {
                                    bonus = character.Level * FirstData;
                                }
                                else
                                {
                                    bonus = FirstData;
                                }
                                if (character.Hp + bonus <= character.HPLoad())
                                {
                                    character.Hp += bonus;
                                }
                                else
                                {
                                    bonus = (int)character.HPLoad() - character.Hp;
                                    character.Hp = (int)character.HPLoad();
                                }
                                character.Session.CurrentMapInstance?.Broadcast(character.Session, character.GenerateRc(bonus));
                            }
                            if (SubType == (byte)AdditionalTypes.HealingBurningAndCasting.RestoreMP / 10)
                            {
                                if (IsLevelScaled)
                                {
                                    bonus = character.Level * FirstData;
                                }
                                else
                                {
                                    bonus = FirstData;
                                }
                                if (character.Mp + bonus <= character.MPLoad())
                                {
                                    character.Mp += bonus;
                                }
                                else
                                {
                                    bonus = (int)character.MPLoad() - character.Mp;
                                    character.Mp = (int)character.MPLoad();
                                }
                            }
                            character.Session.SendPacket(character.GenerateStat());
                        }
                    }
                    else if (type == typeof(MapMonster))
                    {
                        if (ServerManager.RandomNumber() < FirstData && session is MapMonster mapMonster)
                        {
                            mapMonster.AddBuff(new Buff((short)SecondData, mapMonster.Monster.Level));
                        }
                    }
                    else if (type == typeof(MapNpc))
                    {
                    }
                    else if (type == typeof(Mate))
                    {
                    }
                    break;

                case BCardType.CardType.HPMP:
                    break;

                case BCardType.CardType.SpecialisationBuffResistance:
                    break;

                case BCardType.CardType.SpecialEffects:
                    break;

                case BCardType.CardType.Capture:
                    if (type == typeof(MapMonster))
                    {
                        if (session is MapMonster mapMonster && sender is ClientSession senderSession)
                        {
                            NpcMonster mateNpc = ServerManager.GetNpc(mapMonster.MonsterVNum);
                            if (mateNpc != null)
                            {
                                if (mapMonster.Monster.Catch)
                                {
                                    if (mapMonster.IsAlive && mapMonster.CurrentHp <= (int)((double)mapMonster.MaxHp / 2))
                                    {
                                        if (mapMonster.Monster.Level < senderSession.Character.Level)
                                        {
                                            int[] chance = { 100, 80, 60, 40, 20, 0 };
                                            if (ServerManager.RandomNumber() < chance[ServerManager.RandomNumber(0, 5)])
                                            {
                                                Mate mate = new Mate(senderSession.Character, mateNpc, (byte)(mapMonster.Monster.Level - 15 > 0 ? mapMonster.Monster.Level - 15 : 1), MateType.Pet);
                                                if (senderSession.Character.CanAddMate(mate))
                                                {
                                                    senderSession.Character.AddPetWithSkill(mate);
                                                    senderSession.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CATCH_SUCCESS"), 0));
                                                    senderSession.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, senderSession.Character.CharacterId, 197));
                                                    senderSession.CurrentMapInstance?.Broadcast(StaticPacketHelper.SkillUsed(UserType.Player, senderSession.Character.CharacterId, 3, mapMonster.MapMonsterId, -1, 0, 15, -1, -1, -1, true, (int)((float)mapMonster.CurrentHp / (float)mapMonster.MaxHp * 100), 0, -1, 0));
                                                    mapMonster.SetDeathStatement();
                                                    senderSession.CurrentMapInstance?.Broadcast(StaticPacketHelper.Out(UserType.Monster, mapMonster.MapMonsterId));
                                                }
                                                else
                                                {
                                                    senderSession.SendPacket(senderSession.Character.GenerateSay(Language.Instance.GetMessageFromKey("PET_SLOT_FULL"), 10));
                                                    senderSession.SendPacket(StaticPacketHelper.Cancel(2, mapMonster.MapMonsterId));
                                                }
                                            }
                                            else
                                            {
                                                senderSession.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CATCH_FAIL"), 0));
                                                senderSession.CurrentMapInstance?.Broadcast(StaticPacketHelper.SkillUsed(UserType.Player, senderSession.Character.CharacterId, 3, mapMonster.MapMonsterId, -1, 0, 15, -1, -1, -1, true, (int)((float)mapMonster.CurrentHp / (float)mapMonster.MaxHp * 100), 0, -1, 0));
                                            }
                                        }
                                        else
                                        {
                                            senderSession.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("LEVEL_LOWER_THAN_MONSTER"), 0));
                                            senderSession.SendPacket(StaticPacketHelper.Cancel(2, mapMonster.MapMonsterId));
                                        }
                                    }
                                    else
                                    {
                                        senderSession.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CURRENT_HP_TOO_HIGH"), 0));
                                        senderSession.SendPacket(StaticPacketHelper.Cancel(2, mapMonster.MapMonsterId));
                                    }
                                }
                                else
                                {
                                    senderSession.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("MONSTER_CANT_BE_CAPTURED"), 0));
                                    senderSession.SendPacket(StaticPacketHelper.Cancel(2, mapMonster.MapMonsterId));
                                }
                            }
                        }
                    }
                    break;

                case BCardType.CardType.SpecialDamageAndExplosions:
                    break;

                case BCardType.CardType.SpecialEffects2:
                    break;

                case BCardType.CardType.CalculatingLevel:
                    break;

                case BCardType.CardType.Recovery:
                    break;

                case BCardType.CardType.MaxHPMP:
                    break;

                case BCardType.CardType.MultAttack:
                    break;

                case BCardType.CardType.MultDefence:
                    break;

                case BCardType.CardType.TimeCircleSkills:
                    break;

                case BCardType.CardType.RecoveryAndDamagePercent:
                    break;

                case BCardType.CardType.Count:
                    break;

                case BCardType.CardType.NoDefeatAndNoDamage:
                    break;

                case BCardType.CardType.SpecialActions:
                    if (type == typeof(Character))
                    {
                        if (session is Character character && character.Hp > 0)
                        {

                            if (SubType == (byte)AdditionalTypes.SpecialActions.Hide / 10)
                            {
                                //Invisiblity
                                character.Session.Character.Invisible = true;
                                character.Session.CurrentMapInstance?.Broadcast(character.Session.Character.GenerateInvisible());
                                character.Session.SendPacket(character.Session.Character.GenerateEq());
                                if (character.Session.Character.Invisible)
                                {
                                    character.Session.Character.Mates.Where(s => s.IsTeamMember).ToList()
                                        .ForEach(s => character.Session.CurrentMapInstance?.Broadcast(s.GenerateOut()));
                                    character.Session.CurrentMapInstance?.Broadcast(character.Session,
                                        StaticPacketHelper.Out(UserType.Player, character.Session.Character.CharacterId), ReceiverType.AllExceptMe);
                                }

                                //Remove invisibility
                                System.Threading.Tasks.Task.Delay(120000).ContinueWith(t =>
                                {

                                    character.Session.Character.Invisible = false;
                                    character.Session.CurrentMapInstance?.Broadcast(character.Session.Character.GenerateInvisible());
                                    character.Session.SendPacket(character.Session.Character.GenerateEq());

                                    character.Session.Character.Mates.Where(m => m.IsTeamMember).ToList().ForEach(m =>
                                        character.Session.CurrentMapInstance?.Broadcast(m.GenerateIn(), ReceiverType.AllExceptMe));
                                    character.Session.CurrentMapInstance?.Broadcast(character.Session, character.Session.Character.GenerateIn(),
                                        ReceiverType.AllExceptMe);
                                    character.Session.CurrentMapInstance?.Broadcast(character.Session, character.Session.Character.GenerateGidx(),
                                        ReceiverType.AllExceptMe);

                                });


                            }
                            if (SubType == (byte)AdditionalTypes.SpecialActions.Charge / 10)
                            {
                                character.Session.Character.isAbsorbing = true;
                                System.Threading.Tasks.Task.Delay(2000).ContinueWith(t =>
                                {

                                    character.Session.Character.isAbsorbing = false;

                                });
                            }
                        }
                    }
                    break;
                case BCardType.CardType.Mode:
                    break;

                case BCardType.CardType.NoCharacteristicValue:
                    break;

                case BCardType.CardType.LightAndShadow:
                    break;

                case BCardType.CardType.Item:
                    break;

                case BCardType.CardType.DebuffResistance:
                    break;

                case BCardType.CardType.SpecialBehaviour:
                    break;

                case BCardType.CardType.Quest:
                    break;

                case BCardType.CardType.SecondSPCard:
                    break;

                case BCardType.CardType.SPCardUpgrade:
                    break;

                case BCardType.CardType.HugeSnowman:
                    break;

                case BCardType.CardType.Drain:
                    break;

                case BCardType.CardType.BossMonstersSkill:
                    break;

                case BCardType.CardType.LordHatus:
                    break;

                case BCardType.CardType.LordCalvinas:
                    break;

                case BCardType.CardType.SESpecialist:
                    break;

                case BCardType.CardType.FourthGlacernonFamilyRaid:
                    break;

                case BCardType.CardType.SummonedMonsterAttack:
                    break;

                case BCardType.CardType.BearSpirit:
                    break;

                case BCardType.CardType.SummonSkill:
                    break;

                case BCardType.CardType.InflictSkill:
                    break;

                case BCardType.CardType.HideBarrelSkill:
                    break;

                case BCardType.CardType.FocusEnemyAttentionSkill:
                    break;

                case BCardType.CardType.TauntSkill:
                    break;

                case BCardType.CardType.FireCannoneerRangeBuff:
                    break;

                case BCardType.CardType.VulcanoElementBuff:
                    break;

                case BCardType.CardType.DamageConvertingSkill:
                    break;

                case BCardType.CardType.MeditationSkill:
                    {
                        if (type == typeof(Character) && session is Character character)
                        {
                            if (SkillVNum.HasValue && SubType.Equals((byte)AdditionalTypes.MeditationSkill.CausingChance / 10) && ServerManager.RandomNumber() < FirstData)
                            {
                                Skill skill = ServerManager.GetSkill(SkillVNum.Value);
                                Skill newSkill = ServerManager.GetSkill((short)SecondData);
                                Observable.Timer(TimeSpan.FromMilliseconds(100)).Subscribe(observer =>
                                {
                                    foreach (QuicklistEntryDTO quicklistEntry in character.QuicklistEntries.Where(s => s.Pos.Equals(skill.CastId)))
                                    {
                                        character.Session.SendPacket($"qset {quicklistEntry.Q1} {quicklistEntry.Q2} {quicklistEntry.Type}.{quicklistEntry.Slot}.{newSkill.CastId}.0");
                                    }
                                    character.Session.SendPacket($"mslot {newSkill.CastId} -1");
                                });
                                character.SkillComboCount++;
                                character.LastSkillComboUse = DateTime.Now;
                                if (skill.CastId > 10)
                                {
                                    // HACK this way
                                    Observable.Timer(TimeSpan.FromMilliseconds((skill.Cooldown * 100) + 500)).Subscribe(observer => character.Session.SendPacket(StaticPacketHelper.SkillReset(skill.CastId)));
                                }
                            }
                            switch (SubType)
                            {
                                case 2:
                                    character.MeditationDictionary[(short)SecondData] = DateTime.Now.AddSeconds(4);
                                    break;

                                case 3:
                                    character.MeditationDictionary[(short)SecondData] = DateTime.Now.AddSeconds(8);
                                    break;

                                case 4:
                                    character.MeditationDictionary[(short)SecondData] = DateTime.Now.AddSeconds(12);
                                    break;
                            }
                        }
                    }
                    break;

                case BCardType.CardType.FalconSkill:

                    break;

                case BCardType.CardType.AbsorptionAndPowerSkill:
                    break;

                case BCardType.CardType.LeonaPassiveSkill:
                    break;

                case BCardType.CardType.FearSkill:
                    break;

                case BCardType.CardType.SniperAttack:
                    break;

                case BCardType.CardType.FrozenDebuff:
                    break;

                case BCardType.CardType.JumpBackPush:
                    break;

                case BCardType.CardType.FairyXPIncrease:
                    break;

                case BCardType.CardType.SummonAndRecoverHP:
                    break;

                case BCardType.CardType.TeamArenaBuff:
                    break;

                case BCardType.CardType.ArenaCamera:
                    break;

                case BCardType.CardType.DarkCloneSummon:
                    break;

                case BCardType.CardType.AbsorbedSpirit:
                    break;

                case BCardType.CardType.AngerSkill:
                    break;

                case BCardType.CardType.MeteoriteTeleport:
                    break;

                case BCardType.CardType.StealBuff:
                    break;

                case BCardType.CardType.Unknown:
                    break;

                case BCardType.CardType.EffectSummon:
                    break;

                case BCardType.CardType.DragonSkills:
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.DragonSkills.TransformationInverted:
                            if (session is Character reversedMorph)
                            {
                                reversedMorph.Morph = (byte)BrawlerMorphType.Normal;
                                reversedMorph.Session.SendPacket(reversedMorph.GenerateCMode());
                                reversedMorph.Session.SendPacket(reversedMorph.GenerateEff(196));
                                reversedMorph.DragonModeObservable?.Dispose();
                                reversedMorph.RemoveBuff(676);
                            }
                            break;
                        case (byte)AdditionalTypes.DragonSkills.Transformation:
                            Card morphCard = ServerManager.Instance.GetCardByCardId(CardId);

                            if (morphCard == null)
                            {
                                return;
                            }

                            if (session is Character morphedChar)
                            {
                                morphedChar.Morph = (byte)BrawlerMorphType.Dragon;
                                morphedChar.Session.SendPacket(morphedChar.GenerateCMode());
                                morphedChar.Session.SendPacket(morphedChar.GenerateEff(196));
                                morphedChar.DragonModeObservable?.Dispose();

                                morphedChar.DragonModeObservable = Observable.Timer(TimeSpan.FromSeconds(morphCard.Duration * 0.1)).Subscribe(s =>
                                {
                                    morphedChar.Morph = (byte)BrawlerMorphType.Normal;
                                    morphedChar.Session.SendPacket(morphedChar.GenerateCMode());
                                    morphedChar.Session.SendPacket(morphedChar.GenerateEff(196));
                                });
                            }
                            break;

                    }
                    break;
                        default:
                            Logger.Warn($"Card Type {Type} not defined!");
                            break;
                    

                    #endregion
            }
        }
    }
}