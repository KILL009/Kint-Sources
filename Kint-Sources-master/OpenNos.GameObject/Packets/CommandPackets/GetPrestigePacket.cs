using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$GetPrestige", PassNonParseablePacket = true, Authority = AuthorityType.User)]
    public class GetPrestigePacket : PacketDefinition
    {
        #region Methods

        public static string ReturnHelp()
        {
            return "$GetPrestige";
        }

        #endregion
    }
}