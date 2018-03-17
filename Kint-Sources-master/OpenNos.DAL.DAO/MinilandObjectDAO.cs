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
using OpenNos.Data.Enums;
using OpenNos.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.DAL.DAO
{
    public class MinilandObjectDAO : IMinilandObjectDAO
    {
        #region Methods

        public DeleteResult DeleteById(long id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    MinilandObject item = context.MinilandObject.First(i => i.MinilandObjectId.Equals(id));

                    if (item != null)
                    {
                        context.MinilandObject.Remove(item);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref MinilandObjectDTO obj)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long id = obj.MinilandObjectId;
                    MinilandObject entity = context.MinilandObject.FirstOrDefault(c => c.MinilandObjectId.Equals(id));

                    if (entity == null)
                    {
                        obj = insert(obj, context);
                        return SaveResult.Inserted;
                    }

                    obj.MinilandObjectId = entity.MinilandObjectId;
                    obj = update(entity, obj, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<MinilandObjectDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<MinilandObjectDTO> result = new List<MinilandObjectDTO>();
                foreach (MinilandObject obj in context.MinilandObject.Where(s => s.CharacterId == characterId))
                {
                    MinilandObjectDTO dto = new MinilandObjectDTO();
                    Mapper.Mapper.Instance.MinilandObjectMapper.ToMinilandObjectDTO(obj, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        private MinilandObjectDTO insert(MinilandObjectDTO obj, OpenNosContext context)
        {
            try
            {
                MinilandObject entity = new MinilandObject();
                Mapper.Mapper.Instance.MinilandObjectMapper.ToMinilandObject(obj, entity);
                context.MinilandObject.Add(entity);
                context.SaveChanges();
                if(Mapper.Mapper.Instance.MinilandObjectMapper.ToMinilandObjectDTO(entity, obj))
                {
                    return obj;
                }

                return null;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private MinilandObjectDTO update(MinilandObject entity, MinilandObjectDTO respawn, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mapper.Instance.MinilandObjectMapper.ToMinilandObject(respawn, entity);
                context.SaveChanges();
            }
            if(Mapper.Mapper.Instance.MinilandObjectMapper.ToMinilandObjectDTO(entity, respawn))
            {
                return respawn;
            }

            return null;
        }

        #endregion
    }
}