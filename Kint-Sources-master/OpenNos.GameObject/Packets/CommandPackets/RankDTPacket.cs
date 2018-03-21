
using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$RankDT", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class RankDTPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string CharacterName { get; set; }

        #endregion

        #region Methods

        public static string ReturnHelp() => "$RankDT CHARACTERNAME";

        #endregion
    }
}