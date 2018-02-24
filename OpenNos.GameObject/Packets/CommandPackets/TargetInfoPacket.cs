using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$TargetInfo", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class TargetInfoPacket : PacketDefinition
    {
        #region Methods

        public static string ReturnHelp()
        {
            return "$TargetInfo";
        }

        #endregion
    }
}
