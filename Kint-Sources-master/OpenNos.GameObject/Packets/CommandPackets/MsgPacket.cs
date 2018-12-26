using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$Msg", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class MsgPacket : PacketDefinition
    {

        [PacketIndex(0, SerializeToEnd = true)]
        public string Message { get; set; }
    }
}