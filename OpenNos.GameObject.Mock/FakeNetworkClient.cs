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
using OpenNos.Core.Networking.Communication.Scs.Communication.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.GameObject.Mock
{
    public class FakeNetworkClient : INetworkClient
    {
        #region Members

        private long _clientId;

        private ClientSession _clientSession;

        private long _lastPacketId;

        #endregion

        #region Instantiation

        public FakeNetworkClient()
        {
            _clientId = 0;
            SentPackets = new Queue<string>();
            ReceivedPackets = new Queue<string>();
            _lastPacketId = 1;
            IsConnected = true;
        }

        #endregion

        #region Events

        public event EventHandler<MessageEventArgs> MessageReceived;

        #endregion

        #region Properties

        public long ClientId
        {
            get
            {
                if (_clientId == 0)
                {
                    _clientId = GameObjectMockHelper.Instance.GetNextClientId();
                }

                return _clientId;
            }

            set => _clientId = value;
        }

        public string IpAddress => "127.0.0.1";

        public bool IsConnected { get; private set; }

        public bool IsDisposing { get; set; }

        public Queue<string> ReceivedPackets { get; }

        public Queue<string> SentPackets { get; }

        public ClientSession Session => GetClientSession();

        #endregion

        #region Methods

        public async Task ClearLowPriorityQueueAsync() => await Task.CompletedTask;

        public void Disconnect() => IsConnected = false;

        public ClientSession GetClientSession() => _clientSession;

        public void Initialize(CryptographyBase encryptor)
        {
            // nothing to do here
        }

        /// <summary>
        /// Send a Packet to the Server as the Fake client receives it and triggers a Handler method.
        /// </summary>
        /// <param name="packet"></param>
        public void ReceivePacket(string packet)
        {
            Debug.WriteLine($"Enqueued {packet}");
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] buf = encoding.GetBytes($"{_lastPacketId} {packet}");
            MessageReceived?.Invoke(this, new MessageEventArgs(new ScsRawDataMessage(buf), DateTime.Now));
            _lastPacketId++;
        }

        /// <summary>
        /// Send a packet to the Server as the Fake client receives it and triggers a Handler method.
        /// </summary>
        /// <param name="packet">Packet created thru PacketFactory.</param>
        public void ReceivePacket(PacketDefinition packet) => ReceivePacket(PacketFactory.Serialize(packet));

        public void SendPacket(string packet, byte priority = 10) => SentPackets.Enqueue(packet);

        public void SendPacketFormat(string packet, params object[] param) => SentPackets.Enqueue(string.Format(packet, param));

        public void SendPackets(IEnumerable<string> packets, byte priority = 10)
        {
            foreach (string packet in packets)
            {
                SendPacket(packet, priority);
            }
        }

        public void SetClientSession(object clientSession) => _clientSession = (ClientSession)clientSession;

        #endregion
    }
}