using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using System;
using System.Linq;

namespace OpenNos.Handler
{
    internal class GroupPacketHandler : IPacketHandler
    {
        #region Instantiation

        public GroupPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        private ClientSession Session { get; }

        #endregion

        #region Methods

        public static void RaidGroupLeave(ClientSession session, bool isKicked = false, bool isDisolve = false)
        {
            Group grp = session.Character.Group;

            if (grp == null /* Not In Group */)
            {
                return;
            }

            session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey(isDisolve ? "RAID_DISOLVED" : isKicked ? "KICK_RAID" : "LEFT_RAID"), 0));

            session.SendPacket(session.Character.GenerateRaid(1, true));
            session.SendPacket(session.Character.GenerateRaid(2, true));
            grp.LeaveGroup(session);

            if (grp.Characters.Count < 1)
            {
                ServerManager.Instance.GroupList.RemoveAll(s => s.GroupId == grp.GroupId);
                ServerManager.Instance.GroupsThreadSafe.TryRemove(ServerManager.Instance.GroupsThreadSafe.First(s => s.Value.GroupId == grp.GroupId).Key, out _);
            }
            else
            {
                foreach (var s in grp.Characters)
                {
                    s.SendPacket(grp.GenerateRdlst());
                    s.SendPacket(grp.GeneraterRaidmbf());
                    s.SendPacket(s.Character.GenerateRaid(0, false));

                    if (grp.IsLeader(s))
                    {
                        s.SendPacket(s.Character.GenerateRaid(2, false));
                    }
                }
            }

            
        }

        /// <summary>
        /// rdPacket packet
        /// </summary>
        /// <param name="rdPacket"></param>
        public void RaidManage(RdPacket rdPacket)
        {
            if (rdPacket == null /* Invalid Packet */ || Session.Character == null /* Not In-Game */)
            {
                return;
            }

            switch (rdPacket.Type)
            {
                case 1:
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.RaidInstance /* In-Raid Map */)
                    {
                        return;
                    }

                    var target = ServerManager.Instance.GetSessionByCharacterId(rdPacket.CharacterId);

                    if (target == null /* Target Not Found */ || target.Character == null /* Target Not In-Game */)
                    {
                        return;
                    }

                    // Invite

                    if (Session.Character.Group != null /* Has Group */ && Session.Character.Group.Raid != null /* Is Raid Group */ && Session.Character.Group.IsLeader(Session) /* Is Leader of Group */)
                    {
                        if (target.Character.Group != null /* Target Has Group */)
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("TARGET_HAS_GROUP"), 10));
                            return;
                        }

                        if (target.Character.Level < Session.Character.Group.Raid.LevelMinimum /* Target Level < */)
                        {
                            Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("LEVEL_NOT_HIGH_ENOUGH")));
                            return;
                        }

                        GroupJoin(new PJoinPacket { RequestType = GroupRequestType.Invited, CharacterId = target.Character.CharacterId });

                        return;
                    }

                    // Join

                    if (Session.Character.Group != null /* Has Group */ || target.Character.Group == null /* Inviter Has No Group */ || target.Character.Group.Raid == null /* Is Not Raid Group */)
                    {
                        return;
                    }

                    if (Session.Character.Level < target.Character.Group.Raid.LevelMinimum /* Level < */)
                    {
                        Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("LEVEL_NOT_HIGH_ENOUGH")));
                        return;
                    }

                    GroupJoin(new PJoinPacket { RequestType = GroupRequestType.Accepted, CharacterId = target.Character.CharacterId });

                    break;

                case 3:
                    // Kick

                    if (Session.CurrentMapInstance.MapInstanceType != MapInstanceType.RaidInstance /* Not In-Raid */ && Session.Character.Group != null /* Has Group */ && Session.Character.Group.Raid != null /* Is Raid Group */ && Session.Character.Group.IsLeader(Session) /* Is Leader of Group */)
                    {
                        ClientSession session = ServerManager.Instance.GetSessionByCharacterId(rdPacket.CharacterId);

                        if (session == null /* Target Not Found */ || session.Character == null /* Target Not In-Game */)
                        {
                            return;
                        }

                        RaidGroupLeave(session, true);
                    }
                    break;

                case 4:

                    // Dissolve

                    if (Session.CurrentMapInstance.MapInstanceType != MapInstanceType.RaidInstance /* Not In-Raid */ && Session.Character.Group != null /* Has Group */ && Session.Character.Group.Raid != null /* Is Raid Group */ && Session.Character.Group.IsLeader(Session) /* Is Leader of Group */)
                    {
                        var members = Session.Character.Group.Characters.Where(s => s != Session);

                        foreach (var s in members)
                        {
                            RaidGroupLeave(s, true, true);
                        }

                        RaidGroupLeave(Session, false, true);

                        return;
                    }

                    // Leave

                    RaidGroupLeave(Session);

                    break;
            }
        }

        /// <summary>
        /// rlPacket packet
        /// </summary>
        /// <param name="rlPacket"></param>
        public void RaidListRegister(RlPacket rlPacket)
        {
            switch (rlPacket.Type)
            {
                case 0:
                    byte type = 0;

                    if (Session.Character.Group?.IsLeader(Session) == true && Session.Character.Group.GroupType != GroupType.Group && ServerManager.Instance.GroupList.Any(s => s.GroupId == Session.Character.Group.GroupId))
                    {
                        type = 1;
                    }
                    else if (Session.Character.Group != null && Session.Character.Group.GroupType != GroupType.Group && Session.Character.Group.IsLeader(Session))
                    {
                        type = 2;
                    }
                    else if (Session.Character.Group != null)
                    {
                        type = 3;
                    }

                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateRl(type));
                    break;

                case 1:
                    if (Session.Character.Group != null && Session.Character.Group.IsLeader(Session) && Session.Character.Group.GroupType != GroupType.Group && ServerManager.Instance.GroupList.All(s => s.GroupId != Session.Character.Group.GroupId))
                    {
                        if (Session.Character.Group.Raid?.FirstMap?.InstanceBag.Lock == true)
                        {
                            return;
                        }

                        ServerManager.Instance.GroupList.Add(Session.Character.Group);
                        Session.SendPacket(UserInterfaceHelper.Instance.GenerateRl(1));
                        Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo("RAID_REGISTERED"));
                        ServerManager.Instance.Broadcast(Session, $"qnaml 100 #rl {string.Format(Language.Instance.GetMessageFromKey("SEARCH_TEAM_MEMBERS"), Session.Character.Name, Session.Character.Group.Raid?.Label)}", ReceiverType.AllExceptGroup);
                    }
                    break;

                case 2:
                    if (Session.Character.Group != null && Session.Character.Group.IsLeader(Session) && Session.Character.Group.GroupType != GroupType.Group && ServerManager.Instance.GroupList.Any(s => s.GroupId == Session.Character.Group.GroupId))
                    {
                        ServerManager.Instance.GroupList.Remove(Session.Character.Group);
                        Session.SendPacket(UserInterfaceHelper.Instance.GenerateRl(2));
                        Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo("RAID_UNREGISTERED"));
                    }
                    break;

                case 3:
                    var cl = ServerManager.Instance.GetSessionByCharacterName(rlPacket.CharacterName);

                    if (cl != null)
                    {
                        if (Session.Character.Level < cl.Character.Group.Raid.LevelMinimum)
                        {
                            Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("LEVEL_NOT_HIGH_ENOUGH")));
                            return;
                        }

                        cl.Character.GroupSentRequestCharacterIds.Add(Session.Character.CharacterId);
                        GroupJoin(new PJoinPacket { RequestType = GroupRequestType.Accepted, CharacterId = cl.Character.CharacterId });
                    }
                    break;
            }
        }

        /// <summary>
        /// pjoin packet
        /// </summary>
        /// <param name="pjoinPacket"></param>
        public void GroupJoin(PJoinPacket pjoinPacket)
        {
            var createNewGroup = true;
            var targetSession = ServerManager.Instance.GetSessionByCharacterId(pjoinPacket.CharacterId);

            if (targetSession == null && !pjoinPacket.RequestType.Equals(GroupRequestType.Sharing))
            {
                return;
            }

            switch (pjoinPacket.RequestType)
            {
                case GroupRequestType.Requested:
                case GroupRequestType.Invited:
                    if (pjoinPacket.CharacterId == 0)
                    {
                        return;
                    }

                    if (ServerManager.Instance.IsCharactersGroupFull(pjoinPacket.CharacterId))
                    {
                        Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("GROUP_FULL")));
                        return;
                    }

                    if (ServerManager.Instance.IsCharacterMemberOfGroup(pjoinPacket.CharacterId) && ServerManager.Instance.IsCharacterMemberOfGroup(Session.Character.CharacterId))
                    {
                        Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("ALREADY_IN_GROUP")));
                        return;
                    }

                    if (Session.Character.CharacterId == pjoinPacket.CharacterId || targetSession == null)
                    {
                        return;
                    }

                    if (Session.Character.IsBlockedByCharacter(pjoinPacket.CharacterId))
                    {
                        Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("BLACKLIST_BLOCKED")));
                        return;
                    }

                    if (targetSession.Character.GroupRequestBlocked)
                    {
                        Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("GROUP_BLOCKED"), 0));
                    }
                    else
                    {
                        Session.Character.GroupSentRequestCharacterIds.Add(targetSession.Character.CharacterId);

                        if (Session.Character.Group == null || Session.Character.Group.GroupType == GroupType.Group)
                        {
                            if (targetSession.Character?.Group == null || targetSession.Character?.Group.GroupType == GroupType.Group)
                            {
                                Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(string.Format(Language.Instance.GetMessageFromKey("GROUP_REQUEST"), targetSession.Character.Name)));
                                targetSession.SendPacket(UserInterfaceHelper.Instance.GenerateDialog($"#pjoin^3^{ Session.Character.CharacterId} #pjoin^4^{Session.Character.CharacterId} {string.Format(Language.Instance.GetMessageFromKey("INVITED_YOU"), Session.Character.Name)}"));
                            }
                            else
                            {
                                Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(string.Format(Language.Instance.GetMessageFromKey("GROUP_CANT_INVITE"), targetSession.Character.Name)));
                            }
                        }
                        else
                        {
                            targetSession.SendPacket($"qna #rd^1^{Session.Character.CharacterId}^1 {string.Format(Language.Instance.GetMessageFromKey("INVITED_YOU_RAID"), Session.Character.Name)}");
                        }
                    }
                    break;

                case GroupRequestType.Sharing:
                    if (Session.Character.Group == null)
                    {
                        return;
                    }

                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("GROUP_SHARE_INFO")));
                    Session.Character.Group.Characters.Replace(s => s.Character.CharacterId != Session.Character.CharacterId).ToList().ForEach(s =>
                    {
                        s.SendPacket(UserInterfaceHelper.Instance.GenerateDialog($"#pjoin^6^{ Session.Character.CharacterId} #pjoin^7^{Session.Character.CharacterId} {string.Format(Language.Instance.GetMessageFromKey("INVITED_YOU_SHARE"), Session.Character.Name)}"));
                        Session.Character.GroupSentRequestCharacterIds.Add(s.Character.CharacterId);
                    });
                    break;

                case GroupRequestType.Accepted:
                    if (targetSession != null && !targetSession.Character.GroupSentRequestCharacterIds.Contains(Session.Character.CharacterId))
                    {
                        return;
                    }

                    if (targetSession != null)
                    {
                        targetSession.Character.GroupSentRequestCharacterIds.Remove(Session.Character.CharacterId);

                        if (ServerManager.Instance.IsCharacterMemberOfGroup(Session.Character.CharacterId) && ServerManager.Instance.IsCharacterMemberOfGroup(pjoinPacket.CharacterId))
                        {
                            // everyone is in group, return
                            return;
                        }

                        if (ServerManager.Instance.IsCharactersGroupFull(pjoinPacket.CharacterId) || ServerManager.Instance.IsCharactersGroupFull(Session.Character.CharacterId))
                        {
                            Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("GROUP_FULL")));
                            targetSession.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("GROUP_FULL")));
                            return;
                        }

                        // get group and add to group
                        if (ServerManager.Instance.IsCharacterMemberOfGroup(Session.Character.CharacterId))
                        {
                            // target joins source
                            var currentGroup = ServerManager.Instance.GetGroupByCharacterId(Session.Character.CharacterId);

                            if (currentGroup != null)
                            {
                                currentGroup.JoinGroup(targetSession);
                                targetSession.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("JOINED_GROUP")));
                                createNewGroup = false;
                            }
                        }
                        else if (ServerManager.Instance.IsCharacterMemberOfGroup(pjoinPacket.CharacterId))
                        {
                            // source joins target
                            var currentGroup = ServerManager.Instance.GetGroupByCharacterId(pjoinPacket.CharacterId);

                            if (currentGroup != null)
                            {
                                createNewGroup = false;
                                if (currentGroup.GroupType == GroupType.Group)
                                {
                                    currentGroup.JoinGroup(Session);
                                }
                                else
                                {
                                    Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("RAID_JOIN"), Session.Character.Name), 10));
                                    if (Session.Character.Level > currentGroup.Raid?.LevelMaximum || Session.Character.Level < currentGroup.Raid?.LevelMinimum)
                                    {
                                        Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("RAID_LEVEL_INCORRECT"), 10));
                                        if (Session.Character.Level >= currentGroup.Raid?.LevelMaximum + 10 /* && AlreadySuccededToday*/)
                                        {
                                            // modal 1 ALREADY_SUCCEDED_AS_ASSISTANT
                                        }
                                    }

                                    currentGroup.JoinGroup(Session);
                                    Session.SendPacket(Session.Character.GenerateRaid(1, false));
                                    currentGroup.Characters.ToList().ForEach(s =>
                                    {
                                        s.SendPacket(currentGroup.GenerateRdlst());
                                        s.SendPacket(s.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("JOIN_TEAM"), Session.Character.Name), 10));
                                        s.SendPacket(s.Character.GenerateRaid(0, false));
                                        if (!currentGroup.IsLeader(s))
                                        {
                                            s.SendPacket(s.Character.GenerateRaid(2, false));
                                        }
                                    });
                                }
                            }
                        }

                        if (createNewGroup)
                        {
                            var group = new Group(GroupType.Group);
                            group.JoinGroup(pjoinPacket.CharacterId);
                            Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(string.Format(Language.Instance.GetMessageFromKey("GROUP_JOIN"), targetSession.Character.Name)));
                            group.JoinGroup(Session.Character.CharacterId);
                            ServerManager.Instance.AddGroup(group);
                            targetSession.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("GROUP_ADMIN")));

                            // set back reference to group
                            Session.Character.Group = group;
                            targetSession.Character.Group = group;
                        }
                    }

                    if (Session?.Character?.Group.GroupType != GroupType.Group)
                    {
                        return;
                    }

                    ServerManager.Instance.UpdateGroup(pjoinPacket.CharacterId);
                    Session.CurrentMapInstance?.Broadcast(Session.Character.GeneratePidx());
                    break;

                default:
                    switch (pjoinPacket.RequestType)
                    {
                        case GroupRequestType.Declined:
                            if (targetSession != null && !targetSession.Character.GroupSentRequestCharacterIds.Contains(Session.Character.CharacterId))
                            {
                                return;
                            }

                            if (targetSession != null)
                            {
                                targetSession.Character.GroupSentRequestCharacterIds.Remove(Session.Character.CharacterId);
                                targetSession.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("REFUSED_GROUP_REQUEST"), Session.Character.Name), 10));
                            }
                            break;

                        case GroupRequestType.AcceptedShare:
                            if (targetSession != null && !targetSession.Character.GroupSentRequestCharacterIds.Contains(Session.Character.CharacterId))
                            {
                                return;
                            }

                            if (targetSession != null)
                            {
                                targetSession.Character.GroupSentRequestCharacterIds.Remove(Session.Character.CharacterId);
                                Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("ACCEPTED_SHARE"), targetSession.Character.Name), 0));

                                if (Session.Character.Group.IsMemberOfGroup(pjoinPacket.CharacterId))
                                {
                                    Session.Character.SetReturnPoint(targetSession.Character.Return.DefaultMapId, targetSession.Character.Return.DefaultX, targetSession.Character.Return.DefaultY);
                                    targetSession.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("CHANGED_SHARE"), targetSession.Character.Name), 0));
                                }
                            }
                            break;

                        case GroupRequestType.DeclinedShare:
                            if (targetSession != null && !targetSession.Character.GroupSentRequestCharacterIds.Contains(Session.Character.CharacterId))
                            {
                                return;
                            }

                            targetSession?.Character.GroupSentRequestCharacterIds.Remove(Session.Character.CharacterId);
                            Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("REFUSED_SHARE"), 0));
                            break;

                        case GroupRequestType.Requested:
                        case GroupRequestType.Invited:
                        case GroupRequestType.Accepted:
                        case GroupRequestType.Sharing:
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
            }
        }

        /// <summary>
        /// pleave packet
        /// </summary>
        /// <param name="pleavePacket"></param>
        public void GroupLeave(PLeavePacket pleavePacket)
        {
            ServerManager.Instance.GroupLeave(Session);
        }

        /// <summary>
        /// ; packet
        /// </summary>
        /// <param name="groupSayPacket"></param>
        public void GroupTalk(GroupSayPacket groupSayPacket)
        {
            if (!string.IsNullOrEmpty(groupSayPacket.Message))
            {
                ServerManager.Instance.Broadcast(Session, Session.Character.GenerateSpk(groupSayPacket.Message, 3), ReceiverType.Group);
                LogHelper.Instance.InsertChatLog(ChatType.Friend, Session.Character.CharacterId, groupSayPacket.Message, Session.IpAddress);
            }
        }
    }

    #endregion
}