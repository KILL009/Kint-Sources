using OpenNos.Domain;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class CharacterRelationDTO : MappingBaseDTO
    {
        #region Properties

        public long CharacterId { get; set; }

        [Key]
        public long CharacterRelationId { get; set; }

        public long RelatedCharacterId { get; set; }

        public CharacterRelationType RelationType { get; set; }

        #endregion
    }
}