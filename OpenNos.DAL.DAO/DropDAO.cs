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
    public class DropDAO : IDropDAO
    {
        #region Methods

        public void Insert(List<DropDTO> drops)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    foreach (DropDTO Drop in drops)
                    {
                        Drop entity = new Drop();
                        Mapper.Mapper.Instance.DropMapper.ToDrop(Drop, entity);
                        context.Drop.Add(entity);
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

        public DropDTO Insert(DropDTO drop)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Drop entity = new Drop();
                    context.Drop.Add(entity);
                    context.SaveChanges();
                    if(Mapper.Mapper.Instance.DropMapper.ToDropDTO(entity, drop))
                    {
                        return drop;
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

        public List<DropDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<DropDTO> result = new List<DropDTO>();
                foreach(Drop entity in context.Drop)
                {
                    DropDTO dto = new DropDTO();
                    Mapper.Mapper.Instance.DropMapper.ToDropDTO(entity, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public IEnumerable<DropDTO> LoadByMonster(short monsterVNum)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<DropDTO> result = new List<DropDTO>();

                foreach (Drop Drop in context.Drop.Where(s => s.MonsterVNum == monsterVNum || s.MonsterVNum == null))
                {
                    DropDTO dto = new DropDTO();
                    Mapper.Mapper.Instance.DropMapper.ToDropDTO(Drop, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        #endregion
    }
}