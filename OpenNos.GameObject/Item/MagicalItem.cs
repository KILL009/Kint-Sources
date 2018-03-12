using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

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

        public override void Use(ClientSession session, ref ItemInstance inv, byte option = 0, string[] packetsplit = null)
        {
            switch (Effect)
            {
                case 0:
                    if (ItemType == ItemType.Magical)
                    {
                        switch (VNum)
                        {
                            case 2539:
                            case 10066:
                                if (session.Character.MapInstance.MapInstanceType != MapInstanceType.BaseMapInstance)
                                {
                                    return;
                                }

                                session.Character.OpenBank();
                                session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                                return;
                        }
                    }

                    if (ItemType == ItemType.Event)
                    {
                        session.CurrentMapInstance?.Broadcast(session.Character.GenerateEff(EffectValue));
                        if (MappingHelper.Instance.GuriItemEffects.ContainsKey(EffectValue))
                        {
                            session.CurrentMapInstance?.Broadcast(UserInterfaceHelper.Instance.GenerateGuri(19, 1, session.Character.CharacterId, MappingHelper.Instance.GuriItemEffects[EffectValue]), session.Character.MapX, session.Character.MapY);
                        }

                        session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                    }

                    // APPLY SHELL ON EQUIPMENT
                    if (inv.Item.ItemType == ItemType.Shell)
                    {
                        if (!((WearableInstance)inv).EquipmentOptions.Any())
                        {
                            session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("SHELL_MUST_BE_IDENTIFIED"), 0));
                            return;
                        }

                        if (packetsplit == null || packetsplit.Length < 9 || !short.TryParse(packetsplit[9], out short eqSlot) || !Enum.TryParse(packetsplit[8], out InventoryType eqType) || !int.TryParse(packetsplit[6], out int requestType))
                        {
                            return;
                        }

                        var shell = (WearableInstance)inv;
                        var eq = session.Character.Inventory.LoadBySlotAndType<WearableInstance>(eqSlot, eqType);

                        if (eq == null)
                        {
                            return;
                        }

                        if (eq.Item.ItemType != ItemType.Armor && shell.Item.ItemSubType == 1)
                        {
                            session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("SHELL_FOR_ARMOR_ONLY"), 0));
                            return;
                        }

                        if (eq.Item.ItemType != ItemType.Weapon && shell.Item.ItemSubType == 0)
                        {
                            session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("SHELL_FOR_WEAPON_ONLY"), 0));
                            return;
                        }

                        switch (requestType)
                        {
                            case 0:
                                session.SendPacket(eq.EquipmentOptions.Any()
                                    ? $"qna #u_i^1^{session.Character.CharacterId}^{(short)inv.Type}^{inv.Slot}^1^1^{(short)eqType}^{eqSlot} {Language.Instance.GetMessageFromKey("ADD_OPTION_ON_STUFF_NOT_EMPTY")}"
                                    : $"qna #u_i^1^{session.Character.CharacterId}^{(short)inv.Type}^{inv.Slot}^1^1^{(short)eqType}^{eqSlot} {Language.Instance.GetMessageFromKey("ADD_OPTION_ON_STUFF")}");
                                break;

                            case 1:
                                if (shell.EquipmentOptions == null)
                                {
                                    // SHELL NOT IDENTIFIED
                                    return;
                                }

                                if (eq.BoundCharacterId != session.Character.CharacterId && eq.BoundCharacterId != null)
                                {
                                    session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("NEED_PARFUM_TO_CHANGE_SHELL"), 0));
                                    return;
                                }

                                if (eq.Rare < shell.Rare)
                                {
                                    session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("SHELL_RARITY_TOO_HIGH"), 0));
                                    return;
                                }

                                if (eq.Item.LevelMinimum < shell.Upgrade)
                                {
                                    session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("SHELL_LEVEL_TOO_HIGH"), 0));
                                    return;
                                }

                                if (eq.EquipmentOptions == null)
                                {
                                    eq.EquipmentOptions = new List<EquipmentOptionDTO>();
                                }

                                eq.EquipmentOptions.Clear();
                                foreach (EquipmentOptionDTO i in shell.EquipmentOptions)
                                {
                                    session.SendPacket(UserInterfaceHelper.Instance.GenerateGuri(17, 1, session.Character.CharacterId));
                                    session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("SHELL_OPTION_SET"), 0));
                                    eq.EquipmentOptions.Add(i);
                                }

                                eq.BoundCharacterId = session.Character.CharacterId;
                                eq.ShellRarity = shell.Rare;
                                session.Character.Inventory.RemoveItemAmountFromInventory(1, shell.Id);
                                break;
                        }
                    }

                    break;

                // Respawn items
                case 1:
                    if (session.Character.MapInstance.MapInstanceType != MapInstanceType.BaseMapInstance)
                    {
                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_USE_THAT"), 10));
                        return;
                    }
                    
                    if (inv.Item.VNum == 2188 /* LoD Teleport Scroll */)
                    {
                        session.Character.Inventory.RemoveItemAmount(inv.Item.VNum);
                        ServerManager.Instance.ChangeMap(session.Character.CharacterId, 98, (short)ServerManager.Instance.RandomNumber(12, 18), (short)ServerManager.Instance.RandomNumber(18, 24));
                        return;
                    }

                    if (packetsplit != null && int.TryParse(packetsplit[2], out var type) && int.TryParse(packetsplit[3], out var secondaryType) && int.TryParse(packetsplit[4], out var inventoryType) && int.TryParse(packetsplit[5], out var slot))
                    {
                        int packetType;
                        switch (EffectValue)
                        {
                            case 0:
                                if (option == 0)
                                {
                                    session.SendPacket(UserInterfaceHelper.Instance.GenerateDialog($"#u_i^{type}^{secondaryType}^{inventoryType}^{slot}^1 #u_i^{type}^{secondaryType}^{inventoryType}^{slot}^2 {Language.Instance.GetMessageFromKey("WANT_TO_SAVE_POSITION")}"));
                                }
                                else
                                {
                                    if (int.TryParse(packetsplit[6], out packetType))
                                    {
                                        switch (packetType)
                                        {
                                            case 1:
                                                session.SendPacket(UserInterfaceHelper.Instance.GenerateDelay(5000, 7, $"#u_i^{type}^{secondaryType}^{inventoryType}^{slot}^3"));
                                                break;

                                            case 2:
                                                session.SendPacket(UserInterfaceHelper.Instance.GenerateDelay(5000, 7, $"#u_i^{type}^{secondaryType}^{inventoryType}^{slot}^4"));
                                                break;

                                            case 3:
                                                session.Character.SetReturnPoint(session.Character.MapId, session.Character.MapX, session.Character.MapY);
                                                var respawn = session.Character.Respawn;
                                                if (respawn.DefaultX != 0 && respawn.DefaultY != 0 && respawn.DefaultMapId != 0)
                                                {
                                                    ServerManager.Instance.ChangeMap(session.Character.CharacterId, respawn.DefaultMapId, (short)(respawn.DefaultX + ServerManager.Instance.RandomNumber(-5, 5)), (short)(respawn.DefaultY + ServerManager.Instance.RandomNumber(-5, 5)));
                                                }

                                                session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                                                break;

                                            case 4:
                                                var respawnObj = session.Character.Respawn;
                                                if (respawnObj.DefaultX != 0 && respawnObj.DefaultY != 0 && respawnObj.DefaultMapId != 0)
                                                {
                                                    ServerManager.Instance.ChangeMap(session.Character.CharacterId, respawnObj.DefaultMapId, (short)(respawnObj.DefaultX + ServerManager.Instance.RandomNumber(-5, 5)), (short)(respawnObj.DefaultY + ServerManager.Instance.RandomNumber(-5, 5)));
                                                }

                                                session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                                                break;
                                        }
                                    }
                                }

                                break;

                            case 1:
                                if (int.TryParse(packetsplit[6], out packetType))
                                {
                                    var respawn = session.Character.Return;
                                    switch (packetType)
                                    {
                                        case 0:
                                            if (respawn.DefaultX != 0 && respawn.DefaultY != 0 && respawn.DefaultMapId != 0)
                                            {
                                                session.SendPacket(UserInterfaceHelper.Instance.GenerateRp(respawn.DefaultMapId, respawn.DefaultX, respawn.DefaultY, $"#u_i^{type}^{secondaryType}^{inventoryType}^{slot}^1"));
                                            }

                                            break;

                                        case 1:
                                            session.SendPacket(UserInterfaceHelper.Instance.GenerateDelay(5000, 7, $"#u_i^{type}^{secondaryType}^{inventoryType}^{slot}^2"));
                                            break;

                                        case 2:
                                            if (respawn.DefaultX != 0 && respawn.DefaultY != 0 && respawn.DefaultMapId != 0)
                                            {
                                                ServerManager.Instance.ChangeMap(session.Character.CharacterId, respawn.DefaultMapId, respawn.DefaultX, respawn.DefaultY);
                                            }

                                            session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                                            break;
                                    }
                                }

                                break;

                            case 2:
                                if (option == 0)
                                {
                                    session.SendPacket(UserInterfaceHelper.Instance.GenerateDelay(5000, 7, $"#u_i^{type}^{secondaryType}^{inventoryType}^{slot}^1"));
                                }
                                else
                                {
                                    ServerManager.Instance.JoinMiniland(session, session);
                                    session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
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
                                var nextValue = (byte)ServerManager.Instance.RandomNumber(0, 127);
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
                                session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("ADVENTURERS_CANT_USE"), 10));
                                return;
                            }

                            if (EffectValue == 10 || EffectValue == 11 || EffectValue == 13 || EffectValue == 15)
                            {
                                if ((EffectValue == 10 || EffectValue == 11) && session.Character.Gender == GenderType.Female)
                                {
                                    session.Character.HairStyle = Enum.IsDefined(typeof(HairStyleType), (byte)EffectValue) ? (HairStyleType)EffectValue : 0;
                                }
                                else if ((EffectValue == 13 || EffectValue == 15) && session.Character.Gender == GenderType.Male)
                                {
                                    session.Character.HairStyle = Enum.IsDefined(typeof(HairStyleType), (byte)EffectValue) ? (HairStyleType)EffectValue : 0;
                                }
                                else
                                {
                                    session.SendPacket("info You cant use the Item!");
                                    return;
                                }
                            }
                            else
                                session.Character.HairStyle = Enum.IsDefined(typeof(HairStyleType), (byte)EffectValue) ? (HairStyleType)EffectValue : 0;
                        }

                        session.SendPacket(session.Character.GenerateEq());
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateIn());
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateGidx());
                        session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
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
                        session.SendPacket(session.Character.GenerateEff(49 - (byte)session.Character.Faction));
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateIn(), ReceiverType.AllExceptMe);
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateGidx(), ReceiverType.AllExceptMe);
                        session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                    }
                    else if (EffectValue == 2000 && session.Character.Dignity < 100 && !session.Character.IsVehicled)
                    {
                        session.Character.Dignity = 100;
                        session.SendPacket(session.Character.GenerateFd());
                        session.SendPacket(session.Character.GenerateEff(49 - (byte)session.Character.Faction));
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateIn(), ReceiverType.AllExceptMe);
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateGidx(), ReceiverType.AllExceptMe);
                        session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                    }

                    break;

                // speakers
                case 15:
                    if (!session.Character.IsVehicled)
                    {
                        if (option == 0)
                        {
                            session.SendPacket(UserInterfaceHelper.Instance.GenerateGuri(10, 3, session.Character.CharacterId, 1));
                        }
                    }

                    break;

                // bubbles
                case 16:
                    if (!session.Character.IsVehicled)
                    {
                        if (option == 0)
                        {
                            session.SendPacket(UserInterfaceHelper.Instance.GenerateGuri(10, 4, session.Character.CharacterId, 1));
                        }
                    }

                    break;

                // wigs
                case 30:
                    if (!session.Character.IsVehicled)
                    {
                        var wig = session.Character.Inventory.LoadBySlotAndType<WearableInstance>((byte)EquipmentType.Hat, InventoryType.Wear);
                        if (wig != null)
                        {
                            wig.Design = (byte)ServerManager.Instance.RandomNumber(0, 15);
                            session.SendPacket(session.Character.GenerateEq());
                            session.SendPacket(session.Character.GenerateEquipment());
                            session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateIn());
                            session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateGidx());
                            session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                        }
                        else
                        {
                            session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("NO_WIG"), 0));
                        }
                    }

                    break;

                case 31:
                    {
                        if (session.Character.HairStyle == HairStyleType.ShortHairFemaleB || session.Character.HairStyle == HairStyleType.ShortHairMaleA || session.Character.HairStyle == HairStyleType.ShortHairFemaleC || session.Character.HairStyle == HairStyleType.ShortHairMaleC)
                        {
                            session.Character.HairStyle = ((session.Character.Gender == GenderType.Female) ? HairStyleType.ShortHairFemaleC : HairStyleType.ShortHairMaleC);

                            var nextValue = (byte)ServerManager.Instance.RandomNumber(0, 127);
                            session.Character.HairColor = Enum.IsDefined(typeof(HairColorType), nextValue) ? (HairColorType)nextValue : 0;

                            session.SendPacket(session.Character.GenerateEq());
                            session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateIn());
                            session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateGidx());
                            session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                        }
                        else
                        {
                            session.SendPacket("info You dont have the Right Hairs for it!");
                            return;
                        }
                    }

                    break;

                //Raid stone
                case 300:
                    if (session.Character.Group != null && session.Character.Group.GroupType != GroupType.Group && session.Character.Group.IsLeader(session) && session.CurrentMapInstance.Portals.Any(s => s.Type == (short)PortalType.Raid))
                    {
                        Parallel.ForEach(session.Character.Group.Characters.Replace(s => s.Character.Group?.GroupId == session.Character.Group?.GroupId), sess =>
                        {
                            ServerManager.Instance.TeleportOnRandomPlaceInMap(sess, session.CurrentMapInstance.MapInstanceId);
                        });
                        session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                    }

                    break;

                default:
                    Logger.Log.Warn(string.Format(Language.Instance.GetMessageFromKey("NO_HANDLER_ITEM"), GetType()));
                    break;
            }
        }

        #endregion
    }
}