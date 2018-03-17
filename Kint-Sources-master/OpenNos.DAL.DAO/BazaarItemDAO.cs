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
using OpenNos.Data.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace OpenNos.DAL.DAO
{
    public class BazaarItemDAO : IBazaarItemDAO
    {
        #region Methods

        public DeleteResult Delete(long bazaarItemId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    BazaarItem BazaarItem = context.BazaarItem.FirstOrDefault(c => c.BazaarItemId.Equals(bazaarItemId));

                    if (BazaarItem != null)
                    {
                        context.BazaarItem.Remove(BazaarItem);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_ERROR"), bazaarItemId, e.Message), e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref BazaarItemDTO bazaarItem)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long bazaarItemId = bazaarItem.BazaarItemId;
                    BazaarItem entity = context.BazaarItem.FirstOrDefault(c => c.BazaarItemId.Equals(bazaarItemId));

                    if (entity == null)
                    {
                        bazaarItem = insert(bazaarItem, context);
                        return SaveResult.Inserted;
                    }

                    bazaarItem = update(entity, bazaarItem, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"BazaarItemId: {bazaarItem.BazaarItemId} Message: {e.Message}", e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<BazaarItemDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<BazaarItemDTO> result = new List<BazaarItemDTO>();
                foreach (BazaarItem bazaarItem in context.BazaarItem)
                {
                    BazaarItemDTO dto = new BazaarItemDTO();
                    Mapper.Mapper.Instance.BazaarItemMapper.ToBazaarItemDTO(bazaarItem, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public BazaarItemDTO LoadById(long bazaarItemId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    BazaarItemDTO dto = new BazaarItemDTO();
                    if(Mapper.Mapper.Instance.BazaarItemMapper.ToBazaarItemDTO(context.BazaarItem.FirstOrDefault(i => i.BazaarItemId.Equals(bazaarItemId)), dto))
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

        public void RemoveOutDated()
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (BazaarItem entity in context.BazaarItem.Where(e => DbFunctions.AddDays(DbFunctions.AddHours(e.DateStart, e.Duration), e.MedalUsed ? 30 : 7) < DateTime.Now))
                    {
                        context.BazaarItem.Remove(entity);
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private BazaarItemDTO insert(BazaarItemDTO bazaarItem, OpenNosContext context)
        {
            BazaarItem entity = new BazaarItem();
            Mapper.Mapper.Instance.BazaarItemMapper.ToBazaarItem(bazaarItem, entity);
            context.BazaarItem.Add(entity);
            context.SaveChanges();
            if(Mapper.Mapper.Instance.BazaarItemMapper.ToBazaarItemDTO(entity, bazaarItem))
            {
                return bazaarItem;
            }

            return null;
        }

        private BazaarItemDTO update(BazaarItem entity, BazaarItemDTO bazaarItem, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mapper.Instance.BazaarItemMapper.ToBazaarItem(bazaarItem, entity);
                context.SaveChanges();
            }
            if(Mapper.Mapper.Instance.BazaarItemMapper.ToBazaarItemDTO(entity, bazaarItem))
            {
                return bazaarItem;
            }

            return null;
        }

        #endregion
    }
}