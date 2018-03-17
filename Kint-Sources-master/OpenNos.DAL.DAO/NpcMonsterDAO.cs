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
    public class NpcMonsterDAO : INpcMonsterDAO
    {
        #region Methods

        public IEnumerable<NpcMonsterDTO> FindByName(string name)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<NpcMonsterDTO> result = new List<NpcMonsterDTO>();
                foreach (NpcMonster npcMonster in context.NpcMonster.Where(s => string.IsNullOrEmpty(name) ? s.Name.Equals(string.Empty) : s.Name.Contains(name)))
                {
                    NpcMonsterDTO dto = new NpcMonsterDTO();
                    Mapper.Mapper.Instance.NpcMonsterMapper.ToNpcMonsterDTO(npcMonster, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public void Insert(List<NpcMonsterDTO> npcMonsters)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    foreach (NpcMonsterDTO Item in npcMonsters)
                    {
                        NpcMonster entity = new NpcMonster();
                        Mapper.Mapper.Instance.NpcMonsterMapper.ToNpcMonster(Item, entity);
                        context.NpcMonster.Add(entity);
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

        public NpcMonsterDTO Insert(NpcMonsterDTO npc)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    NpcMonster entity = new NpcMonster();
                    Mapper.Mapper.Instance.NpcMonsterMapper.ToNpcMonster(npc, entity);
                    context.NpcMonster.Add(entity);
                    context.SaveChanges();
                    if(Mapper.Mapper.Instance.NpcMonsterMapper.ToNpcMonsterDTO(entity, npc))
                    {
                        return npc;
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

        public SaveResult InsertOrUpdate(ref NpcMonsterDTO npcMonster)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    short npcMonsterVNum = npcMonster.NpcMonsterVNum;
                    NpcMonster entity = context.NpcMonster.FirstOrDefault(c => c.NpcMonsterVNum.Equals(npcMonsterVNum));

                    if (entity == null)
                    {
                        npcMonster = insert(npcMonster, context);
                        return SaveResult.Inserted;
                    }

                    npcMonster = update(entity, npcMonster, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_NPCMONSTER_ERROR"), npcMonster.NpcMonsterVNum, e.Message), e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<NpcMonsterDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<NpcMonsterDTO> result = new List<NpcMonsterDTO>();
                foreach (NpcMonster NpcMonster in context.NpcMonster)
                {
                    NpcMonsterDTO dto = new NpcMonsterDTO();
                    Mapper.Mapper.Instance.NpcMonsterMapper.ToNpcMonsterDTO(NpcMonster, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public NpcMonsterDTO LoadByVNum(short npcMonsterVNum)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    NpcMonsterDTO dto = new NpcMonsterDTO();
                    if(Mapper.Mapper.Instance.NpcMonsterMapper.ToNpcMonsterDTO(context.NpcMonster.FirstOrDefault(i => i.NpcMonsterVNum.Equals(npcMonsterVNum)), dto))
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

        private NpcMonsterDTO insert(NpcMonsterDTO npcMonster, OpenNosContext context)
        {
            NpcMonster entity = new NpcMonster();
            Mapper.Mapper.Instance.NpcMonsterMapper.ToNpcMonster(npcMonster, entity);
            context.NpcMonster.Add(entity);
            context.SaveChanges();
            if(Mapper.Mapper.Instance.NpcMonsterMapper.ToNpcMonsterDTO(entity, npcMonster))
            {
                return npcMonster;
            }

            return null;
        }

        private NpcMonsterDTO update(NpcMonster entity, NpcMonsterDTO npcMonster, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mapper.Instance.NpcMonsterMapper.ToNpcMonster(npcMonster, entity);
                context.SaveChanges();
            }
            if(Mapper.Mapper.Instance.NpcMonsterMapper.ToNpcMonsterDTO(entity, npcMonster))
            {
                return npcMonster;
            }

            return null;
        }

        #endregion
    }
}