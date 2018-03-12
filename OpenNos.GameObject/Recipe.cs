using OpenNos.DAL;
using OpenNos.Data;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.GameObject
{
    public class Recipe : RecipeDTO
    {
        #region Properties

        public List<RecipeItemDTO> Items { get; set; }

        #endregion

        #region Methods

        public override void Initialize()
        {
            Items = new List<RecipeItemDTO>();
            foreach (RecipeItemDTO rec in DAOFactory.RecipeItemDAO.Where(s => s.RecipeId == RecipeId).ToList())
                Items.Add(rec);
        }

        #endregion
    }
}