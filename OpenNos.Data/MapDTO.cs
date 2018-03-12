using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class MapDTO : MappingBaseDTO, IMapDTO
    {
        #region Properties

        public byte[] Data { get; set; }

        [Key]
        public short MapId { get; set; }

        public int Music { get; set; }

        public string Name { get; set; }

        public bool ShopAllowed { get; set; }

        #endregion
    }
}