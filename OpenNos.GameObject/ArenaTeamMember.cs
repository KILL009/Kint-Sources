using OpenNos.Domain;
using System;

namespace OpenNos.GameObject
{
    public class ArenaTeamMember
    {
        #region Instantiation

        public ArenaTeamMember(ClientSession session, ArenaTeamType arenaTeamType, byte? order)
        {
            Session = session;
            ArenaTeamType = arenaTeamType;
            Order = order;
        }

        #endregion

        #region Properties

        public ArenaTeamType ArenaTeamType { get; set; }

        public bool Dead { get; set; }

        public DateTime? LastSummoned { get; set; }

        public byte? Order { get; set; }

        public ClientSession Session { get; set; }

        public byte SummonCount { get; set; }

        #endregion
    }
}