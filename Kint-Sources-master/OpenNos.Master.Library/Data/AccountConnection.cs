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

using System;

namespace OpenNos.Master.Library.Data
{
    public class AccountConnection
    {
        public AccountConnection(long accountId, int sessionId, string ipAddress)
        {
            AccountId = accountId;
            SessionId = sessionId;
            IpAddress = ipAddress;
            LastPulse = DateTime.Now;
        }

        #region CharacterSession

        public class CharacterSession
        {
            public CharacterSession(string name, int level, string gender, string @class)
            {
                Name = name;
                Level = level;
                Gender = gender;
                Class = @class;
            }

            public string Name { get; }
            public int Level { get; }
            public string Gender { get; }
            public string Class { get; }
        }

        #endregion

        public long AccountId { get; }

        public bool CanLoginCrossServer { get; set; }

        public long CharacterId { get; set; }

        public WorldServer ConnectedWorld { get; set; }

        public string IpAddress { get; }

        public DateTime LastPulse { get; set; }

        public WorldServer OriginWorld { get; set; }

        public int SessionId { get; }

        public CharacterSession Character { get; set; }
    }
}