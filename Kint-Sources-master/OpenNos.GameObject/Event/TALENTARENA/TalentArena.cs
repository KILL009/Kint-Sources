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
using OpenNos.Master.Library.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject.Event
{
    public static class TalentArena
    {
        public static bool IsRunning { get; set; }

        public static ThreadSafeSortedList<long, ClientSession> RegisteredParticipants { get; set; }

        public static ThreadSafeSortedList<long, Group> RegisteredGroups { get; set; }

        public static ThreadSafeSortedList<long, List<Group>> PlayingGroups { get; set; }

        #region Methods

        public static void Run()
        {
            RegisteredParticipants = new ThreadSafeSortedList<long, ClientSession>();
            RegisteredGroups = new ThreadSafeSortedList<long, Group>();
            PlayingGroups = new ThreadSafeSortedList<long, List<Group>>();

            ServerManager.Shout(Language.Instance.GetMessageFromKey("TALENTARENA_OPEN"), true);

            GroupingThread groupingThread = new GroupingThread();
            Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(observer => groupingThread.Run());

            MatchmakingThread matchmakingThread = new MatchmakingThread();
            Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(observer => matchmakingThread.Run());

            IsRunning = true;

            Observable.Timer(TimeSpan.FromMinutes(30)).Subscribe(observer =>
            {
                groupingThread.RequestStop();
                matchmakingThread.RequestStop();
                RegisteredParticipants.ClearAll();
                RegisteredGroups.ClearAll();
                IsRunning = false;
                ServerManager.Instance.StartedEvents.Remove(EventType.TALENTARENA);
            });
        }

        private class GroupingThread
        {
            private bool _shouldStop;

            public void Run()
            {
                byte[] levelCaps = { 40, 50, 60, 70, 80, 85, 90, 95, 100, 120, 150, 180, 255 };
                while (!_shouldStop)
                {
                    IEnumerable<IGrouping<byte, ClientSession>> groups = from sess in RegisteredParticipants.GetAllItems()
                                                                         group sess by Array.Find(levelCaps, s => s > sess.Character.Level) into grouping
                                                                         select grouping;
                    foreach (IGrouping<byte, ClientSession> group in groups)
                    {
                        foreach (List<ClientSession> grp in group.ToList().Split(3).Where(s => s.Count == 3))
                        {
                            Group g = new Group
                            {
                                GroupType = GroupType.TalentArena,
                                TalentArenaBattle = new TalentArenaBattle
                                {
                                    GroupLevel = group.Key
                                }
                            };

                            foreach (ClientSession sess in grp)
                            {
                                RegisteredParticipants.Remove(sess);
                                g.JoinGroup(sess);
                                sess.SendPacket(UserInterfaceHelper.GenerateBSInfo(1, 3, -1, 6));
                                Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(observer => sess?.SendPacket(UserInterfaceHelper.GenerateBSInfo(1, 3, 300, 1)));
                            }
                            RegisteredGroups[g.GroupId] = g;
                        }
                    }

                    Thread.Sleep(5000);
                }
            }

            public void RequestStop() => _shouldStop = true;
        }

        private class MatchmakingThread
        {
            private bool _shouldStop;

            public void Run()
            {
                while (!_shouldStop)
                {
                    IEnumerable<IGrouping<byte, Group>> groups = from grp in RegisteredGroups.GetAllItems()
                                                                 where grp.TalentArenaBattle != null
                                                                 group grp by grp.TalentArenaBattle.GroupLevel into grouping
                                                                 select grouping;

                    foreach (IGrouping<byte, Group> group in groups)
                    {
                        Group prevGroup = null;

                        foreach (Group g in group)
                        {
                            if (prevGroup == null)
                            {
                                prevGroup = g;
                            }
                            else
                            {
                                RegisteredGroups.Remove(g);
                                RegisteredGroups.Remove(prevGroup);

                                MapInstance mapInstance = ServerManager.GenerateMapInstance(2015, MapInstanceType.NormalInstance, new InstanceBag());
                                mapInstance.IsPVP = true;

                                g.TalentArenaBattle.MapInstance = mapInstance;
                                prevGroup.TalentArenaBattle.MapInstance = mapInstance;

                                g.TalentArenaBattle.Side = 0;
                                prevGroup.TalentArenaBattle.Side = 1;

                                g.TalentArenaBattle.Calls = 5;
                                prevGroup.TalentArenaBattle.Calls = 5;

                                IEnumerable<ClientSession> gs = g.Characters.GetAllItems().Concat(prevGroup.Characters.GetAllItems());
                                foreach (ClientSession sess in gs)
                                {
                                    sess.SendPacket(UserInterfaceHelper.GenerateBSInfo(1, 3, -1, 2));
                                }
                                Thread.Sleep(1000);
                                foreach (ClientSession sess in gs)
                                {
                                    sess.SendPacket(UserInterfaceHelper.GenerateBSInfo(2, 3, 0, 0));
                                    sess.SendPacket(UserInterfaceHelper.GenerateTeamArenaClose());
                                }
                                Thread.Sleep(5000);
                                foreach (ClientSession sess in gs)
                                {
                                    sess.SendPacket(UserInterfaceHelper.GenerateTeamArenaMenu(0, 0, 0, 0, 0));
                                    short x = 125;
                                    if (sess.Character.Group.TalentArenaBattle.Side == 0)
                                    {
                                        x = 15;
                                    }
                                    ServerManager.Instance.ChangeMapInstance(sess.Character.CharacterId, mapInstance.MapInstanceId, x, 39);
                                    sess.SendPacketAfter(UserInterfaceHelper.GenerateTeamArenaMenu(3, 0, 0, 60, 0), 5000);
                                }

                                PlayingGroups[g.GroupId] = new List<Group> { g, prevGroup };

                                BattleThread battleThread = new BattleThread();
                                Observable.Timer(TimeSpan.FromSeconds(0)).Subscribe(observer => battleThread.Run(PlayingGroups[g.GroupId]));

                                prevGroup = null;
                            }
                        }
                    }

                    Thread.Sleep(5000);
                }
            }

            public void RequestStop() => _shouldStop = true;
        }

        private class BattleThread
        {
            private List<ClientSession> Characters { get; set; }

            public void Run(List<Group> groups) => Characters = groups[0].Characters.GetAllItems().Concat(groups[1].Characters.GetAllItems()).ToList();

        }

        #endregion
    }
}