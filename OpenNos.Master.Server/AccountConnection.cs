using System;

namespace OpenNos.Master.Library.Data
{
    internal class AccountConnection
    {
        #region Instantiation

        public AccountConnection(long accountId, long session, string accountName)
        {
            AccountId = accountId;
            SessionId = session;
            LastPulse = DateTime.Now;
            AccountName = accountName;
        }

        #endregion

        #region Properties

        public long AccountId { get; private set; }

        public string AccountName { get; private set; }

        public bool CanSwitchChannel { get; set; }

        public long CharacterId { get; set; }

        public WorldServer ConnectedWorld { get; set; }

        public DateTime LastPulse { get; set; }

        public WorldServer PreviousChannel { get; set; }

        public long SessionId { get; private set; }

        #endregion
    }
}