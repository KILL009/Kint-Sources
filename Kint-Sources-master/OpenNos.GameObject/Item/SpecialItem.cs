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
using OpenNos.Core.Extensions;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;

namespace OpenNos.GameObject
{
    public class SpecialItem : Item
    {
        #region Instantiation

        public SpecialItem(ItemDTO item) : base(item)
        {
        }

        #endregion

        #region Methods

        public override void Use(ClientSession session, ref ItemInstance inv, short Option = 0, string[] packetsplit = null)
        {
            inv.Item.BCards.ForEach(c => c.ApplyBCards(session.Character));

            switch (Effect)
            {
                // Honour Medals
                case 69:
                    session.Character.Reputation += ReputPrice;
                    session.SendPacket(session.Character.GenerateFd());
                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    break;

                // SP Potions
                case 150:
                case 151:
                    session.Character.SpAdditionPoint += EffectValue;
                    if (session.Character.SpAdditionPoint > 1000000)
                    {
                        session.Character.SpAdditionPoint = 1000000;
                    }
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("SP_POINTSADDED"), EffectValue), 0));
                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    session.SendPacket(session.Character.GenerateSpPoint());
                    break;

                // Specialist Medal
                case 204:
                    session.Character.SpPoint += EffectValue;
                    session.Character.SpAdditionPoint += EffectValue * 3;
                    if (session.Character.SpAdditionPoint > 1000000)
                    {
                        session.Character.SpAdditionPoint = 1000000;
                    }
                    if (session.Character.SpPoint > 10000)
                    {
                        session.Character.SpPoint = 10000;
                    }
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("SP_POINTSADDEDBOTH"), EffectValue, EffectValue * 3), 0));
                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    session.SendPacket(session.Character.GenerateSpPoint());
                    break;

                // Raid Seals
                case 301:
                    if (ServerManager.Instance.IsCharacterMemberOfGroup(session.Character.CharacterId))
                    {
                        //TODO you are in group
                        return;
                    }
                    ItemInstance raidSeal = session.Character.Inventory.LoadBySlotAndType<ItemInstance>(inv.Slot, InventoryType.Main);
                    session.Character.Inventory.RemoveItemFromInventory(raidSeal.Id);

                    ScriptedInstance raid = ServerManager.Instance.Raids.FirstOrDefault(s => s.RequiredItems?.Any(obj => obj?.VNum == raidSeal.ItemVNum) == true)?.Copy();
                    if (raid != null)
                    {
                        Group group = new Group()
                        {
                            GroupType = GroupType.BigTeam,
                            Raid = raid
                        };
                        group.JoinGroup(session.Character.CharacterId);
                        ServerManager.Instance.AddGroup(group);
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("RAID_LEADER"), session.Character.Name), 0));
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("RAID_LEADER"), session.Character.Name), 10));
                        if (session.Character.Level > raid.LevelMaximum || session.Character.Level < raid.LevelMinimum)
                        {
                            session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("RAID_LEVEL_INCORRECT"), 10));
                        }
                        session.SendPacket(session.Character.GenerateRaid(2, false));
                        session.SendPacket(session.Character.GenerateRaid(0, false));
                        session.SendPacket(session.Character.GenerateRaid(1, false));
                        session.SendPacket(group.GenerateRdlst());
                    }
                    break;

                // Partner Suits/Skins
                case 305:
                    Mate mate = session.Character.Mates.Find(s => s.MateTransportId == int.Parse(packetsplit[3]));
                    if (mate != null && EffectValue == mate.NpcMonsterVNum && mate.Skin == 0)
                    {
                        mate.Skin = Morph;
                        session.SendPacket(mate.GenerateCMode(mate.Skin));
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    break;

                // Fairy Booster
                case 250:
                    if (!session.Character.Buff.ContainsKey(131))
                    {
                        session.Character.AddStaticBuff(new StaticBuffDTO() { CardId = 131 });
                        session.CurrentMapInstance?.Broadcast(session.Character.GeneratePairy());
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), inv.Item.Name), 0));
                        session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3011), session.Character.MapX, session.Character.MapY);
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));
                    }
                    break;

                // Fairy booster 2
                case 253:
                    if (!session.Character.Buff.ContainsKey(131))
                    {
                        session.Character.AddStaticBuff(new StaticBuffDTO() { CardId = 131 });
                        session.CurrentMapInstance?.Broadcast(session.Character.GeneratePairy());
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), inv.Item.Name), 0));
                        session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3011), session.Character.MapX, session.Character.MapY);
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));
                    }
                    break;

                // exp/job booster
                case 251:
                    if (!session.Character.Buff.ContainsKey(121))
                    {
                        session.Character.AddStaticBuff(new StaticBuffDTO() { CardId = 121 });
                        session.CurrentMapInstance?.Broadcast(session.Character.GeneratePairy());
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), inv.Item.Name), 0));
                        session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3011), session.Character.MapX, session.Character.MapY);
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));
                    }
                    break;
                //sp job booster
                case 146:
                    if (!session.Character.Buff.ContainsKey(146))
                    {
                        session.Character.AddStaticBuff(new StaticBuffDTO() { CardId = 146 });
                        session.CurrentMapInstance?.Broadcast(session.Character.GeneratePairy());
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), inv.Item.Name), 0));
                        session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3011), session.Character.MapX, session.Character.MapY);
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));
                    }
                    break;

                // Ice oil
                case 252:
                    if (!session.Character.Buff.ContainsKey(340))
                    {
                        session.Character.AddStaticBuff(new StaticBuffDTO() { CardId = 340 });
                        session.CurrentMapInstance?.Broadcast(session.Character.GeneratePairy());
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), inv.Item.Name), 0));
                        session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 16), session.Character.MapX, session.Character.MapY);
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));
                    }
                    break;

                // Valentin Buff
                case 254:
                    if (!session.Character.Buff.ContainsKey(109))
                    {
                        session.Character.AddStaticBuff(new StaticBuffDTO() { CardId = 109 });
                        session.CurrentMapInstance?.Broadcast(session.Character.GeneratePairy());
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), inv.Item.Name), 0));
                        session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3410), session.Character.MapX, session.Character.MapY);
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));
                    }
                    break;

                // Buff exp 20%
                case 256:
                    if (!session.Character.Buff.ContainsKey(119))
                    {
                        session.Character.AddStaticBuff(new StaticBuffDTO() { CardId = 119 });
                        session.CurrentMapInstance?.Broadcast(session.Character.GeneratePairy());
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), inv.Item.Name), 0));
                        session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 4756), session.Character.MapX, session.Character.MapY);
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));
                    }
                    break;

                // Ancella fate
                case 258:
                    if (!session.Character.Buff.ContainsKey(393))
                    {
                        session.Character.AddStaticBuff(new StaticBuffDTO() { CardId = 393 });
                        session.CurrentMapInstance?.Broadcast(session.Character.GeneratePairy());
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), inv.Item.Name), 0));
                        session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 4756), session.Character.MapX, session.Character.MapY);
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));
                    }
                    break;

                // Speedbooster
                case 260:
                    if (!session.Character.IsVehicled || session.Character.Buff.Any(s => s.Card.CardId == 336))
                    {
                        return;
                    }
                    session.CurrentMapInstance?.Broadcast(session.Character.GenerateEff(885), session.Character.MapX, session.Character.MapY);
                    session.Character.AddBuff(new Buff(336));
                    session.Character.Speed += 5;
                    switch (session.Character.Morph)
                    {
                        case 2517: // Nossi M
                        case 2518: // Nossi F
                        case 2522: // Roller M
                        case 2523: // Roller F
                                   // Removes <= lv 4 debuffs
                            List<BuffType> bufftodisable = new List<BuffType> { BuffType.Bad };
                            session.Character.DisableBuffs(bufftodisable, 4);
                            break;
                    }
                    Observable.Timer(TimeSpan.FromSeconds(session.Character.BuffRandomTime * 0.1D)).Subscribe(o =>
                    {
                        session.Character.Speed -= 5;
                        session.Character.LoadSpeed();
                        switch (session.Character.Morph)
                        {
                            case 2526: // White male unicorn
                            case 2527: // White female unicorn
                            case 2528: // Pink male unicorn
                            case 2529: // Pink female unicorn
                            case 2530: // Black male unicorn
                            case 2531: // Black Female Unicorn
                            case 2928: // Male UFO
                            case 2929: // Female UFO
                            case 3679: // Male squelettic dragon
                            case 3680: // Female squelettic dragon
                                ServerManager.Instance.TeleportOnRandomPlaceInMap(session, session.Character.MapInstanceId, true);
                                break;

                            case 2432: // Magic broom
                            case 2433: // Magic broom F
                            case 2520: // VTT M
                            case 2521: // VTT F
                                switch (session.Character.Direction)
                                {
                                    case 0:
                                        // -y
                                        ServerManager.Instance.TeleportForward(session, session.Character.MapInstanceId, session.Character.PositionX, (short)(session.Character.PositionY - 5));
                                        break;
                                    case 1:
                                        // +x
                                        ServerManager.Instance.TeleportForward(session, session.Character.MapInstanceId, (short)(session.Character.PositionX + 5), session.Character.PositionY);
                                        break;
                                    case 2:
                                        // +y
                                        ServerManager.Instance.TeleportForward(session, session.Character.MapInstanceId, session.Character.PositionX, (short)(session.Character.PositionY + 5));
                                        break;
                                    case 3:
                                        // -x
                                        ServerManager.Instance.TeleportForward(session, session.Character.MapInstanceId, (short)(session.Character.PositionX - 5), session.Character.PositionY);
                                        break;
                                    case 4:
                                        ServerManager.Instance.TeleportForward(session, session.Character.MapInstanceId, (short)(session.Character.PositionX - 5), (short)(session.Character.PositionY - 5));
                                        // -x -y
                                        break;
                                    case 5:
                                        // +x +y
                                        ServerManager.Instance.TeleportForward(session, session.Character.MapInstanceId, (short)(session.Character.PositionX - 5), (short)(session.Character.PositionY - 5));
                                        break;
                                    case 6:
                                        // +x -y
                                        ServerManager.Instance.TeleportForward(session, session.Character.MapInstanceId, (short)(session.Character.PositionX + 5), (short)(session.Character.PositionY + 5));
                                        break;
                                    case 7:
                                        // -x +y
                                        ServerManager.Instance.TeleportForward(session, session.Character.MapInstanceId, (short)(session.Character.PositionX - 5), (short)(session.Character.PositionY + 5));
                                        break;
                                }
                                break;

                          
                        }
                    });
                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    break;



                // Rainbow Pearl/Magic Eraser
                case 666:
                    if (EffectValue == 1 && byte.TryParse(packetsplit[9], out byte islot))
                    {
                        ItemInstance wearInstance = session.Character.Inventory.LoadBySlotAndType(islot, InventoryType.Equipment);

                        if (wearInstance != null && (wearInstance.Item.ItemType == ItemType.Weapon || wearInstance.Item.ItemType == ItemType.Armor) && wearInstance.ShellEffects.Count != 0 && !wearInstance.Item.IsHeroic)
                        {
                            wearInstance.ShellEffects.Clear();
                            DAOFactory.ShellEffectDAO.DeleteByEquipmentSerialId(wearInstance.EquipmentSerialId);
                            if (wearInstance.EquipmentSerialId == Guid.Empty)
                            {
                                wearInstance.EquipmentSerialId = Guid.NewGuid();
                            }
                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("OPTION_DELETE"), 0));
                        }
                    }
                    else
                    {
                        session.SendPacket("guri 18 0");
                    }
                    break;

                // Atk/Def/HP/Exp potions
                case 6600:
                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    break;

                // Ancelloan's Blessing
                case 208:
                    if (!session.Character.Buff.ContainsKey(121))
                    {
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.Character.AddStaticBuff(new StaticBuffDTO() { CardId = 121 });
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));
                    }
                    break;

                case 2081:
                    if (!session.Character.Buff.ContainsKey(146))
                    {
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.Character.AddStaticBuff(new StaticBuffDTO() { CardId = 146 });
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));
                    }
                    break;

                // Divorce letter
                case 6969: // this is imaginary number I = √(-1)
                    var rel = session.Character.CharacterRelations.FirstOrDefault(s => s.RelationType == CharacterRelationType.Spouse);
                    if (rel != null)
                    {
                        session.Character.DeleteRelation(rel.CharacterId == session.Character.CharacterId ? rel.RelatedCharacterId : rel.CharacterId);
                        session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("DIVORCED")));

                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }

                    break;

                // Cupid's arrow
                case 34: // this is imaginary number I = √(-1)
                    if (packetsplit != null && packetsplit.Length > 3)
                    {
                        if (long.TryParse(packetsplit[3], out long characterId))
                        {
                            if (session.Character.CharacterRelations.Any(s => s.RelationType == CharacterRelationType.Spouse))
                            {
                                session.SendPacket($"info {Language.Instance.GetMessageFromKey("ALREADY_MARRIED")}");
                                return;
                            }

                            if (session.Character.IsFriendOfCharacter(characterId))
                            {
                                var otherSession = ServerManager.Instance.GetSessionByCharacterId(characterId);
                                if (otherSession != null)
                                {
                                    otherSession.SendPacket(UserInterfaceHelper.GenerateDialog(
                                        $"#fins^-34^{session.Character.CharacterId} #fins^-69^{session.Character.CharacterId} {string.Format(Language.Instance.GetMessageFromKey("MARRY_REQUEST"), session.Character.Name)}"));
                                    session.Character.FriendRequestCharacters.Add(characterId);
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                }
                            }
                            else
                            {
                                session.SendPacket($"info {Language.Instance.GetMessageFromKey("NOT_FRIEND")}");
                            }
                        }
                    }

                    break;

                case 100:
                    {

                    }
                    break;
                case 5836:
                    {
                        session.SendPacket($"gb 0 {session.Account.BankGold / 1000} {session.Character.Gold} 0 0");
                        session.SendPacket($"s_memo 6 [Account balance]: {session.Account.BankGold} gold; [Owned]: {session.Character.Gold} gold\nWe will do our best. Thank you for using the services of Cuarry Bank.");
                    }
                    break;

                // Faction Egg
                case 570:
                    if (session.Character.Faction == (FactionType)EffectValue)
                    {
                        return;
                    }
                    if (EffectValue < 3)
                    {
                        session.SendPacket(session.Character.Family == null
                            ? $"qna #guri^750^{EffectValue} {Language.Instance.GetMessageFromKey($"ASK_CHANGE_FACTION{EffectValue}")}"
                            : UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("IN_FAMILY"),
                            0));
                    }
                    else
                    {
                        session.SendPacket(session.Character.Family != null
                            ? $"qna #guri^750^{EffectValue} {Language.Instance.GetMessageFromKey($"ASK_CHANGE_FACTION{EffectValue}")}"
                            : UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_FAMILY"),
                            0));
                    }

                    break;

                // SP Wings
                case 650:
                    ItemInstance specialistInstance = session.Character.Inventory.LoadBySlotAndType((byte)EquipmentType.Sp, InventoryType.Wear);
                    if (session.Character.UseSp && specialistInstance != null)
                    {
                        if (Option == 0)
                        {
                            session.SendPacket($"qna #u_i^1^{session.Character.CharacterId}^{(byte)inv.Type}^{inv.Slot}^3 {Language.Instance.GetMessageFromKey("ASK_WINGS_CHANGE")}");
                        }
                        else
                        {
                            void disposeBuff(short vNum)
                            {
                                if (session.Character.BuffObservables.ContainsKey(vNum))
                                {
                                    session.Character.BuffObservables[vNum].Dispose();
                                    session.Character.BuffObservables.Remove(vNum);
                                }
                                session.Character.RemoveBuff(vNum);
                            }

                            disposeBuff(387);
                            disposeBuff(395);
                            disposeBuff(396);
                            disposeBuff(397);
                            disposeBuff(398);
                            disposeBuff(410);
                            disposeBuff(411);
                            disposeBuff(444);

                            specialistInstance.Design = (byte)EffectValue;

                            session.Character.MorphUpgrade2 = EffectValue;
                            session.CurrentMapInstance?.Broadcast(session.Character.GenerateCMode());
                            session.SendPacket(session.Character.GenerateStat());
                            session.SendPacket(session.Character.GenerateStatChar());
                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        }
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_SP"), 0));
                    }
                    break;

                // Self-Introduction
                case 203:
                    if (!session.Character.IsVehicled && Option == 0)
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateGuri(10, 2, session.Character.CharacterId, 1));
                    }
                    break;

                // Magic Lamp
                case 651:
                    if (session.Character.Inventory.All(i => i.Type != InventoryType.Wear))
                    {
                        if (Option == 0)
                        {
                            session.SendPacket($"qna #u_i^1^{session.Character.CharacterId}^{(byte)inv.Type}^{inv.Slot}^3 {Language.Instance.GetMessageFromKey("ASK_USE")}");
                        }
                        else
                        {
                            session.Character.ChangeSex();
                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        }
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("EQ_NOT_EMPTY"), 0));
                    }
                    break;

                // Vehicles
                case 1000:
                    if (EffectValue != 0 || ServerManager.Instance.ChannelId == 51 || session.CurrentMapInstance?.MapInstanceType == MapInstanceType.EventGameInstance)
                    {
                        return;
                    }
                    if (Morph > 0)
                    {
                        if (Option == 0 && !session.Character.IsVehicled)
                        {
                            if (session.Character.IsSitting)
                            {
                                session.Character.IsSitting = false;
                                session.CurrentMapInstance?.Broadcast(session.Character.GenerateRest());
                            }
                            session.Character.LastDelay = DateTime.Now;
                            session.SendPacket(UserInterfaceHelper.GenerateDelay(3000, 3, $"#u_i^1^{session.Character.CharacterId}^{(byte)inv.Type}^{inv.Slot}^2"));
                        }
                        else
                        {
                            if (!session.Character.IsVehicled && Option != 0)
                            {
                                DateTime delay = DateTime.Now.AddSeconds(-4);
                                if (session.Character.LastDelay > delay && session.Character.LastDelay < delay.AddSeconds(2))
                                {
                                    session.Character.Speed = Speed;
                                    session.Character.IsVehicled = true;
                                    session.Character.VehicleSpeed = Speed;
                                    session.Character.MorphUpgrade = 0;
                                    session.Character.MorphUpgrade2 = 0;
                                    session.Character.Morph = Morph + (byte)session.Character.Gender;
                                    session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 4809), session.Character.MapX, session.Character.MapY);
                                    session.CurrentMapInstance?.Broadcast(session.Character.GenerateCMode());
                                    session.SendPacket(session.Character.GenerateCond());
                                    session.Character.LastSpeedChange = DateTime.Now;
                                }
                            }
                            else if (session.Character.IsVehicled)
                            {
                                session.Character.Mates?.Where(s => s.IsTeamMember).ToList().ForEach(x =>
                                {
                                    x.PositionX = session.Character.PositionX;
                                    x.PositionY = session.Character.PositionY;
                                    session.CurrentMapInstance?.Broadcast(x.GenerateIn());
                                });
                                session.Character.RemoveVehicle();
                            }
                        }
                    }
                    break;

                // Sealed Vessel
                case 1002:
                    if (EffectValue == 69)
                    {
                        int rnd = ServerManager.RandomNumber(0, 1000);
                        if (rnd < 5)
                        {
                            short[] vnums =
                            {
                                5560, 5591, 4099, 907, 1160, 4705, 4706, 4707, 4708, 4709, 4710, 4711, 4712, 4713, 4714,
                                4715, 4716
                            };
                            byte[] counts = { 1, 1, 1, 1, 10, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
                            int item = ServerManager.RandomNumber(0, 17);
                            session.Character.GiftAdd(vnums[item], counts[item]);
                        }
                        else if (rnd < 30)
                        {
                            short[] vnums = { 361, 362, 363, 366, 367, 368, 371, 372, 373 };
                            session.Character.GiftAdd(vnums[ServerManager.RandomNumber(0, 9)], 1);
                        }
                        else
                        {
                            short[] vnums =
                            {
                                1161, 2282, 1030, 1244, 1218, 5369, 1012, 1363, 1364, 2160, 2173, 5959, 5983, 2514,
                                2515, 2516, 2517, 2518, 2519, 2520, 2521, 1685, 1686, 5087, 5203, 2418, 2310, 2303,
                                2169, 2280, 5892, 5893, 5894, 5895, 5896, 5897, 5898, 5899, 5332, 5105, 2161, 2162
                            };
                            byte[] counts =
                            {
                                10, 10, 20, 5, 1, 1, 99, 1, 1, 5, 5, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 5, 20,
                                20, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
                            };
                            int item = ServerManager.RandomNumber(0, 42);
                            session.Character.GiftAdd(vnums[item], counts[item]);
                        }
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    else if (session.HasCurrentMapInstance && session.CurrentMapInstance.Map.MapTypes.All(m => m.MapTypeId != (short)MapTypeEnum.Act4))
                    {
                        short[] vnums = { 1386, 1387, 1388, 1389, 1390, 1391, 1392, 1393, 1394, 1395, 1396, 1397, 1398, 1399, 1400, 1401, 1402, 1403, 1404, 1405 };
                        short vnum = vnums[ServerManager.RandomNumber(0, 20)];

                        NpcMonster npcmonster = ServerManager.GetNpc(vnum);
                        if (npcmonster == null)
                        {
                            return;
                        }
                        MapMonster monster = new MapMonster
                        {
                            MonsterVNum = vnum,
                            MapY = session.Character.MapY,
                            MapX = session.Character.MapX,
                            MapId = session.Character.MapInstance.Map.MapId,
                            Position = session.Character.Direction,
                            IsMoving = true,
                            MapMonsterId = session.CurrentMapInstance.GetNextMonsterId(),
                            ShouldRespawn = false
                        };
                        monster.Initialize(session.CurrentMapInstance);
                        session.CurrentMapInstance.AddMonster(monster);
                        session.CurrentMapInstance.Broadcast(monster.GenerateIn());
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    break;

                // Golden Bazaar Medal
                case 1003:
                    if (!session.Character.StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.BazaarMedalGold || s.StaticBonusType == StaticBonusType.BazaarMedalSilver))
                    {
                        session.Character.StaticBonusList.Add(new StaticBonusDTO
                        {
                            CharacterId = session.Character.CharacterId,
                            DateEnd = DateTime.Now.AddDays(EffectValue),
                            StaticBonusType = StaticBonusType.BazaarMedalGold
                        });
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                    }
                    break;

                // Silver Bazaar Medal
                case 1004:
                    if (!session.Character.StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.BazaarMedalGold || s.StaticBonusType == StaticBonusType.BazaarMedalGold))
                    {
                        session.Character.StaticBonusList.Add(new StaticBonusDTO
                        {
                            CharacterId = session.Character.CharacterId,
                            DateEnd = DateTime.Now.AddDays(EffectValue),
                            StaticBonusType = StaticBonusType.BazaarMedalSilver
                        });
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                    }
                    break;

                // Pet Slot Expansion
                case 1006:
                    if (Option == 0)
                    {
                        session.SendPacket($"qna #u_i^1^{session.Character.CharacterId}^{(byte)inv.Type}^{inv.Slot}^2 {Language.Instance.GetMessageFromKey("ASK_PET_MAX")}");
                    }
                    else if (session.Character.MaxMateCount < 30)
                    {
                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("GET_PET_PLACES"), 10));
                        session.SendPacket(session.Character.GenerateScpStc());
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    break;

                // Pet Basket
                case 1007:
                    if (session.Character.StaticBonusList.All(s => s.StaticBonusType != StaticBonusType.PetBasket))
                    {
                        session.Character.StaticBonusList.Add(new StaticBonusDTO
                        {
                            CharacterId = session.Character.CharacterId,
                            DateEnd = DateTime.Now.AddDays(EffectValue),
                            StaticBonusType = StaticBonusType.PetBasket
                        });
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateExts());
                        session.SendPacket("ib 1278 1");
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                    }
                    break;

                // Partner's Backpack
                case 1008:
                    if (session.Character.StaticBonusList.All(s => s.StaticBonusType != StaticBonusType.PetBackPack))
                    {
                        session.Character.StaticBonusList.Add(new StaticBonusDTO
                        {
                            CharacterId = session.Character.CharacterId,
                            DateEnd = DateTime.Now.AddDays(EffectValue),
                            StaticBonusType = StaticBonusType.PetBackPack
                        });
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateExts());
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                    }
                    break;

                // Backpack Expansion
                case 1009:
                    if (session.Character.StaticBonusList.All(s => s.StaticBonusType != StaticBonusType.BackPack))
                    {
                        session.Character.StaticBonusList.Add(new StaticBonusDTO
                        {
                            CharacterId = session.Character.CharacterId,
                            DateEnd = DateTime.Now.AddDays(EffectValue),
                            StaticBonusType = StaticBonusType.BackPack
                        });
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateExts());
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                    }
                    break;

                // Sealed Tarot Card
                case 1005:
                    session.Character.GiftAdd((short)(VNum - Effect), 1);
                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    break;

                // Tarot Card Game
                case 1894:
                    if (EffectValue == 0)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            session.Character.GiftAdd((short)(Effect + ServerManager.RandomNumber(0, 10)), 1);
                        }
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    break;

                // Sealed Tarot Card
                case 2152:
                    session.Character.GiftAdd((short)(VNum + Effect), 1);
                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    break;

                default:
                    switch (VNum)
                    {
                        case 5841:
                            int rnd = ServerManager.RandomNumber(0, 1000);
                            short[] vnums = null;
                            if (rnd < 900)
                            {
                                vnums = new short[] { 4356, 4357, 4358, 4359 };
                            }
                            else
                            {
                                vnums = new short[] { 4360, 4361, 4362, 4363 };
                            }
                            session.Character.GiftAdd(vnums[ServerManager.RandomNumber(0, 4)], 1);
                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                            break;

                        default:
                            Logger.Warn(string.Format(Language.Instance.GetMessageFromKey("NO_HANDLER_ITEM"), GetType()) + $" ItemVNum: {VNum} Effect: {Effect} EffectValue: {EffectValue}");
                            break;
                    }
                    break;
            }
        }

        #endregion
    }
}