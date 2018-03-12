using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class RespawnDTO : MappingBaseDTO
    {
        #region Properties

        public long CharacterId { get; set; }

        public short MapId { get; set; }

        [Key]
        public long RespawnId { get; set; }

        public long RespawnMapTypeId { get; set; }

        public short X { get; set; }

        public short Y { get; set; }

        #endregion
    }
}