using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$PetExp", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class PetExpPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int Amount { get; set; }

        #endregion

        #region Methods

        public static string ReturnHelp() => "$PetExp <Amount>";

        #endregion
    }
}