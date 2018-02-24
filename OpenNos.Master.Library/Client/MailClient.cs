using OpenNos.Data;
using OpenNos.Master.Library.Data;
using OpenNos.Master.Library.Interface;
using System.Threading.Tasks;

namespace OpenNos.Master.Library.Client
{
    internal class MailClient : IMailClient
    {
        #region Methods

        public void MailSent(MailDTO mail) => Task.Run(() => MailServiceClient.Instance.OnMailSent(mail));

        #endregion
    }
}