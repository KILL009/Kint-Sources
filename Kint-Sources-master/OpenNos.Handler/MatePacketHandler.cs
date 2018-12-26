using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using System;
using System.Linq;
using System.Reactive.Linq;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Packets.ClientPackets;
using OpenNos.GameObject.Battle;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;

namespace OpenNos.Handler
{
    public class MatePacketHandler : IPacketHandler
    {
        public MatePacketHandler(ClientSession session) => Session = session;

        private ClientSession Session { get; }

        /// <summary>
        /// suctl packet
        /// </summary>
        /// <param name="suctlPacket"></param>
        public void Attack(SuctlPacket suctlPacket)
        {
            PenaltyLogDTO penalty = Session.Account.PenaltyLogs.OrderByDescending(s => s.DateEnd).FirstOrDefault();
            if (Session.Character.IsMuted() && penalty != null)
            {
                if (Session.Character.Gender == GenderType.Female)
                {
                    Session.CurrentMapInstance?.Broadcast(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_FEMALE"), 1));
                    Session.SendPacket(Session.Character.GenerateSay(
                        string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"),
                            (penalty.DateEnd - DateTime.Now).ToString("hh\\:mm\\:ss")), 11));
                }
                else
                {
                    Session.CurrentMapInstance?.Broadcast(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_MALE"), 1));
                    Session.SendPacket(Session.Character.GenerateSay(
                        string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"),
                            (penalty.DateEnd - DateTime.Now).ToString("hh\\:mm\\:ss")), 11));
                }

                return;
            }

            Mate attacker = Session.Character.Mates.First(x => x.MateTransportId == suctlPacket.MateTransportId);
            if (attacker != null)
            {
                switch (suctlPacket.TargetType)
                {
                    case UserType.Monster:
                        if (attacker.Hp > 0)
                        {
                            MapMonster target = Session.CurrentMapInstance?.GetMonster(suctlPacket.TargetId);
                            NpcMonsterSkill skill =
                                attacker.Monster.Skills.Find(x => x.NpcMonsterSkillId == suctlPacket.CastId);
                            AttackMonster(attacker, skill, target);
                        }

                        return;

                    case UserType.Npc:
                        return;

                    case UserType.Player:
                        if (attacker.Hp > 0)
                        {
                            Character target = Session.CurrentMapInstance?.GetSessionByCharacterId(suctlPacket.TargetId)
                                ?.Character;
                            NpcMonsterSkill skill =
                                attacker.Monster.Skills.Find(x => x.NpcMonsterSkillId == suctlPacket.CastId);

                            if (target != null && Session.CurrentMapInstance != null &&
                                (target.Session.CurrentMapInstance == Session.CurrentMapInstance
                                 && target.CharacterId != Session.Character.CharacterId &&
                                 Session.CurrentMapInstance.Map.MapTypes.Any(s =>
                                     s.MapTypeId == (short)MapTypeEnum.Act4) && Session.Character.Faction !=
                                 target.Faction && Session.CurrentMapInstance.Map
                                     .MapId != 130 && Session.CurrentMapInstance.Map
                                     .MapId != 131 ||
                                 Session.CurrentMapInstance.Map.MapTypes.Any(m =>
                                     m.MapTypeId == (short)MapTypeEnum.PVPMap) &&
                                 (Session.Character.Group == null
                                  || !Session.Character.Group
                                      .IsMemberOfGroup(
                                          target.CharacterId)) ||
                                 Session.CurrentMapInstance.IsPVP && (Session.Character.Group == null
                                                                      || !Session.Character.Group.IsMemberOfGroup(
                                                                          target.CharacterId))))
                            {
                                AttackCharacter(attacker, skill, target);
                            }
                        }

                        return;

                    case UserType.Object:
                        return;
                }
            }
        }

        private void AttackMonster(Mate attacker, NpcMonsterSkill skill, MapMonster target)
        {
            if (target == null || attacker == null)
            {
                return;
            }

            if (target.CurrentHp > 0)
            {
                Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.CastOnTarget(UserType.Npc,
                    attacker.MateTransportId, 3, target.MapMonsterId, -1, -1, 0));
                target.HitQueue.Enqueue(new HitRequest(TargetHitType.SingleTargetHit, Session, attacker, skill));
            }
        }

