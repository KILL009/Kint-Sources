using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$RankDT", PassNonParseablePacket = true, Authority = AuthorityType.Operator)]
    public class RankDTPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string CharacterName { get; set; }

        public override string ToString()
        {
            return $"RankDT Command CharacterName: {CharacterName}";
        }

        #endregion
    }
}