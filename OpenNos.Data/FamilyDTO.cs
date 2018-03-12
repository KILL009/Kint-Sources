using OpenNos.Domain;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class FamilyDTO : MappingBaseDTO
    {
        #region Properties

        public int FamilyExperience { get; set; }

        public byte FamilyFaction { get; set; }

        public GenderType FamilyHeadGender { get; set; }

        [Key]
        public long FamilyId { get; set; }

        public byte FamilyLevel { get; set; }

        public string FamilyMessage { get; set; }

        public FamilyAuthorityType ManagerAuthorityType { get; set; }

        public bool ManagerCanGetHistory { get; set; }

        public bool ManagerCanInvite { get; set; }

        public bool ManagerCanNotice { get; set; }

        public bool ManagerCanShout { get; set; }

        public byte MaxSize { get; set; }

        public FamilyAuthorityType MemberAuthorityType { get; set; }

        public bool MemberCanGetHistory { get; set; }

        public string Name { get; set; }

        public byte WarehouseSize { get; set; }

        #endregion
    }
}