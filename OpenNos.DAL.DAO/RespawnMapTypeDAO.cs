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
using System.Linq;

namespace OpenNos.DAL.DAO
{
    public class RespawnMapTypeDAO : IRespawnMapTypeDAO
    {
        #region Methods

        public void Insert(List<RespawnMapTypeDTO> respawnMapTypes)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    foreach (RespawnMapTypeDTO RespawnMapType in respawnMapTypes)
                    {
                        RespawnMapType entity = new RespawnMapType();
                        Mapper.Mapper.Instance.RespawnMapTypeMapper.ToRespawnMapType(RespawnMapType, entity);
                        context.RespawnMapType.Add(entity);
                    }
                    context.Configuration.AutoDetectChangesEnabled = true;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public SaveResult InsertOrUpdate(ref RespawnMapTypeDTO respawnMapType)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    short mapId = respawnMapType.DefaultMapId;
                    RespawnMapType entity = context.RespawnMapType.FirstOrDefault(c => c.DefaultMapId.Equals(mapId));

                    if (entity == null)
                    {
                        respawnMapType = insert(respawnMapType, context);
                        return SaveResult.Inserted;
                    }

                    respawnMapType.RespawnMapTypeId = entity.RespawnMapTypeId;
                    respawnMapType = update(entity, respawnMapType, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return SaveResult.Error;
            }
        }

        public RespawnMapTypeDTO LoadById(long respawnMapTypeId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    RespawnMapTypeDTO dto = new RespawnMapTypeDTO();
                    if(Mapper.Mapper.Instance.RespawnMapTypeMapper.ToRespawnMapTypeDTO(context.RespawnMapType.FirstOrDefault(s => s.RespawnMapTypeId.Equals(respawnMapTypeId)), dto))
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

        public RespawnMapTypeDTO LoadByMapId(short mapId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    RespawnMapTypeDTO dto = new RespawnMapTypeDTO();
                    if(Mapper.Mapper.Instance.RespawnMapTypeMapper.ToRespawnMapTypeDTO(context.RespawnMapType.FirstOrDefault(s => s.DefaultMapId.Equals(mapId)), dto))
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

        private RespawnMapTypeDTO insert(RespawnMapTypeDTO respawnMapType, OpenNosContext context)
        {
            try
            {
                RespawnMapType entity = new RespawnMapType();
                Mapper.Mapper.Instance.RespawnMapTypeMapper.ToRespawnMapType(respawnMapType, entity);
                context.RespawnMapType.Add(entity);
                context.SaveChanges();
                if(Mapper.Mapper.Instance.RespawnMapTypeMapper.ToRespawnMapTypeDTO(entity, respawnMapType))
                {
                    return respawnMapType;
                }

                return null;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private RespawnMapTypeDTO update(RespawnMapType entity, RespawnMapTypeDTO respawnMapType, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mapper.Instance.RespawnMapTypeMapper.ToRespawnMapType(respawnMapType, entity);
                context.SaveChanges();
            }
            if(Mapper.Mapper.Instance.RespawnMapTypeMapper.ToRespawnMapTypeDTO(entity, respawnMapType))
            {
                return respawnMapType;
            }

            return null;
        }

        #endregion
    }
}