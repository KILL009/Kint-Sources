using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class ComboDTO : MappingBaseDTO
    {
        #region Properties

        public short Animation { get; set; }

        [Key]
        public int ComboId { get; set; }

        public short Effect { get; set; }

        public short Hit { get; set; }

        public short SkillVNum { get; set; }

        #endregion
    }
}