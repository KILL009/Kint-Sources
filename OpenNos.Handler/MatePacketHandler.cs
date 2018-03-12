using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Packets.ClientPackets;
using System;
using System.Linq;

namespace OpenNos.Handler
{
    public class MatePacketHandler : IPacketHandler
    {
        #region Instantiation

        public MatePacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        private ClientSession Session { get; }

        #endregion

        #region Methods

        /// <summary>
        /// suctl packet
        /// </summary>
        /// <param name="suctlPacket"></param>
        public void Attack(SuctlPacket suctlPacket)
        {
            var penalty = Session.Account.PenaltyLogs.OrderByDescending(s => s.DateEnd).FirstOrDefault();

            if (Session.Character.IsMuted() && penalty != null)
            {
                Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey(Session.Character.Gender == GenderType.Female ? "MUTED_FEMALE" : "MUTED_MALE"), 1));
                Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"), (penalty.DateEnd - DateTime.Now).ToString("hh\\:mm\\:ss")), 11));
                return;
            }

            var attacker = Session.Character.Mates.First(x => x.MateTransportId == suctlPacket.MateTransportId);

            switch (suctlPacket.TargetType)
            {
                case UserType.Monster:
                    if (attacker.Hp > 0)
                    {
                        AttackMonster(attacker, attacker.Monster.Skills.FirstOrDefault(x => x.NpcMonsterSkillId == suctlPacket.CastId), Session?.CurrentMapInstance?.GetMonster(suctlPacket.TargetId));
                    }
                    break;
            }
        }

        public void AttackCharacter(Mate attacker, NpcMonsterSkill skill, Character target)
        {
        }

        public void AttackMonster(Mate attacker, NpcMonsterSkill skill, MapMonster target)
        {
            if (target == null || attacker == null)
            {
                return;
            }

            if (target.CurrentHp > 0 && skill == null)
            {
                // TODO: Implement official damage calculation
                var dmg = (attacker.Level * 15);

                Session?.CurrentMapInstance?.Broadcast($"ct 2 {attacker.MateTransportId} 3 {target.MapMonsterId} -1 -1 0");
                target.CurrentHp -= dmg;

                if (target.DamageList.ContainsKey(attacker.MateTransportId))
                {
                    target.DamageList[attacker.MateTransportId] += dmg;
                }
                else
                {
                    target.DamageList.Add(attacker.MateTransportId, dmg);
                }

                if (target.CurrentHp <= 0)
                {
                    target.CurrentHp = 0;
                    target.IsAlive = false;
                    Session.Character.GenerateKillBonus(target);
                }

                Session?.CurrentMapInstance?.Broadcast($"su 2 {attacker.MateTransportId} 3 {target.MapMonsterId} 0 12 11 200 0 0 {(target.IsAlive ? 1 : 0)} {(int)((double)target.CurrentHp / target.Monster.MaxHP * 100)} {dmg} 0 0");
            }
        }

        /// <summary>
        /// psl packet
        /// </summary>
        /// <param name="pslPacket"></param>
        public void Psl(PslPacket pslPacket)
        {
            var mate = Session.Character.Mates.FirstOrDefault(x => x.IsTeamMember && x.MateType == MateType.Partner);

            if (mate == null)
            {
                return;
            }

            if (pslPacket.Type == 0)
            {
                if (mate.IsUsingSp)
                {
                    mate.IsUsingSp = false;
                    Session.Character.MapInstance.Broadcast(mate.GenerateCMode(-1));
                    Session.SendPacket(mate.GenerateCond());

                    // dpski
                    Session.SendPacket(mate.GenerateScPacket());
                    Session.Character.MapInstance.Broadcast(mate.GenerateOut());
                    Session.Character.MapInstance.Broadcast(mate.GenerateIn());
                    Session.SendPacket(Session.Character.GeneratePinit());

                    // psd 30
                }
                else
                {
                    Session.SendPacket("delay 5000 3 #psl^1 ");
                    Session.CurrentMapInstance?.Broadcast(UserInterfaceHelper.Instance.GenerateGuri(2, 2, mate.MateTransportId), mate.PositionX, mate.PositionY);
                }
            }
            else if (mate.SpInstance != null)
            {
                mate.IsUsingSp = true;
                Session.SendPacket(mate.GenerateCond());
                Session.Character.MapInstance.Broadcast(mate.GenerateCMode(mate.SpInstance.Item.Morph));

                // pski 1236 1238 1240
                Session.SendPacket(mate.GenerateScPacket());
                Session.Character.MapInstance.Broadcast(mate.GenerateOut());
                Session.Character.MapInstance.Broadcast(mate.GenerateIn());
                Session.SendPacket(Session.Character.GeneratePinit());
                Session.Character.MapInstance.Broadcast(mate.GenerateEff(196));
            }
        }

        #endregion
    }
}