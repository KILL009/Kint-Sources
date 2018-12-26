using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$DonatorShout", PassNonParseablePacket = true, Authority = AuthorityType.Donador)]
    public class DonatorShout : PacketDefinition
    {

        [PacketIndex(0, SerializeToEnd = true)]
        public string Message { get; set; }
    }
}