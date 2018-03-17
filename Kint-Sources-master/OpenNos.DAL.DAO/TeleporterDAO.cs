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
    public class TeleporterDAO : ITeleporterDAO
    {
        #region Methods

        public TeleporterDTO Insert(TeleporterDTO teleporter)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Teleporter entity = new Teleporter();
                    Mapper.Mapper.Instance.TeleporterMapper.ToTeleporter(teleporter, entity);
                    context.Teleporter.Add(entity);
                    context.SaveChanges();
                    if(Mapper.Mapper.Instance.TeleporterMapper.ToTeleporterDTO(entity, teleporter))
                    {
                        return teleporter;
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

        public IEnumerable<TeleporterDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<TeleporterDTO> result = new List<TeleporterDTO>();
                foreach (Teleporter entity in context.Teleporter)
                {
                    TeleporterDTO dto = new TeleporterDTO();
                    Mapper.Mapper.Instance.TeleporterMapper.ToTeleporterDTO(entity, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public TeleporterDTO LoadById(short teleporterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    TeleporterDTO dto = new TeleporterDTO();
                    if(Mapper.Mapper.Instance.TeleporterMapper.ToTeleporterDTO(context.Teleporter.FirstOrDefault(i => i.TeleporterId.Equals(teleporterId)), dto))
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

        public IEnumerable<TeleporterDTO> LoadFromNpc(int npcId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<TeleporterDTO> result = new List<TeleporterDTO>();
                foreach (Teleporter entity in context.Teleporter.Where(c => c.MapNpcId.Equals(npcId)))
                {
                    TeleporterDTO dto = new TeleporterDTO();
                    Mapper.Mapper.Instance.TeleporterMapper.ToTeleporterDTO(entity, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        #endregion
    }
}