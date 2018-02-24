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
using OpenNos.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.DAL.DAO
{
    public class FamilyCharacterDAO : IFamilyCharacterDAO
    {
        #region Methods

        public DeleteResult Delete(string characterName)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Character character = context.Character.FirstOrDefault(c => c.Name.Equals(characterName) && c.State == (byte)CharacterState.Active);
                    FamilyCharacter familyCharacter = context.FamilyCharacter.FirstOrDefault(c => c.CharacterId.Equals(character.CharacterId));
                    if (character != null && familyCharacter != null)
                    {
                        context.FamilyCharacter.Remove(familyCharacter);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_FAMILYCHARACTER_ERROR"), e.Message), e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref FamilyCharacterDTO character)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long familyCharacterId = character.FamilyCharacterId;
                    FamilyCharacter entity = context.FamilyCharacter.FirstOrDefault(c => c.FamilyCharacterId.Equals(familyCharacterId));

                    if (entity == null)
                    {
                        character = insert(character, context);
                        return SaveResult.Inserted;
                    }

                    character = update(entity, character, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("INSERT_ERROR"), character, e.Message), e);
                return SaveResult.Error;
            }
        }

        public FamilyCharacterDTO LoadByCharacterId(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    FamilyCharacterDTO dto = new FamilyCharacterDTO();
                    if(Mapper.Mapper.Instance.FamilyCharacterMapper.ToFamilyCharacterDTO(context.FamilyCharacter.FirstOrDefault(c => c.CharacterId == characterId), dto))
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

        public IList<FamilyCharacterDTO> LoadByFamilyId(long familyId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<FamilyCharacterDTO> result = new List<FamilyCharacterDTO>();
                foreach(FamilyCharacter entity in context.FamilyCharacter.Where(fc => fc.FamilyId.Equals(familyId)))
                {
                    FamilyCharacterDTO dto = new FamilyCharacterDTO();
                    Mapper.Mapper.Instance.FamilyCharacterMapper.ToFamilyCharacterDTO(entity, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public FamilyCharacterDTO LoadById(long familyCharacterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    FamilyCharacterDTO dto = new FamilyCharacterDTO();
                    if(Mapper.Mapper.Instance.FamilyCharacterMapper.ToFamilyCharacterDTO(context.FamilyCharacter.FirstOrDefault(c => c.FamilyCharacterId.Equals(familyCharacterId)), dto))
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

        private FamilyCharacterDTO insert(FamilyCharacterDTO character, OpenNosContext context)
        {
            FamilyCharacter entity = new FamilyCharacter();
            Mapper.Mapper.Instance.FamilyCharacterMapper.ToFamilyCharacter(character, entity);
            context.FamilyCharacter.Add(entity);
            context.SaveChanges();
            if(Mapper.Mapper.Instance.FamilyCharacterMapper.ToFamilyCharacterDTO(entity, character))
            {
                return character;
            }

            return null;
        }

        private FamilyCharacterDTO update(FamilyCharacter entity, FamilyCharacterDTO character, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mapper.Instance.FamilyCharacterMapper.ToFamilyCharacter(character, entity);
                context.SaveChanges();
            }
            if(Mapper.Mapper.Instance.FamilyCharacterMapper.ToFamilyCharacterDTO(entity, character))
            {
                return character;
            }

            return null;
        }

        #endregion
    }
}