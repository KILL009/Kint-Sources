using OpenNos.Domain;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class AccountDTO : MappingBaseDTO
    {
        #region Properties

        [Key]
        public long AccountId { get; set; }

        public AuthorityType Authority { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string RegistrationIP { get; set; }

        public string VerificationToken { get; set; }

        public long BankGold { get; set; }

        public int NosDollar { get; set; }

        #endregion
    }
}