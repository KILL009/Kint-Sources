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
using System;
using System.Linq;

namespace OpenNos.GameObject
{
    public class TeacherItem : Item
    {
        #region Instantiation

        public TeacherItem(ItemDTO item) : base(item)
        {
        }

        #endregion

        #region Methods

        public override void Use(ClientSession session, ref ItemInstance inv, byte Option = 0, string[] packetsplit = null)
        {
            if (packetsplit == null)
            {
                return;
            }

            void releasePet(MateType mateType, Guid itemToRemoveId)
            {
                if (int.TryParse(packetsplit[3], out int mateTransportId))
                {
                    Mate mate = session.Character.Mates.Find(s => s.MateTransportId == mateTransportId && s.MateType == mateType);
                    if (mate != null)
                    {
                        if (!mate.IsTeamMember)
                        {
                            session.Character.Mates.Remove(mate);
                            session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("PET_RELEASED")));
                            session.SendPacket(UserInterfaceHelper.GeneratePClear());
                            session.SendPackets(session.Character.GenerateScP());
                            session.SendPackets(session.Character.GenerateScN());
                            session.CurrentMapInstance?.Broadcast(mate.GenerateOut());
                            session.Character.Inventory.RemoveItemFromInventory(itemToRemoveId);
                        }
                        else
                        {
                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("PET_IN_TEAM_UNRELEASABLE"), 0));
                        }
                    }
                }
            }

            switch (Effect)
            {
                case 11:
                    if (int.TryParse(packetsplit[3], out int mateTransportId))
                    {
                        Mate mate = session.Character.Mates.Find(s => s.MateTransportId == mateTransportId);
                        if (mate == null || mate.Level >= session.Character.Level - 5)
                        {
                            return;
                        }
                        mate.Level++;
                        session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 16), mate.PositionX, mate.PositionY);
                        session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 3406), mate.PositionX, mate.PositionY);
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    break;

                case 13:
                    if (int.TryParse(packetsplit[3], out mateTransportId) && session.Character.Mates.Any(s => s.MateTransportId == mateTransportId))
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateGuri(10, 1, mateTransportId, 2));
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    break;

                case 14:
                    if (int.TryParse(packetsplit[3], out mateTransportId))
                    {
                        Mate mate = session.Character.Mates.Find(s => s.MateTransportId == mateTransportId && s.MateType == MateType.Pet);
                        if (mate?.CanPickUp == false)
                        {
                            session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 12));
                            session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5001));
                            mate.CanPickUp = true;
                            session.SendPackets(session.Character.GenerateScP());
                            session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PET_CAN_PICK_UP"), 10));
                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        }
                    }
                    break;

                case 17:
                    if (int.TryParse(packetsplit[3], out mateTransportId))
                    {
                        Mate mate = session.Character.Mates.Find(s => s.MateTransportId == mateTransportId);
                        if (mate?.IsSummonable == false)
                        {
                            mate.IsSummonable = true;
                            session.SendPackets(session.Character.GenerateScP());
                            session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("PET_SUMMONABLE"), mate.Name), 10));
                            session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("PET_SUMMONABLE"), mate.Name), 0));
                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        }
                    }
                    break;

                case 1000:
                    releasePet(MateType.Pet, inv.Id);
                    break;

                case 1001:
                    releasePet(MateType.Partner, inv.Id);
                    break;

                default:
                    Logger.Warn(string.Format(Language.Instance.GetMessageFromKey("NO_HANDLER_ITEM"), GetType()));
                    break;
            }
        }

        #endregion
    }
}