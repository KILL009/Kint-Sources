using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class DropDTO : MappingBaseDTO
    {
        #region Properties

        public int Amount { get; set; }

        public int DropChance { get; set; }

        [Key]
        public short DropId { get; set; }

        public short ItemVNum { get; set; }

        public short? MapTypeId { get; set; }

        public short? MonsterVNum { get; set; }

        #endregion
    }
}