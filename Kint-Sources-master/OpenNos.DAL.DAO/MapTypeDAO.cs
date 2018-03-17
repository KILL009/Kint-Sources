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
    public class MapTypeDAO : IMapTypeDAO
    {
        #region Methods

        public MapTypeDTO Insert(ref MapTypeDTO mapType)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    MapType entity = new MapType();
                    Mapper.Mapper.Instance.MapTypeMapper.ToMapType(mapType, entity);
                    context.MapType.Add(entity);
                    context.SaveChanges();
                    if(Mapper.Mapper.Instance.MapTypeMapper.ToMapTypeDTO(entity, mapType))
                    {
                        return mapType;
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

        public IEnumerable<MapTypeDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<MapTypeDTO> result = new List<MapTypeDTO>();
                foreach (MapType MapType in context.MapType)
                {
                    MapTypeDTO dto = new MapTypeDTO();
                    Mapper.Mapper.Instance.MapTypeMapper.ToMapTypeDTO(MapType, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public MapTypeDTO LoadById(short maptypeId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    MapTypeDTO dto = new MapTypeDTO();
                    if(Mapper.Mapper.Instance.MapTypeMapper.ToMapTypeDTO(context.MapType.FirstOrDefault(s => s.MapTypeId.Equals(maptypeId)), dto))
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