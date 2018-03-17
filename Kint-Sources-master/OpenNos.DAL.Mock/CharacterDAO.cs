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

using OpenNos.DAL.Interface;
using OpenNos.Data;
using OpenNos.Data.Enums;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.DAL.Mock
{
    public class CharacterDAO : BaseDAO<CharacterDTO>, ICharacterDAO
    {
        #region Methods

        public DeleteResult DeleteByPrimaryKey(long accountId, byte characterSlot)
        {
            CharacterDTO dto = LoadBySlot(accountId, characterSlot);
            Container.Remove(dto);
            return DeleteResult.Deleted;
        }

        public List<CharacterDTO> GetTopCompliment() => new List<CharacterDTO>();

        public List<CharacterDTO> GetTopPoints() => new List<CharacterDTO>();

        public List<CharacterDTO> GetTopReputation() => new List<CharacterDTO>();

        public override CharacterDTO Insert(CharacterDTO dto)
        {
            dto.CharacterId = Container.Count > 0 ? Container.Max(c => c.CharacterId) + 1 : 1;
            return base.Insert(dto);
        }

        public SaveResult InsertOrUpdate(ref CharacterDTO character)
        {
            CharacterDTO dto = LoadById(character.CharacterId);
            if (dto != null)
            {
                dto = character;
                return SaveResult.Updated;
            }
            Insert(character);
            return SaveResult.Inserted;
        }

        public IEnumerable<CharacterDTO> LoadAllByAccount(long accountId) => throw new System.NotImplementedException();

        public IEnumerable<CharacterDTO> LoadByAccount(long accountId) => Container.Where(c => c.AccountId == accountId).ToList();

        public CharacterDTO LoadById(long characterId) => Container.SingleOrDefault(c => c.CharacterId == characterId);

        public CharacterDTO LoadByName(string name) => Container.SingleOrDefault(c => c.Name == name);

        public CharacterDTO LoadBySlot(long accountId, byte slot) => Container.SingleOrDefault(c => c.AccountId == accountId && c.Slot == slot);

        #endregion
    }
}