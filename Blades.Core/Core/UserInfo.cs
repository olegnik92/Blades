using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blades.Extensions;

namespace Blades.Core
{
    public class UserInfo
    {
        public UserInfo(ClaimsPrincipal principal)
        {
            if(principal == null)
            {
                principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            Principal = principal;
            Id = principal.Identity.GetUserId();
            Login = principal.Identity.GetUserLogin();
        }

        public Guid Id { get; set; }

        public string Login { get; set; }

        public string Location { get; set; }

        public string UserAgent { get; set; }

        public ClaimsPrincipal Principal { get; set; }

        public bool IsGuest()
        {
            return Guid.Empty.Equals(Id);
        }
    }
}
