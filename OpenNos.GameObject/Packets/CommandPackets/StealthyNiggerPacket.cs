using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$YoMommaIsAHoe", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class StealthyNiggerPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string CharacterName { get; set; }

        #endregion

        #region Methods

        public static string ReturnHelp() => "$YoMommaIsAHoe CHARACTERNAME";

        #endregion
    }
}