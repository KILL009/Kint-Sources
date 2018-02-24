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
    public class PenaltyLogDAO : IPenaltyLogDAO
    {
        #region Methods

        public DeleteResult Delete(int penaltyLogId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    PenaltyLog PenaltyLog = context.PenaltyLog.FirstOrDefault(c => c.PenaltyLogId.Equals(penaltyLogId));

                    if (PenaltyLog != null)
                    {
                        context.PenaltyLog.Remove(PenaltyLog);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_PENALTYLOG_ERROR"), penaltyLogId, e.Message), e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref PenaltyLogDTO log)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    int id = log.PenaltyLogId;
                    PenaltyLog entity = context.PenaltyLog.FirstOrDefault(c => c.PenaltyLogId.Equals(id));

                    if (entity == null)
                    {
                        log = insert(log, context);
                        return SaveResult.Inserted;
                    }

                    log = update(entity, log, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_PENALTYLOG_ERROR"), log.PenaltyLogId, e.Message), e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<PenaltyLogDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<PenaltyLogDTO> result = new List<PenaltyLogDTO>();
                foreach (PenaltyLog entity in context.PenaltyLog)
                {
                    PenaltyLogDTO dto = new PenaltyLogDTO();
                    Mapper.Mapper.Instance.PenaltyLogMapper.ToPenaltyLogDTO(entity, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public IEnumerable<PenaltyLogDTO> LoadByAccount(long accountId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<PenaltyLogDTO> result = new List<PenaltyLogDTO>();
                foreach (PenaltyLog PenaltyLog in context.PenaltyLog.Where(s => s.AccountId.Equals(accountId)))
                {
                    PenaltyLogDTO dto = new PenaltyLogDTO();
                    Mapper.Mapper.Instance.PenaltyLogMapper.ToPenaltyLogDTO(PenaltyLog, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public PenaltyLogDTO LoadById(int penaltyLogId)
        {
            try
            {

                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    PenaltyLogDTO dto = new PenaltyLogDTO();
                    if(Mapper.Mapper.Instance.PenaltyLogMapper.ToPenaltyLogDTO(context.PenaltyLog.FirstOrDefault(s => s.PenaltyLogId.Equals(penaltyLogId)), dto))
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

        private PenaltyLogDTO insert(PenaltyLogDTO penaltylog, OpenNosContext context)
        {
            PenaltyLog entity = new PenaltyLog();
            Mapper.Mapper.Instance.PenaltyLogMapper.ToPenaltyLog(penaltylog, entity);
            context.PenaltyLog.Add(entity);
            context.SaveChanges();
            if(Mapper.Mapper.Instance.PenaltyLogMapper.ToPenaltyLogDTO(entity,penaltylog))
            {
                return penaltylog;
            }

            return null;
        }

        private PenaltyLogDTO update(PenaltyLog entity, PenaltyLogDTO penaltylog, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mapper.Instance.PenaltyLogMapper.ToPenaltyLog(penaltylog, entity);
                context.SaveChanges();
            }
            if(Mapper.Mapper.Instance.PenaltyLogMapper.ToPenaltyLogDTO(entity, penaltylog))
            {
                return penaltylog;
            }

            return null;
        }

        #endregion
    }
}