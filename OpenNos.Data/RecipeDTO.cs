using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class RecipeDTO : MappingBaseDTO
    {
        #region Properties

        public byte Amount { get; set; }

        public short ItemVNum { get; set; }

        public int MapNpcId { get; set; }

        [Key]
        public short RecipeId { get; set; }

        #endregion
    }
}