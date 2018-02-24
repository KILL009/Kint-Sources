using OpenNos.DAL.EF;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public class FamilyCharacterMapper
    {
        public FamilyCharacterMapper()
        {

        }

        public bool ToFamilyCharacterDTO(FamilyCharacter input, FamilyCharacterDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.Authority = input.Authority;
            output.CharacterId = input.CharacterId;
            output.DailyMessage = input.DailyMessage;
            output.Experience = input.Experience;
            output.FamilyCharacterId = input.FamilyCharacterId;
            output.FamilyId = input.FamilyId;
            output.Rank = input.Rank;
            return true;
        }

        public bool ToFamilyCharacter(FamilyCharacterDTO input, FamilyCharacter output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.Authority = input.Authority;
            output.CharacterId = input.CharacterId;
            output.DailyMessage = input.DailyMessage;
            output.Experience = input.Experience;
            output.FamilyCharacterId = input.FamilyCharacterId;
            output.FamilyId = input.FamilyId;
            output.Rank = input.Rank;
            return true;
        }
    }
}
