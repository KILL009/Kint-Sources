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
    public class RecipeListDAO : IRecipeListDAO
    {
        #region Methods

        public RecipeListDTO Insert(RecipeListDTO recipeList)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    RecipeList entity = new RecipeList();
                    Mapper.Mapper.Instance.RecipeListMapper.ToRecipeList(recipeList, entity);
                    context.RecipeList.Add(entity);
                    context.SaveChanges();
                    if(Mapper.Mapper.Instance.RecipeListMapper.ToRecipeListDTO(entity, recipeList))
                    {
                        return recipeList;
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

        public IEnumerable<RecipeListDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<RecipeListDTO> result = new List<RecipeListDTO>();
                foreach (RecipeList recipeList in context.RecipeList)
                {
                    RecipeListDTO dto = new RecipeListDTO();
                    Mapper.Mapper.Instance.RecipeListMapper.ToRecipeListDTO(recipeList, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public RecipeListDTO LoadById(int recipeListId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    RecipeListDTO dto = new RecipeListDTO();
                    if(Mapper.Mapper.Instance.RecipeListMapper.ToRecipeListDTO(context.RecipeList.SingleOrDefault(s => s.RecipeListId.Equals(recipeListId)), dto))
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

        public IEnumerable<RecipeListDTO> LoadByItemVNum(short itemVNum)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<RecipeListDTO> result = new List<RecipeListDTO>();
                foreach (RecipeList recipeList in context.RecipeList.Where(r => r.ItemVNum == itemVNum))
                {
                    RecipeListDTO dto = new RecipeListDTO();
                    Mapper.Mapper.Instance.RecipeListMapper.ToRecipeListDTO(recipeList, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public IEnumerable<RecipeListDTO> LoadByMapNpcId(int mapNpcId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<RecipeListDTO> result = new List<RecipeListDTO>();
                foreach (RecipeList recipeList in context.RecipeList.Where(r => r.MapNpcId == mapNpcId))
                {
                    RecipeListDTO dto = new RecipeListDTO();
                    Mapper.Mapper.Instance.RecipeListMapper.ToRecipeListDTO(recipeList, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public IEnumerable<RecipeListDTO> LoadByRecipeId(short recipeId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<RecipeListDTO> result = new List<RecipeListDTO>();
                foreach (RecipeList recipeList in context.RecipeList.Where(r => r.RecipeId.Equals(recipeId)))
                {
                    RecipeListDTO dto = new RecipeListDTO();
                    Mapper.Mapper.Instance.RecipeListMapper.ToRecipeListDTO(recipeList, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public void Update(RecipeListDTO recipe)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    RecipeList result = context.RecipeList.FirstOrDefault(r => r.RecipeListId.Equals(recipe.RecipeListId));
                    if (result != null)
                    {
                        Mapper.Mapper.Instance.RecipeListMapper.ToRecipeList(recipe, result);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        #endregion
    }
}