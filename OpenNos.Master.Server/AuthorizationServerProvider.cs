using Microsoft.Owin.Security.OAuth;
using OpenNos.Core;
using OpenNos.DAL;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OpenNos.Master.Server
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        #region Methods

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            var account = DAOFactory.AccountDAO.FirstOrDefault(s => s.Name == context.UserName);

            if (account != null && account.Password.ToLower().Equals(EncryptionBase.Sha512(context.Password)))
            {
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, context.UserName));
                identity.AddClaim(new Claim(ClaimTypes.Role, account.Authority.ToString()));
                context.Validated(identity);
            }
            else
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }
        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        #endregion
    }
}