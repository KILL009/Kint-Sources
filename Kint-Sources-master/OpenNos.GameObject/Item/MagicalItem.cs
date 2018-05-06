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
using OpenNos.GameObject.Networking;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace OpenNos.GameObject
{
    public class MagicalItem : Item
    {
        #region Instantiation

        public MagicalItem(ItemDTO item) : base(item)
        {
        }

        #endregion

        #region Methods

        public override void Use(ClientSession session, ref ItemInstance inv, byte Option = 0, string[] packetsplit = null)
        {
            switch (Effect)
            {
                // Shell
                case 0:
                    if (inv.Item.ItemType == ItemType.Shell)
                    {
                        if (inv.ShellEffects.Count != 0 && packetsplit?.Length > 9 && byte.TryParse(packetsplit[9], out byte islot))
                        {
                            ItemInstance wearable = session.Character.Inventory.LoadBySlotAndType<ItemInstance>(islot, InventoryType.Equipment);
                            if (wearable != null && (wearable.Item.ItemType == ItemType.Weapon || wearable.Item.ItemType == ItemType.Armor) && wearable.Item.LevelMinimum >= inv.Upgrade && wearable.Rare >= inv.Rare && !wearable.Item.IsHeroic)
                            {
                                bool weapon = false;
                                if ((inv.ItemVNum >= 565 && inv.ItemVNum <= 576) || (inv.ItemVNum >= 589 && inv.ItemVNum <= 598))
                                {
                                    weapon = true; 
                                }
                                else if ((inv.ItemVNum >= 577 && inv.ItemVNum <= 588) || (inv.ItemVNum >= 656 && inv.ItemVNum <= 664) || inv.ItemVNum == 599)
                                {
                                    weapon = false;
                                }
                                else
                                {
                                    return;
                                }
                                if ((wearable.Item.ItemType == ItemType.Weapon && weapon) || (wearable.Item.ItemType == ItemType.Armor && !weapon))
                                {
                                    if (inv.ShellEffects.Count > 0 && ServerManager.RandomNumber() < 50)
                                    {
                                        session.Character.DeleteItemByItemInstanceId(inv.Id);
                                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("OPTION_APPLY_FAIL"), 0));
                                        return;
                                    }
                                    wearable.ShellEffects.Clear();
                                    DAOFactory.ShellEffectDAO.DeleteByEquipmentSerialId(wearable.EquipmentSerialId);
                                    wearable.ShellEffects.AddRange(inv.ShellEffects);
                                    if (wearable.EquipmentSerialId == Guid.Empty)
                                    {
                                        wearable.EquipmentSerialId = Guid.NewGuid();
                                    }
                                    DAOFactory.ShellEffectDAO.InsertOrUpdateFromList(wearable.ShellEffects, wearable.EquipmentSerialId);

                                    session.Character.DeleteItemByItemInstanceId(inv.Id);
                                    session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("OPTION_APPLY_SUCCESS"), 0));
                                }
                            }
                        }
                        return;
                    }

                    if (ItemType == ItemType.Event)
                    {
                        session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, EffectValue));
                        if (MappingHelper.GuriItemEffects.ContainsKey(EffectValue))
                        {
                            session.CurrentMapInstance?.Broadcast(UserInterfaceHelper.GenerateGuri(19, 1, session.Character.CharacterId, MappingHelper.GuriItemEffects[EffectValue]), session.Character.MapX, session.Character.MapY);
                        }
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    break;

                case 10066:
                    if (session.Character.MapInstance.MapInstanceType != MapInstanceType.BaseMapInstance)
                    {
                        return;
                    }

                    session.Character.OpenBank();
                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    return;

                case 10541:
                   if (ItemType == ItemType.Event)
                    {
                        session.CurrentMapInstance?.Broadcast(session.Character.GenerateEff(EffectValue));
                        if (MappingHelper.GuriItemEffects.ContainsKey(EffectValue))
                        {
                            session.CurrentMapInstance?.Broadcast(UserInterfaceHelper.GenerateGuri(19, 1, session.Character.CharacterId, MappingHelper.GuriItemEffects[EffectValue]), session.Character.MapX, session.Character.MapY);
                        }

                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }

                    break;


                //respawn objects
                case 1:
                    if (session.Character.MapInstance.MapInstanceType != MapInstanceType.BaseMapInstance || ServerManager.Instance.ChannelId == 51)
                    {
                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_USE_THAT"), 10));
                        return;
                    }

                    if (inv.Item.VNum == 2188 /* LoD Teleport Scroll */)
                    {
                        session.Character.Inventory.RemoveItemAmount(inv.Item.VNum);
                        ServerManager.Instance.ChangeMap(session.Character.CharacterId, 98, (short)ServerManager.RandomNumber(12, 18), (short)ServerManager.RandomNumber(18, 24));
                        return;
                    }

                    int type, secondaryType, inventoryType, slot;
                    if (packetsplit != null && int.TryParse(packetsplit[2], out type) && int.TryParse(packetsplit[3], out secondaryType) && int.TryParse(packetsplit[4], out inventoryType) && int.TryParse(packetsplit[5], out slot))
                    {
                        int packetType;
                        switch (EffectValue)
                        {
                            case 0:
                                if (Option == 0)
                                {
                                    session.SendPacket(UserInterfaceHelper.GenerateDialog($"#u_i^{type}^{secondaryType}^{inventoryType}^{slot}^1 #u_i^{type}^{secondaryType}^{inventoryType}^{slot}^2 {Language.Instance.GetMessageFromKey("WANT_TO_SAVE_POSITION")}"));
                                }
                                else if (int.TryParse(packetsplit[6], out packetType))
                                {
                                    switch (packetType)
                                    {
                                        case 1:
                                            session.SendPacket(UserInterfaceHelper.GenerateDelay(5000, 7, $"#u_i^{type}^{secondaryType}^{inventoryType}^{slot}^3"));
                                            break;

                                        case 2:
                                            session.SendPacket(UserInterfaceHelper.GenerateDelay(5000, 7, $"#u_i^{type}^{secondaryType}^{inventoryType}^{slot}^4"));
                                            break;

                                        case 3:
                                            session.Character.SetReturnPoint(session.Character.MapId, session.Character.MapX, session.Character.MapY);
                                            RespawnMapTypeDTO respawn = session.Character.Respawn;
                                            if (respawn.DefaultX != 0 && respawn.DefaultY != 0 && respawn.DefaultMapId != 0)
                                            {
                                                ServerManager.Instance.ChangeMap(session.Character.CharacterId, respawn.DefaultMapId, (short)(respawn.DefaultX + ServerManager.RandomNumber(-5, 5)), (short)(respawn.DefaultY + ServerManager.RandomNumber(-5, 5)));
                                            }
                                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                            break;

                                        case 4:
                                            RespawnMapTypeDTO respawnObj = session.Character.Respawn;
                                            if (respawnObj.DefaultX != 0 && respawnObj.DefaultY != 0 && respawnObj.DefaultMapId != 0)
                                            {
                                                ServerManager.Instance.ChangeMap(session.Character.CharacterId, respawnObj.DefaultMapId, (short)(respawnObj.DefaultX + ServerManager.RandomNumber(-5, 5)), (short)(respawnObj.DefaultY + ServerManager.RandomNumber(-5, 5)));
                                            }
                                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                            break;
                                    }
                                }
                                break;

                          


                            case 1:
                                if (int.TryParse(packetsplit[6], out packetType))
                                {
                                    RespawnMapTypeDTO respawn = session.Character.Return;
                                    switch (packetType)
                                    {
                                        case 0:
                                            if (respawn.DefaultX != 0 && respawn.DefaultY != 0 && respawn.DefaultMapId != 0)
                                            {
                                                session.SendPacket(UserInterfaceHelper.GenerateRp(respawn.DefaultMapId, respawn.DefaultX, respawn.DefaultY, $"#u_i^{type}^{secondaryType}^{inventoryType}^{slot}^1"));
                                            }
                                            break;

                                        case 1:
                                            session.SendPacket(UserInterfaceHelper.GenerateDelay(5000, 7, $"#u_i^{type}^{secondaryType}^{inventoryType}^{slot}^2"));
                                            break;

                                        case 2:
                                            if (respawn.DefaultX != 0 && respawn.DefaultY != 0 && respawn.DefaultMapId != 0)
                                            {
                                                ServerManager.Instance.ChangeMap(session.Character.CharacterId, respawn.DefaultMapId, respawn.DefaultX, respawn.DefaultY);
                                            }
                                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                            break;
                                    }
                                }
                                break;

                            case 2:
                                if (Option == 0)
                                {
                                    session.SendPacket(UserInterfaceHelper.GenerateDelay(5000, 7, $"#u_i^{type}^{secondaryType}^{inventoryType}^{slot}^1"));
                                }
                                else
                                {
                                    ServerManager.Instance.JoinMiniland(session, session);
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                }
                                break;
                        }
                    }
                    break;

                // dyes or waxes
                case 10:
                case 11:
                    if (!session.Character.IsVehicled)
                    {
                        if (Effect == 10)
                        {
                            if (EffectValue == 99)
                            {
                                byte nextValue = (byte)ServerManager.RandomNumber(0, 127);
                                session.Character.HairColor = Enum.IsDefined(typeof(HairColorType), nextValue) ? (HairColorType)nextValue : 0;
                            }
                            else
                            {
                                session.Character.HairColor = Enum.IsDefined(typeof(HairColorType), (byte)EffectValue) ? (HairColorType)EffectValue : 0;
                            }
                        }
                        else
                        {
                            if (session.Character.Class == (byte)ClassType.Adventurer && EffectValue > 1)
                            {
                                session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ADVENTURERS_CANT_USE"), 10));
                                return;
                            }
                            session.Character.HairStyle = Enum.IsDefined(typeof(HairStyleType), (byte)EffectValue) ? (HairStyleType)EffectValue : 0;
                        }
                        session.SendPacket(session.Character.GenerateEq());
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateIn());
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateGidx());
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    break;

                // dignity restoration
                case 14:
                    if ((EffectValue == 100 || EffectValue == 200) && session.Character.Dignity < 100 && !session.Character.IsVehicled)
                    {
                        session.Character.Dignity += EffectValue;
                        if (session.Character.Dignity > 100)
                        {
                            session.Character.Dignity = 100;
                        }
                        session.SendPacket(session.Character.GenerateFd());
                        session.SendPacket(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 50 - (byte)session.Character.Faction));
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateIn(), ReceiverType.AllExceptMe);
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateGidx(), ReceiverType.AllExceptMe);
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    else if (EffectValue == 2000 && session.Character.Dignity < 100 && !session.Character.IsVehicled)
                    {
                        session.Character.Dignity = 100;
                        session.SendPacket(session.Character.GenerateFd());
                        session.SendPacket(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 50 - (byte)session.Character.Faction));
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateIn(), ReceiverType.AllExceptMe);
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateGidx(), ReceiverType.AllExceptMe);
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    break;

                // speakers
                case 15:
                    if (!session.Character.IsVehicled && Option == 0)
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateGuri(10, 3, session.Character.CharacterId, 1));
                    }
                    break;

                // bubbles
                case 16:
                    if (!session.Character.IsVehicled && Option == 0)
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateGuri(10, 4, session.Character.CharacterId, 1));
                    }
                    break;

                // wigs
                case 30:
                    if (!session.Character.IsVehicled)
                    {
                        ItemInstance wig = session.Character.Inventory.LoadBySlotAndType((byte)EquipmentType.Hat, InventoryType.Wear);
                        if (wig != null)
                        {
                            wig.Design = (byte)ServerManager.RandomNumber(0, 15);
                            session.SendPacket(session.Character.GenerateEq());
                            session.SendPacket(session.Character.GenerateEquipment());
                            session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateIn());
                            session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateGidx());
                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        }
                        else
                        {
                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_WIG"), 0));
                        }
                    }
                    break;

                case 300:
                    if (session.Character.Group != null && session.Character.Group.GroupType != GroupType.Group && session.Character.Group.IsLeader(session) && session.CurrentMapInstance.Portals.Any(s => s.Type == (short)PortalType.Raid))
                    {
                        int delay = 0;
                        foreach (ClientSession sess in session.Character.Group.Characters.GetAllItems())
                        {
                            Observable.Timer(TimeSpan.FromMilliseconds(delay)).Subscribe(o =>
                            {
                                if (sess?.Character != null && session?.CurrentMapInstance != null && session?.Character != null)
                                {
                                    ServerManager.Instance.ChangeMapInstance(sess.Character.CharacterId, session.CurrentMapInstance.MapInstanceId, session.Character.PositionX, session.Character.PositionY);
                                }
                            });
                            delay += 100;
                        }
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    break;

                default:
                    Logger.Warn(string.Format(Language.Instance.GetMessageFromKey("NO_HANDLER_ITEM"), GetType()));
                    break;
            }
        }

        #endregion
    }
}