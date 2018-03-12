using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class ShopItemDTO : MappingBaseDTO
    {
        #region Properties

        public byte Color { get; set; }

        public short ItemVNum { get; set; }

        public sbyte Rare { get; set; }

        public int ShopId { get; set; }

        [Key]
        public int ShopItemId { get; set; }

        public byte Slot { get; set; }

        public byte Type { get; set; }

        public byte Upgrade { get; set; }

        #endregion
    }
}