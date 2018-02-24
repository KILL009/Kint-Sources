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
using OpenNos.GameObject.Networking;
using OpenNos.GameObject.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

namespace OpenNos.GameObject.Event
{
    public static class Act4Raid
    {
        #region Methods

        public static void GenerateRaid(MapInstanceType raidType, byte faction)
        {
            ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId((short)(129 + faction))).CreatePortal(new Portal()
            {
                SourceMapId = (short)(129 + faction),
                SourceX = 53,
                SourceY = 53,
                DestinationMapId = 0,
                DestinationX = 1,
                DestinationY = 1,
                Type = (short)(9 + faction)
            });

            Act4RaidThread raidThread = new Act4RaidThread();
            Observable.Timer(TimeSpan.FromMinutes(0)).Subscribe(X => raidThread.Run(raidType, faction));
        }

        #endregion
    }

    public class Act4RaidThread
    {
        #region Members

        private readonly List<long> _wonFamilies = new List<long>();
        private short _bossMapId = 136;
        private bool _bossMove;
        private short _bossVNum = 563;
        private short _bossX = 55;
        private short _bossY = 11;
        private short _destPortalX = 55;
        private short _destPortalY = 80;
        private byte _faction;
        private short _mapId = 135;
        private MapInstanceType _raidType;
        private short _sourcePortalX = 146;
        private short _sourcePortalY = 43;

        #endregion

        #region Methods

        public void Run(MapInstanceType raidType, byte faction)
        {
            _raidType = raidType;
            _faction = faction;
            switch (raidType)
            {
                // Morcos is default
                case MapInstanceType.Act4Hatus:
                    _mapId = 137;
                    _bossMapId = 138;
                    _bossVNum = 577;
                    _bossX = 36;
                    _bossY = 18;
                    _sourcePortalX = 37;
                    _sourcePortalY = 156;
                    _destPortalX = 36;
                    _destPortalY = 58;
                    _bossMove = false;
                    break;

                case MapInstanceType.Act4Calvina:
                    _mapId = 139;
                    _bossMapId = 140;
                    _bossVNum = 629;
                    _bossX = 26;
                    _bossY = 26;
                    _sourcePortalX = 194;
                    _sourcePortalY = 17;
                    _destPortalX = 9;
                    _destPortalY = 41;
                    _bossMove = true;
                    break;

                case MapInstanceType.Act4Berios:
                    _mapId = 141;
                    _bossMapId = 142;
                    _bossVNum = 624;
                    _bossX = 29;
                    _bossY = 29;
                    _sourcePortalX = 188;
                    _sourcePortalY = 96;
                    _destPortalX = 29;
                    _destPortalY = 54;
                    _bossMove = true;
                    break;
            }

            int raidTime = 3600;
            const int interval = 30;

            //Run once to load everything in place
            refreshRaid(raidTime);

            ServerManager.Instance.Act4RaidStart = DateTime.Now;

            while (raidTime > 0)
            {
                raidTime -= interval;
                Thread.Sleep(interval * 1000);
                refreshRaid(raidTime);
            }

            endRaid();
        }

        private void endRaid()
        {
            foreach (Family fam in ServerManager.Instance.FamilyList.GetAllItems())
            {
                if (fam.Act4Raid != null)
                {
                    EventHelper.Instance.RunEvent(new EventContainer(fam.Act4Raid, EventActionType.DISPOSEMAP, null));
                    fam.Act4Raid = null;
                }
                if (fam.Act4RaidBossMap != null)
                {
                    EventHelper.Instance.RunEvent(new EventContainer(fam.Act4RaidBossMap, EventActionType.DISPOSEMAP, null));
                    fam.Act4RaidBossMap = null;
                }
            }

            ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(130)).Portals.RemoveAll(s => s.Type.Equals(10));
            ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(131)).Portals.RemoveAll(s => s.Type.Equals(11));
            switch (_faction)
            {
                case 1:
                    ServerManager.Instance.Act4AngelStat.Mode = 0;
                    ServerManager.Instance.Act4AngelStat.IsMorcos = false;
                    ServerManager.Instance.Act4AngelStat.IsHatus = false;
                    ServerManager.Instance.Act4AngelStat.IsCalvina = false;
                    ServerManager.Instance.Act4AngelStat.IsBerios = false;
                    break;

                case 2:
                    ServerManager.Instance.Act4DemonStat.Mode = 0;
                    ServerManager.Instance.Act4DemonStat.IsMorcos = false;
                    ServerManager.Instance.Act4DemonStat.IsHatus = false;
                    ServerManager.Instance.Act4DemonStat.IsCalvina = false;
                    ServerManager.Instance.Act4DemonStat.IsBerios = false;
                    break;
            }

            ServerManager.Instance.StartedEvents.Remove(EventType.Act4Raid);
        }

        private void openRaid(Family fami)
        {
            List<EventContainer> onDeathEvents = new List<EventContainer>
            {
                new EventContainer(fami.Act4RaidBossMap, EventActionType.THROWITEMS, new Tuple<int, short, byte, int, int>(_bossVNum, 1046, 10, 20000, 20001)),
                new EventContainer(fami.Act4RaidBossMap, EventActionType.THROWITEMS, new Tuple<int, short, byte, int, int>(_bossVNum, 1244, 10, 5, 6))
            };
            if (_raidType.Equals(MapInstanceType.Act4Berios))
            {
                onDeathEvents.Add(new EventContainer(fami.Act4RaidBossMap, EventActionType.THROWITEMS, new Tuple<int, short, byte, int, int>(_bossVNum, 2395, 3, 1, 2)));
                onDeathEvents.Add(new EventContainer(fami.Act4RaidBossMap, EventActionType.THROWITEMS, new Tuple<int, short, byte, int, int>(_bossVNum, 2396, 5, 1, 2)));
                onDeathEvents.Add(new EventContainer(fami.Act4RaidBossMap, EventActionType.THROWITEMS, new Tuple<int, short, byte, int, int>(_bossVNum, 2397, 10, 1, 2)));
            }
            onDeathEvents.Add(new EventContainer(fami.Act4RaidBossMap, EventActionType.SCRIPTEND, (byte)1));
            MonsterToSummon bossMob = new MonsterToSummon(_bossVNum, new MapCell() { X = _bossX, Y = _bossY }, -1, _bossMove)
            {
                DeathEvents = onDeathEvents
            };
            EventHelper.Instance.RunEvent(new EventContainer(fami.Act4RaidBossMap, EventActionType.SPAWNMONSTER, bossMob));
            EventHelper.Instance.RunEvent(new EventContainer(fami.Act4Raid, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ACT4RAID_OPEN"), 0)));

            Observable.Timer(TimeSpan.FromSeconds(90)).Subscribe(o =>
            {
                //TODO: Summon Monsters
            });
        }

        private void refreshRaid(int remaining)
        {
            foreach (Family fam in ServerManager.Instance.FamilyList.GetAllItems())
            {
                if (fam.Act4Raid == null)
                {
                    fam.Act4Raid = ServerManager.GenerateMapInstance(_mapId, _raidType, new InstanceBag());
                }
                if (fam.Act4RaidBossMap == null)
                {
                    fam.Act4RaidBossMap = ServerManager.GenerateMapInstance(_bossMapId, _raidType, new InstanceBag());
                }
                if (remaining <= 1800 && !fam.Act4Raid.Portals.Any(s => s.DestinationMapInstanceId.Equals(fam.Act4RaidBossMap.MapInstanceId)))
                {
                    fam.Act4Raid.CreatePortal(new Portal()
                    {
                        DestinationMapInstanceId = fam.Act4RaidBossMap.MapInstanceId,
                        DestinationX = _destPortalX,
                        DestinationY = _destPortalY,
                        SourceX = _sourcePortalX,
                        SourceY = _sourcePortalY,
                    });
                    openRaid(fam);
                }

                int count = fam.Act4RaidBossMap.Sessions.Count();

                if (remaining < 1800 && count != 0)
                {
                    if (count > 5)
                    {
                        count = 5;
                    }
                    List<MonsterToSummon> mobWave = new List<MonsterToSummon>();
                    for (int i = 0; i < count; i++)
                    {
                        switch (_raidType)
                        {
                            case MapInstanceType.Act4Morcos:
                                mobWave.Add(new MonsterToSummon(561, fam.Act4RaidBossMap.Map.GetRandomPosition(), -1, true));
                                mobWave.Add(new MonsterToSummon(561, fam.Act4RaidBossMap.Map.GetRandomPosition(), -1, true));
                                mobWave.Add(new MonsterToSummon(561, fam.Act4RaidBossMap.Map.GetRandomPosition(), -1, true));
                                mobWave.Add(new MonsterToSummon(562, fam.Act4RaidBossMap.Map.GetRandomPosition(), -1, true));
                                mobWave.Add(new MonsterToSummon(562, fam.Act4RaidBossMap.Map.GetRandomPosition(), -1, true));
                                mobWave.Add(new MonsterToSummon(562, fam.Act4RaidBossMap.Map.GetRandomPosition(), -1, true));
                                break;

                            case MapInstanceType.Act4Hatus:
                                mobWave.Add(new MonsterToSummon(574, fam.Act4RaidBossMap.Map.GetRandomPosition(), -1, true));
                                mobWave.Add(new MonsterToSummon(574, fam.Act4RaidBossMap.Map.GetRandomPosition(), -1, true));
                                mobWave.Add(new MonsterToSummon(575, fam.Act4RaidBossMap.Map.GetRandomPosition(), -1, true));
                                mobWave.Add(new MonsterToSummon(575, fam.Act4RaidBossMap.Map.GetRandomPosition(), -1, true));
                                mobWave.Add(new MonsterToSummon(576, fam.Act4RaidBossMap.Map.GetRandomPosition(), -1, true));
                                mobWave.Add(new MonsterToSummon(576, fam.Act4RaidBossMap.Map.GetRandomPosition(), -1, true));
                                break;

                            case MapInstanceType.Act4Calvina:
                                mobWave.Add(new MonsterToSummon(770, fam.Act4RaidBossMap.Map.GetRandomPosition(), -1, true));
                                mobWave.Add(new MonsterToSummon(770, fam.Act4RaidBossMap.Map.GetRandomPosition(), -1, true));
                                mobWave.Add(new MonsterToSummon(770, fam.Act4RaidBossMap.Map.GetRandomPosition(), -1, true));
                                mobWave.Add(new MonsterToSummon(771, fam.Act4RaidBossMap.Map.GetRandomPosition(), -1, true));
                                mobWave.Add(new MonsterToSummon(771, fam.Act4RaidBossMap.Map.GetRandomPosition(), -1, true));
                                mobWave.Add(new MonsterToSummon(771, fam.Act4RaidBossMap.Map.GetRandomPosition(), -1, true));
                                break;

                            case MapInstanceType.Act4Berios:
                                mobWave.Add(new MonsterToSummon(780, fam.Act4RaidBossMap.Map.GetRandomPosition(), -1, true));
                                mobWave.Add(new MonsterToSummon(781, fam.Act4RaidBossMap.Map.GetRandomPosition(), -1, true));
                                mobWave.Add(new MonsterToSummon(782, fam.Act4RaidBossMap.Map.GetRandomPosition(), -1, true));
                                mobWave.Add(new MonsterToSummon(782, fam.Act4RaidBossMap.Map.GetRandomPosition(), -1, true));
                                mobWave.Add(new MonsterToSummon(783, fam.Act4RaidBossMap.Map.GetRandomPosition(), -1, true));
                                mobWave.Add(new MonsterToSummon(783, fam.Act4RaidBossMap.Map.GetRandomPosition(), -1, true));
                                break;
                        }
                    }
                    fam.Act4RaidBossMap.SummonMonsters(mobWave);
                }
            }
        }

        #endregion
    }
}