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
    public class ItemDAO : IItemDAO
    {
        #region Methods

        public IEnumerable<ItemDTO> FindByName(string name)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<ItemDTO> result = new List<ItemDTO>();
                foreach (Item item in context.Item.Where(s => string.IsNullOrEmpty(name) ? s.Name.Equals(string.Empty) : s.Name.Contains(name)))
                {
                    ItemDTO dto = new ItemDTO();
                    Mapper.Mapper.Instance.ItemMapper.ToItemDTO(item, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public void Insert(IEnumerable<ItemDTO> items)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (ItemDTO Item in items)
                    {
                        Item entity = new Item();
                        Mapper.Mapper.Instance.ItemMapper.ToItem(Item, entity);
                        context.Item.Add(entity);
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public ItemDTO Insert(ItemDTO item)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Item entity = new Item();
                    Mapper.Mapper.Instance.ItemMapper.ToItem(item, entity);
                    context.Item.Add(entity);
                    context.SaveChanges();
                    if(Mapper.Mapper.Instance.ItemMapper.ToItemDTO(entity, item))
                    {
                        return item;
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

        public IEnumerable<ItemDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<ItemDTO> result = new List<ItemDTO>();
                foreach (Item item in context.Item)
                {
                    ItemDTO dto = new ItemDTO();
                    Mapper.Mapper.Instance.ItemMapper.ToItemDTO(item, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public ItemDTO LoadById(short vNum)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    ItemDTO dto = new ItemDTO();
                    if(Mapper.Mapper.Instance.ItemMapper.ToItemDTO(context.Item.FirstOrDefault(i => i.VNum.Equals(vNum)), dto))
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

        #endregion
    }
}