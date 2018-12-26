using OpenNos.Core;
using OpenNos.Domain;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.Packets.ClientPackets
{
    [PacketHeader("Char_NEW_JOB", PassNonParseablePacket = true)]
    public class CharNewJobPacket : PacketDefinition
    {
        [PacketIndex(0)]
        [StringLength(15, MinimumLength = 3)]
        public string Name { get; set; }
        [PacketIndex(1)]
        [Range(0, 3)]
        public byte Slot { get; set; }
        [PacketIndex(2)]
        public GenderType Gender { get; set; }
        [PacketIndex(3)]
        public HairStyleType HairStyle { get; set; }
        [PacketIndex(4)]
        public HairColorType HairColor { get; set; }
    }
}