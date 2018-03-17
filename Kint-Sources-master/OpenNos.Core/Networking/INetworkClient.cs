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

using OpenNos.Core.Networking.Communication.Scs.Communication.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenNos.Core
{
    public interface INetworkClient
    {
        #region Events

        event EventHandler<MessageEventArgs> MessageReceived;

        #endregion

        #region Properties

        long ClientId { get; set; }

        string IpAddress { get; }

        bool IsConnected { get; }

        bool IsDisposing { get; set; }

        #endregion

        #region Methods

        Task ClearLowPriorityQueueAsync();

        void Disconnect();

        void Initialize(CryptographyBase encryptor);

        void SendPacket(string packet, byte priority = 10);

        void SendPacketFormat(string packet, params object[] param);

        void SendPackets(IEnumerable<string> packets, byte priority = 10);

        void SetClientSession(object clientSession);

        #endregion
    }
}