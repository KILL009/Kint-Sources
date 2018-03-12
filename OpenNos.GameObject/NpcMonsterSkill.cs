using OpenNos.Data;
using System;

namespace OpenNos.GameObject
{
    public class NpcMonsterSkill : NpcMonsterSkillDTO
    {
        #region Members

        private Skill skill;

        #endregion

        #region Properties

        public short Hit { get; set; }

        public DateTime LastSkillUse
        {
            get; set;
        }

        public Skill Skill => skill ?? (skill = ServerManager.Instance.GetSkill(SkillVNum));

        #endregion

        #region Methods

        public override void Initialize()
        {
            LastSkillUse = DateTime.Now.AddHours(-1);
            Hit = 0;
        }

        #endregion
    }
}