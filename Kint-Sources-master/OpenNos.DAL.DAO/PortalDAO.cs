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
    public class PortalDAO : IPortalDAO
    {
        #region Methods

        public void Insert(List<PortalDTO> portals)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    foreach (PortalDTO Item in portals)
                    {
                        Portal entity = new Portal();
                        Mapper.Mapper.Instance.PortalMapper.ToPortal(Item, entity);
                        context.Portal.Add(entity);
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

        public PortalDTO Insert(PortalDTO portal)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Portal entity = new Portal();
                    Mapper.Mapper.Instance.PortalMapper.ToPortal(portal, entity);
                    context.Portal.Add(entity);
                    context.SaveChanges();
                    if(Mapper.Mapper.Instance.PortalMapper.ToPortalDTO(entity, portal))
                    {
                        return portal;
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

        public IEnumerable<PortalDTO> LoadByMap(short mapId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<PortalDTO> result = new List<PortalDTO>();
                foreach (Portal Portalobject in context.Portal.Where(c => c.SourceMapId.Equals(mapId)))
                {
                    PortalDTO dto = new PortalDTO();
                    Mapper.Mapper.Instance.PortalMapper.ToPortalDTO(Portalobject, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        #endregion
    }
}