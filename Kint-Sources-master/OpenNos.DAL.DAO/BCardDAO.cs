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
    public class BCardDAO : IBCardDAO
    {
        #region Methods

        public BCardDTO Insert(ref BCardDTO cardObject)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    BCard entity = new BCard();
                    Mapper.Mapper.Instance.BCardMapper.ToBCard(cardObject, entity);
                    context.BCard.Add(entity);
                    context.SaveChanges();
                    if(Mapper.Mapper.Instance.BCardMapper.ToBCardDTO(entity, cardObject))
                    {
                        return cardObject;
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

        public void Insert(List<BCardDTO> cards)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    foreach (BCardDTO card in cards)
                    {
                        BCard entity = new BCard();
                        Mapper.Mapper.Instance.BCardMapper.ToBCard(card, entity);
                        context.BCard.Add(entity);
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

        public IEnumerable<BCardDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<BCardDTO> result = new List<BCardDTO>();
                foreach (BCard card in context.BCard)
                {
                    BCardDTO dto = new BCardDTO();
                    Mapper.Mapper.Instance.BCardMapper.ToBCardDTO(card, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public IEnumerable<BCardDTO> LoadByCardId(short cardId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<BCardDTO> result = new List<BCardDTO>();
                foreach (BCard card in context.BCard.Where(s => s.CardId == cardId))
                {
                    BCardDTO dto = new BCardDTO();
                    Mapper.Mapper.Instance.BCardMapper.ToBCardDTO(card, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public BCardDTO LoadById(short cardId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    BCardDTO dto = new BCardDTO();
                    if(Mapper.Mapper.Instance.BCardMapper.ToBCardDTO(context.BCard.FirstOrDefault(s => s.BCardId.Equals(cardId)), dto))
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

        public IEnumerable<BCardDTO> LoadByItemVNum(short vNum)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<BCardDTO> result = new List<BCardDTO>();
                foreach (BCard card in context.BCard.Where(s => s.ItemVNum == vNum))
                {
                    BCardDTO dto = new BCardDTO();
                    Mapper.Mapper.Instance.BCardMapper.ToBCardDTO(card, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public IEnumerable<BCardDTO> LoadByNpcMonsterVNum(short vNum)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<BCardDTO> result = new List<BCardDTO>();
                foreach (BCard card in context.BCard.Where(s => s.NpcMonsterVNum == vNum))
                {
                    BCardDTO dto = new BCardDTO();
                    Mapper.Mapper.Instance.BCardMapper.ToBCardDTO(card, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public IEnumerable<BCardDTO> LoadBySkillVNum(short vNum)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<BCardDTO> result = new List<BCardDTO>();
                foreach (BCard card in context.BCard.Where(s => s.SkillVNum == vNum))
                {
                    BCardDTO dto = new BCardDTO();
                    Mapper.Mapper.Instance.BCardMapper.ToBCardDTO(card, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        #endregion
    }
}