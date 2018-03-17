using OpenNos.DAL.EF;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public class CharacterRelationMapper
    {
        public CharacterRelationMapper()
        {

        }

        public bool ToCharacterRelationDTO(CharacterRelation input, CharacterRelationDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.CharacterId = input.CharacterId;
            output.CharacterRelationId = input.CharacterRelationId;
            output.RelatedCharacterId = input.RelatedCharacterId;
            output.RelationType = input.RelationType;
            return true;
        }

        public bool ToCharacterRelation(CharacterRelationDTO input, CharacterRelation output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.CharacterId = input.CharacterId;
            output.CharacterRelationId = input.CharacterRelationId;
            output.RelatedCharacterId = input.RelatedCharacterId;
            output.RelationType = input.RelationType;
            return true;
        }
    }
}
