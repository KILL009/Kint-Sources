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
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject.Event.GAMES
{
    public static class MeteoriteGame
    {
        #region Methods

        public static void GenerateMeteoriteGame()
        {
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("METEORITE_MINUTES"), 5), 0));
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("METEORITE_MINUTES"), 5), 1));
            Thread.Sleep(4 * 60 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("METEORITE_MINUTES"), 1), 0));
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("METEORITE_MINUTES"), 1), 1));
            Thread.Sleep(30 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("METEORITE_SECONDS"), 30), 0));
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("METEORITE_SECONDS"), 30), 1));
            Thread.Sleep(20 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("METEORITE_SECONDS"), 10), 0));
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("METEORITE_SECONDS"), 10), 1));
            Thread.Sleep(10 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("METEORITE_STARTED"), 1));
            ServerManager.Instance.Broadcast("qnaml 100 #guri^506 The Meteorite Game is starting! Join now!");
            ServerManager.Instance.EventInWaiting = true;
            Thread.Sleep(30 * 1000);
            ServerManager.Instance.Sessions.Where(s => s.Character?.IsWaitingForEvent == false).ToList().ForEach(s => s.SendPacket("esf"));
            ServerManager.Instance.EventInWaiting = false;
            IEnumerable<ClientSession> sessions = ServerManager.Instance.Sessions.Where(s => s.Character?.IsWaitingForEvent == true && s.Character.MapInstance.MapInstanceType == MapInstanceType.BaseMapInstance);

            MapInstance map = ServerManager.GenerateMapInstance(2004, MapInstanceType.EventGameInstance, new InstanceBag());
            if (map != null)
            {
                foreach (ClientSession sess in sessions)
                {
                    ServerManager.Instance.TeleportOnRandomPlaceInMap(sess, map.MapInstanceId);
                }

                ServerManager.Instance.Sessions.Where(s => s.Character != null).ToList().ForEach(s => s.Character.IsWaitingForEvent = false);
                ServerManager.Instance.StartedEvents.Remove(EventType.METEORITEGAME);

                MeteoriteGameThread task = new MeteoriteGameThread();
                Observable.Timer(TimeSpan.FromSeconds(10)).Subscribe(X => task.Run(map));
            }
        }

        #endregion

        #region Classes

        public class MeteoriteGameThread
        {
            #region Members

            private MapInstance _map;

            #endregion

            #region Methods

            public void Run(MapInstance map)
            {
                _map = map;

                foreach (ClientSession session in _map.Sessions)
                {
                    ServerManager.Instance.TeleportOnRandomPlaceInMap(session, map.MapInstanceId);
                    if (session.Character.IsVehicled)
                    {
                        session.Character.RemoveVehicle();
                    }
                    if (session.Character.UseSp)
                    {
                        session.Character.LastSp = (DateTime.Now - Process.GetCurrentProcess().StartTime.AddSeconds(-50)).TotalSeconds;
                        ItemInstance specialist = session.Character.Inventory.LoadBySlotAndType((byte)EquipmentType.Sp, InventoryType.Wear);
                        if (specialist != null)
                        {
                            removeSP(session, specialist.ItemVNum);
                        }
                    }

                    session.Character.Speed = 8;
                    session.Character.IsVehicled = true;
                    session.Character.IsCustomSpeed = true;
                    session.Character.Morph = 1156;
                    session.Character.ArenaWinner = 0;
                    session.Character.MorphUpgrade = 0;
                    session.Character.MorphUpgrade2 = 0;
                    session.SendPacket(session.Character.GenerateCond());
                    session.Character.LastSpeedChange = DateTime.Now;
                    session.CurrentMapInstance?.Broadcast(session.Character.GenerateCMode());
                }

                int i = 0;

                while (_map?.Sessions?.Any() == true)
                {
                    runRound(i++);
                }

                //ended
            }

            private static IEnumerable<Tuple<short, int, short, short>> generateDrop(Map map, short vnum, int amountofdrop, int amount)
            {
                List<Tuple<short, int, short, short>> dropParameters = new List<Tuple<short, int, short, short>>();
                for (int i = 0; i < amountofdrop; i++)
                {
                    MapCell cell = map.GetRandomPosition();
                    dropParameters.Add(new Tuple<short, int, short, short>(vnum, amount, cell.X, cell.Y));
                }
                return dropParameters;
            }

            private static void removeSP(ClientSession session, short vnum)
            {
                if (session?.HasSession == true && !session.Character.IsVehicled)
                {
                    session.Character.DisableBuffs(BuffType.All);
                    session.Character.EquipmentBCards.RemoveAll(s => s.ItemVNum.Equals(vnum));
                    session.Character.UseSp = false;
                    session.Character.LoadSpeed();
                    session.SendPacket(session.Character.GenerateCond());
                    session.SendPacket(session.Character.GenerateLev());
                    session.Character.SpCooldown = 30;
                    if (session.Character.SkillsSp != null)
                    {
                        foreach (CharacterSkill ski in session.Character.SkillsSp.Where(s => !s.CanBeUsed()))
                        {
                            short time = ski.Skill.Cooldown;
                            double temp = (ski.LastUse - DateTime.Now).TotalMilliseconds + (time * 100);
                            temp /= 1000;
                            session.Character.SpCooldown = temp > session.Character.SpCooldown ? (int)temp : session.Character.SpCooldown;
                        }
                    }
                    session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("STAY_TIME"), session.Character.SpCooldown), 11));
                    session.SendPacket($"sd {session.Character.SpCooldown}");
                    session.CurrentMapInstance?.Broadcast(session.Character.GenerateCMode());
                    session.CurrentMapInstance?.Broadcast(UserInterfaceHelper.GenerateGuri(6, 1, session.Character.CharacterId), session.Character.PositionX, session.Character.PositionY);

                    // ms_c
                    session.SendPacket(session.Character.GenerateSki());
                    session.SendPackets(session.Character.GenerateQuicklist());
                    session.SendPacket(session.Character.GenerateStat());
                    session.SendPacket(session.Character.GenerateStatChar());

                    Logger.LogUserEvent("CHARACTER_SPECIALIST_RETURN", session.GenerateIdentity(), $"SpCooldown: {session.Character.SpCooldown}");

                    Observable.Timer(TimeSpan.FromMilliseconds(session.Character.SpCooldown * 1000)).Subscribe(o =>
                    {
                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("TRANSFORM_DISAPPEAR"), 11));
                        session.SendPacket("sd 0");
                    });
                }
            }

            private void runRound(int number)
            {
                int amount = 120 + (60 * number);

                int i = amount;
                while (i != 0)
                {
                    spawnCircle(number);
                    Thread.Sleep(60000 / amount);
                    i--;
                }
                Thread.Sleep(5000);
                _map.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("METEORITE_ROUND"), number + 1), 0));
                Thread.Sleep(5000);

                // Your dropped reward
                _map.DropItems(generateDrop(_map.Map, 1046, 20, 200 * ((number + 1) > 3 ? 3 : (number + 1))).ToList());
                _map.DropItems(generateDrop(_map.Map, 1030, 5, 3 * ((number + 1) > 3 ? 3 : (number + 1))).ToList());
                _map.DropItems(generateDrop(_map.Map, 2282, 5, 3 * ((number + 1) > 3 ? 3 : (number + 1))).ToList());
                _map.DropItems(generateDrop(_map.Map, 2514, 2, 1 * ((number + 1) > 3 ? 3 : (number + 1))).ToList());
                _map.DropItems(generateDrop(_map.Map, 2515, 2, 1 * ((number + 1) > 3 ? 3 : (number + 1))).ToList());
                _map.DropItems(generateDrop(_map.Map, 2516, 2, 1 * ((number + 1) > 3 ? 3 : (number + 1))).ToList());
                _map.DropItems(generateDrop(_map.Map, 2517, 2, 1 * ((number + 1) > 3 ? 3 : (number + 1))).ToList());
                _map.DropItems(generateDrop(_map.Map, 2518, 2, 1 * ((number + 1) > 3 ? 3 : (number + 1))).ToList());
                _map.DropItems(generateDrop(_map.Map, 2519, 2, 1 * ((number + 1) > 3 ? 3 : (number + 1))).ToList());
                _map.DropItems(generateDrop(_map.Map, 2520, 2, 1 * ((number + 1) > 3 ? 3 : (number + 1))).ToList());
                _map.DropItems(generateDrop(_map.Map, 2521, 2, 1 * ((number + 1) > 3 ? 3 : (number + 1))).ToList());
                foreach (ClientSession session in _map.Sessions)
                {
                    // Your reward that every player should get
                }

                Thread.Sleep(30000);
            }

            private void spawnCircle(int round)
            {
                if (_map != null)
                {
                    MapCell cell = _map.Map.GetRandomPosition();

                    int circleId = _map.GetNextMonsterId();

                    MapMonster circle = new MapMonster { MonsterVNum = 2018, MapX = cell.X, MapY = cell.Y, MapMonsterId = circleId, IsHostile = false, IsMoving = false, ShouldRespawn = false };
                    circle.Initialize(_map);
                    circle.NoAggresiveIcon = true;
                    _map.AddMonster(circle);
                    _map.Broadcast(circle.GenerateIn());
                    _map.Broadcast(StaticPacketHelper.GenerateEff(UserType.Monster, circleId, 4675));
                    Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(observer =>
                    {
                        if (_map != null)
                        {
                            _map.Broadcast(StaticPacketHelper.SkillUsed(UserType.Monster, circleId, 3, circleId, 1220, 220, 0, 4983, cell.X, cell.Y, true, 0, 65535, 0, 0));
                            foreach (Character character in _map.GetCharactersInRange(cell.X, cell.Y, 2))
                            {
                                if (!_map.Sessions.Skip(3).Any())
                                {
                                    // Your reward for the last three living players
                                }
                                character.IsCustomSpeed = false;
                                character.RemoveVehicle();
                                character.GetDamage(655350);
                                Observable.Timer(TimeSpan.FromMilliseconds(1000)).Subscribe(o => ServerManager.Instance.AskRevive(character.CharacterId));
                            }
                            _map.RemoveMonster(circle);
                            _map.Broadcast(StaticPacketHelper.Out(UserType.Monster, circle.MapMonsterId));
                        }
                    });
                }
            }

            #endregion
        }

        #endregion
    }
}