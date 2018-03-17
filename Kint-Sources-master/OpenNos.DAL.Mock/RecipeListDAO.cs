using OpenNos.DAL.Interface;
using OpenNos.Data;
using System;
using System.Collections.Generic;

namespace OpenNos.DAL.Mock
{
    public class RecipeListDAO : BaseDAO<RecipeListDTO>, IRecipeListDAO
    {
        #region Methods

        public RecipeListDTO LoadById(int recipeListId) => throw new NotImplementedException();

        public IEnumerable<RecipeListDTO> LoadByItemVNum(short itemVNum) => throw new NotImplementedException();

        public IEnumerable<RecipeListDTO> LoadByMapNpcId(int mapNpcId) => throw new NotImplementedException();

        public IEnumerable<RecipeListDTO> LoadByRecipeId(short recipeId) => throw new NotImplementedException();

        public void Update(RecipeListDTO recipe) => throw new NotImplementedException();

        #endregion
    }
}