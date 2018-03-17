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
    public class MinigameLogDAO : IMinigameLogDAO
    {
        #region Methods

        public SaveResult InsertOrUpdate(ref MinigameLogDTO minigameLog)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long minigameLogId = minigameLog.MinigameLogId;
                    MinigameLog entity = context.MinigameLog.FirstOrDefault(c => c.MinigameLogId.Equals(minigameLogId));

                    if (entity == null)
                    {
                        minigameLog = insert(minigameLog, context);
                        return SaveResult.Inserted;
                    }
                    minigameLog = update(entity, minigameLog, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<MinigameLogDTO> LoadByCharacterId(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    IEnumerable<MinigameLog> minigameLog = context.MinigameLog.Where(a => a.CharacterId.Equals(characterId)).ToList();
                    if (minigameLog != null)
                    {
                        List<MinigameLogDTO> result = new List<MinigameLogDTO>();
                        foreach (MinigameLog input in minigameLog)
                        {
                            MinigameLogDTO dto = new MinigameLogDTO();
                            if (Mapper.Mappers.MinigameLogMapper.ToMinigameLogDTO(input, dto))
                            {
                                result.Add(dto);
                            }
                        }
                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return null;
        }

        public MinigameLogDTO LoadById(long minigameLogId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    MinigameLog minigameLog = context.MinigameLog.FirstOrDefault(a => a.MinigameLogId.Equals(minigameLogId));
                    if (minigameLog != null)
                    {
                        MinigameLogDTO minigameLogDTO = new MinigameLogDTO();
                        if (Mapper.Mappers.MinigameLogMapper.ToMinigameLogDTO(minigameLog, minigameLogDTO))
                        {
                            return minigameLogDTO;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return null;
        }

        private static MinigameLogDTO insert(MinigameLogDTO account, OpenNosContext context)
        {
            MinigameLog entity = new MinigameLog();
            Mapper.Mappers.MinigameLogMapper.ToMinigameLog(account, entity);
            context.MinigameLog.Add(entity);
            context.SaveChanges();
            Mapper.Mappers.MinigameLogMapper.ToMinigameLogDTO(entity, account);
            return account;
        }

        private static MinigameLogDTO update(MinigameLog entity, MinigameLogDTO account, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mappers.MinigameLogMapper.ToMinigameLog(account, entity);
                context.Entry(entity).State = EntityState.Modified;
                context.SaveChanges();
            }
            if (Mapper.Mappers.MinigameLogMapper.ToMinigameLogDTO(entity, account))
            {
                return account;
            }

            return null;
        }

        #endregion
    }
}