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

using OpenNos.Data;
using System.Collections.Generic;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject
{
    public class Shop : ShopDTO
    {
        #region Instantiation

        public Shop()
        {
        }

        public Shop(ShopDTO input)
        {
            MapNpcId = input.MapNpcId;
            MenuType = input.MenuType;
            Name = input.Name;
            ShopId = input.ShopId;
            ShopType = input.ShopType;
        }

        #endregion

        #region Properties

        public List<ShopItemDTO> ShopItems { get; set; }

        public List<ShopSkillDTO> ShopSkills { get; set; }

        #endregion

        #region Methods

        public void Initialize()
        {
            ShopItems = ServerManager.Instance.GetShopItemsByShopId(ShopId);
            ShopSkills = ServerManager.Instance.GetShopSkillsByShopId(ShopId);
        }

        #endregion
    }
}