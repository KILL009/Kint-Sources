using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class NpcMonsterSkillDTO : MappingBaseDTO
    {
        #region Properties

        [Key]
        public long NpcMonsterSkillId { get; set; }

        public short NpcMonsterVNum { get; set; }

        public short Rate { get; set; }

        public short SkillVNum { get; set; }

        #endregion
    }
}