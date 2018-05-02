using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$GetExp", PassNonParseablePacket = true, Authority = AuthorityType.User)]
    public class GetExpPacket : PacketDefinition
    {
        #region Methods

        public static string ReturnHelp()
        {
            return "$GetExp";
        }

        #endregion
    }
}