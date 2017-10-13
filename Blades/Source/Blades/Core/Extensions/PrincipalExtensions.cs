using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Blades.Core.Extensions
{
    public static class PrincipalExtensions
    {
        public const string UserIdClaimType = "BladesPrincipal/UserIdClaimType";

        public const string UserLoginClaimType = "BladesPrincipal/UserLoginClaimType";


        public const string GuestLogin = "__Guest__";



        public static Guid GetUserId(this IIdentity indentity)
        {
            var claim = (indentity as ClaimsIdentity)?.Claims?.FirstOrDefault(c => c.Type == UserIdClaimType);

            return claim == null ? Guid.Empty : Guid.Parse(claim.Value);
        }

        public static string GetUserLogin(this IIdentity indentity)
        {
            var claim = (indentity as ClaimsIdentity)?.Claims?.FirstOrDefault(c => c.Type == UserLoginClaimType);

            return claim == null ? GuestLogin : claim.Value;
        }


        public static void SetUserId(this ClaimsIdentity identity, Guid id)
        {
            identity?.AddClaim(new Claim(UserIdClaimType, id.ToString(), typeof(Guid).ToString()));
        }

        public static void SetUserLogin(this ClaimsIdentity identity, string login)
        {
            identity?.AddClaim(new Claim(UserLoginClaimType, login));
        }

    }

}