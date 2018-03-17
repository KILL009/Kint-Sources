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
    public class CardDAO : ICardDAO
    {
        #region Methods

        public CardDTO Insert(ref CardDTO card)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Card entity = new Card();
                    Mapper.Mapper.Instance.CardMapper.ToCard(card, entity);
                    context.Card.Add(entity);
                    context.SaveChanges();
                    if(Mapper.Mapper.Instance.CardMapper.ToCardDTO(entity, card))
                    {
                        return card;
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

        public void Insert(List<CardDTO> cards)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    foreach (CardDTO card in cards)
                    {
                        Card entity = new Card();
                        Mapper.Mapper.Instance.CardMapper.ToCard(card, entity);
                        context.Card.Add(entity);
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

        public IEnumerable<CardDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<CardDTO> result = new List<CardDTO>();
                foreach (Card card in context.Card)
                {
                    CardDTO dto = new CardDTO();
                    Mapper.Mapper.Instance.CardMapper.ToCardDTO(card, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public CardDTO LoadById(short cardId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    CardDTO dto = new CardDTO();
                    if(Mapper.Mapper.Instance.CardMapper.ToCardDTO(context.Card.FirstOrDefault(s => s.CardId.Equals(cardId)), dto))
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