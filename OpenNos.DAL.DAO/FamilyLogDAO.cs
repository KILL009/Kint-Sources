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
    public class FamilyLogDAO : IFamilyLogDAO
    {
        #region Methods

        public DeleteResult Delete(long familyLogId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    FamilyLog famlog = context.FamilyLog.FirstOrDefault(c => c.FamilyLogId.Equals(familyLogId));

                    if (famlog != null)
                    {
                        context.FamilyLog.Remove(famlog);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_ERROR"), familyLogId, e.Message), e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref FamilyLogDTO familyLog)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long FamilyLog = familyLog.FamilyLogId;
                    FamilyLog entity = context.FamilyLog.FirstOrDefault(c => c.FamilyLogId.Equals(FamilyLog));

                    if (entity == null)
                    {
                        familyLog = insert(familyLog, context);
                        return SaveResult.Inserted;
                    }

                    familyLog = update(entity, familyLog, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_FAMILYLOG_ERROR"), familyLog.FamilyLogId, e.Message), e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<FamilyLogDTO> LoadByFamilyId(long familyId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<FamilyLogDTO> result = new List<FamilyLogDTO>();
                foreach (FamilyLog familylog in context.FamilyLog.Where(fc => fc.FamilyId.Equals(familyId)))
                {
                    FamilyLogDTO dto = new FamilyLogDTO();
                    Mapper.Mapper.Instance.FamilyLogMapper.ToFamilyLogDTO(familylog, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        private FamilyLogDTO insert(FamilyLogDTO famlog, OpenNosContext context)
        {
            FamilyLog entity = new FamilyLog();
            Mapper.Mapper.Instance.FamilyLogMapper.ToFamilyLog(famlog, entity);
            context.FamilyLog.Add(entity);
            context.SaveChanges();
            if(Mapper.Mapper.Instance.FamilyLogMapper.ToFamilyLogDTO(entity, famlog))
            {
                return famlog;
            }

            return null;
        }

        private FamilyLogDTO update(FamilyLog entity, FamilyLogDTO famlog, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mapper.Instance.FamilyLogMapper.ToFamilyLog(famlog, entity);
                context.SaveChanges();
            }

            if(Mapper.Mapper.Instance.FamilyLogMapper.ToFamilyLogDTO(entity, famlog))
            {
                return famlog;
            }

            return null;
        }

        #endregion
    }
}