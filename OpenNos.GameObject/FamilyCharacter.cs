using OpenNos.DAL;
using OpenNos.Data;

namespace OpenNos.GameObject
{
    public class FamilyCharacter : FamilyCharacterDTO
    {
        #region Members

        private CharacterDTO character;

        #endregion

        #region Properties

        public CharacterDTO Character
        {
            get
            {
                if (character == null)
                {
                    character = DAOFactory.CharacterDAO.FirstOrDefault(s => s.CharacterId == CharacterId);
                }

                return character;
            }
        }

        #endregion

        #region Methods

        public override void Initialize()
        {
            // do nothing
        }

        public static FamilyCharacter FromDTO(FamilyCharacterDTO familyCharacterDTO)
        {
            FamilyCharacter familyCharacter = new FamilyCharacter();

            familyCharacter.Authority = familyCharacterDTO.Authority;
            familyCharacter.CharacterId = familyCharacterDTO.CharacterId;
            familyCharacter.DailyMessage = familyCharacterDTO.DailyMessage;
            familyCharacter.Experience = familyCharacterDTO.Experience;
            familyCharacter.FamilyCharacterId = familyCharacterDTO.FamilyCharacterId;
            familyCharacter.FamilyId = familyCharacterDTO.FamilyId;
            familyCharacter.Rank = familyCharacterDTO.Rank;

            return familyCharacter;
        }

        #endregion
    }
}