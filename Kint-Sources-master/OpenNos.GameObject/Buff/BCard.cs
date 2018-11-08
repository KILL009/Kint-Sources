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
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;
using OpenNos.GameObject.Battle;
using OpenNos.GameObject.Event;



namespace OpenNos.GameObject
{


    public class BCard : BCardDTO
    {
        private object charact;
        private BattleEntity entity;
        private object targetEntity;

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

        public void ApplyBCards(object session, object sender = null,  short? partnerBuffLevel = null, object caster = null)

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
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.Defence.AllIncreased:
                            if (session.GetType() == typeof(Character))
                            {
                                if (session is Character character)
                                {
                                    if (IsLevelScaled)
                                    {
                                        character.Defence += character.Level * FirstData;
                                        character.DistanceDefence += character.Level * FirstData;
                                        character.MagicalDefence += character.Level * FirstData;
                                    }
                                    else
                                    {
                                        character.Defence += FirstData;
                                        character.DistanceDefence += FirstData;
                                        character.MagicalDefence += FirstData;
                                    }
                                }
                            }
                            break;
                    }
                    break;

                case BCardType.CardType.DodgeAndDefencePercent:
                    break;

                case BCardType.CardType.Block:
                    break;

                case BCardType.CardType.Absorption:
                    break;

