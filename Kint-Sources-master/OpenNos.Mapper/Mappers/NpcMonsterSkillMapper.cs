using OpenNos.DAL.EF;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public class NpcMonsterSkillMapper
    {
        public NpcMonsterSkillMapper()
        {
        }

        public bool ToNpcMonsterSkillDTO(NpcMonsterSkill input, NpcMonsterSkillDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.NpcMonsterSkillId = input.NpcMonsterSkillId;
            output.NpcMonsterVNum = input.NpcMonsterVNum;
            output.Rate = input.Rate;
            output.SkillVNum = input.SkillVNum;
            return true;
        }

        public bool ToNpcMonsterSkill(NpcMonsterSkillDTO input, NpcMonsterSkill output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.NpcMonsterSkillId = input.NpcMonsterSkillId;
            output.NpcMonsterVNum = input.NpcMonsterVNum;
            output.Rate = input.Rate;
            output.SkillVNum = input.SkillVNum;
            return true;
        }
    }
}
