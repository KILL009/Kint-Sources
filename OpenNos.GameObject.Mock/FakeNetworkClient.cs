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

        private long clientId;
        private ClientSession clientSession;
        private long lastKeepAliveIdentitiy;

        #endregion

        #region Instantiation

        public FakeNetworkClient()
        {
            clientId = 0;
            SentPackets = new Queue<string>();
            ReceivedPackets = new Queue<string>();
            lastKeepAliveIdentitiy = 1;
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
                if (clientId == 0)
                {
                    clientId = GameObjectMockHelper.Instance.GetNextClientId();
                }

                return clientId;
            }

            set
            {
                clientId = value;
            }
        }

        public string IpAddress => "127.0.0.1";

        public bool IsConnected { get; private set; }

        public bool IsDisposing { get; set; }

        public Queue<string> ReceivedPackets { get; }

        public Queue<string> SentPackets { get; }

        public ClientSession Session => GetClientSession();

        #endregion

        #region Methods

        public async Task ClearLowPriorityQueue() => await Task.CompletedTask;

        public void Disconnect() => IsConnected = false;

        public ClientSession GetClientSession() => clientSession;

        public void Initialize(EncryptionBase encryptor)
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
            var encoding = new UTF8Encoding();
            byte[] buf = encoding.GetBytes($"{lastKeepAliveIdentitiy} {packet}");
            MessageReceived?.Invoke(this, new MessageEventArgs(new ScsRawDataMessage(buf), DateTime.Now));
            lastKeepAliveIdentitiy++;
        }

        /// <summary>
        /// Send a packet to the Server as the Fake client receives it and triggers a Handler method.
        /// </summary>
        /// <param name="packet">Packet created thru PacketFactory.</param>
        public void ReceivePacket(PacketDefinition packet)
        {
            ReceivePacket(PacketFactory.Serialize(packet));
        }

        public void SendPacket(string packet, byte priority = 10) => SentPackets.Enqueue(packet);

        public void SendPacketFormat(string packet, params object[] param)
        {
            SentPackets.Enqueue(string.Format(packet, param));
        }

        public void SendPackets(IEnumerable<string> packets, byte priority = 10)
        {
            foreach (string packet in packets)
                SendPacket(packet, priority);
        }

        public void SetClientSession(object clientSession)
        {
            this.clientSession = (ClientSession)clientSession;
        }

        #endregion
    }
}