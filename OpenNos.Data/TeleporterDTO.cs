using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class TeleporterDTO : MappingBaseDTO
    {
        #region Properties

        public short Index { get; set; }

        public short MapId { get; set; }

        public int MapNpcId { get; set; }

        public short MapX { get; set; }

        public short MapY { get; set; }

        [Key]
        public short TeleporterId { get; set; }

        #endregion
    }
}