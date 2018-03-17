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
using OpenNos.DAL;
using OpenNos.Master.Library.Data;
using OpenNos.Master.Library.Interface;
using OpenNos.SCS.Communication.Scs.Communication;
using OpenNos.SCS.Communication.Scs.Communication.EndPoints.Tcp;
using OpenNos.SCS.Communication.ScsServices.Client;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace OpenNos.Master.Library.Client
{
    public class CommunicationServiceClient : ICommunicationService
    {
        #region Members

        private static CommunicationServiceClient _instance;

        private readonly IScsServiceClient<ICommunicationService> _client;

        private readonly CommunicationClient _commClient;

        #endregion

        #region Instantiation

        public CommunicationServiceClient()
        {
            string ip = ConfigurationManager.AppSettings["MasterIP"];
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["MasterPort"]);
            _commClient = new CommunicationClient();
            _client = ScsServiceClientBuilder.CreateClient<ICommunicationService>(new ScsTcpEndPoint(ip, port), _commClient);
            System.Threading.Thread.Sleep(1000);
            while (_client.CommunicationState != CommunicationStates.Connected)
            {
                try
                {
                    _client.Connect();
                }
                catch (Exception)
                {
                    Logger.Error(Language.Instance.GetMessageFromKey("RETRY_CONNECTION"), memberName: nameof(CommunicationServiceClient));
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        #endregion

        #region Events

        public event EventHandler BazaarRefresh;

        public event EventHandler CharacterConnectedEvent;

        public event EventHandler CharacterDisconnectedEvent;

        public event EventHandler FamilyRefresh;

        public event EventHandler GlobalEvent;

        public event EventHandler MessageSentToCharacter;

        public event EventHandler PenaltyLogRefresh;

        public event EventHandler RelationRefresh;

        public event EventHandler SessionKickedEvent;

        public event EventHandler ShutdownEvent;

        public event EventHandler RestartEvent;

        public event EventHandler StaticBonusRefresh;

        #endregion

        #region Properties

        public static CommunicationServiceClient Instance => _instance ?? (_instance = new CommunicationServiceClient());

        public CommunicationStates CommunicationState => _client.CommunicationState;

        #endregion

        #region Methods

        public bool Authenticate(string authKey) => _client.ServiceProxy.Authenticate(authKey);

        public void Cleanup() => _client.ServiceProxy.Cleanup();

        public void CleanupOutdatedSession() => _client.ServiceProxy.CleanupOutdatedSession();

        public bool ConnectAccount(Guid worldId, long accountId, int sessionId) => _client.ServiceProxy.ConnectAccount(worldId, accountId, sessionId);

        public bool ConnectAccountCrossServer(Guid worldId, long accountId, int sessionId) => _client.ServiceProxy.ConnectAccountCrossServer(worldId, accountId, sessionId);

        public bool ConnectCharacter(Guid worldId, long characterId) => _client.ServiceProxy.ConnectCharacter(worldId, characterId);

        public void DisconnectAccount(long accountId) => _client.ServiceProxy.DisconnectAccount(accountId);

        public void DisconnectCharacter(Guid worldId, long characterId) => _client.ServiceProxy.DisconnectCharacter(worldId, characterId);

        public int? GetChannelIdByWorldId(Guid worldId) => _client.ServiceProxy.GetChannelIdByWorldId(worldId);

        public bool IsAccountConnected(long accountId) => _client.ServiceProxy.IsAccountConnected(accountId);

        public bool IsCharacterConnected(string worldGroup, long characterId) => _client.ServiceProxy.IsCharacterConnected(worldGroup, characterId);

        public bool IsCrossServerLoginPermitted(long accountId, int sessionId) => _client.ServiceProxy.IsCrossServerLoginPermitted(accountId, sessionId);

        public bool IsLoginPermitted(long accountId, int sessionId) => _client.ServiceProxy.IsLoginPermitted(accountId, sessionId);

        public void KickSession(long? accountId, int? sessionId) => _client.ServiceProxy.KickSession(accountId, sessionId);

        public void PulseAccount(long accountId) => _client.ServiceProxy.PulseAccount(accountId);

        public void RefreshPenalty(int penaltyId) => _client.ServiceProxy.RefreshPenalty(penaltyId);

        public void RegisterAccountLogin(long accountId, int sessionId, string ipAddress) => _client.ServiceProxy.RegisterAccountLogin(accountId, sessionId, ipAddress);

        public void RegisterCrossServerAccountLogin(long accountId, int sessionId) => _client.ServiceProxy.RegisterCrossServerAccountLogin(accountId, sessionId);

        public int? RegisterWorldServer(SerializableWorldServer worldServer) => _client.ServiceProxy.RegisterWorldServer(worldServer);

        public void Restart(string worldGroup) => _client.ServiceProxy.Restart(worldGroup);

        public long[][] RetrieveOnlineCharacters(long characterId) => _client.ServiceProxy.RetrieveOnlineCharacters(characterId);

        public string RetrieveOriginWorld(long accountId) => _client.ServiceProxy.RetrieveOriginWorld(accountId);

        public string RetrieveRegisteredWorldServers(string username, int sessionId, bool ignoreUserName) => _client.ServiceProxy.RetrieveRegisteredWorldServers(username, sessionId, ignoreUserName);

        public IEnumerable<string> RetrieveServerStatistics() => _client.ServiceProxy.RetrieveServerStatistics();

        public void RunGlobalEvent(Domain.EventType eventType) => _client.ServiceProxy.RunGlobalEvent(eventType);

        public int? SendMessageToCharacter(SCSCharacterMessage message) => _client.ServiceProxy.SendMessageToCharacter(message);

        public void Shutdown(string worldGroup) => _client.ServiceProxy.Shutdown(worldGroup);

        public void UnregisterWorldServer(Guid worldId) => _client.ServiceProxy.UnregisterWorldServer(worldId);

        public void UpdateBazaar(string worldGroup, long bazaarItemId) => _client.ServiceProxy.UpdateBazaar(worldGroup, bazaarItemId);

        public void UpdateFamily(string worldGroup, long familyId) => _client.ServiceProxy.UpdateFamily(worldGroup, familyId);

        public void UpdateRelation(string worldGroup, long relationId) => _client.ServiceProxy.UpdateRelation(worldGroup, relationId);

        internal void OnCharacterConnected(long characterId)
        {
            string characterName = DAOFactory.CharacterDAO.LoadById(characterId)?.Name;
            CharacterConnectedEvent?.Invoke(new Tuple<long, string>(characterId, characterName), null);
        }

        internal void OnCharacterDisconnected(long characterId)
        {
            string characterName = DAOFactory.CharacterDAO.LoadById(characterId)?.Name;
            CharacterDisconnectedEvent?.Invoke(new Tuple<long, string>(characterId, characterName), null);
        }

        internal void OnKickSession(long? accountId, int? sessionId) => SessionKickedEvent?.Invoke(new Tuple<long?, long?>(accountId, sessionId), null);

        internal void OnRunGlobalEvent(Domain.EventType eventType) => GlobalEvent?.Invoke(eventType, null);

        internal void OnSendMessageToCharacter(SCSCharacterMessage message) => MessageSentToCharacter?.Invoke(message, null);

        internal void OnShutdown() => ShutdownEvent?.Invoke(null, null);

        internal void OnRestart() => RestartEvent?.Invoke(null, null);

        internal void OnUpdateBazaar(long bazaarItemId) => BazaarRefresh?.Invoke(bazaarItemId, null);

        internal void OnUpdateFamily(long familyId) => FamilyRefresh?.Invoke(familyId, null);

        internal void OnUpdatePenaltyLog(int penaltyLogId) => PenaltyLogRefresh?.Invoke(penaltyLogId, null);

        internal void OnUpdateRelation(long relationId) => RelationRefresh?.Invoke(relationId, null);

        internal void OnUpdateStaticBonus(long characterId) => StaticBonusRefresh?.Invoke(characterId, null);

        #endregion
    }
}