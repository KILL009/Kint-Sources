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

using OpenNos.SCS.Communication.Scs.Communication.EndPoints.Tcp;
using OpenNos.SCS.Communication.ScsServices.Service;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Domain;
using OpenNos.Master.Library.Data;
using OpenNos.Master.Library.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reactive.Linq;
using OpenNos.Data;

namespace OpenNos.Master.Server
{
    internal class AdminToolService : ScsService, IAdminToolService
    {
        public void AddPenalty(long accountId, PenaltyType penaltyType, DateTime dateEnd, string reason)
        {
            if (!MSManager.Instance.AuthentificatedAdmins.ContainsKey(CurrentClient.ClientId) || MSManager.Instance.AuthentificatedAdmins[CurrentClient.ClientId].Authority != AuthorityType.GameMaster)
            {
                return;
            }
            PenaltyLogDTO penalty = new PenaltyLogDTO()
            {
                AccountId = accountId,
                Penalty = penaltyType,
                DateStart = DateTime.Now,
                DateEnd = dateEnd,
                Reason = reason,
                AdminName = MSManager.Instance.AuthentificatedAdmins[CurrentClient.ClientId].Name
            };
            DAOFactory.PenaltyLogDAO.InsertOrUpdate(ref penalty);
        }

        public byte Authenticate(string username, string password)
        {
            AccountDTO account = DAOFactory.AccountDAO.LoadByName(username);

            if (account != null && account.Password.Equals(password) && account.Authority >= AuthorityType.Moderator)
            {
                MSManager.Instance.AuthentificatedAdmins[CurrentClient.ClientId] = account;
                return (byte)account.Authority;
            }
            return 0;
        }

        public AccountDTO GetAccountDetails(string accountName)
        {
            if (!MSManager.Instance.AuthentificatedAdmins.ContainsKey(CurrentClient.ClientId))
            {
                return null;
            }
            return DAOFactory.AccountDAO.LoadByName(accountName);
        }

        public List<BazaarItemDTO> GetAllBazaarItems(long characterId)
        {
            if (!MSManager.Instance.AuthentificatedAdmins.ContainsKey(CurrentClient.ClientId))
            {
                return null;
            }
            throw new NotImplementedException();
        }

        public List<CharacterDTO> GetAllCharacters(long accountId)
        {
            if (!MSManager.Instance.AuthentificatedAdmins.ContainsKey(CurrentClient.ClientId))
            {
                return null;
            }
            return DAOFactory.CharacterDAO.LoadByAccount(accountId).ToList();
        }

        public List<ItemInstanceDTO> GetAllInventoryItems(long characterId, InventoryType type)
        {
            if (!MSManager.Instance.AuthentificatedAdmins.ContainsKey(CurrentClient.ClientId))
            {
                return null;
            }
            return DAOFactory.IteminstanceDAO.LoadByType(characterId, type).ToList();
        }

        public List<MapInstance> GetAllMapInstances(Guid worldId, short mapId = -1)
        {
            if (!MSManager.Instance.AuthentificatedAdmins.ContainsKey(CurrentClient.ClientId))
            {
                return null;
            }
            throw new NotImplementedException();
        }

        public CharacterDTO GetCharacterDetails(string characterName)
        {
            if (!MSManager.Instance.AuthentificatedAdmins.ContainsKey(CurrentClient.ClientId))
            {
                return null;
            }
            return DAOFactory.CharacterDAO.LoadByName(characterName);
        }

        public List<PenaltyLogDTO> GetPenalties(long accountId)
        {
            if (!MSManager.Instance.AuthentificatedAdmins.ContainsKey(CurrentClient.ClientId))
            {
                return null;
            }
            return DAOFactory.PenaltyLogDAO.LoadByAccount(accountId).ToList();
        }

        public void Kick(long accountId)
        {
            if (!MSManager.Instance.AuthentificatedAdmins.ContainsKey(CurrentClient.ClientId))
            {
                return;
            }
            MSManager.Instance.ConnectedAccounts.Where(s => s.AccountId.Equals(accountId)).FirstOrDefault().ConnectedWorld?.ServiceClient.GetClientProxy<ICommunicationClient>().KickSession(accountId, null);
        }

        public void SaveCharacter(long characterId)
        {
            if (!MSManager.Instance.AuthentificatedAdmins.ContainsKey(CurrentClient.ClientId))
            {
                return;
            }
            throw new NotImplementedException();
        }

        public void SendItem(string senderName, long receivingCharacterId, short vnum, byte amount = 1, byte rare = 0, byte upgrade = 0, bool isNosmall = false)
        {
            if (!MSManager.Instance.AuthentificatedAdmins.ContainsKey(CurrentClient.ClientId) || MSManager.Instance.AuthentificatedAdmins[CurrentClient.ClientId].Authority != AuthorityType.GameMaster)
            {
                return;
            }
            //TODO: MailRefresh Call to Server
            long charId = DAOFactory.CharacterDAO.LoadByAccount(MSManager.Instance.AuthentificatedAdmins[CurrentClient.ClientId].AccountId).FirstOrDefault().CharacterId;
            MailDTO mail = new MailDTO
            {
                AttachmentAmount = amount,
                IsOpened = false,
                Date = DateTime.Now,
                ReceiverId = receivingCharacterId,
                SenderId = charId,
                AttachmentRarity = (byte)rare,
                AttachmentUpgrade = upgrade,
                IsSenderCopy = false,
                Title = isNosmall ? "NOSMALL" : senderName,
                AttachmentVNum = vnum,
                SenderClass = 0,
                SenderGender = 0,
                SenderHairColor = 0,
                SenderHairStyle = 0,
                EqPacket = "-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1",
                SenderMorphId = -1
            };
            DAOFactory.MailDAO.InsertOrUpdate(ref mail);
        }

