using System;
using System.Collections.Generic;
using System.Security.Claims;
using Blades.Core.Extensions;

namespace Blades.Core
{
    public class UserInfo
    {
        public UserInfo(ClaimsPrincipal principal = null)
        {
            if(principal == null)
            {
                principal = ClaimsPrincipal.Current ?? new ClaimsPrincipal();
            }

            Principal = principal;
            Id = principal.Identity.GetUserId();
            Login = principal.Identity.GetUserLogin();
        }

        public Guid Id { get; set; }

        public string Login { get; set; }

        public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();

        public ClaimsPrincipal Principal { get; set; }

        public bool IsGuest()
        {
            return Id.Equals(Guid.Empty);
        }
    }
}