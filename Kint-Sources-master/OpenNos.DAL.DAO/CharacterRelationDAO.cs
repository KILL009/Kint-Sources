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
    public class CharacterRelationDAO : ICharacterRelationDAO
    {
        #region Methods

        public DeleteResult Delete(long characterRelationId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    CharacterRelation relation = context.CharacterRelation.SingleOrDefault(c => c.CharacterRelationId.Equals(characterRelationId));

                    if (relation != null)
                    {
                        context.CharacterRelation.Remove(relation);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_CHARACTER_ERROR"), characterRelationId, e.Message), e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref CharacterRelationDTO characterRelation)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long characterId = characterRelation.CharacterId;
                    long relatedCharacterId = characterRelation.RelatedCharacterId;
                    CharacterRelation entity = context.CharacterRelation.FirstOrDefault(c => c.CharacterId.Equals(characterId) && c.RelatedCharacterId.Equals(relatedCharacterId));

                    if (entity == null)
                    {
                        characterRelation = insert(characterRelation, context);
                        return SaveResult.Inserted;
                    }
                    characterRelation = update(entity, characterRelation, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_CHARACTERRELATION_ERROR"), characterRelation.CharacterRelationId, e.Message), e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<CharacterRelationDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<CharacterRelationDTO> result = new List<CharacterRelationDTO>();
                foreach (CharacterRelation entity in context.CharacterRelation)
                {
                    CharacterRelationDTO dto = new CharacterRelationDTO();
                    Mapper.Mapper.Instance.CharacterRelationMapper.ToCharacterRelationDTO(entity, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public CharacterRelationDTO LoadById(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    CharacterRelationDTO dto = new CharacterRelationDTO();
                    if(Mapper.Mapper.Instance.CharacterRelationMapper.ToCharacterRelationDTO(context.CharacterRelation.FirstOrDefault(s => s.CharacterRelationId.Equals(characterId)), dto))
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

        private CharacterRelationDTO insert(CharacterRelationDTO relation, OpenNosContext context)
        {
            CharacterRelation entity = new CharacterRelation();
            Mapper.Mapper.Instance.CharacterRelationMapper.ToCharacterRelation(relation, entity);
            context.CharacterRelation.Add(entity);
            context.SaveChanges();
            if(Mapper.Mapper.Instance.CharacterRelationMapper.ToCharacterRelationDTO(entity, relation))
            {
                return relation;
            }

            return null;
        }

        private CharacterRelationDTO update(CharacterRelation entity, CharacterRelationDTO relation, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mapper.Instance.CharacterRelationMapper.ToCharacterRelation(relation, entity);
                context.SaveChanges();
            }

            if(Mapper.Mapper.Instance.CharacterRelationMapper.ToCharacterRelationDTO(entity, relation))
            {
                return relation;
            }

            return null;
        }

        #endregion
    }
}