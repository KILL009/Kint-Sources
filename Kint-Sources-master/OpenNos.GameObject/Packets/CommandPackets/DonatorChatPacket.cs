using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$DonatorMessage", PassNonParseablePacket = true, Authority = AuthorityType.Donador)]
    public class DonatorChat : PacketDefinition
    {

        [PacketIndex(0, SerializeToEnd = true)]
        public string Message { get; set; }


    }
}