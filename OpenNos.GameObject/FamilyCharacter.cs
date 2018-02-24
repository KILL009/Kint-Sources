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

using OpenNos.DAL;
using OpenNos.Data;

namespace OpenNos.GameObject
{
    public class FamilyCharacter : FamilyCharacterDTO
    {
        #region Members

        private CharacterDTO _character;

        #endregion

        #region Instantiation

        public FamilyCharacter()
        {

        }

        public FamilyCharacter(FamilyCharacterDTO input)
        {
            Authority = input.Authority;
            CharacterId = input.CharacterId;
            DailyMessage = input.DailyMessage;
            Experience = input.Experience;
            FamilyCharacterId = input.FamilyCharacterId;
            FamilyId = input.FamilyId;
            Rank = input.Rank;
        }

        #endregion

        #region Properties

        public CharacterDTO Character => _character ?? (_character = DAOFactory.CharacterDAO.LoadById(CharacterId));

        #endregion
    }
}