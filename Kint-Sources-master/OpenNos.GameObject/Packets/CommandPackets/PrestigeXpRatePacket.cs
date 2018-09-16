using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$PrestigeXpRate", PassNonParseablePacket = true, Authority = AuthorityType.Operator)]
    public class PrestigeXpRatePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int Value { get; set; }

        public static string ReturnHelp()
        {
            return "$PrestigeXpRate VALUE";
        }

        #endregion
    }
}