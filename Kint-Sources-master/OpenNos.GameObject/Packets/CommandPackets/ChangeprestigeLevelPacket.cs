using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$PrestigeLvl", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ChangeprestigeLevelPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte prestigeLevel { get; set; }

        public static string ReturnHelp()
        {
            return "$PrestigeLvl PRESTIGELEVEL";
        }

        #endregion
    }
}