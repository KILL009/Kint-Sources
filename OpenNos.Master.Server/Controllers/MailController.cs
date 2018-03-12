using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Server.Controllers.ControllersParam;
using System;
using System.Web.Http;

namespace OpenNos.Master.Server.Controllers
{
    public class MailController : ApiController
    {
        #region Methods

        // POST /mail
        [AuthorizeRole(AuthorityType.GameMaster)]
        public void Post([FromBody]MailPostParameter mail)
        {
            var mail2 = new MailDTO
            {
                AttachmentAmount = mail.Amount,
                IsOpened = false,
                Date = DateTime.Now,
                ReceiverId = mail.CharacterId,
                SenderId = mail.CharacterId,
                AttachmentRarity = (byte)mail.Rare,
                AttachmentUpgrade = mail.Upgrade,
                IsSenderCopy = false,
                Title = mail.IsNosmall ? "NOSMALL" : mail.Title,
                AttachmentVNum = mail.VNum,
            };

            CommunicationServiceClient.Instance.SendMail(mail.WorldGroup, mail2);
        }

        #endregion
    }
}