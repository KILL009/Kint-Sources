using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class MapTypeDTO : MappingBaseDTO
    {
        #region Properties

        [Key]
        public short MapTypeId { get; set; }

        public string MapTypeName { get; set; }

        public short PotionDelay { get; set; }

        public long? RespawnMapTypeId { get; set; }

        public long? ReturnMapTypeId { get; set; }

        #endregion
    }
}