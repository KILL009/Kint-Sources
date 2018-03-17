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
    public class ShopDAO : IShopDAO
    {
        #region Methods

        public DeleteResult DeleteById(int mapNpcId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Shop shop = context.Shop.First(i => i.MapNpcId.Equals(mapNpcId));
                    IEnumerable<ShopItem> shopItem = context.ShopItem.Where(s => s.ShopId.Equals(shop.ShopId));
                    IEnumerable<ShopSkill> shopSkill = context.ShopSkill.Where(s => s.ShopId.Equals(shop.ShopId));

                    if (shop != null)
                    {
                        foreach (ShopItem item in shopItem)
                        {
                            context.ShopItem.Remove(item);
                        }
                        foreach (ShopSkill skill in shopSkill)
                        {
                            context.ShopSkill.Remove(skill);
                        }
                        context.Shop.Remove(shop);
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

        public void Insert(List<ShopDTO> shops)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    foreach (ShopDTO Item in shops)
                    {
                        Shop entity = new Shop();
                        Mapper.Mapper.Instance.ShopMapper.ToShop(Item, entity);
                        context.Shop.Add(entity);
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

        public ShopDTO Insert(ShopDTO shop)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    if (context.Shop.FirstOrDefault(c => c.MapNpcId.Equals(shop.MapNpcId)) == null)
                    {
                        Shop entity = new Shop();
                        Mapper.Mapper.Instance.ShopMapper.ToShop(shop, entity);
                        context.Shop.Add(entity);
                        context.SaveChanges();
                        if(Mapper.Mapper.Instance.ShopMapper.ToShopDTO(entity, shop))
                        {
                            return shop;
                        }

                        return null;
                    }
                    return new ShopDTO();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<ShopDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<ShopDTO> result = new List<ShopDTO>();
                foreach (Shop entity in context.Shop)
                {
                    ShopDTO dto = new ShopDTO();
                    Mapper.Mapper.Instance.ShopMapper.ToShopDTO(entity, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public ShopDTO LoadById(int shopId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    ShopDTO dto = new ShopDTO();
                    if(Mapper.Mapper.Instance.ShopMapper.ToShopDTO(context.Shop.FirstOrDefault(s => s.ShopId.Equals(shopId)), dto))
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

        public ShopDTO LoadByNpc(int mapNpcId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    ShopDTO dto = new ShopDTO();
                    if(Mapper.Mapper.Instance.ShopMapper.ToShopDTO(context.Shop.FirstOrDefault(s => s.MapNpcId.Equals(mapNpcId)), dto))
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

        #endregion
    }
}