        private void AttackCharacter(Mate attacker, NpcMonsterSkill skill, Character target)
        {
            if (attacker == null || target == null)
            {
                return;
            }

            if (target.Hp > 0 && attacker.Hp > 0)
            {
                if ((Session.CurrentMapInstance.MapInstanceId == ServerManager.Instance.ArenaInstance.MapInstanceId
                     || Session.CurrentMapInstance.MapInstanceId
                     == ServerManager.Instance.FamilyArenaInstance.MapInstanceId)
                    && (Session.CurrentMapInstance.Map.JaggedGrid[Session.Character.PositionX][
                            Session.Character.PositionY]?.Value != 0
                        || target.Session.CurrentMapInstance.Map.JaggedGrid[target.PositionX][
                                target.PositionY]
                            ?.Value != 0))
                {
                    // User in SafeZone
                    Session.SendPacket(StaticPacketHelper.Cancel(2, target.CharacterId));
                    return;
                }

                if (target.IsSitting)
                {
                    target.Rest();
                }

                int hitmode = 0;
                bool onyxWings = false;
                BattleEntity battleEntity = new BattleEntity(attacker);
                BattleEntity battleEntityDefense = new BattleEntity(target, null);
                Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.CastOnTarget(UserType.Npc,
                    attacker.MateTransportId, 1, target.CharacterId, -1, -1, 0));
                int damage = DamageHelper.Instance.CalculateDamage(battleEntity, battleEntityDefense, skill?.Skill,
                    ref hitmode, ref onyxWings);
                if (target.HasGodMode)
                {
                    damage = 0;
                    hitmode = 1;
                }
                else if (target.LastPVPRevive > DateTime.Now.AddSeconds(-10)
                         || Session.Character.LastPVPRevive > DateTime.Now.AddSeconds(-10))
                {
                    damage = 0;
                    hitmode = 1;
                }

                int[] manaShield = target.GetBuff(BCardType.CardType.LightAndShadow,
                    (byte)AdditionalTypes.LightAndShadow.InflictDamageToMP);
                if (manaShield[0] != 0 && hitmode != 1)
                {
                    int reduce = damage / 100 * manaShield[0];
                    if (target.Mp < reduce)
                    {
                        target.Mp = 0;
                    }
                    else
                    {
                        target.Mp -= reduce;
                    }
                }

                target.GetDamage(damage / 2);
                target.LastDefence = DateTime.Now;
                target.Session.SendPacket(target.GenerateStat());
                bool isAlive = target.Hp > 0;
                if (!isAlive && target.Session.HasCurrentMapInstance)
                {
                    if (target.Session.CurrentMapInstance.Map?.MapTypes.Any(
                            s => s.MapTypeId == (short)MapTypeEnum.Act4)
                        == true)
                    {
                        if (ServerManager.Instance.ChannelId == 51 && ServerManager.Instance.Act4DemonStat.Mode == 0
                                                                   && ServerManager.Instance.Act4AngelStat.Mode == 0)
                        {
                            switch (Session.Character.Faction)
                            {
                                case FactionType.Angel:
                                    ServerManager.Instance.Act4AngelStat.Percentage += 100;
                                    break;

                                case FactionType.Demon:
                                    ServerManager.Instance.Act4DemonStat.Percentage += 100;
                                    break;
                            }
                        }

                        Session.Character.Act4Kill++;
                        target.Act4Dead++;
                        target.GetAct4Points(-1);
                        if (target.Level + 10 >= Session.Character.Level
                            && Session.Character.Level <= target.Level - 10)
                        {
                            Session.Character.GetAct4Points(2);
                        }

                        if (target.Reputation < 50000)
                        {
                            target.Session.SendPacket(Session.Character.GenerateSay(
                                string.Format(Language.Instance.GetMessageFromKey("LOSE_REP"), 0), 11));
                        }
                        else
                        {
                            target.Reputation -= target.Level * 50;
                            Session.Character.Reputation += target.Level * 50;
                            Session.SendPacket(Session.Character.GenerateLev());
                            target.Session.SendPacket(target.GenerateSay(
                                string.Format(Language.Instance.GetMessageFromKey("LOSE_REP"),
                                    (short)(target.Level * 50)), 11));
                        }

                        foreach (ClientSession sess in ServerManager.Instance.Sessions.Where(
                            s => s.HasSelectedCharacter))
                        {
                            if (sess.Character.Faction == Session.Character.Faction)
                            {
                                sess.SendPacket(sess.Character.GenerateSay(
                                    string.Format(
                                        Language.Instance.GetMessageFromKey(
                                            $"ACT4_PVP_KILL{(int)target.Faction}"), Session.Character.Name),
                                    12));
                            }
                            else if (sess.Character.Faction == target.Faction)
                            {
                                sess.SendPacket(sess.Character.GenerateSay(
                                    string.Format(
                                        Language.Instance.GetMessageFromKey(
                                            $"ACT4_PVP_DEATH{(int)target.Faction}"), target.Name),
                                    11));
                            }
                        }

                        target.Session.SendPacket(target.GenerateFd());
                        target.DisableBuffs(BuffType.All);
                        target.Session.CurrentMapInstance.Broadcast(target.Session, target.GenerateIn(),
                            ReceiverType.AllExceptMe);
                        target.Session.CurrentMapInstance.Broadcast(target.Session, target.GenerateGidx(),
                            ReceiverType.AllExceptMe);
                        target.Session.SendPacket(
                            target.GenerateSay(Language.Instance.GetMessageFromKey("ACT4_PVP_DIE"), 11));
                        target.Session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ACT4_PVP_DIE"), 0));
                        Observable.Timer(TimeSpan.FromMilliseconds(2000)).Subscribe(o =>
                        {
                            target.Session.CurrentMapInstance?.Broadcast(target.Session,
                                $"c_mode 1 {target.CharacterId} 1564 0 0 0");
                            target.Session.CurrentMapInstance?.Broadcast(target.GenerateRevive());
                        });
                        Observable.Timer(TimeSpan.FromMilliseconds(30000)).Subscribe(o =>
                        {
                            target.Hp = (int)target.HPLoad();
                            target.Mp = (int)target.MPLoad();
                            short x = (short)(12 + ServerManager.RandomNumber(-2, 3));
                            short y = (short)(13 + ServerManager.RandomNumber(-2, 3));
                            if (target.Faction == FactionType.Angel)
                            {
                                ServerManager.Instance.ChangeMap(target.CharacterId, 2503, x, y);
                            }
                            else if (target.Faction == FactionType.Demon)
                            {
                                ServerManager.Instance.ChangeMap(target.CharacterId, 2503, x, y);
                            }
                            else
                            {
                                target.MapId = 2503;
                                target.MapX = 12;
                                target.MapY = 13;
                                string connection =
                                    CommunicationServiceClient.Instance.RetrieveOriginWorld(Session.Account.AccountId);
                                if (string.IsNullOrWhiteSpace(connection))
                                {
                                    return;
                                }

                                int port = Convert.ToInt32(connection.Split(':')[1]);
                                Session.Character.ChangeChannel(connection.Split(':')[0], port, 3);
                                return;
                            }

                            target.Session.CurrentMapInstance?.Broadcast(target.Session, target.GenerateTp());
                            target.Session.CurrentMapInstance?.Broadcast(target.GenerateRevive());
                            target.Session.SendPacket(target.GenerateStat());
                        });
                    }
                    else
                    {
                        Session.Character.TalentWin++;
                        target.TalentLose++;
                        Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("PVP_KILL"),
                                Session.Character.Name, target.Name), 10));
                        Observable.Timer(TimeSpan.FromMilliseconds(1000)).Subscribe(o =>
                            ServerManager.Instance.AskPvpRevive(target.CharacterId));
                    }
                }

                if (hitmode != 1)
                {
                    skill?.Skill?.BCards.Where(s => s.Type.Equals((byte)BCardType.CardType.Buff)).ToList()
                        .ForEach(s => s.ApplyBCards(target, Session.Character));
                }

                Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.SkillUsed(UserType.Npc,
                    attacker.MateTransportId, 1, target.CharacterId, 0, 12, 11, 200, 0, 0, isAlive,
                    (int)(target.Hp / target.HPLoad() * 100), damage, hitmode, 0));

                //
                //                switch (hitRequest.TargetHitType)
                //                {
                //                    case TargetHitType.SingleTargetHit:
                //                        hitRequest.Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.SkillUsed(UserType.Player,
                //                            hitRequest.Session.Character.CharacterId, 1, target.Character.CharacterId,
                //                            hitRequest.Skill.SkillVNum, hitRequest.Skill.Cooldown, hitRequest.Skill.AttackAnimation,
                //                            hitRequest.SkillEffect, hitRequest.Session.Character.PositionX,
                //                            hitRequest.Session.Character.PositionY, isAlive,
                //                            (int) (target.Character.Hp / (float) target.Character.HPLoad() * 100), damage, hitmode,
                //                            (byte) (hitRequest.Skill.SkillType - 1)));
                //                        break;
                //
                //                    case TargetHitType.SingleTargetHitCombo:
                //                        hitRequest.Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.SkillUsed(UserType.Player,
                //                            hitRequest.Session.Character.CharacterId, 1, target.Character.CharacterId,
                //                            hitRequest.Skill.SkillVNum, hitRequest.Skill.Cooldown, hitRequest.SkillCombo.Animation,
                //                            hitRequest.SkillCombo.Effect, hitRequest.Session.Character.PositionX,
                //                            hitRequest.Session.Character.PositionY, isAlive,
                //                            (int) (target.Character.Hp / (float) target.Character.HPLoad() * 100), damage, hitmode,
                //                            (byte) (hitRequest.Skill.SkillType - 1)));
                //                        break;
                //
                //                    case TargetHitType.SingleAOETargetHit:
                //                        switch (hitmode)
                //                        {
                //                            case 1:
                //                                hitmode = 4;
                //                                break;
                //
                //                            case 3:
                //                                hitmode = 6;
                //                                break;
                //
                //                            default:
                //                                hitmode = 5;
                //                                break;
                //                        }
                //
                //                        if (hitRequest.ShowTargetHitAnimation)
                //                        {
                //                            hitRequest.Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.SkillUsed(
                //                                UserType.Player, hitRequest.Session.Character.CharacterId, 1,
                //                                target.Character.CharacterId, hitRequest.Skill.SkillVNum, hitRequest.Skill.Cooldown,
                //                                hitRequest.Skill.AttackAnimation, hitRequest.SkillEffect, 0, 0, isAlive,
                //                                (int) (target.Character.Hp / (float) target.Character.HPLoad() * 100), 0, 0,
                //                                (byte) (hitRequest.Skill.SkillType - 1)));
                //                        }
                //
                //                        hitRequest.Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.SkillUsed(UserType.Player,
                //                            hitRequest.Session.Character.CharacterId, 1, target.Character.CharacterId,
                //                            hitRequest.Skill.SkillVNum, hitRequest.Skill.Cooldown, hitRequest.Skill.AttackAnimation,
                //                            hitRequest.SkillEffect, hitRequest.Session.Character.PositionX,
                //                            hitRequest.Session.Character.PositionY, isAlive,
                //                            (int) (target.Character.Hp / (float) target.Character.HPLoad() * 100), damage, hitmode,
                //                            (byte) (hitRequest.Skill.SkillType - 1)));
                //                        break;
                //
                //                    case TargetHitType.AOETargetHit:
                //                        switch (hitmode)
                //                        {
                //                            case 1:
                //                                hitmode = 4;
                //                                break;
                //
                //                            case 3:
                //                                hitmode = 6;
                //                                break;
                //
                //                            default:
                //                                hitmode = 5;
                //                                break;
                //                        }
                //
                //                        hitRequest.Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.SkillUsed(UserType.Player,
                //                            hitRequest.Session.Character.CharacterId, 1, target.Character.CharacterId,
                //                            hitRequest.Skill.SkillVNum, hitRequest.Skill.Cooldown, hitRequest.Skill.AttackAnimation,
                //                            hitRequest.SkillEffect, hitRequest.Session.Character.PositionX,
                //                            hitRequest.Session.Character.PositionY, isAlive,
                //                            (int) (target.Character.Hp / (float) target.Character.HPLoad() * 100), damage, hitmode,
                //                            (byte) (hitRequest.Skill.SkillType - 1)));
                //                        break;
                //
                //                    case TargetHitType.ZoneHit:
                //                        hitRequest.Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.SkillUsed(UserType.Player,
                //                            hitRequest.Session.Character.CharacterId, 1, target.Character.CharacterId,
                //                            hitRequest.Skill.SkillVNum, hitRequest.Skill.Cooldown, hitRequest.Skill.AttackAnimation,
                //                            hitRequest.SkillEffect, hitRequest.MapX, hitRequest.MapY, isAlive,
                //                            (int) (target.Character.Hp / (float) target.Character.HPLoad() * 100), damage, 5,
                //                            (byte) (hitRequest.Skill.SkillType - 1)));
                //                        break;
                //
                //                    case TargetHitType.SpecialZoneHit:
                //                        hitRequest.Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.SkillUsed(UserType.Player,
                //                            hitRequest.Session.Character.CharacterId, 1, target.Character.CharacterId,
                //                            hitRequest.Skill.SkillVNum, hitRequest.Skill.Cooldown, hitRequest.Skill.AttackAnimation,
                //                            hitRequest.SkillEffect, hitRequest.Session.Character.PositionX,
                //                            hitRequest.Session.Character.PositionY, isAlive,
                //                            (int) (target.Character.Hp / target.Character.HPLoad() * 100), damage, 0,
                //                            (byte) (hitRequest.Skill.SkillType - 1)));
                //                        break;
                //
                //                    default:
                //                        Logger.Warn("Not Implemented TargetHitType Handling!");
                //                        break;
                //                }
            }
            else
            {
                // monster already has been killed, send cancel
                Session.SendPacket(StaticPacketHelper.Cancel(2, target.CharacterId));
            }

        }

        /// <summary>
        /// psl packet
        /// </summary>
        /// <param name="pslPacket"></param>
        public void Psl(PslPacket pslPacket)
        {
            Mate mate = Session.Character.Mates.Find(x => x.IsTeamMember && x.MateType == MateType.Partner);
            if (mate == null)
            {
                return;
            }

            if (pslPacket.Type == 0)
            {
                if (mate.IsUsingSp)
                {
                    mate.IsUsingSp = false;
                    mate.Skills = null;
                    Session.Character.MapInstance.Broadcast(mate.GenerateCMode(-1));
                    Session.SendPacket(mate.GenerateCond());
                    Session.SendPacket(mate.GenPski());
                    Session.SendPacket(mate.GenerateScPacket());
                    Session.Character.MapInstance.Broadcast(mate.GenerateOut());
                    Session.Character.MapInstance.Broadcast(mate.GenerateIn());
                    Session.SendPacket(Session.Character.GeneratePinit());
                    //psd 30
                }
                else
                {
                    // Doesn't work right now, atleast not on the ON Client
                    // Session.SendPacket("pdelay 5000 3 #psl^1 ");
                    Session.SendPacket("delay 5000 3 #psl^1"); // Hotfix, pdelay is broken smh
                    Session.CurrentMapInstance?.Broadcast(UserInterfaceHelper.GenerateGuri(2, 2, mate.MateTransportId),
                        mate.PositionX, mate.PositionY);
                }
            }
            else
            {
                if (mate.SpInstance == null)
                {
                    return;
                }

                mate.IsUsingSp = true;
                //TODO: update pet skills
                Session.SendPacket(mate.GenerateCond());
                Session.Character.MapInstance.Broadcast(mate.GenerateCMode(mate.SpInstance.Item.Morph));
                Session.SendPacket(mate.GenPski());
                Session.SendPacket(mate.GenerateScPacket());
                Session.Character.MapInstance.Broadcast(mate.GenerateOut());
                Session.Character.MapInstance.Broadcast(mate.GenerateIn());
                Session.SendPacket(Session.Character.GeneratePinit());
                Session.Character.MapInstance.Broadcast(
                    StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 196));
            }
        }
    }
}