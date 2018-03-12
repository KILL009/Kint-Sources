using OpenNos.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace OpenNos.Master.Server
{
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        #region Instantiation

        public AuthorizeRoleAttribute(AuthorityType allowedRole)
        {
            var allowedRolesAsStrings = string.Empty;
            IEnumerable<AuthorityType> enums = Enum.GetValues(typeof(AuthorityType)).Cast<AuthorityType>().ToList().Where(s => s >= allowedRole);
            Roles = string.Join(",", enums.ToArray());
        }

        #endregion
    }
}