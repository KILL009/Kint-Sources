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
    [Serializable]
    public class SerializableWorldServer
    {
        #region Instantiation

        public SerializableWorldServer(Guid id, string epIP, int epPort, int accountLimit, string worldGroup)
        {
            Id = id;
            EndPointIP = epIP;
            EndPointPort = epPort;
            AccountLimit = accountLimit;
            WorldGroup = worldGroup;
        }

        #endregion

        #region Properties

        public int AccountLimit { get; set; }

        public int ChannelId { get; set; }

        public string EndPointIP { get; set; }

        public int EndPointPort { get; set; }

        public Guid Id { get; set; }

        public string WorldGroup { get; set; }

        #endregion
    }
}