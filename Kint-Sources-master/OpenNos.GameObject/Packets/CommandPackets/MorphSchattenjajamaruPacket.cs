using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$MorphToSchattenjajamaru", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class MorphSchattenjajamaruPacket : PacketDefinition
    {
        [PacketIndex(0, SerializeToEnd = true)]
        public string Contents { get; set; }
    }
}