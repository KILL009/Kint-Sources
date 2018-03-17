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
    public class MapMonsterDAO : IMapMonsterDAO
    {
        #region Methods

        public DeleteResult DeleteById(int mapMonsterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    MapMonster monster = context.MapMonster.First(i => i.MapMonsterId.Equals(mapMonsterId));

                    if (monster != null)
                    {
                        context.MapMonster.Remove(monster);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return DeleteResult.Error;
            }
        }

        public bool DoesMonsterExist(int mapMonsterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.MapMonster.Any(i => i.MapMonsterId.Equals(mapMonsterId));
            }
        }

        public void Insert(IEnumerable<MapMonsterDTO> mapMonsters)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    foreach (MapMonsterDTO monster in mapMonsters)
                    {
                        MapMonster entity = new MapMonster();
                        Mapper.Mapper.Instance.MapMonsterMapper.ToMapMonster(monster, entity);
                        context.MapMonster.Add(entity);
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

        public MapMonsterDTO Insert(MapMonsterDTO mapMonster)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    MapMonster entity = new MapMonster();
                    Mapper.Mapper.Instance.MapMonsterMapper.ToMapMonster(mapMonster, entity);
                    context.MapMonster.Add(entity);
                    context.SaveChanges();
                    if(Mapper.Mapper.Instance.MapMonsterMapper.ToMapMonsterDTO(entity, mapMonster))
                    {
                        return mapMonster;
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

        public MapMonsterDTO LoadById(int mapMonsterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    MapMonsterDTO dto = new MapMonsterDTO();
                    if(Mapper.Mapper.Instance.MapMonsterMapper.ToMapMonsterDTO(context.MapMonster.FirstOrDefault(i => i.MapMonsterId == mapMonsterId), dto))
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

        public IEnumerable<MapMonsterDTO> LoadFromMap(short mapId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<MapMonsterDTO> result = new List<MapMonsterDTO>();
                foreach (MapMonster MapMonsterobject in context.MapMonster.Where(c => c.MapId.Equals(mapId)))
                {
                    MapMonsterDTO dto = new MapMonsterDTO();
                    Mapper.Mapper.Instance.MapMonsterMapper.ToMapMonsterDTO(MapMonsterobject, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        #endregion
    }
}