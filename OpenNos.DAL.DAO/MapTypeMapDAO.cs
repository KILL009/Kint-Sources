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
    public class MapTypeMapDAO : IMapTypeMapDAO
    {
        #region Methods

        public void Insert(List<MapTypeMapDTO> mapTypeMaps)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    foreach (MapTypeMapDTO mapTypeMap in mapTypeMaps)
                    {
                        MapTypeMap entity = new MapTypeMap();
                        Mapper.Mapper.Instance.MapTypeMapMapper.ToMapTypeMap(mapTypeMap, entity);
                        context.MapTypeMap.Add(entity);
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

        public IEnumerable<MapTypeMapDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<MapTypeMapDTO> result = new List<MapTypeMapDTO>();
                foreach (MapTypeMap MapTypeMap in context.MapTypeMap)
                {
                    MapTypeMapDTO dto = new MapTypeMapDTO();
                    Mapper.Mapper.Instance.MapTypeMapMapper.ToMapTypeMapDTO(MapTypeMap, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public MapTypeMapDTO LoadByMapAndMapType(short mapId, short maptypeId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    MapTypeMapDTO dto = new MapTypeMapDTO();
                    if(Mapper.Mapper.Instance.MapTypeMapMapper.ToMapTypeMapDTO(context.MapTypeMap.FirstOrDefault(i => i.MapId.Equals(mapId) && i.MapTypeId.Equals(maptypeId)), dto))
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

        public IEnumerable<MapTypeMapDTO> LoadByMapId(short mapId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<MapTypeMapDTO> result = new List<MapTypeMapDTO>();
                foreach (MapTypeMap MapTypeMap in context.MapTypeMap.Where(c => c.MapId.Equals(mapId)))
                {
                    MapTypeMapDTO dto = new MapTypeMapDTO();
                    Mapper.Mapper.Instance.MapTypeMapMapper.ToMapTypeMapDTO(MapTypeMap, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public IEnumerable<MapTypeMapDTO> LoadByMapTypeId(short maptypeId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<MapTypeMapDTO> result = new List<MapTypeMapDTO>();
                foreach (MapTypeMap MapTypeMap in context.MapTypeMap.Where(c => c.MapTypeId.Equals(maptypeId)))
                {
                    MapTypeMapDTO dto = new MapTypeMapDTO();
                    Mapper.Mapper.Instance.MapTypeMapMapper.ToMapTypeMapDTO(MapTypeMap, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        #endregion
    }
}