using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject
{
    [PacketHeader("arena")]
    public class ArenaPacket : PacketDefinition
    {

        [PacketIndex(0)]
        public byte Answer1 { get; set; }

        [PacketIndex(1)]
        public byte Answer2 { get; set; }
    }
}
