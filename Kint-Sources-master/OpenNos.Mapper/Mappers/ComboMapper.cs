using OpenNos.DAL.EF;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public class ComboMapper
    {
        public ComboMapper()
        {

        }

        public bool ToComboDTO(Combo input, ComboDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.Animation = input.Animation;
            output.ComboId = input.ComboId;
            output.Effect = input.Effect;
            output.Hit = input.Hit;
            output.SkillVNum = input.SkillVNum;
            return true;
        }

        public bool ToCombo(ComboDTO input, Combo output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.Animation = input.Animation;
            output.ComboId = input.ComboId;
            output.Effect = input.Effect;
            output.Hit = input.Hit;
            output.SkillVNum = input.SkillVNum;
            return true;
        }
    }
}
