using OpenNos.Master.Library.Client;
using System.Web.Http;

namespace OpenNos.Master.Server.Controllers
{
    public class SessionController : ApiController
    {
        #region Methods

        // GET /stats
        public void Delete(long accountId)
        {
            CommunicationServiceClient.Instance.KickSession(accountId, null);
        }

        #endregion
    }
}