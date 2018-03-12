using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject
{
    [PacketHeader("suctl")]
    public class SuctlPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int CastId { get; set; }

        [PacketIndex(2)]
        public int MateTransportId { get; set; }

        [PacketIndex(4)]
        public int TargetId { get; set; }

        [PacketIndex(3)]
        public UserType TargetType { get; set; }

        [PacketIndex(1)]
        public int Unknown2 { get; set; }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"{CastId} {Unknown2} {MateTransportId} {TargetType} {TargetId}";
        }

        #endregion
    }
}