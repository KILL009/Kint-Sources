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
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using System;
using System.Collections.Generic;
using OpenNos.GameObject.Networking;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

namespace OpenNos.GameObject.Event
{
    public static class ACT4SHIP
    {
        #region Methods

        public static void GenerateAct4Ship(byte faction)
        {
            EventHelper.Instance.RunEvent(new EventContainer(ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(261)), EventActionType.NPCSEFFECTCHANGESTATE, true));
            Act4ShipThread shipThread = new Act4ShipThread();
            DateTime result = Core.Extensions.TimeExtensions.RoundUp(DateTime.Now, TimeSpan.FromMinutes(5));
            Observable.Timer(result - DateTime.Now).Subscribe(X => shipThread.Run(faction));
        }

        #endregion
    }

    public class Act4ShipThread
    {
        #region Methods

        public void Run(byte faction)
        {
            MapInstance map = ServerManager.GenerateMapInstance(149, faction == 1 ? MapInstanceType.Act4ShipAngel : MapInstanceType.Act4ShipDemon, null);
            MapNpc mapNpc1 = new MapNpc() { NpcVNum = 613, MapNpcId = map.GetNextNpcId(), Dialog = 434, MapId = 149, MapX = 8, MapY = 28, IsMoving = false, Position = 1, IsSitting = false };
            mapNpc1.Initialize(map);
            map.AddNPC(mapNpc1);
            MapNpc mapNpc2 = new MapNpc() { NpcVNum = 540, MapNpcId = map.GetNextNpcId(), Dialog = 433, MapId = 149, MapX = 31, MapY = 28, IsMoving = false, Position = 3, IsSitting = false };
            mapNpc2.Initialize(map);
            map.AddNPC(mapNpc2);
            while (true)
            {
                openShip();
                Thread.Sleep(60 * 1000);
                map.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SHIP_MINUTE"), 0));
                lockShip();
                Thread.Sleep(30 * 1000);
                map.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("SHIP_SECONDS"), 30), 0));
                Thread.Sleep(20 * 1000);
                map.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("SHIP_SECONDS"), 10), 0));
                Thread.Sleep(10 * 1000);
                map.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SHIP_SETOFF"), 0));
                List<ClientSession> sessions = map.Sessions.Where(s => s?.Character != null).ToList();
                Observable.Timer(TimeSpan.FromSeconds(0)).Subscribe(X => teleportPlayers(sessions));
            }
        }

        private void lockShip() => EventHelper.Instance.RunEvent(new EventContainer(ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(261)), EventActionType.NPCSEFFECTCHANGESTATE, true));

        private void openShip() => EventHelper.Instance.RunEvent(new EventContainer(ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(261)), EventActionType.NPCSEFFECTCHANGESTATE, false));

        private void teleportPlayers(List<ClientSession> sessions)
        {
            foreach (ClientSession s in sessions)
            {
                switch (s.Character.Faction)
                {
                    case FactionType.None:
                        ServerManager.Instance.ChangeMap(s.Character.CharacterId, 261, 114, 160);
                        s.SendPacket(UserInterfaceHelper.GenerateInfo("You need to be part of a faction to join Act 4"));
                        return;

                    case FactionType.Angel:
                        s.Character.MapId = 130;
                        s.Character.MapX = 12;
                        s.Character.MapY = 40;
                        break;

                    case FactionType.Demon:
                        s.Character.MapId = 131;
                        s.Character.MapX = 12;
                        s.Character.MapY = 40;
                        break;
                }

                //todo: get act4 channel dynamically
                // Change IP to yours
                s.Character.ChangeChannel(ServerManager.Instance.Configuration.Act4IP, ServerManager.Instance.Configuration.Act4Port, 1);
            }
        }

        #endregion
    }
}