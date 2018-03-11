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
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Packets.ClientPackets;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;
using System;
using OpenNos.GameObject.Networking;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using OpenNos.ChatLog.Networking;
using OpenNos.ChatLog.Shared;
using System.Collections.Concurrent;
using OpenNos.XMLModel.Models.Quest;
using OpenNos.Core.Extensions;
using static OpenNos.Domain.AdditionalTypes;

namespace OpenNos.Handler
{
    public class BasicPacketHandler : IPacketHandler
    {
        private object qstlist2;
        #region Instantiation

        public BasicPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        private ClientSession Session { get; }
        public object Same { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// blins packet
        /// </summary>
        /// <param name="blInsPacket"></param>
        public void BlacklistAdd(BlInsPacket blInsPacket)
        {
            Session.Character.AddRelation(blInsPacket.CharacterId, CharacterRelationType.Blocked);
            Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("BLACKLIST_ADDED")));
            Session.SendPacket(Session.Character.GenerateBlinit());
        }

        /// <summary>
        /// bldel packet
        /// </summary>
        /// <param name="blDelPacket"></param>
        public void BlacklistDelete(BlDelPacket blDelPacket)
        {
            Session.Character.DeleteBlackList(blDelPacket.CharacterId);
            Session.SendPacket(Session.Character.GenerateBlinit());
            Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("BLACKLIST_DELETED")));
        }

        /// <summary>
        /// gop packet
        /// </summary>
        /// <param name="characterOptionPacket"></param>
        public void CharacterOptionChange(CharacterOptionPacket characterOptionPacket)
        {
            switch (characterOptionPacket.Option)
            {
                case CharacterOption.BuffBlocked:
                    Session.Character.BuffBlocked = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.BuffBlocked ? "BUFF_BLOCKED" : "BUFF_UNLOCKED"), 0));
                    break;

                case CharacterOption.EmoticonsBlocked:
                    Session.Character.EmoticonsBlocked = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.EmoticonsBlocked ? "EMO_BLOCKED" : "EMO_UNLOCKED"), 0));
                    break;

                case CharacterOption.ExchangeBlocked:
                    Session.Character.ExchangeBlocked = !characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.ExchangeBlocked ? "EXCHANGE_BLOCKED" : "EXCHANGE_UNLOCKED"), 0));
                    break;

                case CharacterOption.FriendRequestBlocked:
                    Session.Character.FriendRequestBlocked = !characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.FriendRequestBlocked ? "FRIEND_REQ_BLOCKED" : "FRIEND_REQ_UNLOCKED"), 0));
                    break;

                case CharacterOption.GroupRequestBlocked:
                    Session.Character.GroupRequestBlocked = !characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.GroupRequestBlocked ? "GROUP_REQ_BLOCKED" : "GROUP_REQ_UNLOCKED"), 0));
                    break;

                case CharacterOption.HeroChatBlocked:
                    Session.Character.HeroChatBlocked = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.HeroChatBlocked ? "HERO_CHAT_BLOCKED" : "HERO_CHAT_UNLOCKED"), 0));
                    break;

                case CharacterOption.HpBlocked:
                    Session.Character.HpBlocked = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.HpBlocked ? "HP_BLOCKED" : "HP_UNLOCKED"), 0));
                    break;

                case CharacterOption.MinilandInviteBlocked:
                    Session.Character.MinilandInviteBlocked = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.MinilandInviteBlocked ? "MINI_INV_BLOCKED" : "MINI_INV_UNLOCKED"), 0));
                    break;

                case CharacterOption.MouseAimLock:
                    Session.Character.MouseAimLock = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.MouseAimLock ? "MOUSE_LOCKED" : "MOUSE_UNLOCKED"), 0));
                    break;

                case CharacterOption.QuickGetUp:
                    Session.Character.QuickGetUp = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.QuickGetUp ? "QUICK_GET_UP_ENABLED" : "QUICK_GET_UP_DISABLED"), 0));
                    break;

                case CharacterOption.WhisperBlocked:
                    Session.Character.WhisperBlocked = !characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.WhisperBlocked ? "WHISPER_BLOCKED" : "WHISPER_UNLOCKED"), 0));
                    break;

                case CharacterOption.FamilyRequestBlocked:
                    Session.Character.FamilyRequestBlocked = !characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.FamilyRequestBlocked ? "FAMILY_REQ_LOCKED" : "FAMILY_REQ_UNLOCKED"), 0));
                    break;

                case CharacterOption.GroupSharing:
                    Group grp = ServerManager.Instance.Groups.Find(g => g.IsMemberOfGroup(Session.Character.CharacterId));
                    if (grp == null)
                    {
                        return;
                    }
                    if (!grp.IsLeader(Session))
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_MASTER"), 0));
                        return;
                    }
                    if (!characterOptionPacket.IsActive)
                    {
                        Group group = ServerManager.Instance.Groups.Find(s => s.IsMemberOfGroup(Session.Character.CharacterId));
                        if (group != null)
                        {
                            group.SharingMode = 1;
                        }
                        Session.CurrentMapInstance?.Broadcast(Session, UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SHARING"), 0), ReceiverType.Group);
                    }
                    else
                    {
                        Group group = ServerManager.Instance.Groups.Find(s => s.IsMemberOfGroup(Session.Character.CharacterId));
                        if (group != null)
                        {
                            group.SharingMode = 0;
                        }
                        Session.CurrentMapInstance?.Broadcast(Session, UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SHARING_BY_ORDER"), 0), ReceiverType.Group);
                    }
                    break;
            }
            Session.SendPacket(Session.Character.GenerateStat());
        }

        /// <summary>
        /// compl packet
        /// </summary>
        /// <param name="complimentPacket"></param>
        public void Compliment(ComplimentPacket complimentPacket)
        {
            if (complimentPacket != null)
            {
                ClientSession sess = ServerManager.Instance.GetSessionByCharacterId(complimentPacket.CharacterId);
                if (sess != null)
                {
                    if (Session.Character.Level >= 30)
                    {
                        GeneralLogDTO dto =
                            Session.Character.GeneralLogs.LastOrDefault(s =>
                                s.LogData == "World" && s.LogType == "Connection");
                        GeneralLogDTO lastcompliment =
                            Session.Character.GeneralLogs.LastOrDefault(s =>
                                s.LogData == "World" && s.LogType == nameof(Compliment));
                        if (dto?.Timestamp.AddMinutes(60) <= DateTime.Now)
                        {
                            if (lastcompliment == null || lastcompliment.Timestamp.AddDays(1) <= DateTime.Now.Date)
                            {
                                sess.Character.Compliment++;
                                Session.SendPacket(Session.Character.GenerateSay(
                                    string.Format(Language.Instance.GetMessageFromKey("COMPLIMENT_GIVEN"),
                                        sess.Character.Name), 12));
                                Session.Character.GeneralLogs.Add(new GeneralLogDTO
                                {
                                    AccountId = Session.Account.AccountId,
                                    CharacterId = Session.Character.CharacterId,
                                    IpAddress = Session.IpAddress,
                                    LogData = "World",
                                    LogType = nameof(Compliment),
                                    Timestamp = DateTime.Now
                                });

                                Session.CurrentMapInstance?.Broadcast(Session,
                                    Session.Character.GenerateSay(
                                        string.Format(Language.Instance.GetMessageFromKey("COMPLIMENT_RECEIVED"),
                                            Session.Character.Name), 12), ReceiverType.OnlySomeone,
                                    characterId: complimentPacket.CharacterId);
                            }
                            else
                            {
                                Session.SendPacket(
                                    Session.Character.GenerateSay(
                                        Language.Instance.GetMessageFromKey("COMPLIMENT_COOLDOWN"), 11));
                            }
                        }
                        else if (dto != null)
                        {
                            Session.SendPacket(Session.Character.GenerateSay(
                                string.Format(Language.Instance.GetMessageFromKey("COMPLIMENT_LOGIN_COOLDOWN"),
                                    (dto.Timestamp.AddMinutes(60) - DateTime.Now).Minutes), 11));
                        }
                    }
                    else
                    {
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("COMPLIMENT_NOT_MINLVL"),
                                11));
                    }
                }
            }
        }

        /// <summary>
        /// GBox packet
        /// </summary>
        /// <param name="GBox"></param>
        public void GBoxPacket(GBox gBox)
        {
            if (gBox.Type == 1)
            {
                if (gBox.Type2 == 0)
                {
                    Session.SendPacket($"qna #gbox^1^{gBox.Amount}^1 Want to deposit {gBox.Amount}000 gold?");
                }
                else if (gBox.Type2 == 1)
                {

                    Session.SendPacket($"s_memo 6 You pay {gBox.Amount}000 gold to the bank.");
                    if ((Session.Account.BankGold + (gBox.Amount * 1000)) > ServerManager.Instance.MaxBankGold)
                    {
                        Session.SendPacket("info You can not have more than 100.000.000.000 gold on the bank!");

                        Session.SendPacket("s_memo 5 You can not have more than 100.000.000.000 gold on the bank!");
                    }
                    else if (Session.Character.Gold >= (gBox.Amount * 1000))
                    {
                        Session.Account.BankGold += (gBox.Amount * 1000);
                        Session.Character.Gold -= (gBox.Amount * 1000);
                        Session.SendPacket(Session.Character.GenerateGold());

                        Session.SendPacket($"gb 1 {Session.Account.BankGold / 1000} {Session.Character.Gold} 0 0");
                        Session.SendPacket($"say 1 740062 12 [Balance]: {Session.Account.BankGold} Gold; [Owned]: {Session.Character.Gold} Gold");

                        Session.SendPacket($"s_memo 4 [Balance]: {Session.Account.BankGold} Gold; [Owned]: {Session.Character.Gold} Gold");
                    }
                    else
                    {

                        Session.SendPacket("info You do not have enough gold");

                        Session.SendPacket("s_memo 5 You do not have enough gold");
                    }
                }
            }
            else if (gBox.Type == 2)
            {
                if (gBox.Type2 == 0)
                {

                    Session.SendPacket($"qna #gbox^2^{gBox.Amount}^1 Would you like to withdraw {gBox.Amount}000 gold? (Fee: 0 gold)");
                }
                else if (gBox.Type2 == 1)
                {

                    Session.SendPacket($"s_memo 6 You collect {gBox.Amount}000 gold. (Fee: 0 gold)");
                    if ((Session.Character.Gold + (gBox.Amount * 1000)) > ServerManager.Instance.MaxGold)
                    {
                        Session.SendPacket("info You can not carry more than 1.000.000.000 gold with you!");

                        Session.SendPacket("s_memo 5 You can not carry more than 1.000.000.000 gold with you!");
                    }
                    else if (Session.Account.BankGold >= (gBox.Amount * 1000))
                    {
                        Session.Account.BankGold -= (gBox.Amount * 1000);
                        Session.Character.Gold += (gBox.Amount * 1000);
                        Session.SendPacket(Session.Character.GenerateGold());

                        Session.SendPacket($"gb 2 {Session.Account.BankGold / 1000} {Session.Character.Gold} 0 0");
                        Session.SendPacket($"say 1 740062 12 [Balance]: {Session.Account.BankGold} Gold; [Owned]: {Session.Character.Gold} Gold");

                        Session.SendPacket($"s_memo 4 [Balance]: {Session.Account.BankGold} Gold; [Owned]: {Session.Character.Gold} Gold");
                    }
                    else
                    {

                        Session.SendPacket("info You do not have enough funds.");

                        Session.SendPacket("s_memo 5 You do not have enough funds.");
                    }
                }
            }
        }

        

        /// <summary>
        /// dir packet
        /// </summary>
        /// <param name="directionPacket"></param>
        public void Dir(DirectionPacket directionPacket)
        {
            if (directionPacket.CharacterId == Session.Character.CharacterId)
            {
                Session.Character.Direction = directionPacket.Direction;
                Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateDir());
            }
        }

        /// <summary>
        /// fins packet
        /// </summary>
        /// <param name="fInsPacket"></param>
        public void FriendAdd(FInsPacket fInsPacket)
        {
            if (!Session.Character.IsFriendlistFull())
            {
                long characterId = fInsPacket.CharacterId;
                if (!Session.Character.IsFriendOfCharacter(characterId))
                {
                    if (!Session.Character.IsBlockedByCharacter(characterId))
                    {
                        if (!Session.Character.IsBlockingCharacter(characterId))
                        {
                            ClientSession otherSession = ServerManager.Instance.GetSessionByCharacterId(characterId);
                            if (otherSession != null)
                            {
                                if (otherSession.Character.FriendRequestBlocked)
                                {
                                    Session.SendPacket(
                                        $"info {Language.Instance.GetMessageFromKey("FRIEND_REJECTED")}");
                                    return;
                                }

                                if (otherSession.Character.FriendRequestCharacters.GetAllItems()
                                    .Contains(Session.Character.CharacterId))
                                {
                                    switch (fInsPacket.Type)
                                    {
                                        case 1:
                                            Session.Character.AddRelation(characterId, CharacterRelationType.Friend);
                                            Session.SendPacket(
                                                $"info {Language.Instance.GetMessageFromKey("FRIEND_ADDED")}");
                                            otherSession.SendPacket(
                                                $"info {Language.Instance.GetMessageFromKey("FRIEND_ADDED")}");
                                            break;

                                        case 2:
                                            otherSession.SendPacket(
                                                $"info {Language.Instance.GetMessageFromKey("FRIEND_REJECTED")}");
                                            break;

                                        default:
                                            if (Session.Character.IsFriendlistFull())
                                            {
                                                Session.SendPacket(
                                                    $"info {Language.Instance.GetMessageFromKey("FRIEND_FULL")}");
                                                otherSession.SendPacket(
                                                    $"info {Language.Instance.GetMessageFromKey("FRIEND_FULL")}");
                                            }

                                            break;
                                    }
                                }
                                else
                                {
                                    otherSession.SendPacket(UserInterfaceHelper.GenerateDialog(
                                        $"#fins^1^{Session.Character.CharacterId} #fins^2^{Session.Character.CharacterId} {string.Format(Language.Instance.GetMessageFromKey("FRIEND_ADD"), Session.Character.Name)}"));
                                    Session.Character.FriendRequestCharacters.Add(characterId);
                                }
                            }
                        }
                        else
                        {
                            Session.SendPacket($"info {Language.Instance.GetMessageFromKey("BLACKLIST_BLOCKING")}");
                        }
                    }
                    else
                    {
                        Session.SendPacket($"info {Language.Instance.GetMessageFromKey("BLACKLIST_BLOCKED")}");
                    }
                }
                else
                {
                    Session.SendPacket($"info {Language.Instance.GetMessageFromKey("ALREADY_FRIEND")}");
                }
            }
            else
            {
                Session.SendPacket($"info {Language.Instance.GetMessageFromKey("FRIEND_FULL")}");
            }
        }

        /// <summary>
        /// fdel packet
        /// </summary>
        /// <param name="fDelPacket"></param>
        public void FriendDelete(FDelPacket fDelPacket)
        {
            Session.Character.DeleteRelation(fDelPacket.CharacterId);
            Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("FRIEND_DELETED")));
        }

        /// <summary>
        /// btk packet
        /// </summary>
        /// <param name="btkPacket"></param>
        public void FriendTalk(BtkPacket btkPacket)
        {
            if (string.IsNullOrEmpty(btkPacket.Message))
            {
                return;
            }

            string message = btkPacket.Message;
            if (message.Length > 60)
            {
                message = message.Substring(0, 60);
            }

            message = message.Trim();

            CharacterDTO character = DAOFactory.CharacterDAO.LoadById(btkPacket.CharacterId);
            if (character != null)
            {
                int? sentChannelId = CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                {
                    DestinationCharacterId = character.CharacterId,
                    SourceCharacterId = Session.Character.CharacterId,
                    SourceWorldId = ServerManager.Instance.WorldId,
                    Message = PacketFactory.Serialize(Session.Character.GenerateTalk(message)),
                    Type = MessageType.PrivateChat
                });

                if (ServerManager.Instance.Configuration.UseChatLogService)
                {
                    ChatLogServiceClient.Instance.LogChatMessage(new ChatLogEntry()
                    {
                        Sender = Session.Character.Name,
                        SenderId = Session.Character.CharacterId,
                        Receiver = character.Name,
                        ReceiverId = character.CharacterId,
                        MessageType = ChatLogType.BuddyTalk,
                        Message = btkPacket.Message
                    });
                }

                if (!sentChannelId.HasValue) //character is even offline on different world
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("FRIEND_OFFLINE")));
                }
            }
        }

        /// <summary>
        /// pcl packet
        /// </summary>
        /// <param name="getGiftPacket"></param>
        public void GetGift(GetGiftPacket getGiftPacket)
        {
            int giftId = getGiftPacket.GiftId;
            if (Session.Character.MailList.ContainsKey(giftId))
            {
                MailDTO mail = Session.Character.MailList[giftId];
                if (getGiftPacket.Type == 4 && mail.AttachmentVNum != null)
                {
                    if (Session.Character.Inventory.CanAddItem((short)mail.AttachmentVNum))
                    {
                        ItemInstance newInv = Session.Character.Inventory.AddNewToInventory((short)mail.AttachmentVNum, mail.AttachmentAmount, Upgrade: mail.AttachmentUpgrade, Rare: (sbyte)mail.AttachmentRarity).FirstOrDefault();
                        if (newInv != null)
                        {
                            if (newInv.Rare != 0)
                            {
                                newInv?.SetRarityPoint();
                            }

                            Logger.LogUserEvent("PARCEL_GET", Session.GenerateIdentity(), $"IIId: {newInv.Id} ItemVNum: {newInv.ItemVNum} Amount: {mail.AttachmentAmount} Sender: {mail.SenderId}");

                            Session.SendPacket(Session.Character.GenerateSay($"{Language.Instance.GetMessageFromKey("ITEM_GIFTED")}: {newInv.Item.Name} x {mail.AttachmentAmount}", 12));

                            DAOFactory.MailDAO.DeleteById(mail.MailId);

                            Session.SendPacket($"parcel 2 1 {giftId}");

                            Session.Character.MailList.Remove(giftId);
                        }
                    }
                    else
                    {
                        Session.SendPacket("parcel 5 1 0");
                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"), 0));
                    }
                }
                else if (getGiftPacket.Type == 5)
                {
                    Session.SendPacket($"parcel 7 1 {giftId}");

                    if (DAOFactory.MailDAO.LoadById(mail.MailId) != null)
                    {
                        DAOFactory.MailDAO.DeleteById(mail.MailId);
                    }
                    if (Session.Character.MailList.ContainsKey(giftId))
                    {
                        Session.Character.MailList.Remove(giftId);
                    }
                }
            }
        }

        /// <summary>
        /// ncif packet
        /// </summary>
        /// <param name="ncifPacket"></param>
        public void GetNamedCharacterInformation(NcifPacket ncifPacket)
        {
            switch (ncifPacket.Type)
            {
                // characters
                case 1:
                    Session.SendPacket(ServerManager.Instance.GetSessionByCharacterId(ncifPacket.TargetId)?.Character
                        ?.GenerateStatInfo());
                    break;

                // npcs/mates
                case 2:
                    if (Session.HasCurrentMapInstance)
                    {
                        Session.CurrentMapInstance.Npcs.Where(n => n.MapNpcId == (int)ncifPacket.TargetId).ToList()
                            .ForEach(npc =>
                            {
                                NpcMonster npcinfo = ServerManager.GetNpc(npc.NpcVNum);
                                if (npcinfo == null)
                                {
                                    return;
                                }

                                Session.Character.LastNpcMonsterId = npc.MapNpcId;
                                Session.SendPacket(
                                    $"st 2 {ncifPacket.TargetId} {npcinfo.Level} {npcinfo.HeroLevel} 100 100 50000 50000");
                            });
                        Parallel.ForEach(Session.CurrentMapInstance.Sessions, session =>
                        {
                            Mate mate = session.Character.Mates.Find(
                                s => s.MateTransportId == (int)ncifPacket.TargetId);
                            if (mate != null)
                            {
                                Session.SendPacket(mate.GenerateStatInfo());
                            }
                        });
                    }

                    break;

                // monsters
                case 3:
                    if (Session.HasCurrentMapInstance)
                    {
                        Session.CurrentMapInstance.Monsters.Where(m => m.MapMonsterId == (int)ncifPacket.TargetId)
                            .ToList().ForEach(monster =>
                            {
                                NpcMonster monsterinfo = ServerManager.GetNpc(monster.MonsterVNum);
                                if (monsterinfo == null)
                                {
                                    return;
                                }

                                Session.Character.LastNpcMonsterId = monster.MapMonsterId;
                                Session.SendPacket(
                                    $"st 3 {ncifPacket.TargetId} {monsterinfo.Level} {monsterinfo.HeroLevel} {(int)((float)monster.CurrentHp / (float)monster.MaxHp * 100)} {(int)((float)monster.CurrentMp / (float)monster.MaxMp * 100)} {monster.CurrentHp} {monster.CurrentMp}{monster.Buff.GetAllItems().Aggregate(string.Empty, (current, buff) => current + $" {buff.Card.CardId}")}");
                            });
                    }

                    break;
            }
        }

        /// <summary>
        /// RstartPacket packet
        /// </summary>
        /// <param name="rStartPacket"></param>
        public void GetRStart(RStartPacket rStartPacket)
        {
            if (rStartPacket.Type == 1)
            {
                Session.Character.Timespace.InstanceBag.Lock = true;
                Preq(new PreqPacket());
            }
        }

        /// <summary>
        /// npinfo packet
        /// </summary>
        /// <param name="npinfoPacket"></param>
        public void GetStats(NpinfoPacket npinfoPacket)
        {
            Session.SendPacket(Session.Character.GenerateStatChar());
            if (npinfoPacket.Page != Session.Character.ScPage)
            {
                Session.Character.ScPage = npinfoPacket.Page;
                Session.SendPacket(UserInterfaceHelper.GeneratePClear());
                Session.SendPackets(Session.Character.GenerateScP(npinfoPacket.Page));
            }
        }

        /// <summary>
        /// pjoin packet
        /// </summary>
        /// <param name="pjoinPacket"></param>
        public void GroupJoin(PJoinPacket pjoinPacket)
        {
            if (pjoinPacket != null)
            {
                bool createNewGroup = true;
                ClientSession targetSession = ServerManager.Instance.GetSessionByCharacterId(pjoinPacket.CharacterId);

                if (targetSession == null && !pjoinPacket.RequestType.Equals(GroupRequestType.Sharing))
                {
                    return;
                }

                if (pjoinPacket.RequestType.Equals(GroupRequestType.Requested)
                    || pjoinPacket.RequestType.Equals(GroupRequestType.Invited))
                {
                    if (pjoinPacket.CharacterId == 0)
                    {
                        return;
                    }

                    if (ServerManager.Instance.IsCharactersGroupFull(pjoinPacket.CharacterId))
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("GROUP_FULL")));
                        return;
                    }

                    if (ServerManager.Instance.IsCharacterMemberOfGroup(pjoinPacket.CharacterId)
                        && ServerManager.Instance.IsCharacterMemberOfGroup(Session.Character.CharacterId))
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("ALREADY_IN_GROUP")));
                        return;
                    }

                    if (Session.Character.CharacterId != pjoinPacket.CharacterId && targetSession != null)
                    {
                        if (Session.Character.IsBlockedByCharacter(pjoinPacket.CharacterId))
                        {
                            Session.SendPacket(
                                UserInterfaceHelper.GenerateInfo(
                                    Language.Instance.GetMessageFromKey("BLACKLIST_BLOCKED")));
                            return;
                        }

                        if (targetSession.Character.GroupRequestBlocked)
                        {
                            Session.SendPacket(
                                UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("GROUP_BLOCKED"),
                                    0));
                        }
                        else
                        {
                            // save sent group request to current character
                            Session.Character.GroupSentRequestCharacterIds.Add(targetSession.Character.CharacterId);
                            if (Session.Character.Group == null || Session.Character.Group.GroupType == GroupType.Group)
                            {
                                if (targetSession.Character?.Group == null
                                    || targetSession.Character?.Group.GroupType == GroupType.Group)
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateInfo(
                                        string.Format(Language.Instance.GetMessageFromKey("GROUP_REQUEST"),
                                            targetSession.Character.Name)));
                                    targetSession.SendPacket(UserInterfaceHelper.GenerateDialog(
                                        $"#pjoin^3^{Session.Character.CharacterId} #pjoin^4^{Session.Character.CharacterId} {string.Format(Language.Instance.GetMessageFromKey("INVITED_YOU"), Session.Character.Name)}"));
                                }
                                else
                                {
                                    //can't invite raid member
                                }
                            }
                            else
                            {
                                targetSession.SendPacket(
                                    $"qna #rd^1^{Session.Character.CharacterId}^1 {string.Format(Language.Instance.GetMessageFromKey("INVITE_RAID"), Session.Character.Name)}");
                            }
                        }
                    }
                }
                else if (pjoinPacket.RequestType.Equals(GroupRequestType.Sharing))
                {
                    if (Session.Character.Group != null)
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("GROUP_SHARE_INFO")));
                        Session.Character.Group.Characters
                            .Where(s => s.Character.CharacterId != Session.Character.CharacterId).ToList().ForEach(s =>
                            {
                                s.SendPacket(UserInterfaceHelper.GenerateDialog(
                                    $"#pjoin^6^{Session.Character.CharacterId} #pjoin^7^{Session.Character.CharacterId} {string.Format(Language.Instance.GetMessageFromKey("INVITED_YOU_SHARE"), Session.Character.Name)}"));
                                Session.Character.GroupSentRequestCharacterIds.Add(s.Character.CharacterId);
                            });
                    }
                }
                else if (pjoinPacket.RequestType.Equals(GroupRequestType.Accepted))
                {
                    if (targetSession?.Character.GroupSentRequestCharacterIds.GetAllItems()
                            .Contains(Session.Character.CharacterId) == false)
                    {
                        return;
                    }

                    try
                    {
                        targetSession?.Character.GroupSentRequestCharacterIds.Remove(Session.Character.CharacterId);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }

                    if (ServerManager.Instance.IsCharacterMemberOfGroup(Session.Character.CharacterId)
                        && ServerManager.Instance.IsCharacterMemberOfGroup(pjoinPacket.CharacterId))
                    {
                        // everyone is in group, return
                        return;
                    }

                    if (ServerManager.Instance.IsCharactersGroupFull(pjoinPacket.CharacterId)
                        || ServerManager.Instance.IsCharactersGroupFull(Session.Character.CharacterId))
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("GROUP_FULL")));
                        targetSession?.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("GROUP_FULL")));
                        return;
                    }

                    // get group and add to group
                    if (ServerManager.Instance.IsCharacterMemberOfGroup(Session.Character.CharacterId))
                    {
                        // target joins source
                        Group currentGroup =
                            ServerManager.Instance.GetGroupByCharacterId(Session.Character.CharacterId);

                        if (currentGroup != null)
                        {
                            currentGroup.JoinGroup(targetSession);
                            targetSession?.SendPacket(
                                UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("JOINED_GROUP"),
                                    10));
                            createNewGroup = false;
                        }
                    }
                    else if (ServerManager.Instance.IsCharacterMemberOfGroup(pjoinPacket.CharacterId))
                    {
                        // source joins target
                        Group currentGroup = ServerManager.Instance.GetGroupByCharacterId(pjoinPacket.CharacterId);

                        if (currentGroup != null)
                        {
                            createNewGroup = false;
                            if (currentGroup.GroupType == GroupType.Group)
                            {
                                currentGroup.JoinGroup(Session);
                            }
                            else
                            {
                                Session.SendPacket(
                                    Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("RAID_JOIN"),
                                        10));
                                if (Session.Character.Level > currentGroup.Raid?.LevelMaximum
                                    || Session.Character.Level < currentGroup.Raid?.LevelMinimum)
                                {
                                    Session.SendPacket(Session.Character.GenerateSay(
                                        Language.Instance.GetMessageFromKey("RAID_LEVEL_INCORRECT"), 10));
                                    if (Session.Character.Level
                                        >= currentGroup.Raid?.LevelMaximum + 10 /* && AlreadySuccededToday*/)
                                    {
                                        //modal 1 ALREADY_SUCCEDED_AS_ASSISTANT
                                    }
                                }

                                currentGroup.JoinGroup(Session);
                                Session.SendPacket(Session.Character.GenerateRaid(1));
                                currentGroup.Characters.ForEach(s =>
                                {
                                    s.SendPacket(currentGroup.GenerateRdlst());
                                    s.SendPacket(s.Character.GenerateSay(
                                        string.Format(Language.Instance.GetMessageFromKey("JOIN_TEAM"),
                                            Session.Character.Name), 10));
                                    s.SendPacket(s.Character.GenerateRaid(0));
                                });
                            }
                        }
                    }

                    if (createNewGroup)
                    {
                        Group group = new Group
                        {
                            GroupType = GroupType.Group
                        };
                        group.JoinGroup(pjoinPacket.CharacterId);
                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                            string.Format(Language.Instance.GetMessageFromKey("GROUP_JOIN"),
                                targetSession?.Character.Name), 10));
                        group.JoinGroup(Session.Character.CharacterId);
                        ServerManager.Instance.AddGroup(group);
                        targetSession?.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("GROUP_ADMIN")));

                        // set back reference to group
                        Session.Character.Group = group;
                        if (targetSession != null)
                        {
                            targetSession.Character.Group = @group;
                        }
                    }

                    if (Session.Character.Group.GroupType == GroupType.Group)
                    {
                        // player join group
                        ServerManager.Instance.UpdateGroup(pjoinPacket.CharacterId);
                        Session.CurrentMapInstance?.Broadcast(Session.Character.GeneratePidx());
                    }
                }
                else if (pjoinPacket.RequestType == GroupRequestType.Declined)
                {
                    if (targetSession?.Character.GroupSentRequestCharacterIds.GetAllItems().Contains(Session.Character.CharacterId) == false)
                    {
                        return;
                    }

                    targetSession?.Character.GroupSentRequestCharacterIds.Remove(Session.Character.CharacterId);

                    targetSession?.SendPacket(Session.Character.GenerateSay(
                        string.Format(Language.Instance.GetMessageFromKey("REFUSED_GROUP_REQUEST"),
                            Session.Character.Name), 10));
                }
                else if (pjoinPacket.RequestType == GroupRequestType.AcceptedShare)
                {
                    if (targetSession?.Character.GroupSentRequestCharacterIds.GetAllItems().Contains(Session.Character.CharacterId) == false)
                    {
                        return;
                    }

                    targetSession?.Character.GroupSentRequestCharacterIds.Remove(Session.Character.CharacterId);

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("ACCEPTED_SHARE"),
                            targetSession?.Character.Name), 0));
                    if (Session.Character?.Group?.IsMemberOfGroup(pjoinPacket.CharacterId) == true && targetSession != null)
                    {
                        Session.Character.SetReturnPoint(targetSession.Character.Return.DefaultMapId,
                            targetSession.Character.Return.DefaultX, targetSession.Character.Return.DefaultY);
                        targetSession.SendPacket(UserInterfaceHelper.GenerateMsg(
                            string.Format(Language.Instance.GetMessageFromKey("CHANGED_SHARE"), targetSession.Character.Name), 0));
                    }
                }
                else if (pjoinPacket.RequestType == GroupRequestType.DeclinedShare)
                {
                    if (targetSession?.Character.GroupSentRequestCharacterIds.GetAllItems()
                            .Contains(Session.Character.CharacterId) == false)
                    {
                        return;
                    }

                    targetSession?.Character.GroupSentRequestCharacterIds.Remove(Session.Character.CharacterId);

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("REFUSED_SHARE"), 0));
                }
            }
        }

        /// <summary>
        /// pleave packet
        /// </summary>
        /// <param name="pleavePacket"></param>
        public void GroupLeave(PLeavePacket pleavePacket) => ServerManager.Instance.GroupLeave(Session);

        /// <summary>
        /// ; packet
        /// </summary>
        /// <param name="groupSayPacket"></param>
        public void GroupTalk(GroupSayPacket groupSayPacket)
        {
            if (!string.IsNullOrEmpty(groupSayPacket.Message))
            {
                ServerManager.Instance.Broadcast(Session, Session.Character.GenerateSpk(groupSayPacket.Message, 3), ReceiverType.Group);
            }
        }

        /// <summary>
        /// guri packet
        /// </summary>
        /// <param name="guriPacket"></param>
        public void Guri(GuriPacket guriPacket)
        {
            if (guriPacket != null)
            {
                if (guriPacket.Data.HasValue && guriPacket.Type == 10 && guriPacket.Data.Value >= 973 && guriPacket.Data.Value <= 999 && !Session.Character.EmoticonsBlocked)
                {
                    if (guriPacket.User == Session.Character.CharacterId)
                    {
                        Session.CurrentMapInstance?.Broadcast(Session, StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, guriPacket.Data.Value + 4099), ReceiverType.AllNoEmoBlocked);
                    }
                    else if (int.TryParse(guriPacket.User.ToString(), out int mateTransportId))
                    {
                        Mate mate = Session.Character.Mates.Find(s => s.MateTransportId == mateTransportId);
                        if (mate != null)
                        {
                            Session.CurrentMapInstance?.Broadcast(Session, StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, guriPacket.Data.Value + 4099), ReceiverType.AllNoEmoBlocked);
                        }
                    }
                }
                else if (guriPacket.Type == 204)
                {
                    if (guriPacket.Argument == 0 && short.TryParse(guriPacket.User.ToString(), out short slot))
                    {
                        ItemInstance shell = Session.Character.Inventory.LoadBySlotAndType(slot, InventoryType.Equipment);
                        if (shell?.ShellEffects.Count == 0 && shell.Upgrade > 0 && shell.Rare > 0 && Session.Character.Inventory.CountItem(1429) >= ((shell.Upgrade / 10) + shell.Rare))
                        {
                            shell.SetShellEffects();
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("OPTION_IDENTIFIED"), 0));
                            Session.SendPacket(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 3010));
                            Session.Character.Inventory.RemoveItemAmount(1429, (shell.Upgrade / 10) + shell.Rare);
                        }
                    }
                }
                else if (guriPacket.Type == 300)
                {
                    if (guriPacket.Argument == 8023 && short.TryParse(guriPacket.User.ToString(), out short slot))
                    {
                        ItemInstance box = Session.Character.Inventory.LoadBySlotAndType(slot, InventoryType.Equipment);
                        if (box != null)
                        {
                            box.Item.Use(Session, ref box, 1, new string[] { guriPacket.Data.ToString() });
                        }
                    }
                }
                else if (guriPacket.Type == 506)
                {
                    if (ServerManager.Instance.EventInWaiting)
                    {
                        Session.Character.IsWaitingForEvent = true;
                    }
                }
                else if (guriPacket.Type == 199 && guriPacket.Argument == 2)
                {
                    short[] listWingOfFriendship = { 2160, 2312, 10048 };
                    short vnumToUse = -1;
                    foreach (short vnum in listWingOfFriendship)
                    {
                        if (Session.Character.Inventory.CountItem(vnum) > 0)
                        {
                            vnumToUse = vnum;
                        }
                    }
                    if (vnumToUse != -1)
                    {
                        ClientSession session = ServerManager.Instance.GetSessionByCharacterId(guriPacket.User);
                        if (session != null)
                        {
                            if (Session.Character.IsFriendOfCharacter(guriPacket.User))
                            {
                                if (session.CurrentMapInstance?.MapInstanceType == MapInstanceType.BaseMapInstance)
                                {
                                    if (Session.Character.MapInstance.MapInstanceType != MapInstanceType.BaseMapInstance || (ServerManager.Instance.ChannelId == 51 && Session.Character.Faction != session.Character.Faction))
                                    {
                                        Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_USE_THAT"), 10));
                                        return;
                                    }
                                    short mapy = session.Character.PositionY;
                                    short mapx = session.Character.PositionX;
                                    short mapId = session.Character.MapInstance.Map.MapId;

                                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId, mapId, mapx, mapy);
                                    Session.Character.Inventory.RemoveItemAmount(vnumToUse);
                                }
                                else
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("USER_ON_INSTANCEMAP"), 0));
                                }
                            }
                        }
                        else
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("USER_NOT_CONNECTED"), 0));
                        }
                    }
                    else
                    {
                        Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NO_WINGS"), 10));
                    }
                }
                else if (guriPacket.Type == 400)
                {
                    if (!int.TryParse(guriPacket.Argument.ToString(), out int mapNpcId) || !Session.HasCurrentMapInstance)
                    {
                        return;
                    }
                    MapNpc npc = Session.CurrentMapInstance.Npcs.Find(n => n.MapNpcId.Equals(mapNpcId));
                    if (npc != null)
                    {
                        NpcMonster mapobject = ServerManager.GetNpc(npc.NpcVNum);

                        int RateDrop = ServerManager.Instance.Configuration.RateDrop;
                        int delay = (int)Math.Round((3 + (mapobject.RespawnTime / 1000d)) * Session.Character.TimesUsed);
                        delay = delay > 11 ? 8 : delay;
                        if (Session.Character.LastMapObject.AddSeconds(delay) < DateTime.Now)
                        {
                            if (mapobject.Drops.Any(s => s.MonsterVNum != null) && mapobject.VNumRequired > 10 && Session.Character.Inventory.CountItem(mapobject.VNumRequired) < mapobject.AmountRequired)
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEM"), 0));
                                return;
                            }
                            Random random = new Random();
                            double randomAmount = ServerManager.RandomNumber() * random.NextDouble();
                            DropDTO drop = mapobject.Drops.Find(s => s.MonsterVNum == npc.NpcVNum);
                            if (drop != null)
                            {
                                int dropChance = drop.DropChance;
                                if (randomAmount <= (double)dropChance * RateDrop / 5000.000)
                                {
                                    short vnum = drop.ItemVNum;
                                    ItemInstance newInv = Session.Character.Inventory.AddNewToInventory(vnum).FirstOrDefault();
                                    Session.Character.LastMapObject = DateTime.Now;
                                    Session.Character.TimesUsed++;
                                    if (Session.Character.TimesUsed >= 4)
                                    {
                                        Session.Character.TimesUsed = 0;
                                    }
                                    if (newInv != null)
                                    {
                                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("RECEIVED_ITEM"), newInv.Item.Name), 0));
                                        Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("RECEIVED_ITEM"), newInv.Item.Name), 11));
                                    }
                                    else
                                    {
                                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"), 0));
                                    }
                                }
                                else
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TRY_FAILED"), 0));
                                }
                            }
                        }
                        else
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("TRY_FAILED_WAIT"), (int)(Session.Character.LastMapObject.AddSeconds(delay) - DateTime.Now).TotalSeconds), 0));
                        }
                    }
                }
                else if (guriPacket.Type == 710)
                {
                    if (guriPacket.Value != null)
                    {
                        // TODO: MAP TELEPORTER
                    }
                }
                else if (guriPacket.Type == 750)
                {
                    const short baseVnum = 1623;
                    if (Enum.TryParse(guriPacket.Argument.ToString(), out FactionType faction) && Session.Character.Inventory.CountItem(baseVnum + (byte)faction) > 0)
                    {
                        if ((byte)faction < 3)
                        {
                            if (Session.Character.Family != null)
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("IN_FAMILY"), 0));
                            }
                            Session.Character.Faction = faction;
                            Session.Character.Inventory.RemoveItemAmount(baseVnum + (byte)faction);
                            Session.SendPacket("scr 0 0 0 0 0 0 0");
                            Session.SendPacket(Session.Character.GenerateFaction());
                            Session.SendPacket(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 4793 + (byte)faction));
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey($"GET_PROTECTION_POWER_{(byte)faction}"), 0));
                        }
                        else
                        {
                            if (Session.Character.Family == null || Session.Character.Family.FamilyCharacters.Find(s => s.Authority.Equals(FamilyAuthority.Head))?.CharacterId.Equals(Session.Character.CharacterId) != true)
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_FAMILY"), 0));
                            }

                            Session.Character.Faction = (FactionType)((byte)faction / 2);
                            Session.Character.Inventory.RemoveItemAmount(baseVnum + (byte)faction);
                            Session.SendPacket("scr 0 0 0 0 0 0 0");
                            Session.SendPacket(Session.Character.GenerateFaction());
                            Session.SendPacket(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 4793 + ((byte)faction / 2)));
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey($"GET_PROTECTION_POWER_{(byte)faction / 2}"), 0));
                            Session.Character.Save();
                            ServerManager.Instance.FamilyRefresh(Session.Character.Family.FamilyId);
                            CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage()
                            {
                                DestinationCharacterId = Session.Character.Family.FamilyId,
                                SourceCharacterId = 0,
                                SourceWorldId = ServerManager.Instance.WorldId,
                                Message = "fhis_stc",
                                Type = MessageType.Family
                            });
                        }
                    }
                }
                else if (guriPacket.Type == 2)
                {
                    Session.CurrentMapInstance?.Broadcast(UserInterfaceHelper.GenerateGuri(2, 1, Session.Character.CharacterId), Session.Character.PositionX, Session.Character.PositionY);
                }
                else if (guriPacket.Type == 4)
                {
                    const int speakerVNum = 2173;
                    const int petnameVNum = 2157;
                    if (guriPacket.Argument == 1)
                    {
                        Mate mate = Session.Character.Mates.Find(s => s.MateTransportId == guriPacket.Data);
                        if (mate != null)
                        {
                            mate.Name = guriPacket.Value;
                            Session.CurrentMapInstance?.Broadcast(mate.GenerateOut(), ReceiverType.AllExceptMe);
                            Session.CurrentMapInstance?.Broadcast(mate.GenerateIn());
                            Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("NEW_NAME_PET")));
                            Session.SendPacket(Session.Character.GeneratePinit());
                            Session.SendPackets(Session.Character.GeneratePst());
                            Session.SendPackets(Session.Character.GenerateScP());
                            Session.Character.Inventory.RemoveItemAmount(petnameVNum);
                        }
                    }

                    // presentation message
                    if (guriPacket.Argument == 2)
                    {
                        int presentationVNum = Session.Character.Inventory.CountItem(1117) > 0 ? 1117 : (Session.Character.Inventory.CountItem(9013) > 0 ? 9013 : -1);
                        if (presentationVNum != -1)
                        {
                            string message = string.Empty;
                            string[] valuesplit = guriPacket.Value?.Split(' ');
                            if (valuesplit == null)
                            {
                                return;
                            }
                            for (int i = 0; i < valuesplit.Length; i++)
                            {
                                message += valuesplit[i] + "^";
                            }
                            message = message.Substring(0, message.Length - 1); // Remove the last ^
                            message = message.Trim();
                            if (message.Length > 60)
                            {
                                message = message.Substring(0, 60);
                            }

                            Session.Character.Biography = message;
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("INTRODUCTION_SET"), 10));
                            Session.Character.Inventory.RemoveItemAmount(presentationVNum);
                        }
                    }

                    // Speaker
                    if (guriPacket.Argument == 3 && Session.Character.Inventory.CountItem(speakerVNum) > 0)
                    {
                        string message = $"<{Language.Instance.GetMessageFromKey("SPEAKER")}> [{Session.Character.Name}]:";
                        int baseLength = message.Length;
                        string[] valuesplit = guriPacket.Value?.Split(' ');
                        if (valuesplit == null)
                        {
                            return;
                        }
                        for (int i = 0; i < valuesplit.Length; i++)
                        {
                            message += valuesplit[i] + " ";
                        }
                        if (message.Length > 120 + baseLength)
                        {
                            message = message.Substring(0, 120 + baseLength);
                        }

                        message = message.Trim();

                        if (Session.Character.IsMuted())
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("SPEAKER_CANT_BE_USED"), 10));
                            return;
                        }
                        Session.Character.Inventory.RemoveItemAmount(speakerVNum);
                        ServerManager.Instance.Broadcast(Session.Character.GenerateSay(message, 13));
                    }
                }
                else if (guriPacket.Type == 199 && guriPacket.Argument == 1)
                {
                    if (!Session.Character.IsFriendOfCharacter(guriPacket.User))
                    {
                        Session.SendPacket(Language.Instance.GetMessageFromKey("CHARACTER_NOT_IN_FRIENDLIST"));
                        return;
                    }
                    Session.SendPacket(UserInterfaceHelper.GenerateDelay(3000, 4, $"#guri^199^2^{guriPacket.User}"));
                }
                else if (guriPacket.Type == 201)
                {
                    if (Session.Character.StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.PetBasket))
                    {
                        Session.SendPacket(Session.Character.GenerateStashAll());
                    }
                }
                else if (guriPacket.Type == 202)
                {
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PARTNER_BACKPACK"), 10));
                    Session.SendPacket(Session.Character.GeneratePStashAll());
                }
                else if (guriPacket.Type == 208 && guriPacket.Argument == 0)
                {
                    if (short.TryParse(guriPacket.Value, out short mountSlot) && short.TryParse(guriPacket.User.ToString(), out short pearlSlot))
                    {
                        ItemInstance mount = Session.Character.Inventory.LoadBySlotAndType<ItemInstance>(mountSlot, InventoryType.Main);
                        ItemInstance pearl = Session.Character.Inventory.LoadBySlotAndType(pearlSlot, InventoryType.Equipment);
                        if (mount != null && pearl != null)
                        {
                            pearl.HoldingVNum = mount.ItemVNum;
                            Session.Character.Inventory.RemoveItemFromInventory(mount.Id);
                        }
                    }
                }
                else if (guriPacket.Type == 209 && guriPacket.Argument == 0)
                {
                    if (short.TryParse(guriPacket.Value, out short mountSlot) && short.TryParse(guriPacket.User.ToString(), out short pearlSlot))
                    {
                        ItemInstance fairy = Session.Character.Inventory.LoadBySlotAndType(mountSlot, InventoryType.Equipment);
                        ItemInstance pearl = Session.Character.Inventory.LoadBySlotAndType(pearlSlot, InventoryType.Equipment);
                        if (fairy != null && pearl != null)
                        {
                            pearl.HoldingVNum = fairy.ItemVNum;
                            pearl.ElementRate = fairy.ElementRate;
                            Session.Character.Inventory.RemoveItemFromInventory(fairy.Id);
                        }
                    }
                }
                else if (guriPacket.Type == 203 && guriPacket.Argument == 0)
                {
                    // SP points initialization
                    int[] listPotionResetVNums = { 1366, 1427, 5115, 9040 };
                    int vnumToUse = -1;
                    foreach (int vnum in listPotionResetVNums)
                    {
                        if (Session.Character.Inventory.CountItem(vnum) > 0)
                        {
                            vnumToUse = vnum;
                        }
                    }
                    if (vnumToUse != -1)
                    {
                        if (Session.Character.UseSp)
                        {
                            ItemInstance specialistInstance = Session.Character.Inventory.LoadBySlotAndType((byte)EquipmentType.Sp, InventoryType.Wear);
                            if (specialistInstance != null)
                            {
                                specialistInstance.SlDamage = 0;
                                specialistInstance.SlDefence = 0;
                                specialistInstance.SlElement = 0;
                                specialistInstance.SlHP = 0;

                                specialistInstance.DamageMinimum = 0;
                                specialistInstance.DamageMaximum = 0;
                                specialistInstance.HitRate = 0;
                                specialistInstance.CriticalLuckRate = 0;
                                specialistInstance.CriticalRate = 0;
                                specialistInstance.DefenceDodge = 0;
                                specialistInstance.DistanceDefenceDodge = 0;
                                specialistInstance.ElementRate = 0;
                                specialistInstance.DarkResistance = 0;
                                specialistInstance.LightResistance = 0;
                                specialistInstance.FireResistance = 0;
                                specialistInstance.WaterResistance = 0;
                                specialistInstance.CriticalDodge = 0;
                                specialistInstance.CloseDefence = 0;
                                specialistInstance.DistanceDefence = 0;
                                specialistInstance.MagicDefence = 0;
                                specialistInstance.HP = 0;
                                specialistInstance.MP = 0;

                                Session.Character.Inventory.RemoveItemAmount(vnumToUse);
                                Session.Character.Inventory.DeleteFromSlotAndType((byte)EquipmentType.Sp, InventoryType.Wear);
                                Session.Character.Inventory.AddToInventoryWithSlotAndType(specialistInstance, InventoryType.Wear, (byte)EquipmentType.Sp);
                                Session.SendPacket(Session.Character.GenerateCond());
                                Session.SendPacket(specialistInstance.GenerateSlInfo());
                                Session.SendPacket(Session.Character.GenerateLev());
                                Session.SendPacket(Session.Character.GenerateStatChar());
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("POINTS_RESET"), 0));
                            }
                        }
                        else
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("TRANSFORMATION_NEEDED"), 10));
                        }
                    }
                    else
                    {
                        Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_POINTS"), 10));
                    }
                }
            }
        }

        /// <summary>
        /// hero packet
        /// </summary>
        /// <param name="heroPacket"></param>
        public void Hero(HeroPacket heroPacket)
        {
            if (!string.IsNullOrEmpty(heroPacket.Message))
            {
                if (Session.Character.IsReputationHero() >= 3)
                {
                    heroPacket.Message = heroPacket.Message.Trim();
                    ServerManager.Instance.Broadcast(Session, $"msg 5 [{Session.Character.Name}]:{heroPacket.Message}", ReceiverType.AllNoHeroBlocked);
                }
                else
                {
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_HERO"), 11));
                }
            }
        }

        /// <summary>
        /// PreqPacket packet
        /// </summary>
        /// <param name="packet"></param>
        public void Preq(PreqPacket packet)
        {
            double currentRunningSeconds = (DateTime.Now - Process.GetCurrentProcess().StartTime.AddSeconds(-50)).TotalSeconds;
            double timeSpanSinceLastPortal = currentRunningSeconds - Session.Character.LastPortal;
            if (!(timeSpanSinceLastPortal >= 4) || !Session.HasCurrentMapInstance)
            {
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MOVE"), 10));
                return;
            }
            Parallel.ForEach(Session.CurrentMapInstance.Portals.Concat(Session.Character.GetExtraPortal()), portal =>
            {
                if (Session.Character.PositionY >= portal.SourceY - 1 && Session.Character.PositionY <= portal.SourceY + 1
                    && Session.Character.PositionX >= portal.SourceX - 1 && Session.Character.PositionX <= portal.SourceX + 1)
                {
                    switch (portal.Type)
                    {
                        case (sbyte)PortalType.MapPortal:
                        case (sbyte)PortalType.TSNormal:
                        case (sbyte)PortalType.Open:
                        case (sbyte)PortalType.Miniland:
                        case (sbyte)PortalType.TSEnd:
                        case (sbyte)PortalType.Exit:
                        case (sbyte)PortalType.Effect:
                        case (sbyte)PortalType.ShopTeleport:
                            break;

                        case (sbyte)PortalType.Raid:
                            if (Session.Character.Group?.Raid != null)
                            {
                                if (Session.Character.Group.IsLeader(Session))
                                {
                                    Session.SendPacket($"qna #mkraid^0^275 {Language.Instance.GetMessageFromKey("RAID_START_QUESTION")}");
                                }
                                else
                                {
                                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_TEAM_LEADER"), 10));
                                }
                            }
                            else
                            {
                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NEED_TEAM"), 10));
                            }
                            return;

                        case (sbyte)PortalType.BlueRaid:
                        case (sbyte)PortalType.DarkRaid:
                            if ((int)Session.Character.Faction == portal.Type - 9 && Session.Character.Family?.Act4Raid != null)
                            {
                                Session.Character.LastPortal = currentRunningSeconds;

                                switch (Session.Character.Family.Act4Raid.MapInstanceType)
                                {
                                    case MapInstanceType.Act4Morcos:
                                        ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, Session.Character.Family.Act4Raid.MapInstanceId, 43, 179);
                                        break;

                                    case MapInstanceType.Act4Hatus:
                                        ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, Session.Character.Family.Act4Raid.MapInstanceId, 15, 9);
                                        break;

                                    case MapInstanceType.Act4Calvina:
                                        ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, Session.Character.Family.Act4Raid.MapInstanceId, 24, 6);
                                        break;

                                    case MapInstanceType.Act4Berios:
                                        ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, Session.Character.Family.Act4Raid.MapInstanceId, 20, 20);
                                        break;
                                }
                            }
                            else
                            {
                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PORTAL_BLOCKED"), 10));
                            }
                            return;

                        default:
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PORTAL_BLOCKED"), 10));
                            return;
                    }

                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TimeSpaceInstance && !Session.Character.Timespace.InstanceBag.Lock)
                    {
                        if (Session.Character.CharacterId == Session.Character.Timespace.InstanceBag.Creator)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateDialog($"#rstart^1 rstart {Language.Instance.GetMessageFromKey("FIRST_ROOM_START")}"));
                        }
                        return;
                    }
                    portal.OnTraversalEvents.ForEach(e => EventHelper.Instance.RunEvent(e));
                    if (portal.DestinationMapInstanceId == default)
                    {
                        return;
                    }
                    if (ServerManager.Instance.ChannelId == 51)
                    {
                        if ((Session.Character.Faction == FactionType.Angel && portal.DestinationMapId == 131) || (Session.Character.Faction == FactionType.Demon && portal.DestinationMapId == 130))
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PORTAL_BLOCKED"), 10));
                            return;
                        }
                        if ((portal.DestinationMapId == 130 || portal.DestinationMapId == 131) && timeSpanSinceLastPortal < 60)
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MOVE"), 10));
                            return;
                        }
                    }
                    Session.SendPacket(Session.CurrentMapInstance.GenerateRsfn());

                    Session.Character.LastPortal = currentRunningSeconds;

                    if (ServerManager.GetMapInstance(portal.SourceMapInstanceId).MapInstanceType != MapInstanceType.BaseMapInstance && ServerManager.GetMapInstance(portal.DestinationMapInstanceId).MapInstanceType == MapInstanceType.BaseMapInstance)
                    {
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, Session.Character.MapId, Session.Character.MapX, Session.Character.MapY);
                    }
                    else if (portal.DestinationMapInstanceId == Session.Character.Miniland.MapInstanceId)
                    {
                        ServerManager.Instance.JoinMiniland(Session, Session);
                    }
                    else if (portal.DestinationMapId == 20000)
                    {
                        ClientSession sess = ServerManager.Instance.Sessions.FirstOrDefault(s => s.Character.Miniland.MapInstanceId == portal.DestinationMapInstanceId);
                        if (sess != null)
                        {
                            ServerManager.Instance.JoinMiniland(Session, sess);
                        }
                    }
                    else
                    {
                        ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, portal.DestinationMapInstanceId, portal.DestinationX, portal.DestinationY);
                    }
                }
            });
        }

        /// <summary>
        /// pulse packet
        /// </summary>
        /// <param name="pulsepacket"></param>
        public void Pulse(PulsePacket pulsepacket)
        {
            if (Session.Character.LastPulse.AddMilliseconds(80000) >= DateTime.Now && DateTime.Now >= Session.Character.LastPulse.AddMilliseconds(40000))
            {
                Session.Character.LastPulse = DateTime.Now;
            }
            else
            {
                Session.Disconnect();
            }
            Session.Character.MuteMessage();
            Session.Character.DeleteTimeout();
            CommunicationServiceClient.Instance.PulseAccount(Session.Account.AccountId);
        }

        /// <summary>
        /// rlPacket packet
        /// </summary>
        /// <param name="rdPacket"></param>
        /// <param name="rlPacket"></param>
        public void RaidListRegister(RlPacket rlPacket)
        {
            switch (rlPacket.Type)
            {
                case 0:
                    if (Session.Character.Group?.IsLeader(Session) == true && Session.Character.Group.GroupType != GroupType.Group && ServerManager.Instance.GroupList.Any(s => s.GroupId == Session.Character.Group.GroupId))
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateRl(1));
                    }
                    else if (Session.Character.Group != null && Session.Character.Group.GroupType != GroupType.Group && Session.Character.Group.IsLeader(Session))
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateRl(2));
                    }
                    else if (Session.Character.Group != null)
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateRl(3));
                    }
                    else
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateRl(0));
                    }
                    break;

                case 1:
                    if (Session.Character.Group != null && Session.Character.Group.GroupType != GroupType.Group && !ServerManager.Instance.GroupList.Any(s => s.GroupId == Session.Character.Group.GroupId))
                    {
                        ServerManager.Instance.GroupList.Add(Session.Character.Group);
                        Session.SendPacket(UserInterfaceHelper.GenerateRl(1));
                        Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("RAID_REGISTERED")));
                        ServerManager.Instance.Broadcast(Session, $"qnaml 100 #rl {string.Format(Language.Instance.GetMessageFromKey("SEARCH_TEAM_MEMBERS"), Session.Character.Name, Session.Character.Group.Raid?.Label)}", ReceiverType.AllExceptGroup);
                    }
                    break;

                case 2:
                    if (Session.Character.Group != null && Session.Character.Group.GroupType != GroupType.Group && ServerManager.Instance.GroupList.Any(s => s.GroupId == Session.Character.Group.GroupId))
                    {
                        ServerManager.Instance.GroupList.Remove(Session.Character.Group);
                        Session.SendPacket(UserInterfaceHelper.GenerateRl(2));
                        Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("RAID_UNREGISTERED")));
                    }
                    break;

                case 3:
                    ClientSession cl = ServerManager.Instance.GetSessionByCharacterName(rlPacket.CharacterName);
                    if (cl != null)
                    {
                        cl.Character.GroupSentRequestCharacterIds.Add(Session.Character.CharacterId);
                        GroupJoin(new PJoinPacket() { RequestType = GroupRequestType.Accepted, CharacterId = cl.Character.CharacterId });
                    }
                    break;
            }
        }

        /// <summary>
        /// rdPacket packet
        /// </summary>
        /// <param name="rdPacket"></param>
        public void RaidManage(RdPacket rdPacket)
        {
            Group grp;
            switch (rdPacket.Type)
            {
                // Join Raid
                case 1:
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.RaidInstance)
                    {
                        return;
                    }
                    ClientSession target = ServerManager.Instance.GetSessionByCharacterId(rdPacket.CharacterId);
                    if (rdPacket.Parameter == null && target?.Character?.Group == null && Session.Character.Group.IsLeader(Session))
                    {
                        GroupJoin(new PJoinPacket() { RequestType = GroupRequestType.Invited, CharacterId = rdPacket.CharacterId });
                    }
                    else if (Session.Character.Group == null)
                    {
                        GroupJoin(new PJoinPacket() { RequestType = GroupRequestType.Accepted, CharacterId = rdPacket.CharacterId });
                    }
                    break;

                // Leave Raid
                case 2:
                    ClientSession sender = ServerManager.Instance.GetSessionByCharacterId(rdPacket.CharacterId);
                    if (sender?.Character?.Group == null)
                    {
                        return;
                    }

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("LEFT_RAID")), 0));
                    if (Session?.CurrentMapInstance?.MapInstanceType == MapInstanceType.RaidInstance)
                    {
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, Session.Character.MapId, Session.Character.MapX, Session.Character.MapY);
                    }
                    grp = sender.Character?.Group;
                    Session.SendPacket(Session.Character.GenerateRaid(1, true));
                    Session.SendPacket(Session.Character.GenerateRaid(2, true));

                    grp.Characters.ForEach(s =>
                    {
                        s.SendPacket(grp.GenerateRdlst());
                        s.SendPacket(grp.GeneraterRaidmbf(s));
                        s.SendPacket(s.Character.GenerateRaid(0, false));
                    });
                    break;

                // Kick from Raid
                case 3:
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.RaidInstance)
                    {
                        return;
                    }
                    if (Session.Character.Group?.IsLeader(Session) == true)
                    {
                        ClientSession chartokick = ServerManager.Instance.GetSessionByCharacterId(rdPacket.CharacterId);
                        if (chartokick.Character?.Group == null)
                        {
                            return;
                        }

                        chartokick.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("KICK_RAID"), 0));
                        grp = chartokick.Character?.Group;
                        chartokick.SendPacket(chartokick.Character.GenerateRaid(1, true));
                        chartokick.SendPacket(chartokick.Character.GenerateRaid(2, true));
                        grp.LeaveGroup(chartokick);
                        grp.Characters.ForEach(s =>
                        {
                            s.SendPacket(grp.GenerateRdlst());
                            s.SendPacket(s.Character.GenerateRaid(0, false));
                        });
                    }

                    break;

                // Disolve Raid
                case 4:
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.RaidInstance)
                    {
                        return;
                    }
                    if (Session.Character.Group?.IsLeader(Session) == true)
                    {
                        grp = Session.Character.Group;

                        ClientSession[] grpmembers = new ClientSession[40];
                        grp.Characters.CopyTo(grpmembers);
                        foreach (ClientSession targetSession in grpmembers)
                        {
                            if (targetSession != null)
                            {
                                targetSession.SendPacket(targetSession.Character.GenerateRaid(1, true));
                                targetSession.SendPacket(targetSession.Character.GenerateRaid(2, true));
                                targetSession.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("RAID_DISOLVED"), 0));
                                grp.LeaveGroup(targetSession);
                            }
                        }
                        ServerManager.Instance.GroupList.RemoveAll(s => s.GroupId == grp.GroupId);
                        ServerManager.Instance.GroupsThreadSafe.Remove(grp.GroupId);
                    }

                    break;
            }
        }

        /// <summary>
        /// req_info packet
        /// </summary>
        /// <param name="reqInfoPacket"></param>
        public void ReqInfo(ReqInfoPacket reqInfoPacket)
        {
            if (reqInfoPacket.Type == 6)
            {
                if (reqInfoPacket.MateVNum.HasValue)
                {
                    Mate mate = Session.CurrentMapInstance.Sessions.FirstOrDefault(s => s.Character?.Mates != null && s.Character.Mates.Any(o => o.MateTransportId == reqInfoPacket.MateVNum.Value))?.Character.Mates.Find(o => o.MateTransportId == reqInfoPacket.MateVNum.Value);
                    Session.SendPacket(mate?.GenerateEInfo());
                }
            }
            else if (reqInfoPacket.Type == 5)
            {
                NpcMonster npc = ServerManager.GetNpc((short)reqInfoPacket.TargetVNum);
                if (npc != null)
                {
                    Session.SendPacket(npc.GenerateEInfo());
                }
            }
            else
            {
                Session.SendPacket(ServerManager.Instance.GetSessionByCharacterId(reqInfoPacket.TargetVNum)?.Character?.GenerateReqInfo());
            }
        }

        /// <summary>
        /// rest packet
        /// </summary>
        /// <param name="sitpacket"></param>
        public void Rest(SitPacket sitpacket)
        {
            if (Session.Character.MeditationDictionary.Count != 0)
            {
                Session.Character.MeditationDictionary.Clear();
            }
            sitpacket.Users?.ForEach(u =>
            {
                if (u.UserType == 1)
                {
                    Session.Character.Rest();
                }
                else
                {
                    Session.CurrentMapInstance.Broadcast(Session.Character.Mates.Find(s => s.MateTransportId == (int)u.UserId)?.GenerateRest());
                }
            });
        }

        /// <summary>
        /// revival packet
        /// </summary>
        /// <param name="revivalPacket"></param>
        public void Revive(RevivalPacket revivalPacket)
        {
            if (Session.Character.Hp > 0)
            {
                return;
            }

            switch (revivalPacket.Type)
            {
                case 0:
                    switch (Session.CurrentMapInstance.MapInstanceType)
                    {
                        case MapInstanceType.LodInstance:
                            const int saver = 1211;
                            if (Session.Character.Inventory.CountItem(saver) < 1)
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_SAVER"), 0));
                                ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
                            }
                            else
                            {
                                Session.Character.Inventory.RemoveItemAmount(saver);
                                Session.Character.Hp = (int)Session.Character.HPLoad();
                                Session.Character.Mp = (int)Session.Character.MPLoad();
                                Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateRevive());
                                Session.SendPacket(Session.Character.GenerateStat());
                            }
                            break;

                        default:
                            const int seed = 1012;
                            if (Session.Character.Inventory.CountItem(seed) < 10 && Session.Character.Level > 20)
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_POWER_SEED"), 0));
                                ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_SEED_SAY"), 0));
                            }
                            else
                            {
                                if (Session.Character.Level > 20)
                                {
                                    Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("SEED_USED"), 10), 10));
                                    Session.Character.Inventory.RemoveItemAmount(seed, 10);
                                    Session.Character.Hp = (int)(Session.Character.HPLoad() / 2);
                                    Session.Character.Mp = (int)(Session.Character.MPLoad() / 2);
                                }
                                else
                                {
                                    Session.Character.Hp = (int)Session.Character.HPLoad();
                                    Session.Character.Mp = (int)Session.Character.MPLoad();
                                }
                                Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateTp());
                                Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateRevive());
                                Session.SendPacket(Session.Character.GenerateStat());
                            }
                            break;
                    }
                    break;

                case 1:
                    ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
                    break;

                case 2:
                    if (Session.Character.Gold >= 100)
                    {
                        Session.Character.Hp = (int)Session.Character.HPLoad();
                        Session.Character.Mp = (int)Session.Character.MPLoad();
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateTp());
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateRevive());
                        Session.SendPacket(Session.Character.GenerateStat());
                        Session.Character.Gold -= 100;
                        Session.SendPacket(Session.Character.GenerateGold());
                        Session.Character.LastPVPRevive = DateTime.Now;
                        Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe(observer => Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PVP_ACTIVE"), 10)));
                    }
                    else
                    {
                        ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
                    }
                    break;
            }
        }

        /// <summary>
        /// say packet
        /// </summary>
        /// <param name="sayPacket"></param>
        public void Say(SayPacket sayPacket)
        {
            if (string.IsNullOrWhiteSpace(sayPacket.Message))
            {
                return;
            }
            Session.Character.MessageCounter += 2;
            if (Session.Character.MessageCounter > 11)
            {
                return;
            }
            bool isMuted = Session.Character.MuteMessage();
            string message = sayPacket.Message;

            if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance)
          {
                ConcurrentBag < ArenaTeamMember > member = ServerManager.Instance.ArenaTeams.FirstOrDefault(s => s.Any(e => e.Session == Session));
                if (member != null)
            {
                    ArenaTeamMember member2 = member.FirstOrDefault(o => o.Session == Session);
                    member.Where(s => s.ArenaTeamType == member2.ArenaTeamType && s != member2).Where(s => s.ArenaTeamType == member.FirstOrDefault(o => o.Session == Session).ArenaTeamType).ToList().ForEach(o => o.Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_FEMALE"), 1)));
             }
               }
               else
             {
                Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_FEMALE"), 1));
                }

            if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance)
              {
                ConcurrentBag < ArenaTeamMember > member = ServerManager.Instance.ArenaTeams.FirstOrDefault(s => s.Any(e => e.Session == Session));
                if (member != null)
          {
                    ArenaTeamMember member2 = member.FirstOrDefault(o => o.Session == Session);
                    member.Where(s => s.ArenaTeamType == member2.ArenaTeamType && s != member2).Where(s => s.ArenaTeamType == member.FirstOrDefault(o => o.Session == Session).ArenaTeamType).ToList().ForEach(o => o.Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_MALE"), 1)));
               }
               }
             else
            {
               Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_MALE"), 1));
          }

            if (!isMuted)
            {
                byte type = 0;
                if (Session.Character.Authority == AuthorityType.Moderator)
                {
                    type = 12;
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance)
                          {
                        ConcurrentBag < ArenaTeamMember > member = ServerManager.Instance.ArenaTeams.FirstOrDefault(s => s.Any(e => e.Session == Session));
                           if (member != null)
                             {
                            ArenaTeamMember member2 = member.FirstOrDefault(o => o.Session == Session);
                           member.Where(s => s.ArenaTeamType == member2.ArenaTeamType && s != member2).Where(s => s.ArenaTeamType == member.FirstOrDefault(o => o.Session == Session).ArenaTeamType).ToList().ForEach(o => o.Session.SendPacket(Session.Character.GenerateSay(message.Trim(), 1)));
                       }

                      }

                     else
                    {

                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateSay(message.Trim(), 1), ReceiverType.AllExceptMe);
                                            }
                    message = $"[Support {Session.Character.Name}]: {message}";
                }
                if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance)
                                    {
                    ConcurrentBag < ArenaTeamMember > member = ServerManager.Instance.ArenaTeams.FirstOrDefault(s => s.Any(e => e.Session == Session));
                     if (member != null)
                        {
                        ArenaTeamMember member2 = member.FirstOrDefault(o => o.Session == Session);
                        member.Where(s => s.ArenaTeamType == member2.ArenaTeamType && s != member2).ToList().ForEach(o => o.Session.SendPacket(Session.Character.GenerateSay(message.Trim(), 1)));
                   }

                  }

                  else
                {
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateSay(message.Trim(), type), ReceiverType.AllExceptMe);
                     }
            }
        }

        /// <summary>
        /// pst packet
        /// </summary>
        /// <param name="pstPacket"></param>
        public void SendMail(PstPacket pstPacket)
        {
            if (pstPacket?.Data != null)
            {
                CharacterDTO Receiver = DAOFactory.CharacterDAO.LoadByName(pstPacket.Receiver);
                if (Receiver != null)
                {
                    string[] datasplit = pstPacket.Data.Split(' ');
                    if (datasplit.Length < 2)
                    {
                        return;
                    }
                    if (datasplit[1].Length > 250)
                    {
                        //PenaltyLogDTO log = new PenaltyLogDTO
                        //{
                        //    AccountId = Session.Character.AccountId,
                        //    Reason = "You are an idiot!",
                        //    Penalty = PenaltyType.Banned,
                        //    DateStart = DateTime.Now,
                        //    DateEnd = DateTime.Now.AddYears(69),
                        //    AdminName = "Your mom's ass"
                        //};
                        //Session.Character.InsertOrUpdatePenalty(log);
                        //ServerManager.Instance.Kick(Session.Character.Name);
                        return;
                    }
                    ItemInstance headWearable = Session.Character.Inventory.LoadBySlotAndType((byte)EquipmentType.Hat, InventoryType.Wear);
                    byte color = headWearable?.Item.IsColored == true ? (byte)headWearable.Design : (byte)Session.Character.HairColor;
                    MailDTO mailcopy = new MailDTO
                    {
                        AttachmentAmount = 0,
                        IsOpened = false,
                        Date = DateTime.Now,
                        Title = datasplit[0],
                        Message = datasplit[1],
                        ReceiverId = Receiver.CharacterId,
                        SenderId = Session.Character.CharacterId,
                        IsSenderCopy = true,
                        SenderClass = Session.Character.Class,
                        SenderGender = Session.Character.Gender,
                        SenderHairColor = Enum.IsDefined(typeof(HairColorType), color) ? (HairColorType)color : 0,
                        SenderHairStyle = Session.Character.HairStyle,
                        EqPacket = Session.Character.GenerateEqListForPacket(),
                        SenderMorphId = Session.Character.Morph == 0 ? (short)-1 : (short)(Session.Character.Morph > short.MaxValue ? 0 : Session.Character.Morph)
                    };
                    MailDTO mail = new MailDTO
                    {
                        AttachmentAmount = 0,
                        IsOpened = false,
                        Date = DateTime.Now,
                        Title = datasplit[0],
                        Message = datasplit[1],
                        ReceiverId = Receiver.CharacterId,
                        SenderId = Session.Character.CharacterId,
                        IsSenderCopy = false,
                        SenderClass = Session.Character.Class,
                        SenderGender = Session.Character.Gender,
                        SenderHairColor = Enum.IsDefined(typeof(HairColorType), color) ? (HairColorType)color : 0,
                        SenderHairStyle = Session.Character.HairStyle,
                        EqPacket = Session.Character.GenerateEqListForPacket(),
                        SenderMorphId = Session.Character.Morph == 0 ? (short)-1 : (short)(Session.Character.Morph > short.MaxValue ? 0 : Session.Character.Morph)
                    };

                    MailServiceClient.Instance.SendMail(mailcopy);
                    MailServiceClient.Instance.SendMail(mail);

                    //Session.Character.MailList.Add((Session.Character.MailList.Count > 0 ? Session.Character.MailList.OrderBy(s => s.Key).Last().Key : 0) + 1, mailcopy);
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MAILED"), 11));
                    //Session.SendPacket(Session.Character.GeneratePost(mailcopy, 2));
                }
                else
                {
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_FOUND"), 10));
                }
            }
            else if (int.TryParse(pstPacket.Id.ToString(), out int id) && byte.TryParse(pstPacket.Type.ToString(), out byte type))
            {
                if (pstPacket.Argument == 3)
                {
                    if (Session.Character.MailList.ContainsKey(id))
                    {
                        if (!Session.Character.MailList[id].IsOpened)
                        {
                            Session.Character.MailList[id].IsOpened = true;
                            MailDTO mailupdate = Session.Character.MailList[id];
                            DAOFactory.MailDAO.InsertOrUpdate(ref mailupdate);
                        }
                        Session.SendPacket(Session.Character.GeneratePostMessage(Session.Character.MailList[id], type));
                    }
                }
                else if (pstPacket.Argument == 2)
                {
                    if (Session.Character.MailList.ContainsKey(id))
                    {
                        MailDTO mail = Session.Character.MailList[id];
                        Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MAIL_DELETED"), 11));
                        Session.SendPacket($"post 2 {type} {id}");
                        if (DAOFactory.MailDAO.LoadById(mail.MailId) != null)
                        {
                            DAOFactory.MailDAO.DeleteById(mail.MailId);
                        }
                        if (Session.Character.MailList.ContainsKey(id))
                        {
                            Session.Character.MailList.Remove(id);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// qset packet
        /// </summary>
        /// <param name="qSetPacket"></param>
        public void SetQuicklist(QSetPacket qSetPacket)
        {
            short data1 = 0, data2 = 0, type = qSetPacket.Type, q1 = qSetPacket.Q1, q2 = qSetPacket.Q2;
            if (qSetPacket.Data1.HasValue)
            {
                data1 = qSetPacket.Data1.Value;
            }
            if (qSetPacket.Data2.HasValue)
            {
                data2 = qSetPacket.Data2.Value;
            }
            switch (type)
            {
                case 0:
                case 1:

                    // client says qset 0 1 3 2 6 answer -> qset 1 3 0.2.6.0
                    Session.Character.QuicklistEntries.RemoveAll(n => n.Q1 == q1 && n.Q2 == q2 && (Session.Character.UseSp ? n.Morph == Session.Character.Morph : n.Morph == 0));
                    Session.Character.QuicklistEntries.Add(new QuicklistEntryDTO
                    {
                        CharacterId = Session.Character.CharacterId,
                        Type = type,
                        Q1 = q1,
                        Q2 = q2,
                        Slot = data1,
                        Pos = data2,
                        Morph = Session.Character.UseSp ? (short)Session.Character.Morph : (short)0
                    });
                    Session.SendPacket($"qset {q1} {q2} {type}.{data1}.{data2}.0");
                    break;

                case 2:

                    // DragDrop / Reorder qset type to1 to2 from1 from2 vars -> q1 q2 data1 data2
                    QuicklistEntryDTO qlFrom = Session.Character.QuicklistEntries.SingleOrDefault(n => n.Q1 == data1 && n.Q2 == data2 && (Session.Character.UseSp ? n.Morph == Session.Character.Morph : n.Morph == 0));
                    if (qlFrom != null)
                    {
                        QuicklistEntryDTO qlTo = Session.Character.QuicklistEntries.SingleOrDefault(n => n.Q1 == q1 && n.Q2 == q2 && (Session.Character.UseSp ? n.Morph == Session.Character.Morph : n.Morph == 0));
                        qlFrom.Q1 = q1;
                        qlFrom.Q2 = q2;
                        if (qlTo == null)
                        {
                            // Put 'from' to new position (datax)
                            Session.SendPacket($"qset {qlFrom.Q1} {qlFrom.Q2} {qlFrom.Type}.{qlFrom.Slot}.{qlFrom.Pos}.0");

                            // old 'from' is now empty.
                            Session.SendPacket($"qset {data1} {data2} 7.7.-1.0");
                        }
                        else
                        {
                            // Put 'from' to new position (datax)
                            Session.SendPacket($"qset {qlFrom.Q1} {qlFrom.Q2} {qlFrom.Type}.{qlFrom.Slot}.{qlFrom.Pos}.0");

                            // 'from' is now 'to' because they exchanged
                            qlTo.Q1 = data1;
                            qlTo.Q2 = data2;
                            Session.SendPacket($"qset {qlTo.Q1} {qlTo.Q2} {qlTo.Type}.{qlTo.Slot}.{qlTo.Pos}.0");
                        }
                    }

                    break;

                case 3:

                    // Remove from Quicklist
                    Session.Character.QuicklistEntries.RemoveAll(n => n.Q1 == q1 && n.Q2 == q2 && (Session.Character.UseSp ? n.Morph == Session.Character.Morph : n.Morph == 0));
                    Session.SendPacket($"qset {q1} {q2} 7.7.-1.0");
                    break;

                default:
                    return;
            }
        }

        /// <summary>
        /// game_start packet
        /// </summary>
        /// <param name="gameStartPacket"></param>
        public void StartGame(GameStartPacket gameStartPacket)
        {
            if (Session.IsOnMap || !Session.HasSelectedCharacter)
            {
                // character should have been selected in SelectCharacter
                return;
            }
            if (Session.Character.MapInstance.Map.MapTypes.Any(m => m.MapTypeId == (short)MapTypeEnum.Act4) && ServerManager.Instance.ChannelId != 51)
            {
                // Change IP to yours
                Session.Character.ChangeChannel(ServerManager.Instance.Configuration.Act4IP, ServerManager.Instance.Configuration.Act4Port, 2);
            }
            Session.CurrentMapInstance = Session.Character.MapInstance;
            if (ServerManager.Instance.Configuration.SceneOnCreate && Session.Character.GeneralLogs.CountLinq(s => s.LogType == "Connection") < 2)
            {
                Session.SendPacket("scene 40");
            }
            if (ServerManager.Instance.Configuration.WorldInformation)
            {
                Assembly assembly = Assembly.GetEntryAssembly();
                string productVersion = assembly?.Location != null ? FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion : "1337";
                //Session.SendPacket(Session.Character.GenerateSay("----------[ Welcome ]----------", 10));
                //Session.SendPacket(Session.Character.GenerateSay($"XPRate: {ServerManager.Instance.Configuration.RateXP}", 11));
                //Session.SendPacket(Session.Character.GenerateSay($"DropRate: {ServerManager.Instance.Configuration.RateDrop}", 11));
                //Session.SendPacket(Session.Character.GenerateSay($"GoldRate: {ServerManager.Instance.Configuration.RateGold}", 11));
                //Session.SendPacket(Session.Character.GenerateSay($"FairyXp: {ServerManager.Instance.Configuration.RateFairyXP}", 11));
                Session.SendPacket(Session.Character.GenerateSay("----------[ Follow Us ]----------", 10));
                Session.SendPacket(Session.Character.GenerateSay("Discord: https://discord.gg/Cw36Tvu", 12));
                
            }
            Session.Character.LoadSpeed();
            Session.Character.LoadSkills();
            Session.SendPacket(Session.Character.GenerateTit());
            Session.SendPacket(Session.Character.GenerateSpPoint());
            Session.SendPacket("rsfi 1 1 0 9 0 9");
            if (Session.Character.Hp <= 0)
            {
                ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
            }
            else
            {
                ServerManager.Instance.ChangeMap(Session.Character.CharacterId);
            }
            Session.SendPacket(Session.Character.GenerateSki());
            Session.SendPacket($"fd {Session.Character.Reputation} 0 {(int)Session.Character.Dignity} {Math.Abs(Session.Character.GetDignityIco())}");
            Session.SendPacket(Session.Character.GenerateFd());
            Session.SendPacket("rage 0 250000");
            Session.SendPacket("rank_cool 0 0 18000");
            ItemInstance specialistInstance = Session.Character.Inventory.LoadBySlotAndType(8, InventoryType.Wear);
            StaticBonusDTO medal = Session.Character.StaticBonusList.Find(s => s.StaticBonusType == StaticBonusType.BazaarMedalGold || s.StaticBonusType == StaticBonusType.BazaarMedalSilver);
            if (medal != null)
            {
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("LOGIN_MEDAL"), 12));
            }
            if (Session.Character.StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.PetBasket))
            {
                Session.SendPacket("ib 1278 1");
            }
            if (Session.Character.MapInstance.Map.MapTypes.Any(m => m.MapTypeId == (short)MapTypeEnum.CleftOfDarkness))
            {
                Session.SendPacket("bc 0 0 0");
            }
            if (specialistInstance != null)
            {
                Session.SendPacket(Session.Character.GenerateSpPoint());
            }
            Session.SendPacket("scr 0 0 0 0 0 0");
            for (int i = 0; i < 10; i++)
            {
                Session.SendPacket($"bn {i} {Language.Instance.GetMessageFromKey($"BN{i}")}");
            }
            Session.SendPacket(Session.Character.GenerateExts());
            Session.SendPacket(Session.Character.GenerateMlinfo());
            Session.SendPacket(UserInterfaceHelper.GeneratePClear());

            Session.SendPacket(Session.Character.GeneratePinit());
            Session.SendPackets(Session.Character.GeneratePst());

            Session.SendPacket("zzim");
            Session.SendPacket($"twk 1 {Session.Character.CharacterId} {Session.Account.Name} {Session.Character.Name} shtmxpdlfeoqkr");

            if (Session.Character.Family != null && Session.Character.FamilyCharacter != null)
            {
                Session.SendPacket(Session.Character.GenerateGInfo());
                Session.SendPackets(Session.Character.GetFamilyHistory());
                Session.SendPacket(Session.Character.GenerateFamilyMember());
                Session.SendPacket(Session.Character.GenerateFamilyMemberMessage());
                Session.SendPacket(Session.Character.GenerateFamilyMemberExp());
                try
                {
                    Session.Character.Faction = Session.Character.Family.FamilyCharacters.Find(s => s.Authority.Equals(FamilyAuthority.Head)).Character.Faction;
                }
                catch
                {
                }
                if (!string.IsNullOrWhiteSpace(Session.Character.Family.FamilyMessage))
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateInfo("--- Family Message ---\n" + Session.Character.Family.FamilyMessage));
                }
            }

            long? familyId = DAOFactory.FamilyCharacterDAO.LoadByCharacterId(Session.Character.CharacterId)?.FamilyId;
            if (familyId.HasValue)
            {
                Session.Character.Family = ServerManager.Instance.FamilyList[familyId.Value];
            }

            // qstlist target sqst bf
            Session.SendPacket("act6");
            Session.SendPacket(Session.Character.GenerateFaction());
            Session.SendPackets(Session.Character.GenerateScP());
            Session.SendPackets(Session.Character.GenerateScN());
#pragma warning disable 618
            Session.Character.GenerateStartupInventory();
#pragma warning restore 618

            Session.SendPacket(Session.Character.GenerateGold());
            Session.SendPackets(Session.Character.GenerateQuicklist());

            string clinit = "clinit";
            string flinit = "flinit";
            string kdlinit = "kdlinit";
            foreach (CharacterDTO character in ServerManager.Instance.TopComplimented)
            {
                clinit += $" {character.CharacterId}|{character.Level}|{character.HeroLevel}|{character.Compliment}|{character.Name}";
            }
            foreach (CharacterDTO character in ServerManager.Instance.TopReputation)
            {
                flinit += $" {character.CharacterId}|{character.Level}|{character.HeroLevel}|{character.Reputation}|{character.Name}";
            }
            foreach (CharacterDTO character in ServerManager.Instance.TopPoints)
            {
                kdlinit += $" {character.CharacterId}|{character.Level}|{character.HeroLevel}|{character.Act4Points}|{character.Name}";
            }

            Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateGidx());

            Session.SendPacket(Session.Character.GenerateFinit());
            Session.SendPacket(Session.Character.GenerateBlinit());
            Session.SendPacket(clinit);
            Session.SendPacket(flinit);
            Session.SendPacket(kdlinit);

            Session.Character.LastPVPRevive = DateTime.Now;

            IEnumerable<PenaltyLogDTO> warning = DAOFactory.PenaltyLogDAO.LoadByAccount(Session.Character.AccountId).Where(p => p.Penalty == PenaltyType.Warning);
            if (warning.Any())
            {
                Session.SendPacket(UserInterfaceHelper.GenerateInfo(string.Format(Language.Instance.GetMessageFromKey("WARNING_INFO"), warning.Count())));
            }
            //Messaggio Benvenuto
            if (Session.Character.Level == 1)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateModal("Welcome to NosHeat Adventurer! Join our Discord to get the lastest news and events! Enjoy ;)", 1));
                

            }
            //Messaggio GM
            if (Session.Character.Authority == AuthorityType.GameMaster)
            {
                Session.SendPacket(Session.Character.GenerateSay("------------------------", 10));
                Session.SendPacket(Session.Character.GenerateSay("Welcome " + Session.Character.Name, 12));
                Session.SendPacket(Session.Character.GenerateSay("Use $Help command to get help", 12));
                Session.SendPacket(Session.Character.GenerateSay("------------------------", 10));
                ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 10000, 5, 5);
            }
            //Messaggio Bank
            if (Session.Character.Authority == AuthorityType.User)
            {

                Session.SendPacket(Session.Character.GenerateSay("Use $Bank to deposit your gold.", 10));
                Session.SendPacket(Session.Character.GenerateSay("Use $HelpMe to contact a team member", 10)); 


            }
            //Messaggio Supporter
            if (Session.Character.Authority == AuthorityType.Moderator)
            {

                Session.SendPacket(Session.Character.GenerateSay("Use $Bank to deposit your gold.", 10));
                Session.SendPacket(Session.Character.GenerateSay("--------Commands Supporter--------", 11));
                Session.SendPacket(Session.Character.GenerateSay("$Invisible ", 12));
                Session.SendPacket(Session.Character.GenerateSay("$Warn", 12));

            }

            // finfo - friends info
            Session.Character.LoadMail();
            Session.Character.LoadSentMail();
            Session.Character.DeleteTimeout();

            foreach (StaticBuffDTO staticBuff in DAOFactory.StaticBuffDAO.LoadByCharacterId(Session.Character.CharacterId))
            {
                Session.Character.AddStaticBuff(staticBuff);
            }
            if (Session.Character.Authority == AuthorityType.BitchNiggerFaggot)
            {
                CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage()
                {
                    DestinationCharacterId = null,
                    SourceCharacterId = Session.Character.CharacterId,
                    SourceWorldId = ServerManager.Instance.WorldId,
                    Message = $"User {Session.Character.Name} with rank BitchNiggerFaggot has logged in, don't trust *it*!",
                    Type = MessageType.Shout
                });
            }

            //QuestModel quest = ServerManager.Instance.QuestList.Where(s => s.QuestGiver.Type == QuestGiverType.InitialQuest).FirstOrDefault();
            //if(quest != null)
            //{
            //    quest = quest.Copy();

            //    int current = 0;
            //    int max = 0;

            //    if (quest.KillObjectives != null)
            //    {
            //        max = quest.KillObjectives[0].GoalAmount;
            //        current = quest.KillObjectives[0].CurrentAmount;
            //    }

            //    if(quest.WalkObjective != null)
            //    {
            //        Session.SendPacket($"target {quest.WalkObjective.MapX} {quest.WalkObjective.MapY} {quest.WalkObjective.MapId} {quest.QuestDataVNum}");
            //    }

            //    //Quest Packet Definition: qstlist {Unknown}.{QuestVNUM}.{QuestVNUM}.{GoalType}.{Current}.{Goal}.{Finished}.{GoalType}.{Current}.{Goal}.{Finished}.{GoalType}.{Current}.{Goal}.{Finished}.{ShowDialog}
            //    //Same for qsti
            //    Session.SendPacket($"qstlist 5.{quest.QuestDataVNum}.{quest.QuestDataVNum}.{quest.QuestGoalType}.{current}.{max}.0.0.0.0.0.0.0.0.0.1");

            //}
        }

        /// <summary>
        /// walk packet
        /// </summary>
        /// <param name="walkPacket"></param>
        public void Walk(WalkPacket walkPacket)
        {
            if (!Session.Character.NoMove)
            {
                if (Session.Character.MeditationDictionary.Count != 0)
                {
                    Session.Character.MeditationDictionary.Clear();
                }
                double currentRunningSeconds = (DateTime.Now - Process.GetCurrentProcess().StartTime.AddSeconds(-50)).TotalSeconds;
                double timeSpanSinceLastPortal = currentRunningSeconds - Session.Character.LastPortal;
                int distance = Map.GetDistance(new MapCell { X = Session.Character.PositionX, Y = Session.Character.PositionY }, new MapCell { X = walkPacket.XCoordinate, Y = walkPacket.YCoordinate });

                if (Session.HasCurrentMapInstance && !Session.CurrentMapInstance.Map.IsBlockedZone(walkPacket.XCoordinate, walkPacket.YCoordinate) && !Session.Character.IsChangingMapInstance && !Session.Character.HasShopOpened)
                {
                    if ((Session.Character.Speed >= walkPacket.Speed || Session.Character.LastSpeedChange.AddSeconds(5) > DateTime.Now) && !(distance > 60 && timeSpanSinceLastPortal > 10))
                    {
                        if (Session.Character.MapInstance.MapInstanceType == MapInstanceType.BaseMapInstance)
                        {
                            Session.Character.MapX = walkPacket.XCoordinate;
                            Session.Character.MapY = walkPacket.YCoordinate;
                        }
                        Session.Character.PositionX = walkPacket.XCoordinate;
                        Session.Character.PositionY = walkPacket.YCoordinate;

                        if (Session.Character.LastMonsterAggro.AddSeconds(5) > DateTime.Now)
                        {
                            Session.Character.UpdateBushFire();
                        }
                        if (!Session.Character.InvisibleGm)
                        {
                            Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.Move(UserType.Player, Session.Character.CharacterId, Session.Character.PositionX, Session.Character.PositionY, Session.Character.Speed));
                        }
                        Session.SendPacket(Session.Character.GenerateCond());
                        Session.Character.LastMove = DateTime.Now;

                        Session.CurrentMapInstance?.OnAreaEntryEvents?.Where(s => s.InZone(Session.Character.PositionX, Session.Character.PositionY)).ToList().ForEach(e => e.Events.ForEach(evt => EventHelper.Instance.RunEvent(evt)));
                        Session.CurrentMapInstance?.OnAreaEntryEvents?.RemoveAll(s => s.InZone(Session.Character.PositionX, Session.Character.PositionY));

                        Session.CurrentMapInstance?.OnMoveOnMapEvents?.ForEach(e => EventHelper.Instance.RunEvent(e));
                        Session.CurrentMapInstance?.OnMoveOnMapEvents?.RemoveAll(s => s != null);
                    }
                    else
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateInfo("NosArmy doesn't need cheaters!"));
                    }
                }
            }
        }

        /// <summary>
        /// / packet
        /// </summary>
        /// <param name="whisperPacket"></param>
        public void Whisper(WhisperPacket whisperPacket)
        {
            try
            {
                // TODO: Implement WhisperSupport
                if (string.IsNullOrEmpty(whisperPacket.Message))
                {
                    return;
                }
                string characterName = whisperPacket.Message.Split(' ')[whisperPacket.Message.StartsWith("GM ") ? 1 : 0].Replace("[Support]", string.Empty).Replace("[BitchNiggerFaggot]", string.Empty);
                string message = string.Empty;
                string[] packetsplit = whisperPacket.Message.Split(' ');
                for (int i = packetsplit[0] == "GM" ? 2 : 1; i < packetsplit.Length; i++)
                {
                    message += packetsplit[i] + " ";
                }
                if (message.Length > 60)
                {
                    message = message.Substring(0, 60);
                }
                message = message.Trim();
                Session.SendPacket(Session.Character.GenerateSpk(message, 5));
                CharacterDTO receiver = DAOFactory.CharacterDAO.LoadByName(characterName);
                int? sentChannelId = null;
                if (receiver != null)
                {
                    if (receiver.CharacterId == Session.Character.CharacterId)
                    {
                        return;
                    }
                    if (Session.Character.IsBlockedByCharacter(receiver.CharacterId))
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("BLACKLIST_BLOCKED")));
                        return;
                    }
                    ClientSession receiverSession = ServerManager.Instance.GetSessionByCharacterId(receiver.CharacterId);
                    if (receiverSession?.CurrentMapInstance?.Map.MapId == Session.CurrentMapInstance?.Map.MapId && Session.Account.Authority >= AuthorityType.Moderator)
                    {
                        receiverSession.SendPacket(Session.Character.GenerateSay(message, 2));
                    }
                    sentChannelId = CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage()
                    {
                        DestinationCharacterId = receiver.CharacterId,
                        SourceCharacterId = Session.Character.CharacterId,
                        SourceWorldId = ServerManager.Instance.WorldId,
                        Message = Session.Character.Authority == AuthorityType.Moderator ? Session.Character.GenerateSay($"(whisper)(From Support {Session.Character.Name}):{message}", 11) : Session.Character.GenerateSpk(message, Session.Account.Authority == AuthorityType.GameMaster ? 15 : 5),
                        Type = packetsplit[0] == "GM" ? MessageType.WhisperGM : MessageType.Whisper
                    });
                }

                if (sentChannelId == null)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("USER_NOT_CONNECTED")));
                }
            }
            catch (Exception e)
            {
                Logger.Error("Whisper failed.", e);
            }
        }

        #endregion
    }

    
    
    }
