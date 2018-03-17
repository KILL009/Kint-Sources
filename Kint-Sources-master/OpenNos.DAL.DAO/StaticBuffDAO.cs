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
    public class StaticBuffDAO : IStaticBuffDAO
    {
        #region Methods

        public void Delete(short bonusToDelete, long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    StaticBuff bon = context.StaticBuff.FirstOrDefault(c => c.CardId == bonusToDelete && c.CharacterId == characterId);

                    if (bon != null)
                    {
                        context.StaticBuff.Remove(bon);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_ERROR"), bonusToDelete, e.Message), e);
            }
        }

        public SaveResult InsertOrUpdate(ref StaticBuffDTO staticBuff)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long id = staticBuff.CharacterId;
                    short cardid = staticBuff.CardId;
                    StaticBuff entity = context.StaticBuff.FirstOrDefault(c => c.CardId == cardid && c.CharacterId == id);

                    if (entity == null)
                    {
                        staticBuff = insert(staticBuff, context);
                        return SaveResult.Inserted;
                    }
                    staticBuff.StaticBuffId = entity.StaticBuffId;
                    staticBuff = update(entity, staticBuff, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<StaticBuffDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<StaticBuffDTO> result = new List<StaticBuffDTO>();
                foreach (StaticBuff entity in context.StaticBuff.Where(i => i.CharacterId == characterId))
                {
                    StaticBuffDTO dto = new StaticBuffDTO();
                    Mapper.Mapper.Instance.StaticBuffMapper.ToStaticBuffDTO(entity, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public StaticBuffDTO LoadById(long sbId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    StaticBuffDTO dto = new StaticBuffDTO();
                    if(Mapper.Mapper.Instance.StaticBuffMapper.ToStaticBuffDTO(context.StaticBuff.FirstOrDefault(s => s.StaticBuffId.Equals(sbId)),dto)) //who the fuck was so retarded and set it to respawn ?!?
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

        public IEnumerable<short> LoadByTypeCharacterId(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return context.StaticBuff.Where(i => i.CharacterId == characterId).Select(qle => qle.CardId).ToList();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private StaticBuffDTO insert(StaticBuffDTO sb, OpenNosContext context)
        {
            try
            {
                StaticBuff entity = new StaticBuff();
                Mapper.Mapper.Instance.StaticBuffMapper.ToStaticBuff(sb, entity);
                context.StaticBuff.Add(entity);
                context.SaveChanges();
                if(Mapper.Mapper.Instance.StaticBuffMapper.ToStaticBuffDTO(entity, sb))
                {
                    return sb;
                }

                return null;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private StaticBuffDTO update(StaticBuff entity, StaticBuffDTO sb, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mapper.Instance.StaticBuffMapper.ToStaticBuff(sb, entity);
                context.SaveChanges();
            }
            if(Mapper.Mapper.Instance.StaticBuffMapper.ToStaticBuffDTO(entity, sb))
            {
                return sb;
            }

            return null;
        }

        #endregion
    }
}