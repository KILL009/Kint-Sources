using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$Prestige", PassNonParseablePacket = true, Authority = AuthorityType.User)]
    public class PrestigePacket : PacketDefinition
    {
        //You can add a Helper, but this isnt usefull at all.
    }
}