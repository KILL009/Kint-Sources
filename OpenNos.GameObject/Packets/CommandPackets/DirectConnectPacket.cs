using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject
{
    [PacketHeader("$DirectConnect", PassNonParseablePacket = true, Authority = AuthorityType.Operator)]
    public class DirectConnectPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string IPAddress { get; set; }

        [PacketIndex(1)]
        public int Port { get; set; }

        #endregion

        #region Methods

        public override string ToString() => $"DirectConnect Command IPAddress: {IPAddress} Port: {Port}";

        #endregion
    }
}