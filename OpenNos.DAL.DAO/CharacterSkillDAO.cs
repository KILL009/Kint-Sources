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
    public class CharacterSkillDAO : ICharacterSkillDAO
    {
        #region Methods

        public DeleteResult Delete(long characterId, short skillVNum)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    CharacterSkill invItem = context.CharacterSkill.FirstOrDefault(i => i.CharacterId == characterId && i.SkillVNum == skillVNum);
                    if (invItem != null)
                    {
                        context.CharacterSkill.Remove(invItem);
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

        public IEnumerable<CharacterSkillDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<CharacterSkillDTO> result = new List<CharacterSkillDTO>();
                foreach (CharacterSkill entity in context.CharacterSkill.Where(i => i.CharacterId == characterId))
                {
                    CharacterSkillDTO output = new CharacterSkillDTO();
                    Mapper.Mapper.Instance.CharacterSkillMapper.ToCharacterSkillDTO(entity, output);
                    result.Add(output);
                }
                return result;
            }
        }

        public IEnumerable<Guid> LoadKeysByCharacterId(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return context.CharacterSkill.Where(i => i.CharacterId == characterId).Select(c => c.Id).ToList();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public DeleteResult Delete(Guid id)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                CharacterSkill entity = context.Set<CharacterSkill>().FirstOrDefault(i => i.Id == id);
                if (entity != null)
                {
                    context.Set<CharacterSkill>().Remove(entity);
                    context.SaveChanges();
                }
                return DeleteResult.Deleted;
            }
        }

        public IEnumerable<CharacterSkillDTO> InsertOrUpdate(IEnumerable<CharacterSkillDTO> dtos)
        {
            try
            {
                IList<CharacterSkillDTO> results = new List<CharacterSkillDTO>();
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (CharacterSkillDTO dto in dtos)
                    {
                        results.Add(InsertOrUpdate(context, dto));
                    }
                }
                return results;
            }
            catch (Exception e)
            {
                Logger.Error($"Message: {e.Message}", e);
                return Enumerable.Empty<CharacterSkillDTO>();
            }
        }

        public CharacterSkillDTO InsertOrUpdate(CharacterSkillDTO dto)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return InsertOrUpdate(context, dto);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Message: {e.Message}", e);
                return null;
            }
        }

        public CharacterSkillDTO LoadById(Guid id)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                CharacterSkillDTO characterSkillDTO = new CharacterSkillDTO();
                if(Mapper.Mapper.Instance.CharacterSkillMapper.ToCharacterSkillDTO(context.CharacterSkill.FirstOrDefault(i => i.Id.Equals(id)), characterSkillDTO))
                {
                    return characterSkillDTO;
                }

                return null;
            }
        }

        protected CharacterSkillDTO Insert(CharacterSkillDTO dto, OpenNosContext context)
        {
            CharacterSkill entity = new CharacterSkill();
            Mapper.Mapper.Instance.CharacterSkillMapper.ToCharacterSkill(dto, entity);
            context.Set<CharacterSkill>().Add(entity);
            context.SaveChanges();
            if(Mapper.Mapper.Instance.CharacterSkillMapper.ToCharacterSkillDTO(entity, dto))
            {
                return dto;
            }

            return null;
        }

        protected CharacterSkillDTO InsertOrUpdate(OpenNosContext context, CharacterSkillDTO dto)
        {
            Guid primaryKey = dto.Id;
            CharacterSkill entity = context.Set<CharacterSkill>().FirstOrDefault(c => c.Id == primaryKey);
            if (entity == null)
            {
                return Insert(dto, context);
            }
            else
            {
                return Update(entity, dto, context);
            }
        }

        protected CharacterSkillDTO Update(CharacterSkill entity, CharacterSkillDTO inventory, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mapper.Instance.CharacterSkillMapper.ToCharacterSkill(inventory, entity);
                context.SaveChanges();
            }
            if(Mapper.Mapper.Instance.CharacterSkillMapper.ToCharacterSkillDTO(entity, inventory))
            {
                return inventory;
            }

            return null;
        }


        #endregion
    }
}