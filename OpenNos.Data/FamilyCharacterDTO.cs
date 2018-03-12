using OpenNos.Domain;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class FamilyCharacterDTO : MappingBaseDTO
    {
        #region Properties

        public FamilyAuthority Authority { get; set; }

        public long CharacterId { get; set; }

        public string DailyMessage { get; set; }

        public int Experience { get; set; }

        [Key]
        public long FamilyCharacterId { get; set; }

        public long FamilyId { get; set; }

        public FamilyMemberRank Rank { get; set; }

        #endregion
    }
}