using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Packets.ClientPackets;
using OpenNos.Master.Library.Client;
using System;
using System.Configuration;

namespace OpenNos.Handler
{
    public class LoginPacketHandler : IPacketHandler
    {
        #region Members

        private readonly ClientSession session;

        #endregion

        #region Instantiation

        public LoginPacketHandler(ClientSession session)
        {
            this.session = session;
        }

        #endregion

        #region Methods

        public string BuildServersPacket(long accountId, int sessionId)
        {
            var channelpacket = CommunicationServiceClient.Instance.RetrieveRegisteredWorldServers(sessionId);

            if (channelpacket != null)
            {
                return channelpacket;
            }

            Logger.Log.Error("Could not retrieve Worldserver groups. Please make sure they've already been registered.");
            session.SendPacket($"failc {(byte)LoginFailType.Maintenance}");

            return null;
        }

        /// <summary>
        /// login packet
        /// </summary>
        /// <param name="loginPacket"></param>
        public void VerifyLogin(LoginPacket loginPacket)
        {
            if (loginPacket == null)
            {
                return;
            }

            var user = new UserDTO
            {
                Name = loginPacket.Name,
                Password = loginPacket.Password
            };

            var loadedAccount = DAOFactory.AccountDAO.FirstOrDefault(s => s.Name.Equals(user.Name));
            if (loadedAccount != null && loadedAccount.Password.ToUpper().Equals(user.Password))
            {
                // TODO LOG LOGIN

                // check if the account is connected
                if (!CommunicationServiceClient.Instance.IsAccountConnected(loadedAccount.AccountId))
                {
                    var type = loadedAccount.Authority;
                    var penalty = DAOFactory.PenaltyLogDAO.FirstOrDefault(s => s.AccountId.Equals(loadedAccount.AccountId) && s.DateEnd > DateTime.Now && s.Penalty == PenaltyType.Banned);
                    if (penalty != null)
                    {
                        session.SendPacket($"failc {(byte)LoginFailType.Banned}");
                    }
                    else
                    {
                        switch (type)
                        {
                            // TODO TO ENUM
                            case AuthorityType.Unconfirmed:
                                {
                                    session.SendPacket($"failc {(byte)LoginFailType.AccountOrPasswordWrong}");
                                }

                                break;

                            case AuthorityType.Banned:
                                session.SendPacket($"failc {(byte)LoginFailType.Banned}");

                                break;

                            case AuthorityType.Closed:
                                session.SendPacket($"failc {(byte)LoginFailType.CantConnect}");

                                break;

                            default:
                                {
                                    var newSessionId = SessionFactory.Instance.GenerateSessionId();
                                    Logger.Log.DebugFormat(Language.Instance.GetMessageFromKey("CONNECTION"), user.Name, newSessionId);

                                    // TODO MAINTENANCE MODE (MASTER SERVER) IF MAINTENANCE
                                    // _session.SendPacket($"failc 2"); inform communication service
                                    // about new player from login server
                                    try
                                    {
                                        CommunicationServiceClient.Instance.RegisterAccountLogin(loadedAccount.AccountId, newSessionId, loadedAccount.Name);
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Log.Error("General Error SessionId: " + newSessionId, ex);
                                    }

                                    session.SendPacket(BuildServersPacket(loadedAccount.AccountId, newSessionId));
                                }

                                break;
                        }
                    }
                }
                else
                {
                    session.SendPacket($"failc {(byte)LoginFailType.AlreadyConnected}");
                }
            }
            else
            {
                session.SendPacket($"failc {(byte)LoginFailType.AccountOrPasswordWrong}");
            }
        }

        #endregion
    }
}