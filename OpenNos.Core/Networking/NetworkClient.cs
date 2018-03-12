using OpenNos.Core.Networking.Communication.Scs.Communication;
using OpenNos.Core.Networking.Communication.Scs.Communication.Channels;
using OpenNos.Core.Networking.Communication.Scs.Communication.Messages;
using OpenNos.Core.Networking.Communication.Scs.Server;
using System.Collections.Generic;

namespace OpenNos.Core
{
    public class NetworkClient : ScsServerClient, INetworkClient
    {
        #region Members

        private EncryptionBase encryptor;
        private object session;

        #endregion

        #region Instantiation

        public NetworkClient(ICommunicationChannel communicationChannel) : base(communicationChannel)
        {
        }

        #endregion

        #region Properties

        public string IpAddress => RemoteEndPoint.ToString();

        public bool IsConnected => CommunicationState == CommunicationStates.Connected;

        public bool IsDisposing { get; set; }

        #endregion

        #region Methods

        public void Initialize(EncryptionBase encryptor) => this.encryptor = encryptor;

        public void SendPacket(string packet, byte priority = 10)
        {
            if (!IsDisposing && packet != null && packet != string.Empty)
            {
                var rawMessage = new ScsRawDataMessage(encryptor.Encrypt(packet));
                SendMessage(rawMessage, priority);
            }
        }

        public void SendPacketFormat(string packet, params object[] param)
        {
            SendPacket(string.Format(packet, param));
        }

        public void SendPackets(IEnumerable<string> packets, byte priority = 10)
        {
            // TODO: maybe send at once with delimiter
            foreach (string packet in packets)
                SendPacket(packet, priority);
        }

        public void SetClientSession(object clientSession) => session = clientSession;

        #endregion
    }
}