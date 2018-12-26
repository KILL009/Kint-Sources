using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$DeactivateZoom", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class DeactivateZoomPacket : PacketDefinition
    {
        public static string ReturnHelp()
        {
            return "$DeactivateZoom";
        }
    }
}