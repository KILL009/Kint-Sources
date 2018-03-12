using Hik.Communication.ScsServices.Service;
using OpenNos.Master.Library.Data;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace OpenNos.Master.Server
{
    internal class MSManager
    {
        #region Members

        private static MSManager instance;

        #endregion

        #region Instantiation

        public MSManager()
        {
            WorldServers = new List<WorldServer>();
            LoginServers = new List<IScsServiceClient>();
            ConnectedAccounts = new ConcurrentBag<AccountConnection>();
            AuthentificatedClients = new List<long>();
        }

        #endregion

        #region Properties

        public static MSManager Instance => instance ?? (instance = new MSManager());

        public List<long> AuthentificatedClients { get; set; }

        public ConcurrentBag<AccountConnection> ConnectedAccounts { get; set; }

        public List<IScsServiceClient> LoginServers { get; set; }

        public List<WorldServer> WorldServers { get; set; }

        #endregion
    }
}