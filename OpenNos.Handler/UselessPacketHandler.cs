using OpenNos.Core;
using OpenNos.GameObject;

namespace OpenNos.Handler
{
    public class UselessPacketHandler : IPacketHandler
    {
        #region Members

        private readonly ClientSession _session;

        #endregion

        #region Instantiation

        public UselessPacketHandler(ClientSession session)
        {
            _session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session
        {
            get
            {
                return _session;
            }
        }

        #endregion

        #region Methods

        public void CClose(CClosePacket packet)
        {
            // idk
        }

        public void FStashEnd(FStashEndPacket packet)
        {
            // idk
        }

        public void FStashEnd(StashEndPacket packet)
        {
            // idk
        }

        public void Lbs(LbsPacket packet)
        {
            // idk
        }

        public void ShopClose(ShopClosePacket packet)
        {
            // Not needed for now.
        }

        public void Snap(SnapPacket packet)
        {
            // Not needed for now. (pictures)
        }

        #endregion
    }
}