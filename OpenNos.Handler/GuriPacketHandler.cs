using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Event;
using OpenNos.GameObject.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenNos.Handler
{
    internal class GuriPacketHandler : IPacketHandler
    {
        #region Instantiation

        public GuriPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        private ClientSession Session { get; }

        #endregion

        #region Methods

        /// <summary>
        /// guri packet
        /// </summary>
        /// <param name="guriPacket"></param>
        public void Guri(GuriPacket guriPacket)
        {
            if (guriPacket == null)
            {
                return;
            }

            if (guriPacket.Type == 10 && guriPacket.Data >= 973 && guriPacket.Data <= 999 && !Session.Character.EmoticonsBlocked)
            {
                if (guriPacket.User != null && Convert.ToInt64(guriPacket.User.Value) == Session.Character.CharacterId)
                {
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateEff(guriPacket.Data + 4099), ReceiverType.AllNoEmoBlocked);
                }
                else
                {
                    var mate = Session.Character.Mates.FirstOrDefault(s => guriPacket.User != null && s.MateTransportId == Convert.ToInt32(guriPacket.User.Value));
                    if (mate != null)
                    {
                        Session.CurrentMapInstance?.Broadcast(Session, mate.GenerateEff(guriPacket.Data + 4099), ReceiverType.AllNoEmoBlocked);
                    }
                }

                return;
            }

            switch (guriPacket.Type)
            {
                case 2:
                    Session.CurrentMapInstance?.Broadcast(UserInterfaceHelper.Instance.GenerateGuri(2, 1, Session.Character.CharacterId), Session.Character.PositionX,
                        Session.Character.PositionY);
                    break;

                case 4:
                    const int SPEAKER_VNUM = 2173;
                    const int PETNAME_VNUM = 2157;

                    switch (guriPacket.Argument)
                    {
                        case 1:
                            var mate = Session.Character.Mates.FirstOrDefault(s => s.MateTransportId == guriPacket.Data);
                            if (guriPacket.Value.Length > 15)
                            {
                                Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("NEW_NAME_PET_MAX_LENGTH")));
                                return;
                            }

                            if (mate != null)
                            {
                                mate.Name = guriPacket.Value;
                                Session.CurrentMapInstance.Broadcast(mate.GenerateOut());
                                Session.CurrentMapInstance.Broadcast(mate.GenerateIn());
                                Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("NEW_NAME_PET")));
                                Session.SendPacket(Session.Character.GeneratePinit());
                                Session.SendPackets(Session.Character.GeneratePst());
                                Session.SendPackets(Session.Character.GenerateScP());
                                Session.Character.Inventory.RemoveItemAmount(PETNAME_VNUM);
                            }

                            break;

                        case 2:
                            var presentationVNum = Session.Character.Inventory.CountItem(1117) > 0 ? 1117 : (Session.Character.Inventory.CountItem(9013) > 0 ? 9013 : -1);
                            if (presentationVNum != -1)
                            {
                                var message = string.Empty;

                                // message = $" ";
                                string[] valuesplit = guriPacket.Value.Split(' ');
                                message = valuesplit.Aggregate(message, (current, t) => current + t + "^");
                                message = message.Substring(0, message.Length - 1); // Remove the last ^
                                message = message.Trim();
                                if (message.Length > 60)
                                {
                                    message = message.Substring(0, 60);
                                }

                                Session.Character.Biography = message;
                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("INTRODUCTION_SET"), 10));
                                Session.Character.Inventory.RemoveItemAmount(presentationVNum);
                            }

                            break;

                        case 3:
                            if (Session.Character.Inventory.CountItem(SPEAKER_VNUM) > 0)
                            {
                                if (Session.Character == null || guriPacket.Value == null)
                                {
                                    return;
                                }

                                var message = $"<{Language.Instance.GetMessageFromKey("SPEAKER")}> [{Session.Character.Name}]:";
                                string[] valuesplit = guriPacket.Value.Split(' ');
                                message = valuesplit.Aggregate(message, (current, t) => current + t + " ");
                                if (message.Length > 120)
                                {
                                    message = message.Substring(0, 120);
                                }

                                message = message.Trim();

                                if (Session.Character.IsMuted())
                                {
                                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("SPEAKER_CANT_BE_USED"), 10));
                                    return;
                                }

                                Session.Character.Inventory.RemoveItemAmount(SPEAKER_VNUM);
                                ServerManager.Instance.Broadcast(Session.Character.GenerateSay(message, 13));
                                LogHelper.Instance.InsertChatLog(ChatType.Speaker, Session.Character.CharacterId, message, Session.IpAddress);
                            }

                            break;
                    }
                    break;

                case 199:
                    if (guriPacket.Argument == 1)
                    {
                        if (guriPacket.User != null && long.TryParse(guriPacket.User.Value.ToString(), out long charId))
                        {
                            if (!Session.Character.IsFriendOfCharacter(charId))
                            {
                                Session.SendPacket(Language.Instance.GetMessageFromKey("CHARACTER_NOT_IN_FRIENDLIST"));
                                return;
                            }

                            Session.SendPacket(UserInterfaceHelper.Instance.GenerateDelay(3000, 4, $"#guri^199^2^{guriPacket.User.Value}"));
                        }
                    }
                    else if (guriPacket.Argument == 2)
                    {
                        short[] listWingOfFriendship = { 2160, 2312, 10048 };
                        short vnumToUse = -1;
                        foreach (short vnum in listWingOfFriendship)
                        {
                            if (Session.Character.Inventory.CountItem(vnum) > 0)
                            {
                                vnumToUse = vnum;
                            }
                        }

                        if (vnumToUse != -1)
                        {
                            if (guriPacket.User == null)
                            {
                                return;
                            }

                            if (!long.TryParse(guriPacket.User.Value.ToString(), out long charId))
                            {
                                return;
                            }

                            var session = ServerManager.Instance.GetSessionByCharacterId(charId);
                            if (session != null)
                            {
                                if (Session.Character.IsFriendOfCharacter(charId))
                                {
                                    if (session.CurrentMapInstance.MapInstanceType == MapInstanceType.BaseMapInstance)
                                    {
                                        if (Session.Character.MapInstance.MapInstanceType != MapInstanceType.BaseMapInstance)
                                        {
                                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_USE_THAT"), 10));
                                            return;
                                        }

                                        var mapy = session.Character.PositionY;
                                        var mapx = session.Character.PositionX;
                                        var mapId = session.Character.MapInstance.Map.MapId;

                                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, mapId, mapx, mapy);
                                        Session.Character.Inventory.RemoveItemAmount(vnumToUse);
                                    }
                                    else
                                    {
                                        if (Session.Character.MapInstance.MapInstanceType == MapInstanceType.Act4Instance && session.Character.Faction == Session.Character.Faction)
                                        {
                                            var mapy = session.Character.PositionY;
                                            var mapx = session.Character.PositionX;
                                            var mapId = session.CurrentMapInstance.MapInstanceId;

                                            ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, mapId, mapx, mapy);
                                            Session.Character.Inventory.RemoveItemAmount(vnumToUse);
                                        }
                                        else
                                        {
                                            Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("USER_ON_INSTANCEMAP"), 0));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("USER_NOT_CONNECTED"), 0));
                            }
                        }
                        else
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NO_WINGS"), 10));
                        }
                    }

                    break;

                case 201:
                    if (Session.Character.StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.PetBasket))
                    {
                        Session.SendPacket(Session.Character.GenerateStashAll());
                    }

                    break;

                case 202:
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PARTNER_BACKPACK"), 10));
                    Session.SendPacket(Session.Character.GeneratePStashAll());
                    break;

                case 203:
                    if (guriPacket.Argument == 0)
                    {
                        // SP points initialization
                        int[] listPotionResetVNums = { 1366, 1427, 5115, 9040 };
                        var vnumToUse = -1;
                        foreach (int vnum in listPotionResetVNums)
                        {
                            if (Session.Character.Inventory.CountItem(vnum) > 0)
                            {
                                vnumToUse = vnum;
                            }
                        }

                        if (vnumToUse != -1)
                        {
                            if (Session.Character.UseSp)
                            {
                                var specialistInstance =
                                    Session.Character.Inventory.LoadBySlotAndType<SpecialistInstance>((byte)EquipmentType.Sp, InventoryType.Wear);
                                if (specialistInstance != null)
                                {
                                    specialistInstance.SlDamage = 0;
                                    specialistInstance.SlDefence = 0;
                                    specialistInstance.SlElement = 0;
                                    specialistInstance.SlHP = 0;

                                    specialistInstance.DamageMinimum = 0;
                                    specialistInstance.DamageMaximum = 0;
                                    specialistInstance.HitRate = 0;
                                    specialistInstance.CriticalLuckRate = 0;
                                    specialistInstance.CriticalRate = 0;
                                    specialistInstance.DefenceDodge = 0;
                                    specialistInstance.DistanceDefenceDodge = 0;
                                    specialistInstance.ElementRate = 0;
                                    specialistInstance.DarkResistance = 0;
                                    specialistInstance.LightResistance = 0;
                                    specialistInstance.FireResistance = 0;
                                    specialistInstance.WaterResistance = 0;
                                    specialistInstance.CriticalDodge = 0;
                                    specialistInstance.CloseDefence = 0;
                                    specialistInstance.DistanceDefence = 0;
                                    specialistInstance.MagicDefence = 0;
                                    specialistInstance.HP = 0;
                                    specialistInstance.MP = 0;

                                    Session.Character.Inventory.RemoveItemAmount(vnumToUse);
                                    Session.Character.Inventory.DeleteFromSlotAndType((byte)EquipmentType.Sp, InventoryType.Wear);
                                    Session.Character.Inventory.AddToInventoryWithSlotAndType(specialistInstance, InventoryType.Wear, (byte)EquipmentType.Sp);
                                    Session.SendPacket(Session.Character.GenerateCond());
                                    Session.SendPacket(specialistInstance.GenerateSlInfo());
                                    Session.SendPacket(Session.Character.GenerateLev());
                                    Session.SendPacket(Session.Character.GenerateStatChar());
                                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("POINTS_RESET"), 0));
                                }
                            }
                            else
                            {
                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("TRANSFORMATION_NEEDED"), 10));
                            }
                        }
                        else
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_POINTS"), 10));
                        }
                    }

                    break;

                case 204:
                    if (guriPacket.User == null)
                    {
                        // WRONG PACKET
                        return;
                    }

                    var inventoryType = (InventoryType)guriPacket.Argument;
                    var pearls = Session.Character.Inventory.FirstOrDefault(s => s.Value.ItemVNum == 1429).Value;
                    var shell = (WearableInstance)Session.Character.Inventory.LoadBySlotAndType((short)guriPacket.User.Value, inventoryType);

                    if (pearls == null)
                    {
                        // USING PACKET LOGGER
                        return;
                    }

                    if (shell.EquipmentOptions.Any())
                    {
                        // ALREADY IDENTIFIED
                        Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("SHELL_ALREADY_IDENTIFIED"), 0));
                        return;
                    }

                    if (!ShellGeneratorHelper.Instance.ShellTypes.TryGetValue(shell.ItemVNum, out byte shellType))
                    {
                        // SHELL TYPE NOT IMPLEMENTED
                        return;
                    }

                    if (shellType != 8 && shellType != 9)
                    {
                        if (shell.Upgrade < 50 || shell.Upgrade > 90)
                        {
                            return;
                        }
                    }

                    if (shellType == 8 || shellType == 9)
                    {
                        switch (shell.Upgrade)
                        {
                            case 25:
                            case 30:
                            case 40:
                            case 55:
                            case 60:
                            case 65:
                            case 70:
                            case 75:
                            case 80:
                            case 85:
                                break;

                            default:
                                Session.Character.Inventory.RemoveItemAmountFromInventory(1, shell.Id);
                                Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("STOP_SPAWNING_BROKEN_SHELL"), 0));
                                return;
                        }
                    }

                    var perlsNeeded = shell.Upgrade / 10 + shell.Rare;

                    if (Session.Character.Inventory.CountItem(pearls.ItemVNum) < perlsNeeded)
                    {
                        // NOT ENOUGH PEARLS
                        return;
                    }

                    List<EquipmentOptionDTO> shellOptions = ShellGeneratorHelper.Instance.GenerateShell(shellType, shell.Rare, shell.Upgrade);

                    if (!shellOptions.Any())
                    {
                        Session.Character.Inventory.RemoveItemAmountFromInventory(1, shell.Id);
                        Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("STOP_SPAWNING_BROKEN_SHELL"), 0));
                        return;
                    }

                    shell.EquipmentOptions.AddRange(shellOptions);

                    Session.Character.Inventory.RemoveItemAmount(pearls.ItemVNum, perlsNeeded);
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateEff(3006));
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("SHELL_IDENTIFIED"), 0));
                    break;

                case 205:
                    if (guriPacket.User == null)
                    {
                        return;
                    }

                    const int PERFUME_VNUM = 1428;
                    var perfumeInventoryType = (InventoryType)guriPacket.Argument;
                    var eq = (WearableInstance)Session.Character.Inventory.LoadBySlotAndType((short)guriPacket.User.Value, perfumeInventoryType);

                    if (eq.BoundCharacterId == Session.Character.CharacterId)
                    {
                        // ALREADY YOURS
                        return;
                    }

                    if (eq.ShellRarity == null)
                    {
                        // NO SHELL APPLIED
                        return;
                    }

                    var perfumesNeeded = ShellGeneratorHelper.Instance.PerfumeFromItemLevelAndShellRarity(eq.Item.LevelMinimum, (byte)eq.ShellRarity.Value);
                    if (Session.Character.Inventory.CountItem(PERFUME_VNUM) < perfumesNeeded)
                    {
                        // NOT ENOUGH PEARLS
                        return;
                    }

                    Session.Character.Inventory.RemoveItemAmount(PERFUME_VNUM, perfumesNeeded);
                    eq.BoundCharacterId = Session.Character.CharacterId;
                    break;

                case 208:
                    if (guriPacket.Argument == 0)
                    {
                        if (guriPacket.User != null && short.TryParse(guriPacket.User.Value.ToString(), out short pearlSlot) &&
                            short.TryParse(guriPacket.Value, out short mountSlot))
                        {
                            var mount = Session.Character.Inventory.LoadBySlotAndType<ItemInstance>(mountSlot, InventoryType.Main);
                            var pearl = Session.Character.Inventory.LoadBySlotAndType<BoxInstance>(pearlSlot, InventoryType.Equipment);
                            if (mount != null && pearl != null)
                            {
                                pearl.HoldingVNum = mount.ItemVNum;
                                Session.Character.Inventory.RemoveItemAmountFromInventory(1, mount.Id);
                            }
                        }
                    }

                    break;

                case 209:
                    if (guriPacket.Argument == 0)
                    {
                        if (guriPacket.User != null && short.TryParse(guriPacket.User.Value.ToString(), out short pearlSlot) &&
                            short.TryParse(guriPacket.Value, out short mountSlot))
                        {
                            var fairy = Session.Character.Inventory.LoadBySlotAndType<WearableInstance>(mountSlot, InventoryType.Equipment);
                            var pearl = Session.Character.Inventory.LoadBySlotAndType<BoxInstance>(pearlSlot, InventoryType.Equipment);
                            if (fairy != null && pearl != null)
                            {
                                pearl.HoldingVNum = fairy.ItemVNum;
                                pearl.ElementRate = fairy.ElementRate;
                                Session.Character.Inventory.RemoveItemAmountFromInventory(1, fairy.Id);
                            }
                        }
                    }

                    break;

                case 300:
                    if (guriPacket.Argument == 8023)
                    {
                        if (guriPacket.User == null)
                        {
                            return;
                        }

                        var slot = (short)guriPacket.User.Value;
                        ItemInstance box = Session.Character.Inventory.LoadBySlotAndType<BoxInstance>(slot, InventoryType.Equipment);
                        if (box != null)
                        {
                            if (guriPacket.Data > 0)
                            {
                                box.Item.Use(Session, ref box, 1, new[] { guriPacket.Data.ToString() });
                            }
                            else
                            {
                                box.Item.Use(Session, ref box, 1);
                            }
                        }
                    }

                    break;

                case 400:
                    if (guriPacket.Argument != 0)
                    {
                        if (!Session.HasCurrentMapInstance)
                        {
                            return;
                        }

                        var npc = Session.CurrentMapInstance.Npcs.FirstOrDefault(n => n.MapNpcId.Equals(guriPacket.Argument));
                        if (npc != null)
                        {
                            var mapobject = ServerManager.Instance.GetNpc(npc.NpcVNum);

                            var rateDrop = ServerManager.Instance.DropRate;
                            var delay = (int)Math.Round((3 + mapobject.RespawnTime / 1000d) * Session.Character.TimesUsed);
                            delay = delay > 11 ? 8 : delay;
                            if (Session.Character.LastMapObject.AddSeconds(delay) < DateTime.Now)
                            {
                                if (mapobject.Drops.Any(s => s.MonsterVNum != null))
                                {
                                    if (mapobject.VNumRequired > 10 && Session.Character.Inventory.CountItem(mapobject.VNumRequired) < mapobject.AmountRequired)
                                    {
                                        Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(String.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEM"), DAOFactory.ItemDAO.FirstOrDefault(s => s.VNum == mapobject.VNumRequired)?.Name), 0));
                                        return;
                                    }
                                }

                                var random = new Random();
                                var randomAmount = ServerManager.Instance.RandomNumber() * random.NextDouble();
                                var drop = mapobject.Drops.FirstOrDefault(s => s.MonsterVNum == npc.NpcVNum);
                                if (drop != null)
                                {
                                    if (npc.NpcVNum == 2004 && npc.IsOut == false)
                                    {
                                        var newInv = Session.Character.Inventory.AddNewToInventory(drop.ItemVNum).FirstOrDefault();
                                        if (newInv == null)
                                        {
                                            return;
                                        }

                                        Session.CurrentMapInstance.Broadcast(npc.GenerateOut());
                                        Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                                            string.Format(Language.Instance.GetMessageFromKey("RECEIVED_ITEM"), newInv.Item.Name), 0));
                                        Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("RECEIVED_ITEM"), newInv.Item.Name), 11));
                                        return;
                                    }

                                    var dropChance = drop.DropChance;
                                    if (randomAmount <= (double)dropChance * rateDrop / 5000.000)
                                    {
                                        var vnum = drop.ItemVNum;
                                        var newInv = Session.Character.Inventory.AddNewToInventory(vnum).FirstOrDefault();
                                        Session.Character.LastMapObject = DateTime.Now;
                                        Session.Character.TimesUsed++;
                                        if (Session.Character.TimesUsed >= 4)
                                        {
                                            Session.Character.TimesUsed = 0;
                                        }

                                        if (newInv != null)
                                        {
                                            Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                                                string.Format(Language.Instance.GetMessageFromKey("RECEIVED_ITEM"), newInv.Item.Name), 0));
                                            Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("RECEIVED_ITEM"), newInv.Item.Name),
                                                11));
                                        }
                                        else
                                        {
                                            Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"), 0));
                                        }
                                    }
                                    else
                                    {
                                        Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("TRY_FAILED"), 0));
                                    }
                                }
                            }
                            else
                            {
                                Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                                    string.Format(Language.Instance.GetMessageFromKey("TRY_FAILED_WAIT"),
                                        (int)(Session.Character.LastMapObject.AddSeconds(delay) - DateTime.Now).TotalSeconds), 0));
                            }
                        }
                    }

                    break;

                case 501:
                    if (ServerManager.Instance.IceBreakerInWaiting && IceBreaker.Map.Sessions.Count() < IceBreaker.MaxAllowedPlayers)
                    {
                        ServerManager.Instance.TeleportOnRandomPlaceInMap(Session, IceBreaker.Map.MapInstanceId);
                    }

                    break;

                case 502:
                    long? charid = guriPacket.User;
                    if (charid == null)
                    {
                        return;
                    }

                    var target = ServerManager.Instance.GetSessionByCharacterId(charid.Value);
                    IceBreaker.FrozenPlayers.Remove(target);
                    IceBreaker.AlreadyFrozenPlayers.Add(target);
                    target?.CurrentMapInstance?.Broadcast(
                        UserInterfaceHelper.Instance.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("ICEBREAKER_PLAYER_UNFROZEN"), target.Character?.Name), 0));
                    break;

                case 506:
                    if (ServerManager.Instance.EventInWaiting)
                    {
                        Session.Character.IsWaitingForEvent = true;
                    }

                    break;

                case 750:
                    if (!guriPacket.User.HasValue)
                    {
                        const short BASE_VNUM = 1623;
                        if (short.TryParse(guriPacket.Argument.ToString(), out short faction))
                        {
                            if ((Session.Character.Family == null && (faction == 1 || faction == 2) // Individual can only use 1 and 2
                                    || Session.Character.Family != null && (faction == 3 || faction == 4)) // Family can only use 3 and 4
                                && Session.Character.Inventory.CountItem(BASE_VNUM + faction) > 0)
                            {
                                // Remove Item
                                Session.Character.Inventory.RemoveItemAmount(BASE_VNUM + faction);

                                if (Session.Character.Family == null) // Individual
                                {
                                    Session.Character.Faction = (FactionType)faction;

                                    Session.SendPacket("scr 0 0 0 0 0 0 0");
                                    Session.SendPacket(Session.Character.GenerateFaction());
                                    Session.SendPacket(Session.Character.GenerateEff(4799 + faction));
                                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey($"GET_PROTECTION_POWER_{faction}"), 0));
                                }
                                else // Family
                                {
                                    if (Session.Character.FamilyCharacter.Authority == FamilyAuthority.Head
                                        || Session.Character.FamilyCharacter.Authority == FamilyAuthority.Assistant)
                                    {
                                        // Apply Faction to all Family members
                                    }
                                    else
                                    {
                                        Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey($"NEITHER_HEAD_NOR_ASSISTANT"), 0));
                                    }
                                }
                            }
                        }
                    }

                    break;
            }

        }

        #endregion
    }
}