                case BCardType.CardType.ElementResistance:
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.ElementResistance.AllIncreased:
                            if (session.GetType() == typeof(Character))
                            {
                                if (session is Character character)
                                {
                                    if (IsLevelScaled)
                                    {
                                        character.DarkResistance += character.Level * FirstData;
                                        character.FireResistance += character.Level * FirstData;
                                        character.LightResistance += character.Level * FirstData;
                                        character.WaterResistance += character.Level * FirstData;
                                    }
                                    else
                                    {
                                        character.DarkResistance += FirstData;
                                        character.FireResistance += FirstData;
                                        character.LightResistance += FirstData;
                                        character.WaterResistance += FirstData;
                                    }
                                }
                            }
                            break;
                    }
                    break;

                case BCardType.CardType.EnemyElementResistance:
                    break;

                case BCardType.CardType.Damage:
                    break;

                case BCardType.CardType.GuarantedDodgeRangedAttack:
                    break;

                case BCardType.CardType.Morale:
                    switch(SubType)
                    {
                        case (byte)AdditionalTypes.Morale.MoraleIncreased:
                            if (session is Character moralechar)
                            {
                                var moralecard = ServerManager.GetCardByCardId(CardId);

                                if (moralecard == null)
                                {
                                    return;

                                }

                                if (IsLevelScaled)
                                {
                                    moralechar.HitRate += FirstData * moralechar.Level;
                                    Observable.Timer(TimeSpan.FromSeconds(moralecard.Duration * 100)).Subscribe(s =>
                                   {
                                       moralechar.HitRate -= FirstData * moralechar.Level;

                                   });
                                }
                            }
                            break;
                    }

                    break;
                    

                case BCardType.CardType.Casting:
                    break;

                case BCardType.CardType.Reflection:
                    break;

                /*case BCardType.CardType.DrainAndSteal:
                    if (ServerManager.RandomNumber() < FirstData)
                    {
                        return;
                    }
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.DrainAndSteal.LeechEnemyHP:
                            int heal = 0;
                            switch (session)
                            {
                                case MapMonster toDrain when caster is Character drainer:
                                    heal = drainer.Level * SecondData;
                                    drainer.Hp = (int)(heal + drainer.Hp > drainer.HPLoad() ? drainer.HPLoad() : drainer.Hp + heal);
                                    drainer.MapInstance?.Broadcast(drainer.GenerateRc((int)(heal + drainer.Hp > drainer.HPLoad() ? drainer.HPLoad() - drainer.Hp : heal)));
                                    toDrain.CurrentHp -= heal;
                                    drainer.Session.SendPacket(drainer.GenerateStat());
                                    if (toDrain.CurrentHp <= 0)
                                    {
                                        toDrain.CurrentHp = 1;
                                    }

                                    break;
                                case Character characterDrained when caster is Character drainerCharacter:
                                    heal = drainerCharacter.Level * SecondData;
                                    drainerCharacter.Hp = (int)(heal + drainerCharacter.Hp > drainerCharacter.HPLoad() ? drainerCharacter.HPLoad() : drainerCharacter.Hp + heal);
                                    drainerCharacter.MapInstance?.Broadcast(drainerCharacter.GenerateRc((int)(heal + drainerCharacter.Hp > drainerCharacter.HPLoad() ? drainerCharacter.HPLoad() - drainerCharacter.Hp : heal)));
                                    characterDrained.Hp -= heal;
                                    characterDrained.Session.SendPacket(characterDrained.GenerateStat());
                                    drainerCharacter.Session.SendPacket(drainerCharacter.GenerateStat());
                                    if (characterDrained.Hp <= 0)
                                    {
                                        characterDrained.Hp = 1;
                                    }

                                    break;
                                case Character characterDrained when caster is MapMonster drainerMapMonster:
                                    heal = drainerMapMonster.Monster.Level * SecondData;
                                    drainerMapMonster.CurrentHp = (heal + drainerMapMonster.CurrentHp > drainerMapMonster.MaxHp ? drainerMapMonster.MaxHp : drainerMapMonster.CurrentHp + heal);
                                    drainerMapMonster.MapInstance?.Broadcast(drainerMapMonster.GenerateRc((heal + drainerMapMonster.CurrentHp > drainerMapMonster.MaxHp ? drainerMapMonster.MaxHp - drainerMapMonster.CurrentHp : heal)));
                                    characterDrained.Hp -= heal;
                                    characterDrained.Session.SendPacket(characterDrained.GenerateStat());
                                    if (characterDrained.Hp <= 0)
                                    {
                                        characterDrained.Hp = 1;
                                    }
                                    break;
                            }
                            break;
                        case (byte)AdditionalTypes.DrainAndSteal.LeechEnemyMP:
                            int mpDrain = 0;
                            switch (session)
                            {
                                case MapMonster toDrain when caster is Character drainer:
                                    mpDrain = drainer.Level * SecondData;
                                    drainer.Mp = (int)(mpDrain + drainer.Mp > drainer.MPLoad() ? drainer.MPLoad() : drainer.Mp + mpDrain);
                                    toDrain.CurrentMp -= mpDrain;
                                    drainer.Session.SendPacket(drainer.GenerateStat());
                                    if (toDrain.CurrentMp <= 0)
                                    {
                                        toDrain.CurrentMp = 1;
                                    }

                                    break;
                                case Character characterDrained when caster is Character drainerCharacter:
                                    mpDrain = drainerCharacter.Level * SecondData;
                                    drainerCharacter.Mp = (int)(mpDrain + drainerCharacter.Mp > drainerCharacter.MPLoad() ? drainerCharacter.MPLoad() : drainerCharacter.Mp + mpDrain);
                                    characterDrained.Mp -= mpDrain;
                                    characterDrained.Session.SendPacket(characterDrained.GenerateStat());
                                    drainerCharacter.Session.SendPacket(drainerCharacter.GenerateStat());
                                    if (characterDrained.Mp <= 0)
                                    {
                                        characterDrained.Mp = 1;
                                    }

                                    break;
                                case Character characterDrained when caster is MapMonster drainerMapMonster:
                                    // TODO: Add a MaxMp property to MapMonsters
                                    
                                    mpDrain = drainerMapMonster.Monster.Level * SecondData;
                                    drainerMapMonster.CurrentMp = (mpDrain + drainerMapMonster.CurrentMp > drainerMapMonster.MaxHp ? drainerMapMonster.MaxHp : drainerMapMonster.CurrentHp + mpDrain);
                                    drainerMapMonster.MapInstance?.Broadcast(drainerMapMonster.GenerateRc((mpDrain + drainerMapMonster.CurrentHp > drainerMapMonster.MaxHp ? drainerMapMonster.MaxHp - drainerMapMonster.CurrentHp : mpDrain)));
                                    characterDrained.Hp -= mpDrain;
                                    characterDrained.MapInstance?.Broadcast(characterDrained.GenerateStat());
                                    if (characterDrained.Hp <= 0)
                                    {
                                        characterDrained.Hp = 1;
                                    }
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;*/

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

              /*  case BCardType.CardType.SpecialisationBuffResistance:
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.SpecialisationBuffResistance.RemoveBadEffects:
                            List<BuffType> buffsToDisable = new List<BuffType> { BuffType.Bad };
                            switch (session)
                            {
                                case Character isCharacter:
                                    {
                                        if (FirstData <= ServerManager.RandomNumber())
                                        {
                                            break;
                                        }

                                        isCharacter.DisableBuffs(buffsToDisable, FirstData);
                                    }
                                    break;
                                case Mate isMate:
                                    {
                                        if (FirstData <= ServerManager.RandomNumber())
                                        {
                                            break;
                                        }

                                        
                                    }
                                    break;
                            }
                            break;
                        case (byte)AdditionalTypes.SpecialisationBuffResistance.RemoveGoodEffects:
                            List<BuffType> buffsToDisable2 = new List<BuffType> { BuffType.Good };
                            switch (session)
                            {
                                case Character isCharacter:
                                    {
                                        if (FirstData <= ServerManager.RandomNumber())
                                        {
                                            break;
                                        }
                                        isCharacter.DisableBuffs(buffsToDisable2, FirstData);
                                    }
                                    break;
                                case Mate isMate:
                                    {
                                        if (FirstData <= ServerManager.RandomNumber())
                                        {
                                            break;
                                        }

                                        
                                    }
                                    break;
                            }
                            break;
                    }
                    break;*/

                case BCardType.CardType.SpecialEffects:
                    Card speedCard = ServerManager.GetCard(CardId.Value);
                    if (speedCard == null)
                    {
                        break;
                    }

                    if (session is Character fun)
                    {
                        switch (SubType)
                        {
                            case (byte)AdditionalTypes.SpecialEffects.ShadowAppears:
                                fun.Session.CurrentMapInstance?.Broadcast($"guri 0 1 {fun.CharacterId} {FirstData} {SecondData}");
                                Observable.Timer(TimeSpan.FromSeconds(speedCard.Duration * 0.1)).Subscribe(s =>
                                {
                                    fun.Session.CurrentMapInstance?.Broadcast($"guri 0 1 {fun.CharacterId} 0 {SecondData}");
                                });
                                break;
                        }
                    }
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

               /* case BCardType.CardType.SpecialEffects2:
                    if (session is Character tp)
                    {
                        switch (SubType)
                        {
                            case (byte)AdditionalTypes.SpecialEffects2.TeleportInRadius:
                                tp.TeleportInRadius(FirstData);
                                break;
                        }
                    }

                    if (sender is Character teleportedUser)
                    {
                        switch (SubType)
                        {
                            case (byte)AdditionalTypes.SpecialEffects2.TeleportInRadius:
                                teleportedUser.TeleportInRadius(FirstData);
                                break;
                        }
                    }
                    break;*/

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
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.RecoveryAndDamagePercent.HPRecovered:
                            IDisposable obs = null;
                            switch (session)
                            {
                                case Character receiverCharacter:
                                    if (IsLevelScaled)
                                    {
                                        Card hcard = ServerManager.GetCardByCardId(CardId);
                                        if (hcard == null)
                                        {
                                            break;
                                        }

                                        int bonus = receiverCharacter.Level / FirstData;
                                        int heal = (int)(receiverCharacter.HPLoad() * (bonus * 0.01));

                                        obs = Observable.Interval(TimeSpan.FromSeconds(ThirdData + 1 < 0 ? 2 : ThirdData + 1)).Subscribe(s =>
                                        {
                                            if (receiverCharacter.Hp > 0)
                                            {
                                                receiverCharacter.Hp = (int)(receiverCharacter.Hp + heal > receiverCharacter.HPLoad() ? receiverCharacter.HPLoad() : receiverCharacter.Hp + heal);
                                                receiverCharacter.MapInstance?.Broadcast(receiverCharacter.GenerateRc(heal));
                                                receiverCharacter.Session.SendPacket(receiverCharacter.GenerateStat());
                                            }
                                            else
                                            {
                                                obs?.Dispose();
                                            }
                                        });

                                        Observable.Timer(TimeSpan.FromSeconds(hcard.Duration * 0.1)).Subscribe(s =>
                                        {
                                            obs?.Dispose();
                                        });
                                    }
                                    break;
                            }
                            break;

                       /* case (byte)AdditionalTypes.RecoveryAndDamagePercent.HPReduced:
                            switch (session)
                            {
                                case Character receiverCharacter:
                                    int loss = (int)(receiverCharacter.HPLoad() * (FirstData * 0.01));
                                    IDisposable rObs;
                                    Card rCard  = ServerManager.GetCardByCardId(CardId);

                                    if (rCard == null)
                                    {
                                        return;
                                    }

                                    if (rCard.Duration <= 0)
                                    {
                                        receiverCharacter.DotDebuff = Observable.Interval(TimeSpan.FromSeconds(ThirdData + 1)).Subscribe(s =>
                                        {
                                            if (receiverCharacter.Hp > 0)
                                            {
                                                receiverCharacter.Hp = receiverCharacter.Hp - loss <= 0 ? 1 : receiverCharacter.Hp - loss;
                                                receiverCharacter.MapInstance?.Broadcast(receiverCharacter.GenerateDm((ushort)loss));
                                                receiverCharacter.Session.SendPacket(receiverCharacter.GenerateStat());
                                            }
                                        });
                                        break;
                                    }

                                    rObs = Observable.Interval(TimeSpan.FromSeconds(ThirdData + 1)).Subscribe(s =>
                                    {
                                        if (receiverCharacter.Hp > 0)
                                        {
                                            receiverCharacter.Hp = receiverCharacter.Hp - loss <= 0 ? 1 : receiverCharacter.Hp - loss;
                                            receiverCharacter.MapInstance?.Broadcast(receiverCharacter.GenerateDm((ushort)loss));
                                            receiverCharacter.Session.SendPacket(receiverCharacter.GenerateStat());
                                        }
                                    });

                                    Observable.Timer(TimeSpan.FromSeconds(rCard.Duration * 0.1)).Subscribe(s =>
                                    {
                                        rObs.Dispose();
                                    });

                                    break;
                            }
                            break;*/
                    }
                    break;

                case BCardType.CardType.Count:
                    break;

                case BCardType.CardType.NoDefeatAndNoDamage:
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.NoDefeatAndNoDamage.TransferAttackPower: // = Charge
                        case (byte)AdditionalTypes.NoDefeatAndNoDamage.NeverReceiveDamage:
                            switch (session)
                            {
                                case Character receiverCharacter:
                                    receiverCharacter.HasGodMode = true;
                                    break;
                            }
                            break;

                    }
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

               /* case BCardType.CardType.LightAndShadow:
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.LightAndShadow.RemoveBadEffects:
                            List<BuffType> buffsToDisable = new List<BuffType> { BuffType.Bad };
                            switch (session)
                            {
                                case Character isCharacter:
                                    isCharacter.DisableBuffs(buffsToDisable, FirstData);
                                    break;
                                case Mate isMate:
                                    isMate.DisableBuffs(buffsToDisable, FirstData);
                                    break;
                            }
                            break;
                    }
                    break;*/

                case BCardType.CardType.Item:
                    if (session is Character charact)
                    {
                        var weapon =
                            charact.Inventory.LoadBySlotAndType<ItemInstance>((short)EquipmentType.MainWeapon,
                                InventoryType.Wear);
                        if (weapon != null)
                        {
                            foreach (BCard bcard in weapon.Item.BCards)
                            {
                                var b = new Buff((short)SecondData, charact.Level);
                                switch (b.Card?.BuffType)
                                {
                                    case BuffType.Good:
                                        bcard.ApplyBCards(charact, charact);
                                        break;
                                    case BuffType.Bad:
                                        bcard.ApplyBCards(targetEntity, charact);
                                        break;
                                }
                            }
                        }
                    }
                        break;

                case BCardType.CardType.DebuffResistance:
                    break;

              /*  case BCardType.CardType.SpecialBehaviour:
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.SpecialBehaviour.InflictOnTeam:
                            int delay = ThirdData + 1;
                            IDisposable teamObs = null;
                            switch (session)
                            {
                                case MapMonster inRangeMapMonster:
                                    {
                                        int range = FirstData;
                                        int timer = ThirdData + 1;
                                        Card buffCard = ServerManager.GetCardByCardId((short)SecondData);
                                        IEnumerable entitiesInRange = inRangeMapMonster.MapInstance?.GetListMonsterInRange(inRangeMapMonster.MapX, inRangeMapMonster.MapY, (byte)range);
                                        if (entitiesInRange == null || buffCard == null)
                                        {
                                            return;
                                        }

                                        teamObs = Observable.Interval(TimeSpan.FromSeconds(timer)).Subscribe(s =>
                                        {
                                            foreach (MapMonster monster in entitiesInRange)
                                            {
                                                if (monster.Buff.All(x => x.Card.CardId != buffCard.CardId))
                                                {
                                                    monster.AddBuff(new Buff(SecondData, entity));
                                                }
                                            }
                                        });

                                        Observable.Timer(TimeSpan.FromSeconds(buffCard.Duration * 0.1)).Subscribe(s =>
                                        {
                                            teamObs.Dispose();
                                        });
                                        break;
                                    }
                                case Character inRangeCharacter:
                                    {
                                        int range = FirstData;
                                        int timer = ThirdData + 1;
                                        Card buffCard = ServerManager.GetCardByCardId((short)SecondData);
                                        IEnumerable entitiesInRange = inRangeCharacter.MapInstance?.GetCharactersInRange(inRangeCharacter.MapX, inRangeCharacter.MapY, (byte)range);
                                        if (entitiesInRange == null || buffCard == null)
                                        {
                                            return;
                                        }

                                        teamObs = Observable.Interval(TimeSpan.FromSeconds(timer)).Subscribe(s =>
                                        {
                                            foreach (Character characterInRange in entitiesInRange)
                                            {
                                                if (characterInRange.Buff.All(x => x.Card.CardId != buffCard.CardId))
                                                {
                                                    characterInRange.AddBuff(new Buff(SecondData, entity: caster));
                                                }
                                            }
                                        });

                                        Observable.Timer(TimeSpan.FromSeconds(buffCard.Duration * 0.1)).Subscribe(s =>
                                        {
                                            teamObs.Dispose();
                                        });
                                        break;
                                    }
                                case Mate inRangeMate:
                                    {
                                        int range = FirstData;
                                        int timer = ThirdData + 1;
                                        Card buffCard = ServerManager.GetCardByCardId((short)SecondData);
                                        IEnumerable entitiesInRange = inRangeMate.MapInstance?.GetMatesInRang(inRangeMate.MapX, inRangeMate.MapY, (byte)range);
                                        if ( buffCard == null)
                                        {
                                            return;
                                        }

                                        teamObs = Observable.Interval(TimeSpan.FromSeconds(timer)).Subscribe(s =>
                                        {
                                            foreach (Mate mateInRange in entitiesInRange)
                                            {
                                                if (mateInRange.Buff.All(x => x.Card.CardId != buffCard.CardId))
                                                {
                                                    mateInRange.AddBuff(new Buff(SecondData, entity: caster));
                                                }
                                            }
                                        });

                                        Observable.Timer(TimeSpan.FromSeconds(buffCard.Duration * 0.1)).Subscribe(s =>
                                        {
                                            teamObs.Dispose();
                                        });
                                        break;
                                    }
                            }
                            break;
                    }
                    break;*/

                case BCardType.CardType.Quest:
                    break;

                case BCardType.CardType.SecondSPCard:
                    break;

                case BCardType.CardType.SPCardUpgrade:
                    break;

                case BCardType.CardType.HugeSnowman:
                    break;

              /*  case BCardType.CardType.Drain:
                    IDisposable drainObservable = null;
                    Card drainCard = ServerManager.GetCard(CardId.Value);
                    int drain = 0;
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.Drain.TransferEnemyHP:
                            switch (session)
                            {
                                case MapMonster targetMonster when  is Character casterChar:
                                    if (IsLevelScaled)
                                    {
                                        if (drainCard == null)
                                        {
                                            break;
                                        }

                                        drain = casterChar.Level * FirstData;
                                        drainObservable = Observable.Interval(TimeSpan.FromSeconds(ThirdData + 1)).Subscribe(s =>
                                        {
                                            if (targetMonster.CurrentHp > 0)
                                            {
                                                targetMonster.CurrentHp = targetMonster.CurrentHp - drain < 0 ? 1 : targetMonster.CurrentHp - drain;
                                                casterChar.Hp = (int)(casterChar.Hp + drain > casterChar.HPLoad() ? casterChar.HPLoad() : casterChar.Hp + drain);
                                                casterChar.MapInstance?.Broadcast(casterChar.GenerateRc(drain));
                                                casterChar.MapInstance?.Broadcast(targetMonster.GenerateDm((ushort)drain));
                                            }
                                            else
                                            {
                                                drainObservable?.Dispose();
                                            }
                                        });

                                        Observable.Timer(TimeSpan.FromSeconds(drainCard.Duration * 0.1)).Subscribe(s =>
                                        {
                                            drainObservable?.Dispose();
                                        });
                                    }
                                    break;
                                case Character targetCharacter when caster is Character casterChar:
                                    if (IsLevelScaled)
                                    {
                                        if (drainCard == null)
                                        {
                                            break;
                                        }

                                        drain = casterChar.Level * FirstData;
                                        drainObservable = Observable.Interval(TimeSpan.FromSeconds(ThirdData + 1)).Subscribe(s =>
                                        {
                                            if (targetCharacter.Hp > 0)
                                            {
                                                targetCharacter.Hp = targetCharacter.Hp - drain < 0 ? 1 : targetCharacter.Hp - drain;
                                                casterChar.Hp = (int)(casterChar.Hp + drain > casterChar.HPLoad() ? casterChar.HPLoad() : casterChar.Hp + drain);
                                                casterChar.MapInstance?.Broadcast(casterChar.GenerateRc(drain));
                                                casterChar.MapInstance?.Broadcast(targetCharacter.GenerateDm((ushort)drain));
                                            }
                                            else
                                            {
                                                drainObservable?.Dispose();
                                            }
                                        });

                                        Observable.Timer(TimeSpan.FromSeconds(drainCard.Duration * 0.1)).Subscribe(s =>
                                        {
                                            drainObservable?.Dispose();
                                        });
                                    }
                                    break;

                                case Character targetCharacter when caster is MapMonster casterMapMonster:
                                    if (IsLevelScaled)
                                    {
                                        if (drainCard == null)
                                        {
                                            break;
                                        }

                                        drain = casterMapMonster.Monster.Level * FirstData;
                                        drainObservable = Observable.Interval(TimeSpan.FromSeconds(ThirdData + 1)).Subscribe(s =>
                                        {
                                            if (targetCharacter.Hp > 0)
                                            {
                                                targetCharacter.Hp = targetCharacter.Hp - drain < 0 ? 1 : targetCharacter.Hp - drain;
                                                casterMapMonster.CurrentHp = casterMapMonster.CurrentHp + drain > casterMapMonster.MaxHp ? casterMapMonster.MaxHp : casterMapMonster.CurrentHp + drain;
                                                casterMapMonster.MapInstance?.Broadcast(casterMapMonster.GenerateRc(drain));
                                                casterMapMonster.MapInstance?.Broadcast(targetCharacter.GenerateDm((ushort)drain));
                                            }
                                            else
                                            {
                                                drainObservable?.Dispose();
                                            }
                                        });

                                        Observable.Timer(TimeSpan.FromSeconds(drainCard.Duration * 0.1)).Subscribe(s =>
                                        {
                                            drainObservable?.Dispose();
                                        });
                                    }
                                    break;
                            }
                            break;
                    }
                    break;*/

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
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.HideBarrelSkill.NoHPConsumption:
                            switch (session)
                            {
                                case Character receiverCharacter:
                                    receiverCharacter.HasGodMode = true;
                                    break;
                            }
                            break;
                    }
                    break;

                case BCardType.CardType.FocusEnemyAttentionSkill:
                    break;

               /* case BCardType.CardType.TauntSkill:
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.TauntSkill.ReflectsMaximumDamageFromNegated:
                            switch (session)
                            {
                                case Character recevierCharacter:
                                    if (!CardId.HasValue || CardId == 663)
                                    {
                                        return;
                                    }

                                    recevierCharacter.BattleEntity.IsReflecting = true;

                                    recevierCharacter.ReflectiveBuffs[CardId.Value] = FirstData;

                                    break;
                                case MapMonster receiverMapMonster:
                                    receiverMapMonster.BattleEntity.IsReflecting = true;
                                    if (!CardId.HasValue)
                                    {
                                        return;
                                    }

                                    receiverMapMonster.ReflectiveBuffs[CardId.Value] = FirstData;
                                    break;
                                case Mate receiverMate:
                                    if (!CardId.HasValue || CardId == 663)
                                    {
                                        return;
                                    }
                                    receiverMate.BattleEntity.IsReflecting = true;

                                    receiverMate.ReflectiveBuffs[CardId.Value] = FirstData;
                                    break;
                            }
                            break;
                    }
                    break;*/

                case BCardType.CardType.FireCannoneerRangeBuff:
                    break;

                case BCardType.CardType.VulcanoElementBuff:
                    break;

               /* case BCardType.CardType.DamageConvertingSkill:
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.DamageConvertingSkill.ReflectMaximumReceivedDamage:
                            switch (session)
                            {
                                case Character recevierCharacter:
                                    if (!CardId.HasValue || CardId == 663)
                                    {
                                        return;
                                    }
                                    recevierCharacter.BattleEntity.IsReflecting = true;

                                    recevierCharacter.ReflectiveBuffs[CardId.Value] = FirstData;

                                    break;
                                case MapMonster receiverMapMonster:
                                    if (!CardId.HasValue || CardId == 663)
                                    {
                                        return;
                                    }
                                    receiverMapMonster.BattleEntity.IsReflecting = true;

                                    receiverMapMonster.ReflectiveBuffs[CardId.Value] = FirstData;
                                    break;
                                case Mate receiverMate:
                                    if (!CardId.HasValue || CardId == 663 || receiverMate == null)
                                    {
                                        return;
                                    }
                                    receiverMate.BattleEntity.IsReflecting = true;

                                    receiverMate.ReflectiveBuffs[CardId.Value] = FirstData;
                                    break;
                            }
                            break;
                    }
                    break;*/

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

               /* case BCardType.CardType.FalconSkill:
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.FalconSkill.CausingChanceLocation:
                            if (session is Character trapper)
                            {
                                var trap = new MapMonster
                                {
                                    MonsterVNum = 1436,
                                    MapX = trapper.PositionX,
                                    MapY = trapper.PositionY,
                                    MapMonsterId = trapper.MapInstance.GetNextId(),
                                    IsHostile = false,
                                    IsMoving = false,
                                    ShouldRespawn = false
                                };

                                trapper.MapInstance?.AddMonster(trap);
                                trap.Initialize();
                                trapper.MapInstance?.Broadcast(trap.GenerateIn());

                                IDisposable dispo = null;

                                Thread.Sleep(1000);

                                dispo = Observable.Interval(TimeSpan.FromMilliseconds(250)).Subscribe(s =>
                                {
                                    if (trapper.MapInstance.IsPvp)
                                    {
                                        foreach (Character trapped in trapper.MapInstance.GetCharactersInRange(trap.MapX, trap.MapY, 2).Where(p => p.CharacterId != trapper.CharacterId))
                                        {
                                            trapper.MapInstance.Broadcast(StaticPacketHelper.SkillUsed(UserType.Monster, trap.MapMonsterId, 3, trap.MapMonsterId, 1250, 600, 11, 4270, trap.MapX, trap.MapY, true, 0, 0, -2, 0));
                                            trapped.AddBuff(new Buff(572, 1));
                                            trapped.AddBuff(new Buff(557, 1));
                                            trapper.MapInstance.RemoveMonster(trap);
                                            trapper.MapInstance.Broadcast(StaticPacketHelper.Out(UserType.Monster, trap.MapMonsterId));
                                            dispo?.Dispose();
                                        }
                                    }
                                    foreach (MapMonster trappedMonster in trapper.MapInstance.GetListMonsterInRange(trap.MapX, trap.MapY, 2).Where(m => m.MapMonsterId != trap.MapMonsterId))
                                    {
                                        trapper.MapInstance.Broadcast(StaticPacketHelper.SkillUsed(UserType.Monster, trap.MapMonsterId, 3, trap.MapMonsterId, 1250, 600, 11, 4270, trap.MapX, trap.MapY, true, 0, 0, -2, 0));
                                        trappedMonster.AddBuff(new Buff(572, 1));
                                        trappedMonster.AddBuff(new Buff(557, 1));
                                        trapper.MapInstance.RemoveMonster(trap);
                                        trapper.MapInstance.Broadcast(StaticPacketHelper.Out(UserType.Monster, trap.MapMonsterId));
                                        dispo?.Dispose();
                                    }

                                });

                                Observable.Timer(TimeSpan.FromSeconds(60)).Subscribe(s =>
                                {
                                    trapper.MapInstance.RemoveMonster(trap);
                                    trapper.MapInstance.Broadcast(StaticPacketHelper.Out(UserType.Monster, trap.MapMonsterId));
                                    dispo?.Dispose();
                                });
                            }
                            break;
                            case (byte)AdditionalTypes.FalconSkill.Hide:
                                if (charact == null)
                                {
                                    break;
                                }

                                charact.Invisible = true;
                                charact.Mates.Where(s => s.IsTeamMember).ToList().ForEach(s =>
                                    charact.Session.CurrentMapInstance?.Broadcast(s.GenerateOut()));
                                charact.Session.CurrentMapInstance?.Broadcast(charact.GenerateInvisible());
                                break;
                        }

                        break;*/

                    

                case BCardType.CardType.AbsorptionAndPowerSkill:
                    break;

                case BCardType.CardType.LeonaPassiveSkill:
                    break;

                case BCardType.CardType.FearSkill:
                    if (session is Character Fear)
                    {
                        switch (SubType)
                        {
                            case (byte)AdditionalTypes.FearSkill.MoveAgainstWill:
                                Fear.Session.SendPacket($"rv_m {Fear.CharacterId} 1 1");
                                Observable.Timer(TimeSpan.FromSeconds(10)).Subscribe(s =>
                                {
                                    Fear.Session.CurrentMapInstance?.Broadcast($"rv_m {Fear.CharacterId} 1 0");
                                });
                                break;
                        }
                    }
                    break;

                case BCardType.CardType.SniperAttack:
                    break;

                case BCardType.CardType.FrozenDebuff:
                    break;

               /* case BCardType.CardType.JumpBackPush:
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.JumpBackPush.JumpBackChance:
                            switch (session)
                            {
                                case MapMonster targetMob when caster is Character pushedbackChar:
                                    pushedbackChar.PushBackToDirection(SecondData / 2);
                                    break;
                            }
                            break;
                    }
                    break;*/

                case BCardType.CardType.FairyXPIncrease:
                    break;

                case BCardType.CardType.SummonAndRecoverHP:
                    break;

                case BCardType.CardType.TeamArenaBuff:
                    break;

                case BCardType.CardType.ArenaCamera:
                    break;

                case BCardType.CardType.DarkCloneSummon:
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.DarkCloneSummon.ConvertDamageToHPChance:
                            switch (session)
                            {
                                case Character thoughtCharacter:
                                    Card thoughtCard = ServerManager.GetCardByCardId(CardId);

                                    if (thoughtCard == null)
                                    {
                                        break;
                                    }

                                    thoughtCharacter.RetainedHp = thoughtCharacter.Hp;

                                    Observable.Timer(TimeSpan.FromSeconds(SecondData)).Subscribe(s =>
                                    {
                                        int total = thoughtCharacter.RetainedHp - thoughtCharacter.AccumulatedDamage;
                                        thoughtCharacter.Hp = total <= 0 ? 1 : total;
                                        thoughtCharacter.AccumulatedDamage = 0;
                                    });
                                    break;
                            }
                            break;
                    }
                    break;

                case BCardType.CardType.AbsorbedSpirit:
                    break;

                case BCardType.CardType.AngerSkill:
                    break;
                    
                case BCardType.CardType.StealBuff:
                    break;

                case BCardType.CardType.Unknown:
                    break;

                case BCardType.CardType.EffectSummon:
                    break;

                case BCardType.CardType.MeteoriteTeleport:
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.MeteoriteTeleport.CauseMeteoriteFall:
                            if (IsLevelScaled)
                            {
                                switch (session)
                                {
                                    case Character meteorCharacter:
                                        if (!SkillVNum.HasValue)
                                        {
                                            break;
                                        }
                                        Skill sk = ServerManager.GetSkill(SkillVNum.Value);
                                        int amount = meteorCharacter.Level / 5 + 10;
                                        int delay = 500;
                                        for (int i = 0; i < amount; i++)
                                        {
                                            meteorCharacter.MapInstance?.SpawnMeteorsOnRadius(20, meteorCharacter.Session, sk);
                                            if (delay > 0)
                                            {
                                                Thread.Sleep(delay);
                                            }
                                            delay -= delay > 100 ? 20 : 0;
                                        }
                                        break;
                                }
                            }
                            break;
                    }
                    break;

                case BCardType.CardType.DragonSkills:
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.DragonSkills.TransformationInverted:
                            if (session is Character reversedMorph && sender is ClientSession senderSession)
                             
                                {
                                reversedMorph.Morph = (byte)BrawlerMorphType.Normal;
                                reversedMorph.Session.SendPacket(reversedMorph.GenerateCMode());                  
                                senderSession.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, senderSession.Character.CharacterId, 196));
                                reversedMorph.DragonModeObservable?.Dispose();
                                reversedMorph.RemoveBuff(676);
                            }
                            break;
                        case (byte)AdditionalTypes.DragonSkills.Transformation:
                            Card morphCard = ServerManager.GetCardByCardId(CardId);

                            if (session is Character morphedChar && sender is ClientSession _senderSession)
                            {
                                morphedChar.Morph = (byte)BrawlerMorphType.Dragon;
                                morphedChar.Session.SendPacket(morphedChar.GenerateCMode());
                                _senderSession.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, _senderSession.Character.CharacterId, 196));
                                morphedChar.DragonModeObservable?.Dispose();
                               

                                morphedChar.DragonModeObservable = Observable.Timer(TimeSpan.FromSeconds(morphCard.Duration * 0.1)).Subscribe(s =>
                               
                                {
                                    morphedChar.Morph = (byte)BrawlerMorphType.Normal;
                                    morphedChar.Session.SendPacket(morphedChar.GenerateCMode());
                                    _senderSession.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, _senderSession.Character.CharacterId, 196));

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