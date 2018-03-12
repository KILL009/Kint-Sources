using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class RecipeItemDTO : MappingBaseDTO
    {
        #region Properties

        public short Amount { get; set; }

        public short ItemVNum { get; set; }

        public short RecipeId { get; set; }

        [Key]
        public short RecipeItemId { get; set; }

        #endregion
    }
}