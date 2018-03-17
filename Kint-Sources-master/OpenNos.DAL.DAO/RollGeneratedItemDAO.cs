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
    public class RollGeneratedItemDAO : IRollGeneratedItemDAO
    {
        #region Methods

        public RollGeneratedItemDTO Insert(RollGeneratedItemDTO item)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    RollGeneratedItem entity = new RollGeneratedItem();
                    Mapper.Mapper.Instance.RollGeneratedItemMapper.ToRollGeneratedItem(item, entity);
                    context.RollGeneratedItem.Add(entity);
                    context.SaveChanges();
                    if(Mapper.Mapper.Instance.RollGeneratedItemMapper.ToRollGeneratedItemDTO(entity, item))
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

        public IEnumerable<RollGeneratedItemDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<RollGeneratedItemDTO> result = new List<RollGeneratedItemDTO>();
                foreach (RollGeneratedItem item in context.RollGeneratedItem)
                {
                    RollGeneratedItemDTO dto = new RollGeneratedItemDTO();
                    Mapper.Mapper.Instance.RollGeneratedItemMapper.ToRollGeneratedItemDTO(item, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public RollGeneratedItemDTO LoadById(short id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    RollGeneratedItemDTO dto = new RollGeneratedItemDTO();
                    if(Mapper.Mapper.Instance.RollGeneratedItemMapper.ToRollGeneratedItemDTO(context.RollGeneratedItem.FirstOrDefault(i => i.RollGeneratedItemId.Equals(id)), dto))
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

        public IEnumerable<RollGeneratedItemDTO> LoadByItemVNum(short vnum)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<RollGeneratedItemDTO> result = new List<RollGeneratedItemDTO>();
                foreach (RollGeneratedItem item in context.RollGeneratedItem.Where(s => s.OriginalItemVNum == vnum))
                {
                    RollGeneratedItemDTO dto = new RollGeneratedItemDTO();
                    Mapper.Mapper.Instance.RollGeneratedItemMapper.ToRollGeneratedItemDTO(item, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        #endregion
    }
}