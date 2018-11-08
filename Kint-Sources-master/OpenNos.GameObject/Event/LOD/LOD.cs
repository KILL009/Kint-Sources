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
using System.Reactive.Linq;
using System.Threading;

namespace OpenNos.GameObject.Event
{
    public static class LOD
    {
        #region Methods

        public static void GenerateLod(int lodtime = 120)
        {
            const int HornTime = 60;
            const int HornRepawn = 4;
            const int HornStay = 1;
            EventHelper.Instance.RunEvent(new EventContainer(ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(98)), EventActionType.NPCSEFFECTCHANGESTATE, true));
            LODThread lodThread = new LODThread();
            Observable.Timer(TimeSpan.FromMinutes(0)).Subscribe(X => lodThread.Run(lodtime * 60, HornTime * 60, HornRepawn * 60, HornStay * 60));
        }

        #endregion
    }

    public class LODThread
    {
        #region Methods

        public void Run(int LODTime, int HornTime, int HornRespawn, int HornStay)
        {
            const int interval = 30;
            int dhspawns = 0;

            while (LODTime > 0)
            {
                refreshLOD(LODTime);

                if (LODTime == HornTime || (LODTime == HornTime - (HornRespawn * dhspawns)))
                {
                    foreach (Family fam in ServerManager.Instance.FamilyList.GetAllItems())
                    {
                        if (fam.LandOfDeath != null)
                        {
                            EventHelper.Instance.RunEvent(new EventContainer(fam.LandOfDeath, EventActionType.CHANGEXPRATE, 3));
                            EventHelper.Instance.RunEvent(new EventContainer(fam.LandOfDeath, EventActionType.CHANGEDROPRATE, 3));
                            spawnDH(fam.LandOfDeath);
                        }
                    }
                }
                else if (LODTime == HornTime - (HornRespawn * dhspawns) - HornStay)
                {
                    foreach (Family fam in ServerManager.Instance.FamilyList.GetAllItems())
                    {
                        if (fam.LandOfDeath != null)
                        {
                            despawnDH(fam.LandOfDeath);
                            dhspawns++;
                        }
                    }
                }

                LODTime -= interval;
                Thread.Sleep(interval * 1000);
            }
            endLOD();
        }

        private void despawnDH(MapInstance LandOfDeath)
        {
            EventHelper.Instance.RunEvent(new EventContainer(ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(98)), EventActionType.NPCSEFFECTCHANGESTATE, false));
            EventHelper.Instance.RunEvent(new EventContainer(LandOfDeath, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("HORN_DISAPEAR"), 0)));
            EventHelper.Instance.RunEvent(new EventContainer(LandOfDeath, EventActionType.UNSPAWNMONSTERS, 443));
        }

        private void endLOD()
        {
            foreach (Family fam in ServerManager.Instance.FamilyList.GetAllItems())
            {
                if (fam.LandOfDeath != null)
                {
                    EventHelper.Instance.RunEvent(new EventContainer(fam.LandOfDeath, EventActionType.DISPOSEMAP, null));
                    fam.LandOfDeath = null;
                }
            }
            ServerManager.Instance.StartedEvents.Remove(EventType.LOD);
            ServerManager.Instance.StartedEvents.Remove(EventType.LODDH);
        }

        private void refreshLOD(int remaining)
        {
            foreach (Family fam in ServerManager.Instance.FamilyList.GetAllItems())
            {
                if (fam.LandOfDeath == null)
                {
                    fam.LandOfDeath = ServerManager.GenerateMapInstance(150, MapInstanceType.LodInstance, new InstanceBag());
                }
                EventHelper.Instance.RunEvent(new EventContainer(fam.LandOfDeath, EventActionType.CLOCK, remaining * 10));
                EventHelper.Instance.RunEvent(new EventContainer(fam.LandOfDeath, EventActionType.STARTCLOCK, new Tuple<List<EventContainer>, List<EventContainer>>(new List<EventContainer>(), new List<EventContainer>())));
            }
        }

        private void spawnDH(MapInstance LandOfDeath)
        {
            EventHelper.Instance.RunEvent(new EventContainer(LandOfDeath, EventActionType.SPAWNONLASTENTRY, 443));
            EventHelper.Instance.RunEvent(new EventContainer(LandOfDeath, EventActionType.SENDPACKET, "df 2"));
            EventHelper.Instance.RunEvent(new EventContainer(LandOfDeath, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("HORN_APPEAR"), 0)));
        }

        #endregion
    }
}