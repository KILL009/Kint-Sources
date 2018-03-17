using OpenNos.Data;
using OpenNos.Master.Library.Data;

namespace OpenNos.Master.Library.Interface
{
    public interface IMailClient
    {
        #region Methods

       
        void MailSent(MailDTO mail);

        #endregion
    }
}