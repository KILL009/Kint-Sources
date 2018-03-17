using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject
{
    [PacketHeader("$Act4Connect", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class Act4ConnectPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string Name { get; set; }

        #endregion

        #region Methods

        public override string ToString() => "Act4Connect Name";

        #endregion
    }
}