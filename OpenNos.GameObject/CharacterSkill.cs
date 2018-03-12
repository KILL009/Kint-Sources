using OpenNos.Data;
using System;

namespace OpenNos.GameObject
{
    public class CharacterSkill : CharacterSkillDTO
    {
        #region Members

        private Skill skill;

        #endregion

        #region Instantiation

        public CharacterSkill(CharacterSkillDTO characterSkill)
        {
            CharacterId = characterSkill.CharacterId;
            Id = characterSkill.Id;
            SkillVNum = characterSkill.SkillVNum;
            LastUse = DateTime.Now.AddHours(-1);
            Hit = 0;
        }

        public CharacterSkill()
        {
            LastUse = DateTime.Now.AddHours(-1);
            Hit = 0;
        }

        #endregion

        #region Properties

        public short Hit { get; set; }

        public DateTime LastUse
        {
            get; set;
        }

        public Skill Skill => skill ?? (skill = ServerManager.Instance.GetSkill(SkillVNum));

        #endregion

        #region Methods

        public bool CanBeUsed()
        {
            return Skill != null && LastUse.AddMilliseconds(Skill.Cooldown * 100) < DateTime.Now;
        }

        #endregion
    }
}