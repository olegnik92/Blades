using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Extensions
{
    public static class PrincipalExtensions
    {
        public const string UserIdClaimType = "BladesPrincipal/UserIdClaimType";

        public const string UserLoginClaimType = "BladesPrincipal/UserLoginClaimType";


        public const string GuestLogin = "__Guest__";



        public static Guid GetUserId(this IIdentity indentity)
        {
            var claim = (indentity as ClaimsIdentity)?.Claims?.FirstOrDefault(c => c.Type == UserIdClaimType);

            if (claim == null)
            {
                return Guid.Empty;
            }

            return Guid.Parse(claim.Value);
        }

        public static string GetUserLogin(this IIdentity indentity)
        {
            var claim = (indentity as ClaimsIdentity)?.Claims?.FirstOrDefault(c => c.Type == UserLoginClaimType);

            if (claim == null)
            {
                return GuestLogin;
            }

            return claim.Value;
        }


        public static void SetUserId(this ClaimsIdentity identity, Guid id)
        {
            if(identity == null)
            {
                return;
            }

            identity.AddClaim(new Claim(UserIdClaimType, id.ToString(), typeof(Guid).ToString()));
        }

        public static void SetUserLogin(this ClaimsIdentity identity, string login)
        {
            if(identity == null)
            {
                return;
            }

            identity.AddClaim(new Claim(UserLoginClaimType, login));
        }

    }
}
