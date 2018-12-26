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
using OpenNos.GameObject.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenNos.Data;

namespace OpenNos.GameObject
{
    public class Group : IDisposable
    {
        #region Members

        private readonly object _syncObj = new object();
        private bool _disposed;
        private int _order;

        #endregion

        #region Instantiation

        public Group()
        {
            Characters = new ThreadSafeGenericList<ClientSession>();
            GroupId = ServerManager.Instance.GetNextGroupId();
            _order = 0;
        }

        #endregion

        #region Properties

        public int CharacterCount => Characters.Count;

        public ThreadSafeGenericList<ClientSession> Characters { get; }

        public long GroupId { get; set; }

        public GroupType GroupType { get; set; }

        public ScriptedInstance Raid { get; set; }

        public byte SharingMode { get; set; }

        public TalentArenaBattle TalentArenaBattle { get; set; }

        #endregion

        #region Methods

        public void Dispose()
        {
            if (!_disposed)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
                _disposed = true;
            }
        }

        public List<string> GeneratePst(ClientSession player)
        {
            List<string> str = new List<string>();
            int i = 0;
            Characters.ForEach(session =>
            {
                if (session == player)
                {
                    str.AddRange(player.Character.Mates.Where(s => s.IsTeamMember).OrderByDescending(s => s.MateType).Select(mate => $"pst 2 {mate.MateTransportId} {(mate.MateType == MateType.Partner ? "0" : "1")} {mate.Hp / mate.MaxHp * 100} {mate.Mp / mate.MaxMp * 100} {mate.Hp} {mate.Mp} 0 0 0"));
                    i = session.Character.Mates.Count(s => s.IsTeamMember);
                    str.Add($"pst 1 {session.Character.CharacterId} {++i} {(int)(session.Character.Hp / session.Character.HPLoad() * 100)} {(int)(session.Character.Mp / session.Character.MPLoad() * 100)} {session.Character.HPLoad()} {session.Character.MPLoad()} {(byte)session.Character.Class} {(byte)session.Character.Gender} {(session.Character.UseSp ? session.Character.Morph : 0)}");
                }
                else
                {
                    str.Add($"pst 1 {session.Character.CharacterId} {++i} {(int)(session.Character.Hp / session.Character.HPLoad() * 100)} {(int)(session.Character.Mp / session.Character.MPLoad() * 100)} {session.Character.HPLoad()} {session.Character.MPLoad()} {(byte)session.Character.Class} {(byte)session.Character.Gender} {(session.Character.UseSp ? session.Character.Morph : 0)}{session.Character.Buff.GetAllItems().Aggregate(string.Empty, (current, buff) => current + $" {buff.Card.CardId}")}");
                }
            });
            return str;
        }

        public string GenerateRdlst()
        {
            string result = string.Empty;
            result = $"rdlst{((GroupType == GroupType.GiantTeam) ? "f" : string.Empty)} {Raid?.LevelMinimum ?? 1} {Raid?.LevelMaximum ?? 99} 0";
            try
            {
                Characters.ForEach(session => result += $" {session.Character.Level}.{(session.Character.UseSp || session.Character.IsVehicled ? session.Character.Morph : -1)}.{(short)session.Character.Class}.{Raid?.InstanceBag.DeadList.Count(s => s == session.Character.CharacterId) ?? 0}.{session.Character.Name}.{(short)session.Character.Gender}.{session.Character.CharacterId}.{session.Character.HeroLevel}.{session.Character.prestigeLevel}");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "GenerateRdlst");
            }
            return result;
        }

        public string GeneraterRaidmbf(ClientSession session) => $"raidmbf {session?.CurrentMapInstance?.InstanceBag?.MonsterLocker.Initial} {session?.CurrentMapInstance?.InstanceBag?.MonsterLocker.Current} {session?.CurrentMapInstance?.InstanceBag?.ButtonLocker.Initial} {session?.CurrentMapInstance?.InstanceBag?.ButtonLocker.Current} {Raid?.InstanceBag?.Lives - Raid?.InstanceBag?.DeadList.Count} {Raid?.InstanceBag?.Lives} 25";

        public long? GetNextOrderedCharacterId(Character character)
        {
            lock (_syncObj)
            {
                _order++;
                List<ClientSession> sessions = Characters.Where(s => Map.GetDistance(s.Character, character) < 50);
                if (_order > sessions.Count - 1) // if order wents out of amount of ppl, reset it -> zero based index
                {
                    _order = 0;
                }

                if (sessions.Count == 0) // group seems to be empty
                {
                    return null;
                }

                return sessions[_order].Character.CharacterId;
            }
        }

        public bool IsLeader(ClientSession session)
        {
            if (Characters.Count > 0)
            {
                return Characters.FirstOrDefault() == session;
            }
            else
            {
                return false;
            }
        }

        public bool IsMemberOfGroup(long characterId) => Characters?.Any(s => s?.Character?.CharacterId == characterId) == true;

        public bool IsMemberOfGroup(ClientSession session) => Characters?.Any(s => s?.Character?.CharacterId == session.Character.CharacterId) == true;

        public void JoinGroup(long characterId)
        {
            ClientSession session = ServerManager.Instance.GetSessionByCharacterId(characterId);
            if (session != null)
            {
                JoinGroup(session);
            }
        }

        public void JoinGroup(ClientSession session)
        {
            session.Character.Group = this;
            Characters.Add(session);
            if (GroupType == GroupType.Group)
            {
                if (Characters.Find(c => c.Character.IsCoupleOfCharacter(session.Character.CharacterId)) is ClientSession couple)
                {
                    session.Character.AddStaticBuff(new StaticBuffDTO { CardId = 319 });
                    couple.Character.AddStaticBuff(new StaticBuffDTO { CardId = 319 });
                }
            }
        }

        public void LeaveGroup(ClientSession session)
        {
            session.Character.Group = null;
            if (Characters.Find(c => c.Character.IsCoupleOfCharacter(session.Character.CharacterId)) is ClientSession couple)
            {
                session.Character.RemoveBuff(319);
                couple.Character.RemoveBuff(319);
            }
            if (IsLeader(session) && GroupType != GroupType.Group && Characters.Count > 1)
            {
                Characters.ForEach(s => s.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("TEAM_LEADER_CHANGE"), Characters.ElementAt(0).Character?.Name), 0)));
            }
            Characters.RemoveAll(s => s?.Character.CharacterId == session.Character.CharacterId);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Characters.Dispose();
            }
        }

        #endregion
    }
}