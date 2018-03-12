using OpenNos.Data;
using System;
using System.Collections.Generic;

namespace OpenNos.GameObject
{
    public class Shop : ShopDTO
    {
        #region Properties

        public List<ShopItemDTO> ShopItems { get; set; }

        public List<ShopSkillDTO> ShopSkills { get; set; }

        #endregion

        #region Methods

        public override void Initialize()
        {
            ShopItems = ServerManager.Instance.GetShopItemsByShopId(ShopId);
            ShopSkills = ServerManager.Instance.GetShopSkillsByShopId(ShopId);
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}