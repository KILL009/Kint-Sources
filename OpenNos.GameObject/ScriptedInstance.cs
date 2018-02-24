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
using OpenNos.XMLModel.Models.ScriptedInstance;
using System;
using System.Collections.Generic;
using System.IO;
using OpenNos.GameObject.Networking;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace OpenNos.GameObject
{
    public class ScriptedInstance : ScriptedInstanceDTO
    {
        #region Members

        private readonly Dictionary<int, MapInstance> _mapInstanceDictionary = new Dictionary<int, MapInstance>();

        private IDisposable _disposable;

        #endregion

        #region Instantiation

        public ScriptedInstance()
        {
        }

        public ScriptedInstance(ScriptedInstanceDTO input)
        {
            MapId = input.MapId;
            PositionX = input.PositionX;
            PositionY = input.PositionY;
            Script = input.Script;
            ScriptedInstanceId = input.ScriptedInstanceId;
            Type = input.Type;
        }

        #endregion

        #region Properties

        public List<Gift> DrawItems { get; set; }

        public MapInstance FirstMap { get; set; }

        public List<Gift> GiftItems { get; set; }

        public long Gold { get; set; }

        public byte Id { get; set; }

        public InstanceBag InstanceBag { get; } = new InstanceBag();

        public string Label { get; set; }

        public byte LevelMaximum { get; set; }

        public byte LevelMinimum { get; set; }

        public byte Lives { get; set; }

        public ScriptedInstanceModel Model { get; set; }

        public int MonsterAmount { get; internal set; }

        public string Name { get; set; }

        public int NpcAmount { get; internal set; }

        public int Reputation { get; set; }

        public List<Gift> RequiredItems { get; set; }

        public int RoomAmount { get; internal set; }

        public List<Gift> SpecialItems { get; set; }

        public short StartX { get; set; }

        public short StartY { get; set; }

        #endregion

        #region Methods

        public void Dispose()
        {
            Thread.Sleep(10000);
            _mapInstanceDictionary.Values.ToList().ForEach(m => m.Dispose());
        }

        public string GenerateMainInfo() => $"minfo 0 1 -1.0/0 -1.0/0 -1/0 -1.0/0 1 {InstanceBag.Lives + 1} 0";

        public List<string> GenerateMinimap()
        {
            List<string> lst = new List<string> { "rsfm 0 0 4 12" };
            _mapInstanceDictionary.Values.ToList().ForEach(s => lst.Add(s.GenerateRsfn(true)));
            return lst;
        }

        public string GenerateRbr()
        {
            string drawgift = string.Empty;
            string requireditem = string.Empty;
            string bonusitems = string.Empty;
            string specialitems = string.Empty;

            for (int i = 0; i < 5; i++)
            {
                Gift gift = DrawItems?.ElementAtOrDefault(i);
                drawgift += $" {(gift == null ? "-1.0" : $"{gift.VNum}.{gift.Amount}")}";
            }
            for (int i = 0; i < 2; i++)
            {
                Gift gift = SpecialItems?.ElementAtOrDefault(i);
                specialitems += $" {(gift == null ? "-1.0" : $"{gift.VNum}.{gift.Amount}")}";
            }

            for (int i = 0; i < 3; i++)
            {
                Gift gift = GiftItems?.ElementAtOrDefault(i);
                bonusitems += $"{(i == 0 ? string.Empty : " ")}{(gift == null ? "-1.0" : $"{gift.VNum}.{gift.Amount}")}";
            }
            const int WinnerScore = 0;
            const string Winner = "";
            return $"rbr 0.0.0 4 15 {LevelMinimum}.{LevelMaximum} {RequiredItems?.Sum(s => s.Amount)} {drawgift} {specialitems} {bonusitems} {WinnerScore}.{(WinnerScore > 0 ? Winner : string.Empty)} 0 0 {Name}\n{Label}";
        }

        public string GenerateWp() => $"wp {PositionX} {PositionY} {ScriptedInstanceId} 0 {LevelMinimum} {LevelMaximum}";

        public void LoadGlobals()
        {
            // initialize script as byte stream
            if (Script != null)
            {
                byte[] xml = Encoding.UTF8.GetBytes(Script);
                MemoryStream memoryStream = new MemoryStream(xml);
                XmlReader reader = XmlReader.Create(memoryStream);
                XmlSerializer serializer = new XmlSerializer(typeof(ScriptedInstanceModel));
                Model = (ScriptedInstanceModel)serializer.Deserialize(reader);
                memoryStream.Close();

                if (Model?.Globals != null)
                {
                    RequiredItems = new List<Gift>();
                    DrawItems = new List<Gift>();
                    SpecialItems = new List<Gift>();
                    GiftItems = new List<Gift>();

                    // set the values
                    Id = Model.Globals.Id?.Value ?? 0;
                    Gold = Model.Globals.Gold?.Value ?? 0;
                    Reputation = Model.Globals.Reputation?.Value ?? 0;
                    StartX = Model.Globals.StartX?.Value ?? 0;
                    StartY = Model.Globals.StartY?.Value ?? 0;
                    Lives = Model.Globals.Lives?.Value ?? 0;
                    LevelMinimum = Model.Globals.LevelMinimum?.Value ?? 1;
                    LevelMaximum = Model.Globals.LevelMaximum?.Value ?? 99;
                    Name = Model.Globals.Name?.Value ?? "No Name";
                    Label = Model.Globals.Label?.Value ?? "No Description";
                    if (Model.Globals.RequiredItems != null)
                    {
                        foreach (XMLModel.Objects.Item item in Model.Globals.RequiredItems)
                        {
                            RequiredItems.Add(new Gift(item.VNum, item.Amount, item.Design, item.IsRandomRare));
                        }
                    }
                    if (Model.Globals.DrawItems != null)
                    {
                        foreach (XMLModel.Objects.Item item in Model.Globals.DrawItems)
                        {
                            DrawItems.Add(new Gift(item.VNum, item.Amount, item.Design, item.IsRandomRare));
                        }
                    }
                    if (Model.Globals.SpecialItems != null)
                    {
                        foreach (XMLModel.Objects.Item item in Model.Globals.SpecialItems)
                        {
                            SpecialItems.Add(new Gift(item.VNum, item.Amount, item.Design, item.IsRandomRare));
                        }
                    }
                    if (Model.Globals.GiftItems != null)
                    {
                        foreach (XMLModel.Objects.Item item in Model.Globals.GiftItems)
                        {
                            GiftItems.Add(new Gift(item.VNum, item.Amount, item.Design, item.IsRandomRare));
                        }
                    }
                }
            }
        }

        public void LoadScript(MapInstanceType mapinstancetype)
        {
            if (Model != null)
            {
                InstanceBag.Lives = Lives;
                if (Model.InstanceEvents?.CreateMap != null)
                {
                    foreach (XMLModel.Objects.CreateMap createMap in Model.InstanceEvents.CreateMap)
                    {
                        MapInstance mapInstance = ServerManager.GenerateMapInstance(createMap.VNum, mapinstancetype, new InstanceBag());
                        mapInstance.Portals?.Clear();
                        mapInstance.MapIndexX = createMap.IndexX;
                        mapInstance.MapIndexY = createMap.IndexY;
                        if (!_mapInstanceDictionary.ContainsKey(createMap.Map))
                        {
                            _mapInstanceDictionary.Add(createMap.Map, mapInstance);
                        }
                    }
                }

                FirstMap = _mapInstanceDictionary.Values.FirstOrDefault();
                Observable.Timer(TimeSpan.FromMinutes(3)).Subscribe(x =>
                {
                    if (!InstanceBag.Lock)
                    {
                        _mapInstanceDictionary.Values.ToList().ForEach(m => EventHelper.Instance.RunEvent(new EventContainer(m, EventActionType.SCRIPTEND, (byte)1)));
                        Dispose();
                    }
                });
                _disposable = Observable.Interval(TimeSpan.FromMilliseconds(100)).Subscribe(x =>
                {
                    if (InstanceBag.Lives - InstanceBag.DeadList.Count < 0)
                    {
                        _mapInstanceDictionary.Values.ToList().ForEach(m => EventHelper.Instance.RunEvent(new EventContainer(m, EventActionType.SCRIPTEND, (byte)3)));
                        Dispose();
                        _disposable.Dispose();
                    }
                    if (InstanceBag.Clock.SecondsRemaining <= 0)
                    {
                        _mapInstanceDictionary.Values.ToList().ForEach(m => EventHelper.Instance.RunEvent(new EventContainer(m, EventActionType.SCRIPTEND, (byte)1)));
                        Dispose();
                        _disposable.Dispose();
                    }
                });

                generateEvent(FirstMap).ForEach(e => EventHelper.Instance.RunEvent(e));
            }
        }

        private ThreadSafeGenericList<EventContainer> generateEvent(MapInstance parentMapInstance)
        {
            // Needs Optimization, look into it.
            ThreadSafeGenericList<EventContainer> evts = new ThreadSafeGenericList<EventContainer>();

            if (Model.InstanceEvents.CreateMap != null)
            {
                Parallel.ForEach(Model.InstanceEvents.CreateMap, createMap =>
                {
                    MapInstance mapInstance = _mapInstanceDictionary.FirstOrDefault(s => s.Key == createMap.Map).Value ?? parentMapInstance;

                    if (mapInstance == null)
                    {
                        return;
                    }

                    // SummonMonster
                    evts.AddRange(summonMonster(mapInstance, createMap.SummonMonster));

                    // SummonNpc
                    evts.AddRange(summonNpc(mapInstance, createMap.SummonNpc));

                    // SpawnPortal
                    evts.AddRange(spawnPortal(mapInstance, createMap.SpawnPortal));

                    // SpawnButton
                    evts.AddRange(spawnButton(mapInstance, createMap.SpawnButton));

                    // OnCharacterDiscoveringMap
                    evts.AddRange(onCharacterDiscoveringMap(mapInstance, createMap));

                    // GenerateMapClock
                    if (createMap.GenerateClock != null)
                    {
                        evts.Add(new EventContainer(mapInstance, EventActionType.CLOCK, createMap.GenerateClock.Value));
                    }

                    // OnMoveOnMap
                    if (createMap.OnMoveOnMap != null)
                    {
                        Parallel.ForEach(createMap.OnMoveOnMap, onMoveOnMap => evts.AddRange(this.onMoveOnMap(mapInstance, onMoveOnMap)));
                    }

                    // OnLockerOpen
                    if (createMap.OnLockerOpen != null)
                    {
                        List<EventContainer> onLockerOpen = new List<EventContainer>();

                        // SendMessage
                        if (createMap.OnLockerOpen.SendMessage != null)
                        {
                            onLockerOpen.Add(new EventContainer(mapInstance, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(createMap.OnLockerOpen.SendMessage.Value, createMap.OnLockerOpen.SendMessage.Type)));
                        }

                        // ChangePortalType
                        if (createMap.OnLockerOpen.ChangePortalType != null)
                        {
                            onLockerOpen.Add(new EventContainer(mapInstance, EventActionType.CHANGEPORTALTYPE, new Tuple<int, PortalType>(createMap.OnLockerOpen.ChangePortalType.IdOnMap, (PortalType)createMap.OnLockerOpen.ChangePortalType.Type)));
                        }

                        // RefreshMapItems
                        if (createMap.OnLockerOpen.RefreshMapItems != null)
                        {
                            onLockerOpen.Add(new EventContainer(mapInstance, EventActionType.REFRESHMAPITEMS, null));
                        }

                        evts.Add(new EventContainer(mapInstance, EventActionType.REGISTEREVENT, new Tuple<string, List<EventContainer>>(nameof(XMLModel.Events.OnLockerOpen), onLockerOpen)));
                    }

                    // OnAreaEntry
                    if (createMap.OnAreaEntry != null)
                    {
                        foreach (XMLModel.Events.OnAreaEntry onAreaEntry in createMap.OnAreaEntry)
                        {
                            List<EventContainer> onAreaEntryEvents = new List<EventContainer>();
                            onAreaEntryEvents.AddRange(summonMonster(mapInstance, onAreaEntry.SummonMonster));
                            evts.Add(new EventContainer(mapInstance, EventActionType.SETAREAENTRY, new ZoneEvent() { X = onAreaEntry.PositionX, Y = onAreaEntry.PositionY, Range = onAreaEntry.Range, Events = onAreaEntryEvents }));
                        }
                    }

                    // SetButtonLockers
                    if (createMap.SetButtonLockers != null)
                    {
                        evts.Add(new EventContainer(mapInstance, EventActionType.SETBUTTONLOCKERS, createMap.SetButtonLockers.Value));
                    }

                    // SetMonsterLockers
                    if (createMap.SetMonsterLockers != null)
                    {
                        evts.Add(new EventContainer(mapInstance, EventActionType.SETMONSTERLOCKERS, createMap.SetMonsterLockers.Value));
                    }
                });
            }

            return evts;
        }

        private List<EventContainer> onCharacterDiscoveringMap(MapInstance mapInstance, XMLModel.Objects.CreateMap createMap)
        {
            List<EventContainer> evts = new List<EventContainer>();

            // OnCharacterDiscoveringMap
            if (createMap.OnCharacterDiscoveringMap != null)
            {
                List<EventContainer> onDiscoverEvents = new List<EventContainer>();

                // GenerateMapClock
                if (createMap.OnCharacterDiscoveringMap.GenerateMapClock != null)
                {
                    onDiscoverEvents.Add(new EventContainer(mapInstance, EventActionType.MAPCLOCK, createMap.OnCharacterDiscoveringMap.GenerateMapClock.Value));
                }

                // NpcDialog
                if (createMap.OnCharacterDiscoveringMap.NpcDialog != null)
                {
                    onDiscoverEvents.Add(new EventContainer(mapInstance, EventActionType.NPCDIALOG, createMap.OnCharacterDiscoveringMap.NpcDialog.Value));
                }

                // SendMessage
                if (createMap.OnCharacterDiscoveringMap.SendMessage != null)
                {
                    onDiscoverEvents.Add(new EventContainer(mapInstance, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(createMap.OnCharacterDiscoveringMap.SendMessage.Value, createMap.OnCharacterDiscoveringMap.SendMessage.Type)));
                }

                // SendPacket
                if (createMap.OnCharacterDiscoveringMap.SendPacket != null)
                {
                    onDiscoverEvents.Add(new EventContainer(mapInstance, EventActionType.SENDPACKET, createMap.OnCharacterDiscoveringMap.SendPacket.Value));
                }

                // SummonMonster
                onDiscoverEvents.AddRange(summonMonster(mapInstance, createMap.OnCharacterDiscoveringMap.SummonMonster));

                // SummonNpc
                onDiscoverEvents.AddRange(summonNpc(mapInstance, createMap.OnCharacterDiscoveringMap.SummonNpc));

                // SpawnPortal
                onDiscoverEvents.AddRange(spawnPortal(mapInstance, createMap.OnCharacterDiscoveringMap.SpawnPortal));

                // OnMoveOnMap
                if (createMap.OnCharacterDiscoveringMap.OnMoveOnMap != null)
                {
                    onDiscoverEvents.AddRange(onMoveOnMap(mapInstance, createMap.OnCharacterDiscoveringMap.OnMoveOnMap));
                }

                evts.Add(new EventContainer(mapInstance, EventActionType.REGISTEREVENT, new Tuple<string, List<EventContainer>>(nameof(XMLModel.Events.OnCharacterDiscoveringMap), onDiscoverEvents)));
            }

            return evts;
        }

        private List<EventContainer> onMapClean(MapInstance mapInstance, XMLModel.Events.OnMapClean onMapClean)
        {
            List<EventContainer> evts = new List<EventContainer>();

            // OnMapClean
            if (onMapClean != null)
            {
                List<EventContainer> onMapCleanEvents = new List<EventContainer>();

                // ChangePortalType
                if (onMapClean.ChangePortalType != null)
                {
                    foreach (XMLModel.Events.ChangePortalType changePortalType in onMapClean.ChangePortalType)
                    {
                        onMapCleanEvents.Add(new EventContainer(mapInstance, EventActionType.CHANGEPORTALTYPE, new Tuple<int, PortalType>(changePortalType.IdOnMap, (PortalType)changePortalType.Type)));
                    }
                }

                // RefreshMapItems
                if (onMapClean.RefreshMapItems != null)
                {
                    onMapCleanEvents.Add(new EventContainer(mapInstance, EventActionType.REFRESHMAPITEMS, null));
                }

                // SendMessage
                if (onMapClean.SendMessage != null)
                {
                    onMapCleanEvents.Add(new EventContainer(mapInstance, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(onMapClean.SendMessage.Value, onMapClean.SendMessage.Type)));
                }

                // SendPacket
                if (onMapClean.SendPacket != null)
                {
                    onMapCleanEvents.Add(new EventContainer(mapInstance, EventActionType.SENDPACKET, onMapClean.SendPacket.Value));
                }

                // NpcDialog
                if (onMapClean.NpcDialog != null)
                {
                    onMapCleanEvents.Add(new EventContainer(mapInstance, EventActionType.NPCDIALOG, onMapClean.NpcDialog.Value));
                }

                evts.Add(new EventContainer(mapInstance, EventActionType.REGISTEREVENT, new Tuple<string, List<EventContainer>>(nameof(XMLModel.Events.OnMapClean), onMapCleanEvents)));
            }

            return evts;
        }

        private List<EventContainer> onMoveOnMap(MapInstance mapInstance, XMLModel.Events.OnMoveOnMap onMoveOnMap)
        {
            List<EventContainer> evts = new List<EventContainer>();

            // OnMoveOnMap
            if (onMoveOnMap != null)
            {
                List<EventContainer> waveEvent = new List<EventContainer>();

                // SendMessage
                if (onMoveOnMap.SendMessage != null)
                {
                    evts.Add(new EventContainer(mapInstance, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(onMoveOnMap.SendMessage.Value, onMoveOnMap.SendMessage.Type)));
                }

                // SendPacket
                if (onMoveOnMap.SendPacket != null)
                {
                    evts.Add(new EventContainer(mapInstance, EventActionType.SENDPACKET, onMoveOnMap.SendPacket.Value));
                }

                // StartClock
                if (onMoveOnMap.StartClock != null)
                {
                    List<EventContainer> onStop = new List<EventContainer>();
                    List<EventContainer> onTimeout = new List<EventContainer>();

                    // OnStop
                    if (onMoveOnMap.StartClock.OnStop != null)
                    {
                        if (onMoveOnMap.StartClock.OnStop.SendMessage != null)
                        {
                            onStop.Add(new EventContainer(mapInstance, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(onMoveOnMap.StartClock.OnStop.SendMessage.Value, onMoveOnMap.StartClock.OnStop.SendMessage.Type)));
                        }
                        if (onMoveOnMap.StartClock.OnStop.SendPacket != null)
                        {
                            onStop.Add(new EventContainer(mapInstance, EventActionType.SENDPACKET, onMoveOnMap.StartClock.OnStop.SendPacket.Value));
                        }
                        if (onMoveOnMap.StartClock.OnStop.RefreshMapItems != null)
                        {
                            onStop.Add(new EventContainer(mapInstance, EventActionType.REFRESHMAPITEMS, null));
                        }
                        foreach (XMLModel.Events.ChangePortalType changePortalType in onMoveOnMap.StartClock.OnStop.ChangePortalType)
                        {
                            onStop.Add(new EventContainer(mapInstance, EventActionType.CHANGEPORTALTYPE, new Tuple<int, PortalType>(changePortalType.IdOnMap, (PortalType)changePortalType.Type)));
                        }
                    }

                    // OnTimeout
                    if (onMoveOnMap.StartClock.OnTimeout?.End != null)
                    {
                        onTimeout.Add(new EventContainer(mapInstance, EventActionType.SCRIPTEND, onMoveOnMap.StartClock.OnTimeout.End.Type));
                    }

                    evts.Add(new EventContainer(mapInstance, EventActionType.STARTCLOCK, new Tuple<List<EventContainer>, List<EventContainer>>(onStop, onTimeout)));
                }

                // StartMapClock
                if (onMoveOnMap.StartMapClock != null)
                {
                    List<EventContainer> onStop = new List<EventContainer>();
                    List<EventContainer> onTimeout = new List<EventContainer>();

                    // OnStop
                    if (onMoveOnMap.StartMapClock.OnStop != null)
                    {
                        if (onMoveOnMap.StartMapClock.OnStop.SendMessage != null)
                        {
                            onStop.Add(new EventContainer(mapInstance, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(onMoveOnMap.StartMapClock.OnStop.SendMessage.Value, onMoveOnMap.StartMapClock.OnStop.SendMessage.Type)));
                        }
                        if (onMoveOnMap.StartMapClock.OnStop.SendPacket != null)
                        {
                            onStop.Add(new EventContainer(mapInstance, EventActionType.SENDPACKET, onMoveOnMap.StartMapClock.OnStop.SendPacket.Value));
                        }
                        if (onMoveOnMap.StartMapClock.OnStop.RefreshMapItems != null)
                        {
                            onStop.Add(new EventContainer(mapInstance, EventActionType.REFRESHMAPITEMS, null));
                        }
                        foreach (XMLModel.Events.ChangePortalType changePortalType in onMoveOnMap.StartMapClock.OnStop.ChangePortalType)
                        {
                            onStop.Add(new EventContainer(mapInstance, EventActionType.CHANGEPORTALTYPE, new Tuple<int, PortalType>(changePortalType.IdOnMap, (PortalType)changePortalType.Type)));
                        }
                    }

                    // OnTimeout
                    if (onMoveOnMap.StartMapClock.OnTimeout?.End != null)
                    {
                        onTimeout.Add(new EventContainer(mapInstance, EventActionType.SCRIPTEND, onMoveOnMap.StartMapClock.OnTimeout.End.Type));
                    }

                    evts.Add(new EventContainer(mapInstance, EventActionType.STARTMAPCLOCK, new Tuple<List<EventContainer>, List<EventContainer>>(onStop, onTimeout)));
                }

                // Wave
                if (onMoveOnMap.Wave != null)
                {
                    foreach (XMLModel.Objects.Wave wave in onMoveOnMap.Wave)
                    {
                        // SummonMonster
                        waveEvent.AddRange(summonMonster(mapInstance, wave.SummonMonster));

                        // SendMessage
                        if (wave.SendMessage != null)
                        {
                            waveEvent.Add(new EventContainer(mapInstance, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(wave.SendMessage.Value, wave.SendMessage.Type)));
                        }

                        evts.Add(new EventContainer(mapInstance, EventActionType.REGISTERWAVE, new EventWave(wave.Delay, waveEvent, wave.Offset)));
                    }
                }

                // SummonMonster
                evts.AddRange(summonMonster(mapInstance, onMoveOnMap.SummonMonster));

                // GenerateClock
                if (onMoveOnMap.GenerateClock != null)
                {
                    evts.Add(new EventContainer(mapInstance, EventActionType.CLOCK, onMoveOnMap.GenerateClock.Value));
                }

                // OnMapClean
                evts.AddRange(onMapClean(mapInstance, onMoveOnMap.OnMapClean));

                evts.Add(new EventContainer(mapInstance, EventActionType.REGISTEREVENT, new Tuple<string, List<EventContainer>>(nameof(XMLModel.Events.OnMoveOnMap), evts)));
            }

            return evts;
        }

        private List<EventContainer> spawnButton(MapInstance mapInstance, XMLModel.Events.SpawnButton[] spawnButton)
        {
            List<EventContainer> evts = new List<EventContainer>();

            // SpawnButton
            if (spawnButton != null)
            {
                foreach (XMLModel.Events.SpawnButton spawn in spawnButton)
                {
                    short positionX = spawn.PositionX;
                    short positionY = spawn.PositionY;

                    if (positionX == 0 || positionY == 0)
                    {
                        MapCell cell = mapInstance?.Map?.GetRandomPosition();
                        if (cell != null)
                        {
                            positionX = cell.X;
                            positionY = cell.Y;
                        }
                    }

                    MapButton button = new MapButton(spawn.Id, positionX, positionY, spawn.VNumEnabled, spawn.VNumDisabled, new List<EventContainer>(), new List<EventContainer>(), new List<EventContainer>());

                    // OnFirstEnable
                    if (spawn.OnFirstEnable != null)
                    {
                        List<EventContainer> onFirst = new List<EventContainer>();

                        // SummonMonster
                        onFirst.AddRange(summonMonster(mapInstance, spawn.OnFirstEnable.SummonMonster));

                        // Teleport
                        if (spawn.OnFirstEnable.Teleport != null)
                        {
                            onFirst.Add(new EventContainer(mapInstance, EventActionType.TELEPORT, new Tuple<short, short, short, short>(spawn.OnFirstEnable.Teleport.PositionX, spawn.OnFirstEnable.Teleport.PositionY, spawn.OnFirstEnable.Teleport.DestinationX, spawn.OnFirstEnable.Teleport.DestinationY)));
                        }

                        // RemoveButtonLocker
                        if (spawn.OnFirstEnable.RemoveButtonLocker != null)
                        {
                            onFirst.Add(new EventContainer(mapInstance, EventActionType.REMOVEBUTTONLOCKER, null));
                        }

                        // RefreshRaidGoals
                        if (spawn.OnFirstEnable.RefreshRaidGoals != null)
                        {
                            onFirst.Add(new EventContainer(mapInstance, EventActionType.REFRESHRAIDGOAL, null));
                        }

                        // SendMessage
                        if (spawn.OnFirstEnable.SendMessage != null)
                        {
                            onFirst.Add(new EventContainer(mapInstance, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(spawn.OnFirstEnable.SendMessage.Value, spawn.OnFirstEnable.SendMessage.Type)));
                        }

                        // OnMapClean
                        if (spawn.OnFirstEnable.OnMapClean != null)
                        {
                            onFirst.AddRange(onMapClean(mapInstance, spawn.OnFirstEnable.OnMapClean));
                        }

                        button.FirstEnableEvents.AddRange(onFirst);
                    }

                    // OnEnable & Teleport
                    if (spawn.OnEnable?.Teleport != null)
                    {
                        button.EnableEvents.Add(new EventContainer(mapInstance, EventActionType.TELEPORT, new Tuple<short, short, short, short>(spawn.OnEnable.Teleport.PositionX, spawn.OnEnable.Teleport.PositionY, spawn.OnEnable.Teleport.DestinationX, spawn.OnEnable.Teleport.DestinationY)));
                    }

                    // OnDisable & Teleport
                    if (spawn.OnDisable?.Teleport != null)
                    {
                        button.DisableEvents.Add(new EventContainer(mapInstance, EventActionType.TELEPORT, new Tuple<short, short, short, short>(spawn.OnDisable.Teleport.PositionX, spawn.OnDisable.Teleport.PositionY, spawn.OnDisable.Teleport.DestinationX, spawn.OnDisable.Teleport.DestinationY)));
                    }

                    evts.Add(new EventContainer(mapInstance, EventActionType.SPAWNBUTTON, button));
                }
            }

            return evts;
        }

        private List<EventContainer> spawnPortal(MapInstance mapInstance, XMLModel.Events.SpawnPortal[] spawnPortal)
        {
            List<EventContainer> evts = new List<EventContainer>();

            // SpawnPortal
            if (spawnPortal != null)
            {
                foreach (XMLModel.Events.SpawnPortal portalEvent in spawnPortal)
                {
                    MapInstance destinationMap = _mapInstanceDictionary.First(s => s.Key == portalEvent.ToMap).Value;
                    Portal portal = new Portal()
                    {
                        PortalId = portalEvent.IdOnMap,
                        SourceX = portalEvent.PositionX,
                        SourceY = portalEvent.PositionY,
                        Type = portalEvent.Type,
                        DestinationX = portalEvent.ToX,
                        DestinationY = portalEvent.ToY,
                        DestinationMapId = (short)(destinationMap.MapInstanceId == default ? -1 : 0),
                        SourceMapInstanceId = mapInstance.MapInstanceId,
                        DestinationMapInstanceId = destinationMap.MapInstanceId
                    };

                    // OnTraversal
                    if (portalEvent.OnTraversal?.End != null)
                    {
                        portal.OnTraversalEvents.Add(new EventContainer(mapInstance, EventActionType.SCRIPTEND, portalEvent.OnTraversal.End.Type));
                    }

                    evts.Add(new EventContainer(mapInstance, EventActionType.SPAWNPORTAL, portal));
                }
            }

            return evts;
        }

        private List<EventContainer> summonMonster(MapInstance mapInstance, XMLModel.Events.SummonMonster[] summonMonster, bool isChildMonster = false)
        {
            List<EventContainer> evts = new List<EventContainer>();

            // SummonMonster
            if (summonMonster != null)
            {
                foreach (XMLModel.Events.SummonMonster summon in summonMonster)
                {
                    short positionX = summon.PositionX;
                    short positionY = summon.PositionY;
                    if (positionX == 0 || positionY == 0)
                    {
                        MapCell cell = mapInstance?.Map?.GetRandomPosition();
                        if (cell != null)
                        {
                            positionX = cell.X;
                            positionY = cell.Y;
                        }
                    }
                    MonsterAmount++;
                    MonsterToSummon monster = new MonsterToSummon(summon.VNum, new MapCell() { X = positionX, Y = positionY }, -1, summon.Move, summon.IsTarget, summon.IsBonus, summon.IsHostile, summon.IsBoss);

                    // OnDeath
                    if (summon.OnDeath != null)
                    {
                        // RemoveButtonLocker
                        if (summon.OnDeath.RemoveButtonLocker != null)
                        {
                            monster.DeathEvents.Add(new EventContainer(mapInstance, EventActionType.REMOVEBUTTONLOCKER, null));
                        }

                        // RemoveMonsterLocker
                        if (summon.OnDeath.RemoveMonsterLocker != null)
                        {
                            monster.DeathEvents.Add(new EventContainer(mapInstance, EventActionType.REMOVEMONSTERLOCKER, null));
                        }

                        // ChangePortalType
                        if (summon.OnDeath.ChangePortalType != null)
                        {
                            foreach (XMLModel.Events.ChangePortalType changePortalType in summon.OnDeath.ChangePortalType)
                            {
                                monster.DeathEvents.Add(new EventContainer(mapInstance, EventActionType.CHANGEPORTALTYPE, new Tuple<int, PortalType>(changePortalType.IdOnMap, (PortalType)changePortalType.Type)));
                            }
                        }

                        // SendMessage
                        if (summon.OnDeath.SendMessage != null)
                        {
                            evts.Add(new EventContainer(mapInstance, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(summon.OnDeath.SendMessage.Value, summon.OnDeath.SendMessage.Type)));
                        }

                        // SendPacket
                        if (summon.OnDeath.SendPacket != null)
                        {
                            evts.Add(new EventContainer(mapInstance, EventActionType.SENDPACKET, summon.OnDeath.SendPacket.Value));
                        }

                        // RefreshRaidGoals
                        if (summon.OnDeath.RefreshRaidGoals != null)
                        {
                            monster.DeathEvents.Add(new EventContainer(mapInstance, EventActionType.REFRESHRAIDGOAL, null));
                        }

                        // RefreshMapItems
                        if (summon.OnDeath.RefreshMapItems != null)
                        {
                            monster.DeathEvents.Add(new EventContainer(mapInstance, EventActionType.REFRESHMAPITEMS, null));
                        }

                        // ThrowItem
                        if (summon.OnDeath.ThrowItem != null)
                        {
                            foreach (XMLModel.Events.ThrowItem throwItem in summon.OnDeath.ThrowItem)
                            {
                                monster.DeathEvents.Add(new EventContainer(mapInstance, EventActionType.THROWITEMS, new Tuple<int, short, byte, int, int>(-1, throwItem.VNum, throwItem.PackAmount == 0 ? (byte)1 : throwItem.PackAmount, throwItem.MinAmount == 0 ? 1 : throwItem.MinAmount, throwItem.MaxAmount == 0 ? 1 : throwItem.MaxAmount)));
                            }
                        }

                        // End
                        if (summon.OnDeath.End != null)
                        {
                            monster.DeathEvents.Add(new EventContainer(mapInstance, EventActionType.SCRIPTEND, summon.OnDeath.End.Type));
                        }

                        // SummonMonster Child
                        if (!isChildMonster)
                        {
                            monster.DeathEvents.AddRange(this.summonMonster(mapInstance, summon.OnDeath.SummonMonster, true));
                        }
                    }

                    // OnNoticing
                    if (summon.OnNoticing != null)
                    {
                        // Effect
                        if (summon.OnNoticing.Effect != null)
                        {
                            monster.NoticingEvents.Add(new EventContainer(mapInstance, EventActionType.EFFECT, summon.OnNoticing.Effect.Value));
                        }

                        // Move
                        if (summon.OnNoticing.Move != null)
                        {
                            List<EventContainer> events = new List<EventContainer>();

                            // Effect
                            if (summon.OnNoticing.Move.Effect != null)
                            {
                                events.Add(new EventContainer(mapInstance, EventActionType.EFFECT, summon.OnNoticing.Move.Effect.Value));
                            }

                            // review OnTarget
                            //if (summon.OnNoticing.Move.OnTarget != null)
                            //{
                            //    summon.OnNoticing.Move.OnTarget.Move
                            //    foreach ()
                            //    //events.Add(new EventContainer(mapInstance, EventActionType.ONTARGET, summon.OnNoticing.Move.OnTarget.));
                            //}

                            monster.NoticingEvents.Add(new EventContainer(mapInstance, EventActionType.MOVE, new ZoneEvent() { X = summon.OnNoticing.Move.PositionX, Y = summon.OnNoticing.Move.PositionY, Events = events }));
                        }

                        // SummonMonster Child
                        if (!isChildMonster)
                        {
                            monster.NoticingEvents.AddRange(this.summonMonster(mapInstance, summon.OnDeath.SummonMonster, true));
                        }
                    }

                    evts.Add(new EventContainer(mapInstance, EventActionType.SPAWNMONSTER, monster));
                }
            }

            return evts;
        }

        private List<EventContainer> summonNpc(MapInstance mapInstance, XMLModel.Events.SummonNpc[] summonNpc)
        {
            List<EventContainer> evts = new List<EventContainer>();

            if (summonNpc != null)
            {
                foreach (XMLModel.Events.SummonNpc summon in summonNpc)
                {
                    short positionX = summon.PositionX;
                    short positionY = summon.PositionY;

                    if (positionX == 0 || positionY == 0)
                    {
                        MapCell cell = mapInstance?.Map?.GetRandomPosition();
                        if (cell != null)
                        {
                            positionX = cell.X;
                            positionY = cell.Y;
                        }
                    }

                    NpcAmount++;
                    NpcToSummon npcToSummon = new NpcToSummon(summon.VNum, new MapCell() { X = positionX, Y = positionY }, -1, summon.IsMate, summon.IsProtected);

                    // OnDeath
                    if (summon.OnDeath != null)
                    {
                        // RemoveButtonLocker
                        if (summon.OnDeath.RemoveButtonLocker != null)
                        {
                            npcToSummon.DeathEvents.Add(new EventContainer(mapInstance, EventActionType.REMOVEBUTTONLOCKER, null));
                        }

                        // RemoveMonsterLocker
                        if (summon.OnDeath.RemoveMonsterLocker != null)
                        {
                            npcToSummon.DeathEvents.Add(new EventContainer(mapInstance, EventActionType.REMOVEMONSTERLOCKER, null));
                        }

                        // ChangePortalType
                        if (summon.OnDeath.ChangePortalType != null)
                        {
                            foreach (XMLModel.Events.ChangePortalType changePortalType in summon.OnDeath.ChangePortalType)
                            {
                                npcToSummon.DeathEvents.Add(new EventContainer(mapInstance, EventActionType.CHANGEPORTALTYPE, new Tuple<int, PortalType>(changePortalType.IdOnMap, (PortalType)changePortalType.Type)));
                            }
                        }

                        // End
                        if (summon.OnDeath.End != null)
                        {
                            npcToSummon.DeathEvents.Add(new EventContainer(mapInstance, EventActionType.SCRIPTEND, summon.OnDeath.End.Type));
                        }

                        // RefreshRaidGoals
                        if (summon.OnDeath.RefreshRaidGoals != null)
                        {
                            npcToSummon.DeathEvents.Add(new EventContainer(mapInstance, EventActionType.REFRESHRAIDGOAL, null));
                        }

                        // RefreshMapItems
                        if (summon.OnDeath.RefreshRaidGoals != null)
                        {
                            npcToSummon.DeathEvents.Add(new EventContainer(mapInstance, EventActionType.REFRESHMAPITEMS, null));
                        }

                        // ThrowItems
                        if (summon.OnDeath.ThrowItem != null)
                        {
                            foreach (XMLModel.Events.ThrowItem throwItem in summon.OnDeath.ThrowItem)
                            {
                                npcToSummon.DeathEvents.Add(new EventContainer(mapInstance, EventActionType.THROWITEMS, new Tuple<int, short, byte, int, int>(-1, throwItem.VNum, throwItem.PackAmount == 0 ? (byte)1 : throwItem.PackAmount, throwItem.MinAmount == 0 ? 1 : throwItem.MinAmount, throwItem.MaxAmount == 0 ? 1 : throwItem.MaxAmount)));
                            }
                        }
                    }

                    evts.Add(new EventContainer(mapInstance, EventActionType.SPAWNNPC, npcToSummon));
                }
            }

            return evts;
        }

        #endregion

        // Use as a idea of what is left to do
        //private void remnants()
        //{
        //    switch (mapEvent.Name)
        //    {
        //        case "SummonMonsters":
        //            evts.Add(new EventContainer(mapInstance, EventActionType.SPAWNMONSTERS, mapInstance.Map.GenerateMonsters(short.Parse(mapEvent?.Attributes["VNum"].Value), short.Parse(mapEvent?.Attributes["Amount"].Value), move, new List<EventContainer>(), isBonus, isHostile, isBoss)));
        //            break;
        //        case "SummonNpcs":
        //            evts.Add(new EventContainer(mapInstance, EventActionType.SPAWNNPCS, mapInstance.Map.GenerateNpcs(short.Parse(mapEvent?.Attributes["VNum"].Value), short.Parse(mapEvent?.Attributes["Amount"].Value), new List<EventContainer>(), isMate, isProtected)));
        //            break;
        //        case "StopClock":
        //            evts.Add(new EventContainer(mapInstance, EventActionType.STOPCLOCK, null));
        //            break;
        //        case "StopMapClock":
        //            evts.Add(new EventContainer(mapInstance, EventActionType.STOPMAPCLOCK, null));
        //            break;
        //    }
        //}
    }
}