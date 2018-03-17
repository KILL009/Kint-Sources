using OpenNos.DAL.EF;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public class RecipeMapper
    {
        public RecipeMapper()
        {
        }

        public bool ToRecipeDTO(Recipe input, RecipeDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.Amount = input.Amount;
            output.ItemVNum = input.ItemVNum;
            output.RecipeId = input.RecipeId;
            return true;
        }

        public bool ToRecipe(RecipeDTO input, Recipe output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.Amount = input.Amount;
            output.ItemVNum = input.ItemVNum;
            output.RecipeId = input.RecipeId;
            return true;
        }
    }
}

