using OpenNos.Data;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.GameObject
{
    public class Account : AccountDTO
    {
        #region Properties

        public List<PenaltyLogDTO> PenaltyLogs
        {
            get
            {
                PenaltyLogDTO[] logs = new PenaltyLogDTO[ServerManager.Instance.PenaltyLogs.Count + 10];
                ServerManager.Instance.PenaltyLogs.CopyTo(logs);
                return logs.Where(s => s != null && s.AccountId == AccountId).ToList();
            }
        }

        #endregion

        #region Methods

        public override void Initialize()
        {
        }

        #endregion
    }
}