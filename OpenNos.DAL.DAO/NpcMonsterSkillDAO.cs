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
    public class NpcMonsterSkillDAO : INpcMonsterSkillDAO
    {
        #region Methods

        public NpcMonsterSkillDTO Insert(ref NpcMonsterSkillDTO npcMonsterSkill)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    NpcMonsterSkill entity = new NpcMonsterSkill();
                    Mapper.Mapper.Instance.NpcMonsterSkillMapper.ToNpcMonsterSkill(npcMonsterSkill, entity);
                    context.NpcMonsterSkill.Add(entity);
                    context.SaveChanges();
                    if(Mapper.Mapper.Instance.NpcMonsterSkillMapper.ToNpcMonsterSkillDTO(entity, npcMonsterSkill))
                    {
                        return npcMonsterSkill;
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

        public void Insert(List<NpcMonsterSkillDTO> skills)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    foreach (NpcMonsterSkillDTO Skill in skills)
                    {
                        NpcMonsterSkill entity = new NpcMonsterSkill();
                        Mapper.Mapper.Instance.NpcMonsterSkillMapper.ToNpcMonsterSkill(Skill, entity);
                        context.NpcMonsterSkill.Add(entity);
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

        public List<NpcMonsterSkillDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<NpcMonsterSkillDTO> result = new List<NpcMonsterSkillDTO>();
                foreach (NpcMonsterSkill NpcMonsterSkillobject in context.NpcMonsterSkill)
                {
                    NpcMonsterSkillDTO dto = new NpcMonsterSkillDTO();
                    Mapper.Mapper.Instance.NpcMonsterSkillMapper.ToNpcMonsterSkillDTO(NpcMonsterSkillobject, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public IEnumerable<NpcMonsterSkillDTO> LoadByNpcMonster(short npcId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<NpcMonsterSkillDTO> result = new List<NpcMonsterSkillDTO>();
                foreach (NpcMonsterSkill NpcMonsterSkillobject in context.NpcMonsterSkill.Where(i => i.NpcMonsterVNum == npcId))
                {
                    NpcMonsterSkillDTO dto = new NpcMonsterSkillDTO();
                    Mapper.Mapper.Instance.NpcMonsterSkillMapper.ToNpcMonsterSkillDTO(NpcMonsterSkillobject, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        #endregion
    }
}