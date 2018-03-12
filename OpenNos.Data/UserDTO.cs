using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class UserDTO
    {
        #region Properties

        [Key]
        public string Name { get; set; }

        public string Password { get; set; }

        public string Unknown { get; set; }

        #endregion
    }
}