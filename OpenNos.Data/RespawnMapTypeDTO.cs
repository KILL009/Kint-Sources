using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class RespawnMapTypeDTO : MappingBaseDTO
    {
        #region Properties

        public short DefaultMapId { get; set; }

        public short DefaultX { get; set; }

        public short DefaultY { get; set; }

        public string Name { get; set; }

        [Key]
        public long RespawnMapTypeId { get; set; }

        #endregion
    }
}