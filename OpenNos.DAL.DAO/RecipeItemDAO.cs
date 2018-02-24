/*
 * This file is part of the OpenNos Emulator Project. See AUTHORS file for Copyright information
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 */

using OpenNos.Core;
using OpenNos.DAL.EF;

using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.DAL.DAO
{
    public class RecipeItemDAO : IRecipeItemDAO
    {
        #region Methods

        public RecipeItemDTO Insert(RecipeItemDTO recipeItem)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    RecipeItem entity = new RecipeItem();
                    Mapper.Mapper.Instance.RecipeItemMapper.ToRecipeItem(recipeItem, entity);
                    context.RecipeItem.Add(entity);
                    context.SaveChanges();
                    if(Mapper.Mapper.Instance.RecipeItemMapper.ToRecipeItemDTO(entity, recipeItem))
                    {
                        return recipeItem;
                    }

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<RecipeItemDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<RecipeItemDTO> result = new List<RecipeItemDTO>();
                foreach (RecipeItem recipeItem in context.RecipeItem)
                {
                    RecipeItemDTO dto = new RecipeItemDTO();
                    Mapper.Mapper.Instance.RecipeItemMapper.ToRecipeItemDTO(recipeItem, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public RecipeItemDTO LoadById(short recipeItemId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    RecipeItemDTO dto = new RecipeItemDTO();
                    if(Mapper.Mapper.Instance.RecipeItemMapper.ToRecipeItemDTO(context.RecipeItem.FirstOrDefault(s => s.RecipeItemId.Equals(recipeItemId)), dto))
                    {
                        return dto;
                    }

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<RecipeItemDTO> LoadByRecipe(short recipeId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<RecipeItemDTO> result = new List<RecipeItemDTO>();
                foreach (RecipeItem recipeItem in context.RecipeItem.Where(s => s.RecipeId.Equals(recipeId)))
                {
                    RecipeItemDTO dto = new RecipeItemDTO();
                    Mapper.Mapper.Instance.RecipeItemMapper.ToRecipeItemDTO(recipeItem, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public IEnumerable<RecipeItemDTO> LoadByRecipeAndItem(short recipeId, short itemVNum)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<RecipeItemDTO> result = new List<RecipeItemDTO>();
                foreach (RecipeItem recipeItem in context.RecipeItem.Where(s => s.ItemVNum.Equals(itemVNum) && s.RecipeId.Equals(recipeId)))
                {
                    RecipeItemDTO dto = new RecipeItemDTO();
                    Mapper.Mapper.Instance.RecipeItemMapper.ToRecipeItemDTO(recipeItem, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        #endregion
    }
}