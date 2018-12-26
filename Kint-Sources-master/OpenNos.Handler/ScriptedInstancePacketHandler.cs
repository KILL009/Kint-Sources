﻿using OpenNos.Core;
using OpenNos.Core.Extensions;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Packets.ServerPackets;
using System;
using System.Linq;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler
{
    internal class ScriptedInstancePacketHandler : IPacketHandler
    {
        #region Instantiation

        public ScriptedInstancePacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        private ClientSession Session { get; }

        #endregion

        #region Methods

        /// <summary>
        /// RSelPacket packet
        /// </summary>
        /// <param name="escapePacket"></param>
        public void Escape(EscapePacket escapePacket)
        {
            if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TimeSpaceInstance)
            {
                ServerManager.Instance.ChangeMap(Session.Character.CharacterId, Session.Character.MapId,
                    Session.Character.MapX, Session.Character.MapY);
                Session.Character.Timespace = null;
            }
            else if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.RaidInstance)
            {
                ServerManager.Instance.ChangeMap(Session.Character.CharacterId, Session.Character.MapId,
                    Session.Character.MapX, Session.Character.MapY);
                Session.Character.Group?.Characters.ForEach(
                    session => session.SendPacket(session.Character.Group.GenerateRdlst()));
                Session.SendPacket(Session.Character.GenerateRaid(1, true));
                Session.SendPacket(Session.Character.GenerateRaid(2, true));
                Session.Character.Group?.LeaveGroup(Session);
            }
        }

        /// <summary>
        /// mkraid packet
        /// </summary>
        /// <param name="mkRaidPacket"></param>
        public void GenerateRaid(MkRaidPacket mkRaidPacket)
        {
            if (Session.Character.Group?.Raid != null && Session.Character.Group.IsLeader(Session))
            {
                if (Session.Character.Group.CharacterCount > 0 && Session.Character.Group.Characters.All(s =>
                        s.CurrentMapInstance.Portals.Any(p => p.Type == (short)PortalType.Raid)))
                {
                    if (Session.Character.Group.Raid.FirstMap == null)
                    {
                        Session.Character.Group.Raid.LoadScript(MapInstanceType.RaidInstance);
                    }

                    if (Session.Character.Group.Raid.FirstMap == null)
                    {
                        return;
                    }

                    Session.Character.Group.Raid.InstanceBag.Lock = true;

                    //Session.Character.Group.Characters.Where(s => s.CurrentMapInstance != Session.CurrentMapInstance).ToList().ForEach(
                    //session =>
                    //{
                    //    Session.Character.Group.LeaveGroup(session);
                    //    session.SendPacket(session.Character.GenerateRaid(1, true));
                    //    session.SendPacket(session.Character.GenerateRaid(2, true));
                    //});

                    Session.Character.Group.Raid.InstanceBag.Lives = (short)Session.Character.Group.CharacterCount;

                    foreach (ClientSession session in Session.Character.Group.Characters.GetAllItems())
                    {
                        if (session != null)
                        {
                            ServerManager.Instance.ChangeMapInstance(session.Character.CharacterId,
                                session.Character.Group.Raid.FirstMap.MapInstanceId,
                                session.Character.Group.Raid.StartX, session.Character.Group.Raid.StartY);
                            session.SendPacket("raidbf 0 0 25");
                            session.SendPacket(session.Character.Group.GeneraterRaidmbf(session));
                            session.SendPacket(session.Character.GenerateRaid(5));
                            session.SendPacket(session.Character.GenerateRaid(4));
                            session.SendPacket(session.Character.GenerateRaid(3));
                        }
                    }

                    ServerManager.Instance.GroupList.Remove(Session.Character.Group);

                    Logger.LogUserEvent("RAID_START", Session.GenerateIdentity(),
                        $"RaidId: {Session.Character.Group.GroupId}");
                }
                else
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg("RAID_TEAM_NOT_READY", 0));
                }
            }
        }

        /// <summary>
        /// RSelPacket packet
        /// </summary>
        /// <param name="rSelPacket"></param>
        public void GetGift(RSelPacket rSelPacket)
        {
            if (Session.Character.Timespace?.FirstMap?.MapInstanceType == MapInstanceType.TimeSpaceInstance)
            {
                ServerManager.GetBaseMapInstanceIdByMapId(Session.Character.MapId);
                if (Session.Character.Timespace?.FirstMap.InstanceBag.EndState == 5)
                {
                    Session.Character.SetReputation(Session.Character.Timespace.Reputation);

                    Session.Character.Gold =
                        Session.Character.Gold + Session.Character.Timespace.Gold
                        > ServerManager.Instance.Configuration.MaxGold
                            ? ServerManager.Instance.Configuration.MaxGold
                            : Session.Character.Gold + Session.Character.Timespace.Gold;
                    Session.SendPacket(Session.Character.GenerateGold());
                    Session.SendPacket(Session.Character.GenerateSay(
                        string.Format(Language.Instance.GetMessageFromKey("GOLD_TS_END"),
                            Session.Character.Timespace.Gold), 10));

                    int rand = new Random().Next(Session.Character.Timespace.DrawItems.Count);
                    string repay = "repay ";
                    Session.Character.GiftAdd(Session.Character.Timespace.DrawItems[rand].VNum,
                        Session.Character.Timespace.DrawItems[rand].Amount);

                    for (int i = 0; i < 3; i++)
                    {
                        Gift gift = Session.Character.Timespace.GiftItems.ElementAtOrDefault(i);
                        repay += gift == null ? "-1.0.0 " : $"{gift.VNum}.0.{gift.Amount} ";
                        if (gift != null)
                        {
                            Session.Character.GiftAdd(gift.VNum, gift.Amount);
                        }
                    }

                    // TODO: Add HasAlreadyDone
                    for (int i = 0; i < 2; i++)
                    {
                        Gift gift = Session.Character.Timespace.SpecialItems.ElementAtOrDefault(i);
                        repay += gift == null ? "-1.0.0 " : $"{gift.VNum}.0.{gift.Amount} ";
                        if (gift != null)
                        {
                            Session.Character.GiftAdd(gift.VNum, gift.Amount);
                        }
                    }

                    repay +=
                        $"{Session.Character.Timespace.DrawItems[rand].VNum}.0.{Session.Character.Timespace.DrawItems[rand].Amount}";
                    Session.SendPacket(repay);
                    Session.Character.Timespace.FirstMap.InstanceBag.EndState = 6;
                    Session.Character.Timespace = null;
                }
            }
        }

        /// <summary>
        /// treq packet
        /// </summary>
        /// <param name="treqPacket"></param>
        public void GetTreq(TreqPacket treqPacket)
        {
            ScriptedInstance timespace = Session.CurrentMapInstance.ScriptedInstances
                .Find(s => treqPacket.X == s.PositionX && treqPacket.Y == s.PositionY).Copy();

            if (timespace != null)
            {
                if (treqPacket.StartPress == 1 || treqPacket.RecordPress == 1)
                {
                    EnterInstance(timespace);
                }
                else
                {
                    Session.SendPacket(timespace.GenerateRbr());
                }
            }
        }

        private void EnterInstance(ScriptedInstance input)
        {
            ScriptedInstance instance = input.Copy();
            instance.LoadScript(MapInstanceType.TimeSpaceInstance);
            if (instance.FirstMap == null)
            {
                return;
            }

            if (Session.Character.Level < instance.LevelMinimum)
            {
                Session.SendPacket(
                    UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_REQUIERED_LEVEL"), 0));
                return;
            }

            foreach (Gift gift in instance.RequiredItems)
            {
                if (Session.Character.Inventory.CountItem(gift.VNum) < gift.Amount)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_REQUIERED_ITEM"),
                            ServerManager.GetItem(gift.VNum).Name), 0));
                    return;
                }

                Session.Character.Inventory.RemoveItemAmount(gift.VNum, gift.Amount);
            }

            Session.Character.MapX = instance.PositionX;
            Session.Character.MapY = instance.PositionY;
            ServerManager.Instance.TeleportOnRandomPlaceInMap(Session, instance.FirstMap.MapInstanceId);
            instance.InstanceBag.CreatorId = Session.Character.CharacterId;
            Session.SendPackets(instance.GenerateMinimap());
            Session.SendPacket(instance.GenerateMainInfo());
            Session.SendPacket(instance.FirstMap.InstanceBag.GenerateScore());

            Session.Character.Timespace = instance;
        }

        /// <summary>
        /// wreq packet
        /// </summary>
        /// <param name="packet"></param>
        public void GetWreq(WreqPacket packet)
        {
            foreach (ScriptedInstance portal in Session.CurrentMapInstance.ScriptedInstances)
            {
                if (Session.Character.PositionY >= portal.PositionY - 1 && Session.Character.PositionY
                                                                        <= portal.PositionY + 1
                                                                        && Session.Character.PositionX
                                                                        >= portal.PositionX - 1
                                                                        && Session.Character.PositionX
                                                                        <= portal.PositionX + 1)
                {
                    switch (packet.Value)
                    {
                        case 0:
                            if (Session.Character.Group?.Characters.Any(s =>
                                    s.CurrentMapInstance.InstanceBag?.Lock == false
                                    && s.CurrentMapInstance.MapInstanceType == MapInstanceType.TimeSpaceInstance
                                    && s.Character.MapId == portal.MapId
                                    && s.Character.CharacterId != Session.Character.CharacterId
                                    && s.Character.MapX == portal.PositionX && s.Character.MapY == portal.PositionY)
                                == true)
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateDialog(
                                    $"#wreq^3^{Session.Character.CharacterId} #wreq^0^1 {Language.Instance.GetMessageFromKey("ASK_JOIN_TEAM_TS")}"));
                            }
                            else
                            {
                                Session.SendPacket(portal.GenerateRbr());
                            }

                            break;

                        case 1:
                            if (!packet.Param.HasValue)
                            {
                                EnterInstance(portal);
                            }
                            else if (packet.Param.Value == 1)
                            {
                                GetTreq(new TreqPacket
                                {
                                    X = portal.PositionX,
                                    Y = portal.PositionY,
                                    RecordPress = packet.Param,
                                    StartPress = 1
                                });
                            }

                            break;

                        case 3:
                            ClientSession clientSession =
                                Session.Character.Group?.Characters.Find(s => s.Character.CharacterId == packet.Param);
                            if (clientSession != null)
                            {
                                if (Session.Character.Level < portal.LevelMinimum)
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("NOT_REQUIERED_LEVEL"), 0));
                                    return;
                                }

                                MapCell mapcell = clientSession.CurrentMapInstance.Map.GetRandomPosition();
                                Session.Character.MapX = portal.PositionX;
                                Session.Character.MapY = portal.PositionY;
                                ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId,
                                    clientSession.CurrentMapInstance.MapInstanceId, mapcell.X, mapcell.Y);
                                Session.SendPacket(portal.GenerateMainInfo());
                                Session.SendPackets(portal.GenerateMinimap());
                                Session.SendPacket(portal.FirstMap.InstanceBag.GenerateScore());
                                Session.Character.Timespace = portal;
                            }

                            // TODO: Implement
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// GitPacket packet
        /// </summary>
        /// <param name="packet"></param>
        public void Git(GitPacket packet)
        {
            MapButton button = Session.CurrentMapInstance.Buttons.Find(s => s.MapButtonId == packet.ButtonId);
            if (button != null)
            {
                Session.CurrentMapInstance.Broadcast(StaticPacketHelper.Out(UserType.Object, button.MapButtonId));
                button.RunAction();
                Session.CurrentMapInstance.Broadcast(button.GenerateIn());
            }
        }

        /// <summary>
        /// rxitPacket packet
        /// </summary>
        /// <param name="rxitPacket"></param>
        public void InstanceExit(RaidExitPacket rxitPacket)
        {
            if (rxitPacket?.State == 1
                && (Session.CurrentMapInstance?.MapInstanceType == MapInstanceType.TimeSpaceInstance
                 || Session.CurrentMapInstance?.MapInstanceType == MapInstanceType.RaidInstance))
            {
                if (Session.CurrentMapInstance.InstanceBag.Lock)
                {
                    //5seed
                    Session.CurrentMapInstance.InstanceBag.DeadList.Add(Session.Character.CharacterId);
                    Session.SendPacket(
                        Session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("DIGNITY_LOST"), 20), 11));
                    Session.Character.Dignity =
                        Session.Character.Dignity < -980 ? -1000 : Session.Character.Dignity - 20;
                }
                else
                {
                    //1seed
                }

                ServerManager.Instance.GroupLeave(Session);
                ServerManager.Instance.ChangeMap(Session.Character.CharacterId, Session.Character.MapId,
                    Session.Character.MapX, Session.Character.MapY);
            }
        }

        #endregion
    }
}