using OpenNos.Core;
using OpenNos.Core.Networking.Communication.Scs.Communication.EndPoints.Tcp;
using OpenNos.Core.Networking.Communication.Scs.Server;
using OpenNos.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.GameObject
{
    public class NetworkManager<EncryptorT> : SessionManager
        where EncryptorT : EncryptionBase
    {
        #region Members

        private IDictionary<string, DateTime> connectionLog;
        private EncryptorT encryptor;
        private EncryptionBase fallbackEncryptor;
        private IScsServer server;

        #endregion

        #region Instantiation

        public NetworkManager(string ipAddress, int port, Type packetHandler, Type fallbackEncryptor, bool isWorldServer) : base(packetHandler, isWorldServer)
        {
            encryptor = (EncryptorT)Activator.CreateInstance(typeof(EncryptorT));

            if (fallbackEncryptor != null)
            {
                this.fallbackEncryptor = (EncryptionBase)Activator.CreateInstance(fallbackEncryptor); // reflection, TODO: optimize.
            }

            server = ScsServerFactory.CreateServer(new ScsTcpEndPoint(ipAddress, port));

            // Register events of the server to be informed about clients
            server.ClientConnected += OnServerClientConnected;
            server.ClientDisconnected += OnServerClientDisconnected;
            server.WireProtocolFactory = new WireProtocolFactory<EncryptorT>();

            // Start the server
            server.Start();

            Logger.Log.Info(Language.Instance.GetMessageFromKey("STARTED"));
        }

        #endregion

        #region Properties

        private IDictionary<string, DateTime> ConnectionLog
        {
            get
            {
                return connectionLog ?? (connectionLog = new Dictionary<string, DateTime>());
            }
        }

        #endregion

        #region Methods

        public override void StopServer()
        {
            server.Stop();
            server.ClientConnected -= OnServerClientDisconnected;
            server.ClientDisconnected -= OnServerClientConnected;
        }

        protected override ClientSession IntializeNewSession(INetworkClient client)
        {
            if (!CheckGeneralLog(client))
            {
                Logger.Log.WarnFormat(Language.Instance.GetMessageFromKey("FORCED_DISCONNECT"), client.ClientId);
                client.Initialize(fallbackEncryptor);
                client.SendPacket($"failc {(byte)LoginFailType.CantConnect}");
                client.Disconnect();
                return null;
            }

            var session = new ClientSession(client);
            session.Initialize(encryptor, _packetHandler, IsWorldServer);

            return session;
        }

        private bool CheckGeneralLog(INetworkClient client)
        {
            if (!client.IpAddress.Contains("127.0.0.1"))
            {
                if (ConnectionLog.Any())
                {
                    foreach (KeyValuePair<string, DateTime> item in ConnectionLog.Where(cl => cl.Key.Contains(client.IpAddress.Split(':')[1]) && (DateTime.Now - cl.Value).TotalSeconds > 3).ToList())
                        ConnectionLog.Remove(item.Key);
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

        private void OnServerClientConnected(object sender, ServerClientEventArgs e)
        {
            AddSession(e.Client as NetworkClient);
        }

        private void OnServerClientDisconnected(object sender, ServerClientEventArgs e)
        {
            RemoveSession(e.Client as NetworkClient);
        }

        #endregion
    }
}