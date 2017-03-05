using Blades.Auth.Interfaces;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Auth
{
    public static class StartupHelper
    {
        public static void OAuthConfiguration(IAppBuilder appBuilder, IAuthManager authManager, 
            bool allowInsecureHttp = true, 
            string tokenEndpointPath = "/token",
            TimeSpan? accessTokenExpireTimeSpan = null)
        {
            var OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = allowInsecureHttp,
                TokenEndpointPath = new PathString(tokenEndpointPath),
                AccessTokenExpireTimeSpan = accessTokenExpireTimeSpan ?? TimeSpan.FromDays(1),
                Provider = new OAuthAuthorizationServerProvider(authManager),               
            };

            // Token Generation

            appBuilder.UseOAuthAuthorizationServer(OAuthServerOptions);
            appBuilder.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}
