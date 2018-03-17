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
using OpenNos.Domain;
using OpenNos.Master.Library.Data;
using OpenNos.Master.Library.Interface;
using OpenNos.SCS.Communication.ScsServices.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reactive.Linq;

namespace OpenNos.Master.Server
{
    internal class MallService : ScsService, IMallService
    {
        public bool Authenticate(string authKey)
        {
            if (string.IsNullOrWhiteSpace(authKey))
            {
                return false;
            }

            if (authKey == ConfigurationManager.AppSettings["MallAuthKey"])
            {
                MSManager.Instance.AuthentificatedClients.Add(CurrentClient.ClientId);
                return true;
            }

            return false;
        }

        public bool IsAuthenticated() => MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId));

        public AuthorityType GetAuthority(long accountId)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return AuthorityType.Closed;
            }

            return DAOFactory.AccountDAO.LoadById(accountId)?.Authority ?? AuthorityType.Closed;
        }

        public IEnumerable<CharacterDTO> GetCharacters(long accountId)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return null;
            }

            return DAOFactory.CharacterDAO.LoadByAccount(accountId);
        }

        public void SendItem(long characterId, MallItem item)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return;
            }
            ItemDTO dto = DAOFactory.ItemDAO.LoadById(item.ItemVNum);
            if (dto != null)
            {
                int limit = 99;

                if (dto.Type == InventoryType.Equipment || dto.Type == InventoryType.Miniland)
                {
                    limit = 1;
                }

                do
                {
                    MailDTO mailDTO = new MailDTO
                    {
                        AttachmentAmount = (byte)(item.Amount > limit ? limit : item.Amount),
                        AttachmentRarity = item.Rare,
                        AttachmentUpgrade = item.Upgrade,
                        AttachmentVNum = item.ItemVNum,
                        Date = DateTime.Now,
                        EqPacket = string.Empty,
                        IsOpened = false,
                        IsSenderCopy = false,
                        Message = string.Empty,
                        ReceiverId = characterId,
                        SenderId = characterId,
                        Title = "NOSMALL"
                    };

                    DAOFactory.MailDAO.InsertOrUpdate(ref mailDTO);

                    AccountConnection account = MSManager.Instance.ConnectedAccounts.Find(a => a.CharacterId.Equals(mailDTO.ReceiverId));
                    if (account?.ConnectedWorld != null)
                    {
                        account.ConnectedWorld.MailServiceClient.GetClientProxy<IMailClient>().MailSent(mailDTO);
                    }

                    item.Amount -= limit;
                } while (item.Amount > 0);
            }
        }

        public void SendStaticBonus(long characterId, MallStaticBonus item)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return;
            }
            StaticBonusDTO dto = DAOFactory.StaticBonusDAO.LoadByCharacterId(characterId).FirstOrDefault(s => s.StaticBonusType == item.StaticBonus);

            if (dto != null)
            {
                dto.DateEnd.AddSeconds(item.Seconds);
            }
            else
            {
                dto = new StaticBonusDTO()
                {
                    CharacterId = characterId,
                    DateEnd = DateTime.Now.AddSeconds(item.Seconds),
                    StaticBonusType = item.StaticBonus
                };
            }

            DAOFactory.StaticBonusDAO.InsertOrUpdate(ref dto);
            AccountConnection account = MSManager.Instance.ConnectedAccounts.Find(a => a.CharacterId.Equals(characterId));
            if (account?.ConnectedWorld != null)
            {
                account.ConnectedWorld.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().UpdateStaticBonus(characterId);
            }
        }

        public AccountDTO ValidateAccount(string userName, string passHash)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passHash))
            {
                return null;
            }

            AccountDTO account = DAOFactory.AccountDAO.LoadByName(userName);

            if (account?.Password == passHash)
            {
                return account;
            }
            return null;
        }
    }
}
