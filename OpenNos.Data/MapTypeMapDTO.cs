using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class MapTypeMapDTO : MappingBaseDTO
    {
        #region Properties

        [Key]
        public object[] Key => new object[] { MapId, MapTypeId };

        public short MapId { get; set; }

        public short MapTypeId { get; set; }

        #endregion
    }
}