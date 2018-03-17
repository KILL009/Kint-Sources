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
    public class RespawnDAO : IRespawnDAO
    {
        #region Methods

        public SaveResult InsertOrUpdate(ref RespawnDTO respawn)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long CharacterId = respawn.CharacterId;
                    long RespawnMapTypeId = respawn.RespawnMapTypeId;
                    Respawn entity = context.Respawn.FirstOrDefault(c => c.RespawnMapTypeId.Equals(RespawnMapTypeId) && c.CharacterId.Equals(CharacterId));

                    if (entity == null)
                    {
                        respawn = insert(respawn, context);
                        return SaveResult.Inserted;
                    }

                    respawn.RespawnId = entity.RespawnId;
                    respawn = update(entity, respawn, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<RespawnDTO> LoadByCharacter(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<RespawnDTO> result = new List<RespawnDTO>();
                foreach (Respawn Respawnobject in context.Respawn.Where(i => i.CharacterId.Equals(characterId)))
                {
                    RespawnDTO dto = new RespawnDTO();
                    Mapper.Mapper.Instance.RespawnMapper.ToRespawnDTO(Respawnobject, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public RespawnDTO LoadById(long respawnId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    RespawnDTO dto = new RespawnDTO();
                    if(Mapper.Mapper.Instance.RespawnMapper.ToRespawnDTO(context.Respawn.FirstOrDefault(s => s.RespawnId.Equals(respawnId)), dto))
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

        private RespawnDTO insert(RespawnDTO respawn, OpenNosContext context)
        {
            try
            {
                Respawn entity = new Respawn();
                Mapper.Mapper.Instance.RespawnMapper.ToRespawn(respawn, entity);
                context.Respawn.Add(entity);
                context.SaveChanges();
                if(Mapper.Mapper.Instance.RespawnMapper.ToRespawnDTO(entity, respawn))
                {
                    return respawn;
                }

                return null;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private RespawnDTO update(Respawn entity, RespawnDTO respawn, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mapper.Instance.RespawnMapper.ToRespawn(respawn, entity);
                context.SaveChanges();
            }
            if(Mapper.Mapper.Instance.RespawnMapper.ToRespawnDTO(entity, respawn))
            {
                return respawn;
            }

            return null;
        }

        #endregion
    }
}