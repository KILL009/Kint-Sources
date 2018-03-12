using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class ShopSkillDTO : MappingBaseDTO
    {
        #region Properties

        public int ShopId { get; set; }

        [Key]
        public int ShopSkillId { get; set; }

        public short SkillVNum { get; set; }

        public byte Slot { get; set; }

        public byte Type { get; set; }

        #endregion
    }
}