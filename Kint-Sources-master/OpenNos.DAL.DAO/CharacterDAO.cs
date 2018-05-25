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
    public class CharacterDAO : ICharacterDAO
    {
        #region Methods

        public DeleteResult DeleteByPrimaryKey(long accountId, byte characterSlot)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    // actually a Character wont be deleted, it just will be disabled for future traces
                    Character character = context.Character.SingleOrDefault(c => c.AccountId.Equals(accountId) && c.Slot.Equals(characterSlot) && c.State.Equals((byte)CharacterState.Active));

                    if (character != null)
                    {
                        character.State = (byte)CharacterState.Inactive;
                        CharacterDTO dto = new CharacterDTO();
                        Mapper.Mapper.Instance.CharacterMapper.ToCharacterDTO(character, dto);
                        update(character, dto, context);
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_CHARACTER_ERROR"), characterSlot, e.Message), e);
                return DeleteResult.Error;
            }
        }

        /// <summary>
        /// Returns first 30 occurences of highest Compliment
        /// </summary>
        /// <returns></returns>
        public List<CharacterDTO> GetTopCompliment()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<CharacterDTO> result = new List<CharacterDTO>();
                foreach(Character entity in context.Character.Where(c => c.Account.Authority == AuthorityType.User && !c.Account.PenaltyLog.Any(l => l.Penalty == PenaltyType.Banned && l.DateEnd > DateTime.Now)).OrderByDescending(c => c.Compliment).Take(30))
                {
                    CharacterDTO dto = new CharacterDTO();
                    Mapper.Mapper.Instance.CharacterMapper.ToCharacterDTO(entity, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        /// <summary>
        /// Returns first 30 occurences of highest Act4Points
        /// </summary>
        /// <returns></returns>
        public List<CharacterDTO> GetTopPoints()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<CharacterDTO> result = new List<CharacterDTO>();
                foreach (Character entity in context.Character.Where(c => c.Account.Authority == AuthorityType.User && !c.Account.PenaltyLog.Any(l => l.Penalty == PenaltyType.Banned && l.DateEnd > DateTime.Now)).OrderByDescending(c => c.Act4Points).Take(30))
                {
                    CharacterDTO dto = new CharacterDTO();
                    Mapper.Mapper.Instance.CharacterMapper.ToCharacterDTO(entity, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        /// <summary>
        /// Returns first 30 occurences of highest Reputation
        /// </summary>
        /// <returns></returns>
        public List<CharacterDTO> GetTopReputation()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<CharacterDTO> result = new List<CharacterDTO>();
                foreach(Character entity in context.Character.Where(c => c.Account.Authority == AuthorityType.User && !c.Account.PenaltyLog.Any(l => l.Penalty == PenaltyType.Banned && l.DateEnd > DateTime.Now)).OrderByDescending(c => c.Reputation).Take(43))
                {
                    CharacterDTO dto = new CharacterDTO();
                    Mapper.Mapper.Instance.CharacterMapper.ToCharacterDTO(entity, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public SaveResult InsertOrUpdate(ref CharacterDTO character)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long characterId = character.CharacterId;
                    Character entity = context.Character.FirstOrDefault(c => c.CharacterId.Equals(characterId));
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

        [Obsolete("LoadAll is obsolete, create a separate DAO statement for your function")]
        public IEnumerable<CharacterDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<CharacterDTO> result = new List<CharacterDTO>();
                foreach (Character chara in context.Character)
                {
                    CharacterDTO dto = new CharacterDTO();
                    Mapper.Mapper.Instance.CharacterMapper.ToCharacterDTO(chara, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public IEnumerable<CharacterDTO> LoadAllByAccount(long accountId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<CharacterDTO> result = new List<CharacterDTO>();
                foreach(Character entity in context.Character.Where(c => c.AccountId.Equals(accountId)).OrderByDescending(c => c.Slot))
                {
                    CharacterDTO dto = new CharacterDTO();
                    Mapper.Mapper.Instance.CharacterMapper.ToCharacterDTO(entity, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public IEnumerable<CharacterDTO> LoadAllCharactersByAccount(long accountId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<CharacterDTO> result = new List<CharacterDTO>();
                foreach (Character entity in context.Character.Where(c => c.AccountId.Equals(accountId) && c.State.Equals((byte)CharacterState.Active)).OrderByDescending(c => c.Slot))
                {
                    CharacterDTO dto = new CharacterDTO();
                    Mapper.Mapper.Instance.CharacterMapper.ToCharacterDTO(entity, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public CharacterDTO LoadById(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    CharacterDTO dto = new CharacterDTO();
                    if(Mapper.Mapper.Instance.CharacterMapper.ToCharacterDTO(context.Character.FirstOrDefault(c => c.CharacterId.Equals(characterId)), dto))
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

        public CharacterDTO LoadByName(string name)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    CharacterDTO dto = new CharacterDTO();
                    if(Mapper.Mapper.Instance.CharacterMapper.ToCharacterDTO(context.Character.SingleOrDefault(c => c.Name.Equals(name) && c.State.Equals((byte)CharacterState.Active)), dto))
                    {
                        return dto;
                    }

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return null;
        }

        public CharacterDTO LoadBySlot(long accountId, byte slot)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    CharacterDTO dto = new CharacterDTO();
                    if(Mapper.Mapper.Instance.CharacterMapper.ToCharacterDTO(context.Character.SingleOrDefault(c => c.AccountId.Equals(accountId) && c.Slot.Equals(slot) && c.State.Equals((byte)CharacterState.Active)), dto))
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

        private CharacterDTO insert(CharacterDTO character, OpenNosContext context)
        {
            Character entity = new Character();
            Mapper.Mapper.Instance.CharacterMapper.ToCharacter(character, entity);
            context.Character.Add(entity);
            context.SaveChanges();
            if(Mapper.Mapper.Instance.CharacterMapper.ToCharacterDTO(entity, character))
            {
                return character;
            }

            return null;
        }

        private CharacterDTO update(Character entity, CharacterDTO character, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mapper.Instance.CharacterMapper.ToCharacter(character, entity);
                context.SaveChanges();
            }

            if(Mapper.Mapper.Instance.CharacterMapper.ToCharacterDTO(entity, character))
            {
                return character;
            }

            return null;
        }

        #endregion
    }
}