        public void SendItemToMap(string senderName, Guid mapInstanceId, short vnum, byte amount = 1, byte rare = 0, byte Upgrade = 0, bool isNosmall = false)
        {
            if (!MSManager.Instance.AuthentificatedAdmins.ContainsKey(CurrentClient.ClientId))
            {
                return;
            }
            throw new NotImplementedException();
        }

        public void SendPacket(string packet, long characterId)
        {
            if (!MSManager.Instance.AuthentificatedAdmins.ContainsKey(CurrentClient.ClientId) || MSManager.Instance.AuthentificatedAdmins[CurrentClient.ClientId].Authority != AuthorityType.GameMaster)
            {
                return;
            }
            long charId = DAOFactory.CharacterDAO.LoadByAccount(MSManager.Instance.AuthentificatedAdmins[CurrentClient.ClientId].AccountId).FirstOrDefault().CharacterId;

            AccountConnection account = MSManager.Instance.ConnectedAccounts.FirstOrDefault(a => a.CharacterId.Equals(characterId));
            if (account != null && account.ConnectedWorld != null)
            {
                SCSCharacterMessage message = new SCSCharacterMessage()
                {
                    SourceCharacterId = charId,
                    SourceWorldId = MSManager.Instance.WorldServers.First().Id,
                    DestinationCharacterId = characterId,
                    Type = MessageType.PrivateChat,
                    Message = packet
                };
                account.ConnectedWorld.ServiceClient.GetClientProxy<ICommunicationClient>().SendMessageToCharacter(message);
            }
        }
     
       public void Shout(Guid destChannelId, string message)
        {
            if (!MSManager.Instance.AuthentificatedAdmins.ContainsKey(CurrentClient.ClientId) || MSManager.Instance.AuthentificatedAdmins[CurrentClient.ClientId].Authority != AuthorityType.GameMaster)
            {
                return;
            }
            long charId = DAOFactory.CharacterDAO.LoadByAccount(MSManager.Instance.AuthentificatedAdmins[CurrentClient.ClientId].AccountId).FirstOrDefault().CharacterId;

            SCSCharacterMessage scsMessage = new SCSCharacterMessage()
            {
                DestinationCharacterId = null,
                SourceCharacterId = charId,
                SourceWorldId = MSManager.Instance.WorldServers.First().Id,
                Message = message,
                Type = MessageType.Shout
            };
            MSManager.Instance.WorldServers.Where(s => s.Id.Equals(destChannelId)).First().ServiceClient.GetClientProxy<ICommunicationClient>().SendMessageToCharacter(scsMessage);
        }

        public void Shutdown(Guid channelId)
        {
            if (!MSManager.Instance.AuthentificatedAdmins.ContainsKey(CurrentClient.ClientId) || MSManager.Instance.AuthentificatedAdmins[CurrentClient.ClientId].Authority != AuthorityType.GameMaster)
            {
                return;
            }
            MSManager.Instance.WorldServers.Where(s => s.Id.Equals(channelId)).First().ServiceClient.GetClientProxy<ICommunicationClient>().Shutdown();
        }

        public void SummonNPCMonster(short npcMonsterVnum, short amount, Guid mapInstanceId, short mapX = 0, short mapY = 0)
        {
            if (!MSManager.Instance.AuthentificatedAdmins.ContainsKey(CurrentClient.ClientId) || MSManager.Instance.AuthentificatedAdmins[CurrentClient.ClientId].Authority != AuthorityType.GameMaster)
            {
                return;
            }
            throw new NotImplementedException();
        }

        public void TeleportToPlayer(long sourceCharacter, long destCharacter)
        {
            if (!MSManager.Instance.AuthentificatedAdmins.ContainsKey(CurrentClient.ClientId))
            {
                return;
            }
            throw new NotImplementedException();
        }

        public void Teleport(long sourceCharacter, short mapId, short mapX, short mapY)
        {
            if (!MSManager.Instance.AuthentificatedAdmins.ContainsKey(CurrentClient.ClientId))
            {
                return;
            }
            throw new NotImplementedException();
        }

        public void SendPacketToMap(string packet, Guid mapInstanceId)
        {
            if (!MSManager.Instance.AuthentificatedAdmins.ContainsKey(CurrentClient.ClientId) || MSManager.Instance.AuthentificatedAdmins[CurrentClient.ClientId].Authority != AuthorityType.GameMaster)
            {
                return;
            }
            throw new NotImplementedException();
        }

        public void SendItemToMap(Guid channelId, string senderName, Guid mapInstanceId, short vnum, byte amount = 1, byte rare = 0, byte Upgrade = 0, bool isNosmall = false)
        {
            throw new NotImplementedException();
        }

      
        public void SendPacketToMap(Guid channelId, string packet, Guid mapInstanceId)
        {
            throw new NotImplementedException();
        }

        public void SummonNPCMonster(Guid channelId, short npcMonsterVnum, short amount, Guid mapInstanceId, short mapX = 0, short mapY = 0)
        {
            throw new NotImplementedException();
        }
    }
}
