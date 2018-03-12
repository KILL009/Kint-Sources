using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
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

        public override void Use(ClientSession session, ref ItemInstance inv, byte option = 0, string[] packetsplit = null)
        {
            if (packetsplit == null)
            {
                return;
            }

            int x1;
            switch (Effect)
            {
                case 10:
                    if (int.TryParse(packetsplit[3], out x1))
                    {
                        var mate = session.Character.Mates.FirstOrDefault(s => s.MateTransportId == x1 && s.MateType == MateType.Pet);
                        if (mate == null)
                        {
                            return;
                        }

                        mate.GenerateXp(inv.Item.EffectValue);
                        session.CurrentMapInstance.Broadcast(mate.GenerateEff(5));
                        session.CurrentMapInstance.Broadcast(mate.GenerateEff(5002));
                        if (mate.Loyalty != 1000)
                            mate.Loyalty += 100;
                        session.Character.Inventory.RemoveItemAmount(inv.ItemVNum);
                    }

                    break;

                case 11:
                    if (int.TryParse(packetsplit[3], out x1))
                    {
                        var mate = session.Character.Mates.FirstOrDefault(s => s.MateTransportId == x1 && s.MateType == MateType.Pet);
                        if (mate == null || mate.Level >= session.Character.Level - 5)
                        {
                            return;
                        }

                        mate.Level++;
                        session.Character.Inventory.RemoveItemAmount(inv.ItemVNum);
                        session.CurrentMapInstance?.Broadcast(mate.GenerateEff(8), mate.PositionX, mate.PositionY);
                        session.CurrentMapInstance?.Broadcast(mate.GenerateEff(198), mate.PositionX, mate.PositionY);
                    }

                    break;

                case 12:
                    if (int.TryParse(packetsplit[3], out x1))
                    {
                        var mate = session.Character.Mates.FirstOrDefault(s => s.MateTransportId == x1 && s.MateType == MateType.Partner);
                        if (mate == null || mate.Level >= session.Character.Level - 5)
                        {
                            return;
                        }

                        mate.Level++;
                        session.Character.Inventory.RemoveItemAmount(inv.ItemVNum);
                        session.CurrentMapInstance?.Broadcast(mate.GenerateEff(8), mate.PositionX, mate.PositionY);
                        session.CurrentMapInstance?.Broadcast(mate.GenerateEff(198), mate.PositionX, mate.PositionY);
                    }

                    break;

                case 13:
                    if (int.TryParse(packetsplit[3], out x1))
                    {
                        if (session.Character.Mates.Any(s => s.MateTransportId == x1))
                        {
                            session.SendPacket(UserInterfaceHelper.Instance.GenerateGuri(10, 1, x1, 2));
                        }
                    }

                    break;

                case 14:
                    if (int.TryParse(packetsplit[3], out x1))
                    {
                        var mate = session.Character.Mates.FirstOrDefault(s => s.MateTransportId == x1 && s.MateType == MateType.Pet);
                        if (mate != null)
                        {
                            if (!mate.CanPickUp)
                            {
                                session.Character.Inventory.RemoveItemAmount(inv.ItemVNum);
                                session.CurrentMapInstance.Broadcast(mate.GenerateEff(5));
                                session.CurrentMapInstance.Broadcast(mate.GenerateEff(5002));
                                mate.CanPickUp = true;
                                session.SendPackets(session.Character.GenerateScP());
                                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PET_CAN_PICK_UP"), 10));
                            }
                        }
                    }

                    break;

                case 17:
                    if (int.TryParse(packetsplit[3], out x1))
                    {
                        var mate = session.Character.Mates.FirstOrDefault(s => s.MateTransportId == x1 && s.MateType == MateType.Pet);
                        if (mate != null)
                        {
                            if (!mate.IsSummonable)
                            {
                                session.Character.Inventory.RemoveItemAmount(inv.ItemVNum, 1);
                                mate.IsSummonable = true;
                                session.SendPackets(session.Character.GenerateScP());
                                session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("PET_SUMMONABLE"), mate.Name), 10));
                                session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("PET_SUMMONABLE"), mate.Name), 0));
                            }
                        }
                    }

                    break;

                case 1000:
                    if (int.TryParse(packetsplit[3], out x1))
                    {
                        var mate = session.Character.Mates.FirstOrDefault(s => s.MateTransportId == x1 && s.MateType == MateType.Pet);
                        if (mate != null)
                        {
                            if (!mate.IsTeamMember)
                            {
                                session.Character.Mates.Remove(mate);
                                session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("PET_RELEASED")));
                                session.SendPacket(UserInterfaceHelper.Instance.GeneratePClear());
                                session.SendPackets(session.Character.GenerateScP());
                                session.SendPackets(session.Character.GenerateScN());
                                session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                                session.CurrentMapInstance?.Broadcast(mate.GenerateOut());
                            }
                            else
                            {
                                session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("PET_IN_TEAM_UNRELEASABLE"), 0));
                            }
                        }
                    }

                    break;

                case 1001:
                    if (int.TryParse(packetsplit[3], out x1))
                    {
                        var mate = session.Character.Mates.FirstOrDefault(s => s.MateTransportId == x1 && s.MateType == MateType.Partner);
                        if (mate != null)
                        {
                            if (!mate.IsTeamMember)
                            {
                                session.Character.Mates.Remove(mate);
                                session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("PET_RELEASED")));
                                session.SendPacket(UserInterfaceHelper.Instance.GeneratePClear());
                                session.SendPackets(session.Character.GenerateScP());
                                session.SendPackets(session.Character.GenerateScN());
                                session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                                session.CurrentMapInstance?.Broadcast(mate.GenerateOut());
                            }
                            else
                            {
                                session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("PET_IN_TEAM_UNRELEASABLE"), 0));
                            }
                        }
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