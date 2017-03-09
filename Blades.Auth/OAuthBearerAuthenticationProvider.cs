using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseOAuthBearerAuthenticationProvider = Microsoft.Owin.Security.OAuth.OAuthBearerAuthenticationProvider;

namespace Blades.Auth
{
    public class OAuthBearerAuthenticationProvider: BaseOAuthBearerAuthenticationProvider
    {
        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            if (string.IsNullOrEmpty(context.Token))
            {
                context.Token = context.Request.Cookies["accessToken"];
            }

            return Task.FromResult<object>(null);
        }
    }
}
