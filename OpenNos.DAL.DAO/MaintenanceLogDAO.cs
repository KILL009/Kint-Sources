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
    public class MaintenanceLogDAO : IMaintenanceLogDAO
    {
        #region Methods

        public MaintenanceLogDTO Insert(MaintenanceLogDTO maintenanceLog)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    MaintenanceLog entity = new MaintenanceLog();
                    Mapper.Mapper.Instance.MaintenanceLogMapper.ToMaintenanceLog(maintenanceLog, entity);
                    context.MaintenanceLog.Add(entity);
                    context.SaveChanges();
                    if(Mapper.Mapper.Instance.MaintenanceLogMapper.ToMaintenanceLogDTO(entity, maintenanceLog))
                    {
                        return maintenanceLog;
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

        public IEnumerable<MaintenanceLogDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<MaintenanceLogDTO> result = new List<MaintenanceLogDTO>();
                foreach (MaintenanceLog maintenanceLog in context.MaintenanceLog)
                {
                    MaintenanceLogDTO dto = new MaintenanceLogDTO();
                    Mapper.Mapper.Instance.MaintenanceLogMapper.ToMaintenanceLogDTO(maintenanceLog, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public MaintenanceLogDTO LoadFirst()
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    MaintenanceLogDTO dto = new MaintenanceLogDTO();
                    if(Mapper.Mapper.Instance.MaintenanceLogMapper.ToMaintenanceLogDTO(context.MaintenanceLog.FirstOrDefault(m => m.DateEnd > DateTime.Now), dto))
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

        #endregion
    }
}