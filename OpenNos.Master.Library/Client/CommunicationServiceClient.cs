using Hik.Communication.Scs.Communication;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Client;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Master.Library.Data;
using OpenNos.Master.Library.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace OpenNos.Master.Library.Client
{
    public class CommunicationServiceClient : ICommunicationService
    {
        #region Members

        private static CommunicationServiceClient instance;
        private IScsServiceClient<ICommunicationService> client;
        private CommunicationClient commClient;

        #endregion

        #region Instantiation

        public CommunicationServiceClient()
        {
            var ip = ConfigurationManager.AppSettings["MasterIP"];
            var port = Convert.ToInt32(ConfigurationManager.AppSettings["MasterPort"]);
            commClient = new CommunicationClient();
            client = ScsServiceClientBuilder.CreateClient<ICommunicationService>(new ScsTcpEndPoint(ip, port), commClient);
            while (client.CommunicationState != CommunicationStates.Connected)
            {
                try
                {
                    client.Connect();
                }
                catch
                {
                    Logger.Log.Error(Language.Instance.GetMessageFromKey("RETRY_CONNECTION"));
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }

        #endregion

        #region Events

        public event EventHandler BazaarRefresh;

        public event EventHandler CharacterConnectedEvent;

        public event EventHandler CharacterDisconnectedEvent;

        public event EventHandler FamilyRefresh;

        public event EventHandler MailSent;

        public event EventHandler MessageSentToCharacter;

        public event EventHandler PenaltyLogRefresh;

        public event EventHandler RelationRefresh;

        public event EventHandler SessionKickedEvent;

        public event EventHandler ShutdownEvent;

        #endregion

        #region Properties

        public static CommunicationServiceClient Instance => instance ?? (instance = new CommunicationServiceClient());

        public CommunicationStates CommunicationState => client.CommunicationState;

        #endregion

        #region Methods

        public bool Authenticate(string authKey) => client.ServiceProxy.Authenticate(authKey);

        public void Cleanup() => client.ServiceProxy.Cleanup();

        public bool ConnectAccount(Guid worldId, long accountId, long sessionId)
        {
            return client.ServiceProxy.ConnectAccount(worldId, accountId, sessionId);
        }

        public bool ConnectAccountInternal(Guid worldId, long accountId, int sessionId)
        {
            return client.ServiceProxy.ConnectAccountInternal(worldId, accountId, sessionId);
        }

        public bool ConnectCharacter(Guid worldId, long characterId)
        {
            return client.ServiceProxy.ConnectCharacter(worldId, characterId);
        }

        public void DisconnectAccount(long accountId)
        {
            client.ServiceProxy.DisconnectAccount(accountId);
        }

        public void DisconnectCharacter(Guid worldId, long characterId)
        {
            client.ServiceProxy.DisconnectCharacter(worldId, characterId);
        }

        public SerializableWorldServer GetAct4ChannelInfo(string worldGroup)
        {
            return client.ServiceProxy.GetAct4ChannelInfo(worldGroup);
        }

        public int? GetChannelIdByWorldId(Guid worldId)
        {
            return client.ServiceProxy.GetChannelIdByWorldId(worldId);
        }

        public SerializableWorldServer GetPreviousChannelByAccountId(long accountId)
        {
            return client.ServiceProxy.GetPreviousChannelByAccountId(accountId);
        }

        public bool IsAccountConnected(long accountId)
        {
            return client.ServiceProxy.IsAccountConnected(accountId);
        }

        public bool IsCharacterConnected(string worldGroup, long characterId)
        {
            return client.ServiceProxy.IsCharacterConnected(worldGroup, characterId);
        }

        public bool IsCrossServerLoginPermitted(long accountId, int sessionId)
        {
            return client.ServiceProxy.IsCrossServerLoginPermitted(accountId, sessionId);
        }

        public bool IsLoginPermitted(long accountId, long sessionId)
        {
            return client.ServiceProxy.IsLoginPermitted(accountId, sessionId);
        }

        public void KickSession(long? accountId, long? sessionId)
        {
            client.ServiceProxy.KickSession(accountId, sessionId);
        }

        public void PulseAccount(long accountId) => client.ServiceProxy.PulseAccount(accountId);

        public void RefreshPenalty(int penaltyId) => client.ServiceProxy.RefreshPenalty(penaltyId);

        public void RegisterAccountLogin(long accountId, long sessionId, string accountName)
        {
            client.ServiceProxy.RegisterAccountLogin(accountId, sessionId, accountName);
        }

        public void RegisterInternalAccountLogin(long accountId, int sessionId)
        {
            client.ServiceProxy.RegisterInternalAccountLogin(accountId, sessionId);
        }

        public int? RegisterWorldServer(SerializableWorldServer worldServer)
        {
            return client.ServiceProxy.RegisterWorldServer(worldServer);
        }

        public string RetrieveRegisteredWorldServers(long sessionId)
        {
            return client.ServiceProxy.RetrieveRegisteredWorldServers(sessionId);
        }

        public IEnumerable<string> RetrieveServerStatistics()
        {
            return client.ServiceProxy.RetrieveServerStatistics();
        }

        public void SendMail(string worldGroup, MailDTO mail)
        {
            client.ServiceProxy.SendMail(worldGroup, mail);
        }

        public int? SendMessageToCharacter(SCSCharacterMessage message)
        {
            return client.ServiceProxy.SendMessageToCharacter(message);
        }

        public void Shutdown(string worldGroup) => client.ServiceProxy.Shutdown(worldGroup);

        public void UnregisterWorldServer(Guid worldId)
        {
            client.ServiceProxy.UnregisterWorldServer(worldId);
        }

        public void UpdateBazaar(string worldGroup, long bazaarItemId)
        {
            client.ServiceProxy.UpdateBazaar(worldGroup, bazaarItemId);
        }

        public void UpdateFamily(string worldGroup, long familyId)
        {
            client.ServiceProxy.UpdateFamily(worldGroup, familyId);
        }

        public void UpdateRelation(string worldGroup, long relationId)
        {
            client.ServiceProxy.UpdateRelation(worldGroup, relationId);
        }

        internal void OnCharacterConnected(long characterId)
        {
            var characterName = DAOFactory.CharacterDAO.FirstOrDefault(s => s.CharacterId == characterId)?.Name;
            CharacterConnectedEvent?.Invoke(new Tuple<long, string>(characterId, characterName), null);
        }

        internal void OnCharacterDisconnected(long characterId)
        {
            var characterName = DAOFactory.CharacterDAO.FirstOrDefault(s => s.CharacterId == characterId)?.Name;
            CharacterDisconnectedEvent?.Invoke(new Tuple<long, string>(characterId, characterName), null);
        }

        internal void OnKickSession(long? accountId, long? sessionId)
        {
            SessionKickedEvent?.Invoke(new Tuple<long?, long?>(accountId, sessionId), null);
        }

        internal void OnSendMail(MailDTO mail) => MailSent?.Invoke(mail, null);

        internal void OnSendMessageToCharacter(SCSCharacterMessage message)
        {
            MessageSentToCharacter?.Invoke(message, null);
        }

        internal void OnShutdown() => ShutdownEvent?.Invoke(null, null);

        internal void OnUpdateBazaar(long bazaarItemId) => BazaarRefresh?.Invoke(bazaarItemId, null);

        internal void OnUpdateFamily(long familyId) => FamilyRefresh?.Invoke(familyId, null);

        internal void OnUpdatePenaltyLog(int penaltyLogId)
        {
            PenaltyLogRefresh?.Invoke(penaltyLogId, null);
        }

        internal void OnUpdateRelation(long relationId) => RelationRefresh?.Invoke(relationId, null);

        #endregion
    }
}