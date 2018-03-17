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
using OpenNos.Data;
using OpenNos.Master.Library.Data;
using OpenNos.Master.Library.Interface;
using OpenNos.SCS.Communication.Scs.Communication;
using OpenNos.SCS.Communication.Scs.Communication.EndPoints.Tcp;
using OpenNos.SCS.Communication.ScsServices.Client;
using System;
using System.Configuration;

namespace OpenNos.Master.Library.Client
{
    public class MailServiceClient : IMailService
    {
        #region Members

        private static MailServiceClient _instance;

        private readonly IScsServiceClient<IMailService> _client;

        private readonly MailClient _mailClient;

        #endregion

        #region Instantiation

        public MailServiceClient()
        {
            string ip = ConfigurationManager.AppSettings["MasterIP"];
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["MasterPort"]);
            _mailClient = new MailClient();
            _client = ScsServiceClientBuilder.CreateClient<IMailService>(new ScsTcpEndPoint(ip, port), _mailClient);

            System.Threading.Thread.Sleep(1000);
            while (_client.CommunicationState != CommunicationStates.Connected)
            {
                try
                {
                    _client.Connect();
                }
                catch (Exception)
                {
                    Logger.Error(Language.Instance.GetMessageFromKey("RETRY_CONNECTION"), memberName: nameof(MailServiceClient));
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        #endregion

        #region Events

        public event EventHandler MailSent;

        #endregion

        #region Properties

        public static MailServiceClient Instance => _instance ?? (_instance = new MailServiceClient());

        public CommunicationStates CommunicationState => _client.CommunicationState;

        #endregion

        #region Methods

        public bool Authenticate(string authKey, Guid serverId) => _client.ServiceProxy.Authenticate(authKey, serverId);

        public void SendMail(MailDTO mail) => _client.ServiceProxy.SendMail(mail);

        internal void OnMailSent(MailDTO mail) => MailSent?.Invoke(mail, null);

        #endregion
    }
}