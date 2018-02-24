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
using System.Configuration;
using OpenNos.ChatLog.Shared;
using Hik.Communication.ScsServices.Client;
using Hik.Communication.Scs.Communication;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using OpenNos.Core;
using System.Collections.Generic;

namespace OpenNos.ChatLog.Networking
{
    public class ChatLogServiceClient : IChatLogService
    {
        #region Members

        private static ChatLogServiceClient _instance;

        private readonly IScsServiceClient<IChatLogService> _client;

        #endregion

        #region Instantiation

        public ChatLogServiceClient()
        {
            string ip = ConfigurationManager.AppSettings["ChatLogIP"];
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["ChatLogPort"]);
            _client = ScsServiceClientBuilder.CreateClient<IChatLogService>(new ScsTcpEndPoint(ip, port));
            System.Threading.Thread.Sleep(1000);
            while (_client.CommunicationState != CommunicationStates.Connected)
            {
                try
                {
                    _client.Connect();
                }
                catch (Exception)
                {
                    Logger.Error(Language.Instance.GetMessageFromKey("RETRY_CONNECTION"), memberName: nameof(ChatLogServiceClient));
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        #endregion

        #region Properties

        public static ChatLogServiceClient Instance => _instance ?? (_instance = new ChatLogServiceClient());

        public CommunicationStates CommunicationState => _client.CommunicationState;

        #endregion

        #region Methods

        public void LogChatMessage(ChatLogEntry logEntry) => _client.ServiceProxy.LogChatMessage(logEntry);

        public bool AuthenticateAdmin(string user, string passHash) => _client.ServiceProxy.AuthenticateAdmin(user, passHash);

        public List<ChatLogEntry> GetChatLogEntries(string sender, long? senderid, string receiver, long? receiverid, string message, DateTime? start, DateTime? end, ChatLogType? logType) => _client.ServiceProxy.GetChatLogEntries(sender, senderid, receiver, receiverid, message, start, end, logType);

        public bool Authenticate(string authKey) => _client.ServiceProxy.Authenticate(authKey);

        #endregion
    }
}