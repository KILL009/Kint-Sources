using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$Move", PassNonParseablePacket = true, Authority = AuthorityType.User)]
    public class UnstuckPacket : PacketDefinition
    {
        #region Methods

        public static string ReturnHelp() => "$Move";

        #endregion
    }
}