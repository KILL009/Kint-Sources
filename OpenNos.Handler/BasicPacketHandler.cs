using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.DAL.EF;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Packets.ClientPackets;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;
using OpenNos.PathFinder;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OpenNos.Handler
{
    public class BasicPacketHandler : IPacketHandler
    {
        #region Instantiation

        public BasicPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        private ClientSession Session { get; }

        #endregion

        #region Methods
        /// <summary>
        /// arena packet
        /// </summary>
        /// <param name="ArenaPacket"></param>
        public void JoinArena(ArenaPacket arenaPacket)
        {
            if(arenaPacket.Answer1 == 0 && arenaPacket.Answer2 == 1)
            {
                if (Session.CurrentMapInstance.MapInstanceType != MapInstanceType.BaseMapInstance)
                {
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("USER_ON_INSTANCEMAP"), 0));
                    return;
                }

                var currentRunningSeconds = (DateTime.Now - Process.GetCurrentProcess().StartTime.AddSeconds(-50)).TotalSeconds;
                var timeSpanSinceLastPortal = currentRunningSeconds - Session.Character.LastPortal;
                if (!(timeSpanSinceLastPortal >= 4) || !Session.HasCurrentMapInstance)
                {
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MOVE"), 10));
                    return;
                }

                if (Session.Character.Gold >= 500 )
                {
                    Session.Character.LastPortal = currentRunningSeconds;
                    Session.Character.Gold -= 500 ;
                    Session.SendPacket(Session.Character.GenerateGold());
                    ServerManager.Instance.TeleportOnRandomPlaceInMap(Session, ServerManager.Instance.ArenaInstance.MapInstanceId);
                }
                else
                {
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                }
            }
            else if(arenaPacket.Answer1 == 1 && arenaPacket.Answer2 == 1)
            {
                if (Session.CurrentMapInstance.MapInstanceType != MapInstanceType.BaseMapInstance)
                {
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("USER_ON_INSTANCEMAP"), 0));
                    return;
                }

                var currentRunningSeconds = (DateTime.Now - Process.GetCurrentProcess().StartTime.AddSeconds(-50)).TotalSeconds;
                var timeSpanSinceLastPortal = currentRunningSeconds - Session.Character.LastPortal;
                if (!(timeSpanSinceLastPortal >= 4) || !Session.HasCurrentMapInstance)
                {
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MOVE"), 10));
                    return;
                }

                if (Session.Character.Gold >= 1000)
                {
                    Session.Character.LastPortal = currentRunningSeconds;
                    Session.Character.Gold -= 1000;
                    Session.SendPacket(Session.Character.GenerateGold());
                    ServerManager.Instance.TeleportOnRandomPlaceInMap(Session, ServerManager.Instance.FamilyArenaInstance.MapInstanceId);
                }
                else
                {
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                }
            }
        }

        /// <summary>
        /// GBox packet
        /// </summary>
        /// <param name="GBox"></param>
        public void GBoxPacket(GBox gBox)
        {
            if(gBox.Type == 1)
            {
                if(gBox.Type2 == 0)
                {
                    Session.SendPacket($"qna #gbox^1^{gBox.Amount}^1 Want to deposit {gBox.Amount}000 gold?");
                }
                else if(gBox.Type2 == 1)
                {

                    Session.SendPacket($"s_memo 6 You pay {gBox.Amount}000 gold to the bank.");
                    if((Session.Account.BankGold + (gBox.Amount * 1000)) > ServerManager.Instance.MaxBankGold)
                    {
                        Session.SendPacket("info You can not have more than 100.000.000.000 gold on the bank!");

                        Session.SendPacket("s_memo 5 You can not have more than 100.000.000.000 gold on the bank!");
                    }
                    else if (Session.Character.Gold >= (gBox.Amount * 1000))
                    {
                        Session.Account.BankGold += (gBox.Amount*1000);
                        Session.Character.Gold -= (gBox.Amount * 1000);
                        Session.SendPacket(Session.Character.GenerateGold());

                        Session.SendPacket($"gb 1 {Session.Account.BankGold /1000} {Session.Character.Gold} 0 0");
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
            else if(gBox.Type == 2)
            {
                if(gBox.Type2 == 0)
                {

                    Session.SendPacket($"qna #gbox^2^{gBox.Amount}^1 Would you like to withdraw {gBox.Amount}000 gold? (Fee: 0 gold)");
                }
                else if(gBox.Type2 == 1)
                {

                    Session.SendPacket($"s_memo 6 You collect {gBox.Amount}000 gold. (Fee: 0 gold)");
                    if((Session.Character.Gold + (gBox.Amount * 1000)) > ServerManager.Instance.MaxGold)
                    {
                        Session.SendPacket("info You can not carry more than 1.000.000.000 gold with you!");

                        Session.SendPacket("s_memo 5 You can not carry more than 1.000.000.000 gold with you!");
                    }
                    else if (Session.Account.BankGold >= (gBox.Amount * 1000))
                    {
                        Session.Account.BankGold -= (gBox.Amount * 1000);
                        Session.Character.Gold += (gBox.Amount*1000);
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
        /// Mall packet
        /// </summary>
        /// <param name="Mall"></param>
        public void MallPacket(MallPacket mall)
        {
            switch (mall.Type)
            {
                case 10:
                    {
                        Logger.Log.Warn("MallID: " + mall.Id);
                        Mall mallitem = ServerManager.Instance.MallItems.FirstOrDefault(s => s.Id == mall.Id);
                        if(mallitem != null && Session.Account.NosDollar >= mallitem.Price )
                        {
                            byte amount = (byte)mallitem.Amount;
                            short id = (short)mallitem.ItemVnum;
                            Session.Character.SendGift(Session.Character.CharacterId, id, amount, 0, 0, true);
                            Session.Account.NosDollar -= mallitem.Price;
                            Session.SendPacket($"cash 0 0 {Session.Account.NosDollar}");
                            Session.SendPacket($"Successfully bought! {Session.Account.NosDollar} ND are available.");
                        }
                    }
                    break;
            }
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
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.BuffBlocked ? "BUFF_BLOCKED" : "BUFF_UNLOCKED"), 0));
                    break;

                case CharacterOption.EmoticonsBlocked:
                    Session.Character.EmoticonsBlocked = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.EmoticonsBlocked ? "EMO_BLOCKED" : "EMO_UNLOCKED"), 0));
                    break;

                case CharacterOption.ExchangeBlocked:
                    Session.Character.ExchangeBlocked = characterOptionPacket.IsActive == false;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.ExchangeBlocked ? "EXCHANGE_BLOCKED" : "EXCHANGE_UNLOCKED"), 0));
                    break;

                case CharacterOption.FriendRequestBlocked:
                    Session.Character.FriendRequestBlocked = characterOptionPacket.IsActive == false;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.FriendRequestBlocked ? "FRIEND_REQ_BLOCKED" : "FRIEND_REQ_UNLOCKED"), 0));
                    break;

                case CharacterOption.GroupRequestBlocked:
                    Session.Character.GroupRequestBlocked = characterOptionPacket.IsActive == false;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.GroupRequestBlocked ? "GROUP_REQ_BLOCKED" : "GROUP_REQ_UNLOCKED"), 0));
                    break;

                case CharacterOption.HeroChatBlocked:
                    Session.Character.HeroChatBlocked = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.HeroChatBlocked ? "HERO_CHAT_BLOCKED" : "HERO_CHAT_UNLOCKED"), 0));
                    break;

                case CharacterOption.HpBlocked:
                    Session.Character.HpBlocked = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.HpBlocked ? "HP_BLOCKED" : "HP_UNLOCKED"), 0));
                    break;

                case CharacterOption.MinilandInviteBlocked:
                    Session.Character.MinilandInviteBlocked = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.MinilandInviteBlocked ? "MINI_INV_BLOCKED" : "MINI_INV_UNLOCKED"), 0));
                    break;

                case CharacterOption.MouseAimLock:
                    Session.Character.MouseAimLock = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.MouseAimLock ? "MOUSE_LOCKED" : "MOUSE_UNLOCKED"), 0));
                    break;

                case CharacterOption.QuickGetUp:
                    Session.Character.QuickGetUp = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.QuickGetUp ? "QUICK_GET_UP_ENABLED" : "QUICK_GET_UP_DISABLED"), 0));
                    break;

                case CharacterOption.WhisperBlocked:
                    Session.Character.WhisperBlocked = characterOptionPacket.IsActive == false;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.WhisperBlocked ? "WHISPER_BLOCKED" : "WHISPER_UNLOCKED"), 0));
                    break;

                case CharacterOption.FamilyRequestBlocked:
                    Session.Character.FamilyRequestBlocked = characterOptionPacket.IsActive == false;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.FamilyRequestBlocked ? "FAMILY_REQ_LOCKED" : "FAMILY_REQ_UNLOCKED"), 0));
                    break;

                case CharacterOption.GroupSharing:
                    var grp = ServerManager.Instance.Groups.FirstOrDefault(g => g.IsMemberOfGroup(Session.Character.CharacterId));
                    if (grp == null)
                    {
                        return;
                    }

                    if (grp.IsLeader(Session))
                    {
                        Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_MASTER"), 0));
                        return;
                    }

                    if (characterOptionPacket.IsActive == false)
                    {
                        var group = ServerManager.Instance.Groups.FirstOrDefault(s => s.IsMemberOfGroup(Session.Character.CharacterId));
                        if (group != null)
                        {
                            group.SharingMode = 1;
                        }

                        Session.CurrentMapInstance?.Broadcast(Session, UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("SHARING"), 0), ReceiverType.Group);
                    }
                    else
                    {
                        var group = ServerManager.Instance.Groups.FirstOrDefault(s => s.IsMemberOfGroup(Session.Character.CharacterId));
                        if (group != null)
                        {
                            group.SharingMode = 0;
                        }

                        Session.CurrentMapInstance?.Broadcast(Session, UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("SHARING_BY_ORDER"), 0), ReceiverType.Group);
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
            if (complimentPacket == null)
            {
                return;
            }

            var complimentedCharacterId = complimentPacket.CharacterId;
            if (Session.Character.Level >= 30)
            {
                var dto = Session.Character.GeneralLogs.LastOrDefault(s => s.LogData == "World" && s.LogType == "Connection");
                var lastcompliment = Session.Character.GeneralLogs.LastOrDefault(s => s.LogData == "World" && s.LogType == "Compliment");
                if (dto != null && dto.Timestamp.AddMinutes(60) <= DateTime.Now)
                {
                    if (lastcompliment == null || lastcompliment.Timestamp.AddDays(1) <= DateTime.Now.Date)
                    {
                        short? compliment = ServerManager.Instance.GetProperty<short?>(complimentedCharacterId, nameof(GameObject.Character.Compliment));
                        compliment++;
                        ServerManager.Instance.SetProperty(complimentedCharacterId, nameof(GameObject.Character.Compliment), compliment);
                        Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("COMPLIMENT_GIVEN"), ServerManager.Instance.GetProperty<string>(complimentedCharacterId, nameof(GameObject.Character.Name))), 12));
                        Session.Character.GeneralLogs.Add(new GeneralLogDTO
                        {
                            AccountId = Session.Account.AccountId,
                            CharacterId = Session.Character.CharacterId,
                            IpAddress = Session.IpAddress,
                            LogData = "World",
                            LogType = "Compliment",
                            Timestamp = DateTime.Now
                        });

                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("COMPLIMENT_RECEIVED"), Session.Character.Name), 12), ReceiverType.OnlySomeone, characterId: complimentedCharacterId);
                    }
                    else
                    {
                        Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("COMPLIMENT_COOLDOWN"), 11));
                    }
                }
                else
                {
                    if (dto != null)
                    {
                        Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("COMPLIMENT_LOGIN_COOLDOWN"), (dto.Timestamp.AddMinutes(60) - DateTime.Now).Minutes), 11));
                    }
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("COMPLIMENT_NOT_MINLVL"), 11));
            }
        }

        /// <summary>
        /// dir packet
        /// </summary>
        /// <param name="directionPacket"></param>
        public void Dir(DirectionPacket directionPacket)
        {
            if (directionPacket.CharacterId != Session.Character.CharacterId)
            {
                return;
            }

            Session.Character.Direction = directionPacket.Direction;
            Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateDir());
        }

        /// <summary>
        /// pcl packet
        /// </summary>
        /// <param name="getGiftPacket"></param>
        public void GetGift(GetGiftPacket getGiftPacket)
        {
            var giftId = getGiftPacket.GiftId;
            if (!Session.Character.MailList.ContainsKey(giftId))
            {
                return;
            }

            var mail = Session.Character.MailList[giftId];
            if (getGiftPacket.Type == 4 && mail.AttachmentVNum != null)
            {
                if (Session.Character.Inventory.CanAddItem((short)mail.AttachmentVNum))
                {
                    var newInv = Session.Character.Inventory.AddNewToInventory((short)mail.AttachmentVNum, mail.AttachmentAmount, Upgrade: mail.AttachmentUpgrade, Rare: (sbyte)mail.AttachmentRarity).FirstOrDefault();
                    if (newInv == null)
                    {
                        return;
                    }

                    if (newInv.Rare != 0)
                    {
                        var wearable = newInv as GameObject.WearableInstance;
                        wearable?.SetRarityPoint();
                    }

                    var log = new GeneralLogDTO
                    {
                        LogType = "CLAIM_GIFT",
                        LogData = $"CLAIM GIFT {giftId}",
                        IpAddress = Session.IpAddress,
                        Timestamp = DateTime.Now,
                    };
                    DAOFactory.GeneralLogDAO.InsertOrUpdate(ref log);

                    Session.SendPacket(Session.Character.GenerateSay($"{Language.Instance.GetMessageFromKey("ITEM_GIFTED")}: {newInv.Item.Name} x {mail.AttachmentAmount}", 12));

                    Session.Character.MailList.Remove(giftId);

                    Session.SendPacket($"parcel 2 1 {giftId}");
                    if (Session.Character.MailList.ContainsKey(giftId))
                    {
                        Session.Character.MailList.Remove(giftId);
                    }
                }
                else
                {
                    Session.SendPacket("parcel 5 1 0");
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"), 0));
                }
            }
            else if (getGiftPacket.Type == 5)
            {
                Session.SendPacket($"parcel 7 1 {giftId}");

                if (Session.Character.MailList.ContainsKey(giftId))
                {
                    Session.Character.MailList.Remove(giftId);
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
                    Session.SendPacket(ServerManager.Instance.GetSessionByCharacterId(ncifPacket.TargetId)?.Character?.GenerateStatInfo());
                    break;

                // npcs/mates (Flow is it you?)
                case 2:
                    if (Session.HasCurrentMapInstance)
                    {
                        Session.CurrentMapInstance.Npcs.Where(n => n.MapNpcId == (int)ncifPacket.TargetId).ToList().ForEach(npc =>
                        {
                            var npcinfo = ServerManager.Instance.GetNpc(npc.NpcVNum);
                            if (npcinfo == null)
                            {
                                return;
                            }

                            Session.SendPacket($"st 2 {ncifPacket.TargetId} {npcinfo.Level} {npcinfo.HeroLevel} 100 100 50000 50000");
                        });

                        Parallel.ForEach(Session.CurrentMapInstance.Sessions, session =>
                        {
                            var mate = session.Character.Mates.FirstOrDefault(s => s.MateTransportId == (int)ncifPacket.TargetId);
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
                        Session.CurrentMapInstance.Monsters.Where(m => m.MapMonsterId == (int)ncifPacket.TargetId).ToList().ForEach(monster =>
                        {
                            var monsterinfo = ServerManager.Instance.GetNpc(monster.MonsterVNum);
                            if (monsterinfo == null)
                            {
                                return;
                            }

                            Session.Character.LastMonsterId = monster.MapMonsterId;
                            Session.SendPacket($"st 3 {ncifPacket.TargetId} {monsterinfo.Level} {monsterinfo.HeroLevel} {(int)((float)monster.CurrentHp / (float)monster.Monster.MaxHP * 100)} {(int)((float)monster.CurrentMp / (float)monster.Monster.MaxMP * 100)} {monster.CurrentHp} {monster.CurrentMp}{monster.Buff.Replace(s => !s.StaticBuff).Aggregate(string.Empty, (current, buff) => current + $" {buff.Card.CardId}.{buff.Level}")}");
                        });
                    }

                    break;
            }
        }

        /// <summary>
        /// npinfo packet
        /// </summary>
        /// <param name="npinfoPacket"></param>
        public void GetStats(NpinfoPacket npinfoPacket)
        {
            Session.SendPacket(Session.Character.GenerateStatChar());
            if (npinfoPacket.Page == Session.Character.ScPage)
            {
                return;
            }

            Session.Character.ScPage = npinfoPacket.Page;
            Session.SendPacket(UserInterfaceHelper.Instance.GeneratePClear());
            Session.SendPackets(Session.Character.GenerateScN());
            Session.SendPackets(Session.Character.GenerateScP(npinfoPacket.Page));
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

            var message = btkPacket.Message;
            if (message.Length > 60)
            {
                message = message.Substring(0, 60);
            }

            message = message.Trim();

            var character = DAOFactory.CharacterDAO.FirstOrDefault(s => s.CharacterId == btkPacket.CharacterId);
            if (character == null)
            {
                return;
            }

            // session is not on current server, check api if the target character is on another server
            int? sentChannelId = CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage()
            {
                DestinationCharacterId = character.CharacterId,
                SourceCharacterId = Session.Character.CharacterId,
                SourceWorldId = ServerManager.Instance.WorldId,
                Message = PacketFactory.Serialize(Session.Character.GenerateTalk(message)),
                Type = MessageType.PrivateChat
            });
            if (!sentChannelId.HasValue) //character is even offline on different world
            {
                Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("FRIEND_OFFLINE")));
            }
            else
            {
                LogHelper.Instance.InsertChatLog(ChatType.Friend, Session.Character.CharacterId, message, Session.IpAddress);
            }
        }

        /// <summary>
        /// fdel packet
        /// </summary>
        /// <param name="fDelPacket"></param>
        public void FriendDelete(FDelPacket fDelPacket)
        {
            Session.Character.DeleteRelation(fDelPacket.CharacterId);
            Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("FRIEND_DELETED")));
        }

        /// <summary>
        /// fins packet
        /// </summary>
        /// <param name="fInsPacket"></param>
        public void FriendAdd(FInsPacket fInsPacket)
        {
            if (!Session.Character.IsFriendlistFull())
            {
                var characterId = fInsPacket.CharacterId;
                if (!Session.Character.IsFriendOfCharacter(characterId))
                {
                    if (!Session.Character.IsBlockedByCharacter(characterId))
                    {
                        if (!Session.Character.IsBlockingCharacter(characterId))
                        {
                            var otherSession = ServerManager.Instance.GetSessionByCharacterId(characterId);
                            if (otherSession == null)
                            {
                                return;
                            }

                            if (otherSession.Character.FriendRequestCharacters.Contains(Session.Character.CharacterId))
                            {
                                switch (fInsPacket.Type)
                                {
                                    case 1:
                                        Session.Character.AddRelation(characterId, CharacterRelationType.Friend);
                                        Session.SendPacket($"info {Language.Instance.GetMessageFromKey("FRIEND_ADDED")}");
                                        otherSession.SendPacket($"info {Language.Instance.GetMessageFromKey("FRIEND_ADDED")}");
                                        break;

                                    case 2:
                                        otherSession.SendPacket(Language.Instance.GetMessageFromKey("FRIEND_REJECTED"));
                                        break;

                                    default:
                                        if (Session.Character.IsFriendlistFull())
                                        {
                                            Session.SendPacket($"info {Language.Instance.GetMessageFromKey("FRIEND_FULL")}");
                                            otherSession.SendPacket($"info {Language.Instance.GetMessageFromKey("FRIEND_FULL")}");
                                        }

                                        break;
                                }
                            }
                            else
                            {
                                otherSession.SendPacket(UserInterfaceHelper.Instance.GenerateDialog($"#fins^1^{Session.Character.CharacterId} #fins^2^{Session.Character.CharacterId} {string.Format(Language.Instance.GetMessageFromKey("FRIEND_ADD"), Session.Character.Name)}"));
                                Session.Character.FriendRequestCharacters.Add(characterId);
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
        /// bldel packet
        /// </summary>
        /// <param name="blDelPacket"></param>
        public void BlacklistDelete(BlDelPacket blDelPacket)
        {
            Session.Character.DeleteBlackList(blDelPacket.CharacterId);
            Session.SendPacket(Session.Character.GenerateBlinit());
            Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("BLACKLIST_DELETED")));
        }

        /// <summary>
        /// blins packet
        /// </summary>
        /// <param name="blInsPacket"></param>
        public void BlacklistAdd(BlInsPacket blInsPacket)
        {
            Session.Character.AddRelation(blInsPacket.CharacterId, CharacterRelationType.Blocked);
            Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("BLACKLIST_ADDED")));
            Session.SendPacket(Session.Character.GenerateBlinit());
        }

        /// <summary>
        /// hero packet
        /// </summary>
        /// <param name="heroPacket"></param>
        public void Hero(HeroPacket heroPacket)
        {
            if (string.IsNullOrEmpty(heroPacket.Message))
            {
                return;
            }

            if (Session.Character.IsReputHero() >= 3)
            {
                heroPacket.Message = heroPacket.Message.Trim();
                ServerManager.Instance.Broadcast(Session, $"msg 5 [{Session.Character.Name}]:{heroPacket.Message}", ReceiverType.AllNoHeroBlocked);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_HERO"), 11));
            }
        }

        /// <summary>
        /// RstartPacket packet
        /// </summary>
        /// <param name="rStartPacket"></param>
        public void GetRStart(RStartPacket rStartPacket)
        {
            if (rStartPacket.Type != 1)
            {
                return;
            }

            Session.CurrentMapInstance.InstanceBag.Lock = true;
            Preq(new PreqPacket());
        }

        /// <summary>
        /// PreqPacket packet
        /// </summary>
        /// <param name="packet"></param>
        public void Preq(PreqPacket packet)
        {
            var currentRunningSeconds = (DateTime.Now - Process.GetCurrentProcess().StartTime.AddSeconds(-50)).TotalSeconds;
            var timeSpanSinceLastPortal = currentRunningSeconds - Session.Character.LastPortal;
            if (!(timeSpanSinceLastPortal >= 4) || !Session.HasCurrentMapInstance)
            {
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MOVE"), 10));
                return;
            }

            Parallel.ForEach(Session.CurrentMapInstance.Portals.Concat(Session.Character.GetExtraPortal()), portal =>
            {
                if (Session.Character.PositionY < portal.SourceY - 1 || Session.Character.PositionY > portal.SourceY + 1 || Session.Character.PositionX < portal.SourceX - 1 ||
                    Session.Character.PositionX > portal.SourceX + 1)
                {
                    return;
                }

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
                            Session.SendPacket(Session.Character.Group.IsLeader(Session)
                                ? $"qna #mkraid^0^275 {Language.Instance.GetMessageFromKey("DO_YOU_WANT_RAID")}"
                                : Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("ONLY_TEAM_LEADER_CAN_START"), 10));
                        }
                        else
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NEED_TEAM"), 1));
                        }

                        return;

                    case (sbyte)PortalType.BlueRaid:
                    case (sbyte)PortalType.DarkRaid:
                        if ((byte)Session.Character.Faction == (portal.Type - 9) && Session.Character.Family?.Act4Raid != null)
                        {
                            Session.Character.LastPortal = currentRunningSeconds;
                            ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, Session.Character.Family.Act4Raid.MapInstanceId, portal.DestinationX, portal.DestinationY);
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

                switch (Session.CurrentMapInstance.MapInstanceType)
                {
                    case MapInstanceType.TimeSpaceInstance when !Session.CurrentMapInstance.InstanceBag.Lock:
                        if (Session.Character.CharacterId == Session.CurrentMapInstance.InstanceBag.Creator)
                        {
                            Session.SendPacket(UserInterfaceHelper.Instance.GenerateDialog($"#rstart^1 rstart {Language.Instance.GetMessageFromKey("ASK_ENTRY_IN_FIRST_ROOM")}"));
                        }

                        return;

                    case MapInstanceType.RaidInstance:
                        var leader = Session?.Character?.Group?.Characters?.ElementAt(0);
                        if (leader != null)
                        {
                            if (Session.Character.CharacterId != leader.Character.CharacterId)
                            {
                                if (leader.CurrentMapInstance.MapInstanceId != portal.DestinationMapInstanceId)
                                {
                                    ServerManager.Instance.ChangeMapInstance(leader.Character.CharacterId, portal.DestinationMapInstanceId, portal.DestinationX, portal.DestinationY);
                                }
                            }
                        }

                        break;

                    case MapInstanceType.ArenaInstance:
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, Session.Character.MapId, Session.Character.MapX, Session.Character.MapY);
                        return;
                }

                switch (Session.Character.Faction)
                {
                    case FactionType.Angel:
                        if (portal.DestinationMapId == 131)
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PORTAL_BLOCKED"), 10));
                            return;
                        }
                        else if (portal.DestinationMapId == 152)
                        {
                            ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, portal.DestinationMapInstanceId, 46, 171);
                        }

                        break;

                    case FactionType.Demon:
                        if (portal.DestinationMapId == 130)
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PORTAL_BLOCKED"), 10));
                            return;
                        }
                        else if (portal.DestinationMapId == 152)
                        {
                            ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, portal.DestinationMapInstanceId, 135, 171);
                        }

                        break;
                }

                portal.OnTraversalEvents.ForEach(e =>
                {
                    EventHelper.Instance.RunEvent(e);
                });
                if (portal.DestinationMapInstanceId == default(Guid))
                {
                    return;
                }

                Session.SendPacket(Session.CurrentMapInstance.GenerateRsfn());

                Session.Character.LastPortal = currentRunningSeconds;

                if (ServerManager.Instance.GetMapInstance(portal.SourceMapInstanceId).MapInstanceType != MapInstanceType.BaseMapInstance &&
                    ServerManager.Instance.GetMapInstance(portal.DestinationMapInstanceId).MapInstanceType == MapInstanceType.BaseMapInstance)
                {
                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId, Session.Character.MapId, Session.Character.MapX, Session.Character.MapY);
                }
                else if (portal.DestinationMapInstanceId == Session.Character.Miniland.MapInstanceId)
                {
                    ServerManager.Instance.JoinMiniland(Session, Session);
                }
                else
                {
                    ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, portal.DestinationMapInstanceId, portal.DestinationX, portal.DestinationY);
                }
            });
        }

        /// <summary>
        /// pulse packet
        /// </summary>
        /// <param name="pulsepacket"></param>
        public void Pulse(PulsePacket pulsepacket)
        {
            Session.Character.LastPulse += 60;
            if (pulsepacket.Tick != Session.Character.LastPulse)
            {
                Session.Disconnect();
            }

            Session.Character.DeleteTimeout();

            try
            {
                CommunicationServiceClient.Instance.PulseAccount(Session.Account.AccountId);
            }
            catch
            {
                Session.Disconnect();
            }
        }

        /// <summary>
        /// req_info packet
        /// </summary>
        /// <param name="reqInfoPacket"></param>
        public void ReqInfo(ReqInfoPacket reqInfoPacket)
        {
            switch (reqInfoPacket.Type)
            {
                case 6:
                    if (reqInfoPacket.MateVNum.HasValue)
                    {
                        var mate = Session.CurrentMapInstance.Sessions.FirstOrDefault(s => s.Character?.Mates != null && s.Character.Mates.Any(o => o.MateTransportId == reqInfoPacket.MateVNum.Value))?.Character.Mates.Find(o => o.MateTransportId == reqInfoPacket.MateVNum.Value);
                        Session.SendPacket(mate?.GenerateEInfo());
                    }

                    break;

                case 5:
                    var npc = ServerManager.Instance.GetNpc((short)reqInfoPacket.TargetVNum);
                    if (npc != null)
                    {
                        Session.SendPacket(npc.GenerateEInfo());
                    }

                    break;

                default:
                    Session.SendPacket(ServerManager.Instance.GetSessionByCharacterId(reqInfoPacket.TargetVNum)?.Character?.GenerateReqInfo());
                    break;
            }
        }

        /// <summary>
        /// rest packet
        /// </summary>
        /// <param name="sitpacket"></param>
        public void Rest(SitPacket sitpacket)
        {
            sitpacket?.Users.ForEach(u =>
            {
                if (u.UserType == 1)
                {
                    Session.Character.Rest();
                }
                else
                {
                    Session.CurrentMapInstance.Broadcast(Session.Character.Mates.FirstOrDefault(s => s.MateTransportId == (int)u.UserId)?.GenerateRest());
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
                            const int SAVER = 1211;
                            if (Session.Character.Inventory.CountItem(SAVER) < 1)
                            {
                                Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_SAVER"), 0));
                                ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
                            }
                            else
                            {
                                Session.Character.Inventory.RemoveItemAmount(SAVER);
                                Session.Character.Hp = (int)Session.Character.HPLoad();
                                Session.Character.Mp = (int)Session.Character.MPLoad();
                                Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateRevive());
                                Session.SendPacket(Session.Character.GenerateStat());
                            }

                            break;

                        default:
                            const int SEED = 1012;
                            if (Session.Character.Inventory.CountItem(SEED) < 10 && Session.Character.Level > 20)
                            {
                                Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_POWER_SEED"), 0));
                                ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_SEED_SAY"), 0));
                            }
                            else
                            {
                                if (Session.Character.Level > 20)
                                {
                                    Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("SEED_USED"), 10), 10));
                                    Session.Character.Inventory.RemoveItemAmount(SEED, 10);
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
            if (string.IsNullOrEmpty(sayPacket.Message))
            {
                return;
            }

            var penalty = Session.Account.PenaltyLogs.OrderByDescending(s => s.DateEnd).FirstOrDefault();
            var message = sayPacket.Message;
            if (Session.Character.IsMuted() && penalty != null)
            {
                if (Session.Character.Gender == GenderType.Female)
                {
                    ConcurrentBag<ArenaTeamMember> member = ServerManager.Instance.ArenaTeams.FirstOrDefault(s => s.Any(e => e.Session == Session));
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance && member != null)
                    {
                        var member2 = member.FirstOrDefault(o => o.Session == Session);
                        member.Replace(s => member2 != null && s.ArenaTeamType == member2.ArenaTeamType && s != member2).Replace(s => s.ArenaTeamType == member.FirstOrDefault(o => o.Session == Session)?.ArenaTeamType).ToList().ForEach(o => o.Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_FEMALE"), 1)));
                    }
                    else
                    {
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_FEMALE"), 1));
                    }

                    Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"), (penalty.DateEnd - DateTime.Now).ToString(@"hh\:mm\:ss")), 11));
                    Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"), (penalty.DateEnd - DateTime.Now).ToString(@"hh\:mm\:ss")), 12));
                }
                else
                {
                    ConcurrentBag<ArenaTeamMember> member = ServerManager.Instance.ArenaTeams.FirstOrDefault(s => s.Any(e => e.Session == Session));
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance && member != null)
                    {
                        var member2 = member.FirstOrDefault(o => o.Session == Session);
                        member.Replace(s => member2 != null && s.ArenaTeamType == member2.ArenaTeamType && s != member2).Replace(s => s.ArenaTeamType == member.FirstOrDefault(o => o.Session == Session)?.ArenaTeamType).ToList().ForEach(o => o.Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_MALE"), 1)));
                    }
                    else
                    {
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_MALE"), 1));
                    }

                    Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"), (penalty.DateEnd - DateTime.Now).ToString(@"hh\:mm\:ss")), 11));
                    Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"), (penalty.DateEnd - DateTime.Now).ToString(@"hh\:mm\:ss")), 12));
                }
            }
            else
            {
                LogHelper.Instance.InsertChatLog(ChatType.General, Session.Character.CharacterId, message, Session.IpAddress);
                byte type = 0;
                ConcurrentBag<ArenaTeamMember> member = ServerManager.Instance.ArenaTeams.FirstOrDefault(s => s.Any(e => e.Session == Session));
                if (Session.Character.Authority == AuthorityType.Moderator)
                {
                    type = 12;
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance && member != null)
                    {
                        var member2 = member.FirstOrDefault(o => o.Session == Session);
                        member.Replace(s => member2 != null && s.ArenaTeamType == member2.ArenaTeamType && s != member2).Replace(s => s.ArenaTeamType == member.FirstOrDefault(o => o.Session == Session)?.ArenaTeamType).ToList().ForEach(o => o.Session.SendPacket(Session.Character.GenerateSay(message.Trim(), 1)));
                    }
                    else
                    {
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateSay(message.Trim(), 1), ReceiverType.AllExceptMe);
                    }

                    message = $"[{Language.Instance.GetMessageFromKey("SUPPORT")} {Session.Character.Name}]: " + message;
                }

                if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance && member != null)
                {
                    var member2 = member.FirstOrDefault(o => o.Session == Session);
                    member.Replace(s => s.ArenaTeamType == member2?.ArenaTeamType && s != member2).ToList().ForEach(o => o.Session.SendPacket(Session.Character.GenerateSay(message.Trim(), type)));
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
        /// <param name="pstpacket"></param>
        public void SendMail(PstPacket pstpacket)
        {
            if (pstpacket.Data != null)
            {
                var receiver = DAOFactory.CharacterDAO.FirstOrDefault(s => s.Name == pstpacket.Receiver);
                if (receiver != null)
                {
                    string[] datasplit = pstpacket.Data.Split(' ');
                    var headWearable = Session.Character.Inventory.LoadBySlotAndType<GameObject.WearableInstance>((byte)EquipmentType.Hat, InventoryType.Wear);
                    var color = headWearable != null && headWearable.Item.IsColored ? (byte)headWearable.Design : (byte)Session.Character.HairColor;
                    var mailcopy = new MailDTO
                    {
                        AttachmentAmount = 0,
                        IsOpened = false,
                        Date = DateTime.Now,
                        Title = datasplit[0],
                        Message = datasplit[1],
                        ReceiverId = receiver.CharacterId,
                        SenderId = Session.Character.CharacterId,
                        IsSenderCopy = true,
                        SenderClass = Session.Character.Class,
                        SenderGender = Session.Character.Gender,
                        SenderHairColor = Enum.IsDefined(typeof(HairColorType), color) ? (HairColorType)color : 0,
                        SenderHairStyle = Session.Character.HairStyle,
                        EqPacket = Session.Character.GenerateEqListForPacket(),
                        SenderMorphId = Session.Character.Morph == 0 ? (short)-1 : (short)(Session.Character.Morph > short.MaxValue ? 0 : Session.Character.Morph)
                    };
                    var mail = new MailDTO
                    {
                        AttachmentAmount = 0,
                        IsOpened = false,
                        Date = DateTime.Now,
                        Title = datasplit[0],
                        Message = datasplit[1],
                        ReceiverId = receiver.CharacterId,
                        SenderId = Session.Character.CharacterId,
                        IsSenderCopy = false,
                        SenderClass = Session.Character.Class,
                        SenderGender = Session.Character.Gender,
                        SenderHairColor = Enum.IsDefined(typeof(HairColorType), color) ? (HairColorType)color : 0,
                        SenderHairStyle = Session.Character.HairStyle,
                        EqPacket = Session.Character.GenerateEqListForPacket(),
                        SenderMorphId = Session.Character.Morph == 0 ? (short)-1 : (short)(Session.Character.Morph > short.MaxValue ? 0 : Session.Character.Morph)
                    };

                    DAOFactory.MailDAO.InsertOrUpdate(ref mail);
                    CommunicationServiceClient.Instance.SendMail(ServerManager.Instance.ServerGroup, mail);

                    Session.Character.MailList.Add((Session.Character.MailList.Any() ? Session.Character.MailList.OrderBy(s => s.Key).Last().Key : 0) + 1, mailcopy);
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MAILED"), 11));
                    Session.SendPacket(Session.Character.GeneratePost(mailcopy, 2));
                }
                else
                {
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_FOUND"), 10));
                }
            }

            if (pstpacket.Unknow1.HasValue)
            {
                return;
            }
            {
                if (!int.TryParse(pstpacket.Id.ToString(), out int id) || !byte.TryParse(pstpacket.Type.ToString(), out byte type))
                {
                    return;
                }

                switch (pstpacket.Argument)
                {
                    case 3:
                        if (Session.Character.MailList.ContainsKey(id))
                        {
                            if (!Session.Character.MailList[id].IsOpened)
                            {
                                Session.Character.MailList[id].IsOpened = true;
                            }

                            Session.SendPacket(Session.Character.GeneratePostMessage(Session.Character.MailList[id], type));
                        }

                        break;

                    case 2:
                        if (Session.Character.MailList.ContainsKey(id))
                        {
                            var mail = Session.Character.MailList[id];
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MAIL_DELETED"), 11));
                            Session.SendPacket($"post 2 {type} {id}");

                            if (Session.Character.MailList.ContainsKey(id))
                            {
                                Session.Character.MailList.Remove(id);
                            }
                        }

                        break;
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
                    var qlFrom = Session.Character.QuicklistEntries.SingleOrDefault(n => n.Q1 == data1 && n.Q2 == data2 && (Session.Character.UseSp ? n.Morph == Session.Character.Morph : n.Morph == 0));

                    if (qlFrom != null)
                    {
                        var qlTo = Session.Character.QuicklistEntries.SingleOrDefault(n => n.Q1 == q1 && n.Q2 == q2 && (Session.Character.UseSp ? n.Morph == Session.Character.Morph : n.Morph == 0));

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
                return;
            }

            Session.CurrentMapInstance = Session.Character.MapInstance;

            if (ConfigurationManager.AppSettings["SceneOnCreate"].ToLower() == "true" & Session.Character.GeneralLogs.Count(s => s.LogType == "Connection") < 2)
            {
                Session.SendPacket("scene 40");
            }

            if (ConfigurationManager.AppSettings["WorldInformation"].ToLower() == "true")
            {
                Session.SendPacket(Session.Character.GenerateSay("----------[ Welcome ]----------", 10));
                Session.SendPacket(Session.Character.GenerateSay($"XPRate: {ServerManager.Instance.XPRate}", 11));
                Session.SendPacket(Session.Character.GenerateSay($"DropRate: {ServerManager.Instance.DropRate}", 11));
                Session.SendPacket(Session.Character.GenerateSay($"GoldRate: {ServerManager.Instance.GoldRate}", 11));
                Session.SendPacket(Session.Character.GenerateSay($"FairyXp: {ServerManager.Instance.FairyXpRate}", 11));
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

            Session.SendPacket($"fd {Session.Character.Reput} 0 {(int)Session.Character.Dignity} {Math.Abs(Session.Character.GetDignityIco())}");
            Session.SendPacket(Session.Character.GenerateFd());

            Session.SendPacket("rage 0 250000");
            Session.SendPacket("rank_cool 0 0 18000");

            if (Session.Character.StaticBonusList.FirstOrDefault(s => s.StaticBonusType == StaticBonusType.BazaarMedalGold || s.StaticBonusType == StaticBonusType.BazaarMedalSilver) != null)
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

            var specialistInstance = Session.Character.Inventory.LoadBySlotAndType<GameObject.SpecialistInstance>(8, InventoryType.Wear);

            if (specialistInstance != null)
            {
                Session.SendPacket(Session.Character.GenerateSpPoint());
            }

            Session.SendPacket("scr 0 0 0 0 0 0");

            for (int i = 0; i < 4; i++)
            {
                Session.SendPacket($"bn {i} {Language.Instance.GetMessageFromKey($"BN{i}")}");
            }

            Session.SendPacket(Session.Character.GenerateExts());
            Session.SendPacket(Session.Character.GenerateMlinfo());
            Session.SendPacket(UserInterfaceHelper.Instance.GeneratePClear());

            Session.SendPackets(Session.Character.GeneratePst());

            Session.SendPacket("zzim");
            Session.SendPacket($"cash 0 0 {Session.Account.NosDollar}");

            Session.SendPacket("act6");
            Session.SendPacket(Session.Character.GenerateFaction());

            Session.SendPackets(Session.Character.GenerateScP());
            Session.SendPackets(Session.Character.GenerateScN());

            Session.SendPacket(Session.Character.GeneratePinit());

#pragma warning disable 618
            Session.Character.GenerateStartupInventory();
#pragma warning restore 618

            Session.SendPacket(Session.Character.GenerateGold());
            Session.SendPackets(Session.Character.GenerateQuicklist());

            var clinit = ServerManager.Instance.TopComplimented.Aggregate("clinit", (current, character) => current + $" {character.CharacterId}|{character.Level}|{character.HeroLevel}|{character.Compliment}|{character.Name}");
            var flinit = ServerManager.Instance.TopReputation.Aggregate("flinit", (current, character) => current + $" {character.CharacterId}|{character.Level}|{character.HeroLevel}|{character.Reput}|{character.Name}");
            var kdlinit = ServerManager.Instance.TopPoints.Aggregate("kdlinit", (current, character) => current + $" {character.CharacterId}|{character.Level}|{character.HeroLevel}|{character.Act4Points}|{character.Name}");

            Session.SendPacket(Session.Character.GenerateFinit());
            Session.SendPacket(Session.Character.GenerateBlinit());
            Session.SendPacket(clinit);
            Session.SendPacket(flinit);
            Session.SendPacket(kdlinit);

            Session.Character.LastPVPRevive = DateTime.Now;

            long? familyId = DAOFactory.FamilyCharacterDAO.FirstOrDefault(s => s.CharacterId == Session.Character.CharacterId)?.FamilyId;

            if (familyId != null)
            {
                Session.Character.Family = ServerManager.Instance.FamilyList.FirstOrDefault(s => s.FamilyId == familyId.Value);
            }

            if (Session.Character.Family != null && Session.Character.FamilyCharacter != null)
            {
                Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateGidx());
                Session.SendPacket(Session.Character.GenerateGInfo());
                Session.SendPackets(Session.Character.GetFamilyHistory());
                Session.SendPacket(Session.Character.GenerateFamilyMember());
                Session.SendPacket(Session.Character.GenerateFamilyMemberMessage());
                Session.SendPacket(Session.Character.GenerateFamilyMemberExp());
                if (!string.IsNullOrWhiteSpace(Session.Character.Family.FamilyMessage))
                {
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo("--- Family Message ---\n" + Session.Character.Family.FamilyMessage));
                }
            }

            IEnumerable<MailDTO> mails = DAOFactory.MailDAO.Where(s => s.ReceiverId.Equals(Session.Character.CharacterId)).ToList();

            foreach (MailDTO mail in mails)
            {
                Session.Character.GenerateMail(mail);
            }

            var giftcount = mails.Count(mail => !mail.IsSenderCopy && mail.ReceiverId == Session.Character.CharacterId && mail.AttachmentVNum != null && !mail.IsOpened);

            if (giftcount > 0)
            {
                Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("GIFTED"), giftcount), 11));
            }

            var mailcount = mails.Count(mail => !mail.IsSenderCopy && mail.ReceiverId == Session.Character.CharacterId && mail.AttachmentVNum == null && !mail.IsOpened);

            if (mailcount > 0)
            {
                Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("NEW_MAIL"), mailcount), 10));
            }

            Session.Character.DeleteTimeout();

            foreach (StaticBuffDTO sb in DAOFactory.StaticBuffDAO.Where(s => s.CharacterId == Session.Character.CharacterId))
            {
                Session.Character.AddStaticBuff(sb);
            }

            if (Session.Character.MapInstance.Map.MapTypes.Any(m => m.MapTypeId == (short)MapTypeEnum.Act4 || m.MapTypeId == (short)MapTypeEnum.Act42))
            {
                Session.Character.ConnectAct4();
            }

            Session.SendPacket(Session.Character.GenerateStat());
        }

        /// <summary>
        /// walk packet
        /// </summary>
        /// <param name="walkPacket"></param>
        public void Walk(WalkPacket walkPacket)
        {
            if (Session.Character == null || !Session.HasCurrentMapInstance || Session.CurrentMapInstance.Map.IsBlockedZone(walkPacket.XCoordinate, walkPacket.YCoordinate) || Session.Character.IsChangingMapInstance || Session.Character.HasShopOpened)
            {
                return;
            }

            var timeSpanSinceLastPortal = (DateTime.Now - Process.GetCurrentProcess().StartTime.AddSeconds(-50)).TotalSeconds - Session.Character.LastPortal;
            var distance = GameObject.Map.GetDistance(new MapCell { X = Session.Character.PositionX, Y = Session.Character.PositionY }, new MapCell { X = walkPacket.XCoordinate, Y = walkPacket.YCoordinate });

            if ((Session.Character.Speed >= walkPacket.Speed || Session.Character.LastSpeedChange.AddSeconds(1) > DateTime.Now) && !(distance > 60 && timeSpanSinceLastPortal > 10))
            {
                if (Session.Character.MapInstance.MapInstanceType == MapInstanceType.BaseMapInstance || Session.Character.MapInstance.MapInstanceType == MapInstanceType.Act4Instance)
                {
                    Session.Character.MapX = walkPacket.XCoordinate;
                    Session.Character.MapY = walkPacket.YCoordinate;
                }

                Session.Character.PositionX = walkPacket.XCoordinate;
                Session.Character.PositionY = walkPacket.YCoordinate;

                Session.Character.BrushFire = BestFirstSearch.LoadBrushFire(new GridPos { X = Session.Character.PositionX, Y = Session.Character.PositionY }, Session.CurrentMapInstance.Map.Grid);

                Session.CurrentMapInstance.Broadcast(Session.Character.GenerateMv());
                Session.SendPacket(Session.Character.GenerateCond());

                if (Session.Character != null)
                {
                    Session.Character.LastMove = DateTime.Now;

                    if (Session.CurrentMapInstance.OnAreaEntryEvents != null)
                    {
                        Session.CurrentMapInstance.OnAreaEntryEvents.Where(s => s != null && s.InZone(Session.Character.PositionX, Session.Character.PositionY)).ToList().ForEach(e => e?.Events?.ToList().ForEach(evt => EventHelper.Instance?.RunEvent(evt)));
                        Session.CurrentMapInstance.OnAreaEntryEvents.RemoveAll(s => s != null && s.InZone(Session.Character.PositionX, Session.Character.PositionY));
                    }
                }

                if (Session.CurrentMapInstance.OnMoveOnMapEvents != null)
                {
                    Session.CurrentMapInstance.OnMoveOnMapEvents.ForEach(e => EventHelper.Instance?.RunEvent(e));
                    Session.CurrentMapInstance.OnMoveOnMapEvents.RemoveAll(s => s != null);
                }
            }
            else
            {
                Session.Disconnect();
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

                var characterName = whisperPacket.Message.Split(' ')[whisperPacket.Message.StartsWith("GM ") ? 1 : 0];
                var message = string.Empty;
                string[] packetsplit = whisperPacket.Message.Split(' ');
                for (int i = packetsplit[0] == "GM" ? 2 : 1; i < packetsplit.Length; i++)
                    message += packetsplit[i] + " ";

                if (message.Length > 60)
                {
                    message = message.Substring(0, 60);
                }

                message = message.Trim();

                Session.SendPacket(Session.Character.GenerateSpk(message, 5));
                var receiver = DAOFactory.CharacterDAO.FirstOrDefault(s => s.Name == characterName);
                if (receiver.CharacterId == Session.Character.CharacterId)
                {
                    return;
                }

                if (Session.Character.IsBlockedByCharacter(receiver.CharacterId))
                {
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("BLACKLIST_BLOCKED")));
                    return;
                }

                int? sentChannelId = CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                {
                    DestinationCharacterId = receiver.CharacterId,
                    SourceCharacterId = Session.Character.CharacterId,
                    SourceWorldId = ServerManager.Instance.WorldId,
                    Message = Session.Character.GenerateSpk(message, Session.Account.Authority == AuthorityType.GameMaster ? 15 : 5),
                    Type = packetsplit[0] == "GM" ? MessageType.WhisperGM : MessageType.Whisper
                });
                if (sentChannelId == null)
                {
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("USER_NOT_CONNECTED")));
                }
                else
                {
                    LogHelper.Instance.InsertChatLog(ChatType.Whisper, Session.Character.CharacterId, message, Session.IpAddress);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error("Whisper failed.", e);
            }
        }

        #endregion
    }
}