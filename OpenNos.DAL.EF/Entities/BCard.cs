using System.ComponentModel.DataAnnotations;

namespace OpenNos.DAL.EF
{
    public class BCard
    {
        #region Properties

        [Key]
        public short BCardId { get; set; }

        public virtual Card Card { get; set; }

        public short? CardId { get; set; }

        public byte CastType { get; set; }

        public int FirstData { get; set; }

        public bool IsLevelDivided { get; set; }

        public bool IsLevelScaled { get; set; }

        public virtual Item Item { get; set; }

        public short? ItemVNum { get; set; }

        public virtual NpcMonster NpcMonster { get; set; }

        public short? NpcMonsterVNum { get; set; }

        public int SecondData { get; set; }

        public virtual Skill Skill { get; set; }

        public short? SkillVNum { get; set; }

        public byte SubType { get; set; }

        public int ThirdData { get; set; }

        public byte Type { get; set; }

        #endregion
    }
}