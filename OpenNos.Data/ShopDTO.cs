using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class ShopDTO : MappingBaseDTO
    {
        #region Properties

        public int MapNpcId { get; set; }

        public byte MenuType { get; set; }

        public string Name { get; set; }

        [Key]
        public int ShopId { get; set; }

        public byte ShopType { get; set; }

        #endregion
    }
}