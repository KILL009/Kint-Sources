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
    public class GeneralLogDAO : IGeneralLogDAO
    {
        #region Methods

        public bool IdAlreadySet(long id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return context.GeneralLog.Any(gl => gl.LogId == id);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return false;
            }
        }

        public GeneralLogDTO Insert(GeneralLogDTO generalLog)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    GeneralLog entity = new GeneralLog();
                    Mapper.Mapper.Instance.GeneralLogMapper.ToGeneralLog(generalLog, entity);
                    context.GeneralLog.Add(entity);
                    context.SaveChanges();
                    if(Mapper.Mapper.Instance.GeneralLogMapper.ToGeneralLogDTO(entity, generalLog))
                    {
                        return generalLog;
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

        public IEnumerable<GeneralLogDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<GeneralLogDTO> result = new List<GeneralLogDTO>();
                foreach (GeneralLog generalLog in context.GeneralLog)
                {
                    GeneralLogDTO dto = new GeneralLogDTO();
                    Mapper.Mapper.Instance.GeneralLogMapper.ToGeneralLogDTO(generalLog, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public IEnumerable<GeneralLogDTO> LoadByAccount(long? accountId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<GeneralLogDTO> result = new List<GeneralLogDTO>();
                foreach (GeneralLog GeneralLog in context.GeneralLog.Where(s => s.AccountId == accountId))
                {
                    GeneralLogDTO dto = new GeneralLogDTO();
                    Mapper.Mapper.Instance.GeneralLogMapper.ToGeneralLogDTO(GeneralLog, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public IEnumerable<GeneralLogDTO> LoadByLogType(string logType, long? characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<GeneralLogDTO> result = new List<GeneralLogDTO>();
                foreach (GeneralLog log in context.GeneralLog.Where(c => c.LogType.Equals(logType) && c.CharacterId == characterId))
                {
                    GeneralLogDTO dto = new GeneralLogDTO();
                    Mapper.Mapper.Instance.GeneralLogMapper.ToGeneralLogDTO(log, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public void SetCharIdNull(long? characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (GeneralLog log in context.GeneralLog.Where(c => c.CharacterId == characterId))
                    {
                        log.CharacterId = null;
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public void WriteGeneralLog(long accountId, string ipAddress, long? characterId, string logType, string logData)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    GeneralLog log = new GeneralLog
                    {
                        AccountId = accountId,
                        IpAddress = ipAddress,
                        Timestamp = DateTime.Now,
                        LogType = logType,
                        LogData = logData,
                        CharacterId = characterId
                    };

                    context.GeneralLog.Add(log);
                    context.SaveChanges();
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