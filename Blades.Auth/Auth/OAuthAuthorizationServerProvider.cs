using Blades.Auth.Interfaces;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BaseOAuthAuthorizationServerProvider = Microsoft.Owin.Security.OAuth.OAuthAuthorizationServerProvider;
using Blades.Extensions;

namespace Blades.Auth
{
    public class OAuthAuthorizationServerProvider : BaseOAuthAuthorizationServerProvider
    {
        private IAuthManager auth;
        public OAuthAuthorizationServerProvider(IAuthManager auth)
        {
            this.auth = auth;
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult(0);
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            ClaimsPrincipal principal = null;
            bool authSuccess = auth.TryAuthenticateUser(context.UserName, context.Password, out principal);
            if(authSuccess && principal != null)
            {
                var originalIdentity = (ClaimsIdentity)principal.Identity;
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaims(originalIdentity.Claims);

                context.Validated(identity);
            }
            else
            {
                context.SetError("invalid_grant", "Неверный логин или пароль");
            }

            return Task.FromResult(0);
        }
    }
}
