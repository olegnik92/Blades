using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Blades.Auth.Interfaces;
using Blades.Core;
using Blades.Extensions;

namespace Blades.Auth.Basis
{
    public class BasisForTests : IAuthManager, IPermissionRequirementChecker
    {

        public bool HasRequirement(UserInfo user, PermissionRequirement requirement)
        {
            return user.Login.ToLower() == "admin";
        }

        public bool HasRequirement(UserInfo user, PermissionType permission, Guid resourceTypeId)
        {
            return user.Login.ToLower() == "admin";
        }

        public bool HasRequirement(UserInfo user, PermissionType permission, Resource resource)
        {
            return user.Login.ToLower() == "admin";
        }

        public bool HasRequirement(UserInfo user, PermissionType permission, Guid resourceTypeId, Guid resourceInstanceId)
        {
            return user.Login.ToLower() == "admin";
        }

        public bool TryAuthenticateUser(string login, string password, out ClaimsPrincipal principal)
        {
            login = login.Trim().ToLower();
            if (password != "w")
            {
                principal = null;
                return false;
            }


            var identity = new ClaimsIdentity();
            identity.SetUserId(Guid.NewGuid());
            identity.SetUserLogin(login);

            principal = new ClaimsPrincipal(identity);
            return true;
        }
    }
}
