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
    public class ScriptedInstanceDAO : IScriptedInstanceDAO
    {
        #region Methods

        public void Insert(List<ScriptedInstanceDTO> scriptedInstances)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    foreach (ScriptedInstanceDTO scriptedInstance in scriptedInstances)
                    {
                        ScriptedInstance entity = new ScriptedInstance();
                        Mapper.Mapper.Instance.ScriptedInstanceMapper.ToScriptedInstance(scriptedInstance, entity);
                        context.ScriptedInstance.Add(entity);
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

        public ScriptedInstanceDTO Insert(ScriptedInstanceDTO scriptedInstance)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    ScriptedInstance entity = new ScriptedInstance();
                    Mapper.Mapper.Instance.ScriptedInstanceMapper.ToScriptedInstance(scriptedInstance, entity);
                    context.ScriptedInstance.Add(entity);
                    context.SaveChanges();
                    if(Mapper.Mapper.Instance.ScriptedInstanceMapper.ToScriptedInstanceDTO(entity, scriptedInstance))
                    {
                        return scriptedInstance;
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

        public IEnumerable<ScriptedInstanceDTO> LoadByMap(short mapId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<ScriptedInstanceDTO> result = new List<ScriptedInstanceDTO>();
                foreach (ScriptedInstance timespaceObject in context.ScriptedInstance.Where(c => c.MapId.Equals(mapId)))
                {
                    ScriptedInstanceDTO dto = new ScriptedInstanceDTO();
                    Mapper.Mapper.Instance.ScriptedInstanceMapper.ToScriptedInstanceDTO(timespaceObject, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        #endregion
    }
}