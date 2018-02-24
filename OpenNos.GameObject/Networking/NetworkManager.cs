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
using OpenNos.Core.Networking.Communication.Scs.Communication.EndPoints.Tcp;
using OpenNos.Core.Networking.Communication.Scs.Server;
using System;
using OpenNos.GameObject.Networking;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.GameObject
{
    public class NetworkManager<EncryptorT> : SessionManager where EncryptorT : CryptographyBase
    {
        #region Members

        private readonly EncryptorT _encryptor;
        private readonly CryptographyBase _fallbackEncryptor;
        private readonly IScsServer _server;
        private IDictionary<string, DateTime> _connectionLog;

        #endregion

        #region Instantiation

        public NetworkManager(string ipAddress, int port, Type packetHandler, Type fallbackEncryptor, bool isWorldServer) : base(packetHandler, isWorldServer)
        {
            _encryptor = (EncryptorT)Activator.CreateInstance(typeof(EncryptorT));

            if (fallbackEncryptor != null)
            {
                _fallbackEncryptor = (CryptographyBase)Activator.CreateInstance(fallbackEncryptor);
            }

            _server = ScsServerFactory.CreateServer(new ScsTcpEndPoint(ipAddress, port));

            // Register events of the server to be informed about clients
            _server.ClientConnected += onServerClientConnected;
            _server.ClientDisconnected += onServerClientDisconnected;
            _server.WireProtocolFactory = new WireProtocolFactory<EncryptorT>();

            // Start the server
            _server.Start();

            Logger.Info(Language.Instance.GetMessageFromKey("STARTED"), memberName: "NetworkManager");
        }

        #endregion

        #region Properties

        private IDictionary<string, DateTime> ConnectionLog => _connectionLog ?? (_connectionLog = new Dictionary<string, DateTime>());

        #endregion

        #region Methods

        public override void StopServer()
        {
            _server.Stop();
            _server.ClientConnected -= onServerClientDisconnected;
            _server.ClientDisconnected -= onServerClientConnected;
        }

        protected override ClientSession IntializeNewSession(INetworkClient client)
        {
            if (!checkGeneralLog(client))
            {
                Logger.Warn(string.Format(Language.Instance.GetMessageFromKey("FORCED_DISCONNECT"), client.ClientId));
                client.Initialize(_fallbackEncryptor);
                client.SendPacket($"fail {Language.Instance.GetMessageFromKey("CONNECTION_LOST")}");
                client.Disconnect();
                return null;
            }

            ClientSession session = new ClientSession(client);
            session.Initialize(_encryptor, _packetHandler, IsWorldServer);

            return session;
        }

        private bool checkGeneralLog(INetworkClient client)
        {
            if (!client.IpAddress.Contains("127.0.0.1") && ServerManager.Instance.ChannelId != 51)
            {
                if (ConnectionLog.Count > 0)
                {
                    foreach (KeyValuePair<string, DateTime> item in ConnectionLog.Where(cl => cl.Key.Contains(client.IpAddress.Split(':')[1]) && (DateTime.Now - cl.Value).TotalSeconds > 3).ToList())
                    {
                        ConnectionLog.Remove(item.Key);
                    }
                }

                if (ConnectionLog.Any(c => c.Key.Contains(client.IpAddress.Split(':')[1])))
                {
                    return false;
                }
                ConnectionLog.Add(client.IpAddress, DateTime.Now);
                return true;
            }

            return true;
        }

        private void onServerClientConnected(object sender, ServerClientEventArgs e) => AddSession(e.Client as NetworkClient);

        private void onServerClientDisconnected(object sender, ServerClientEventArgs e) => RemoveSession(e.Client as NetworkClient);

        #endregion
    }
}