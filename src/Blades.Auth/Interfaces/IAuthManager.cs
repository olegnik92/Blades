using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Blades.Interfaces;

namespace Blades.Auth.Interfaces
{
    public interface IAuthManager: IBladesService
    {
        bool TryAuthenticateUser(string login, string password, out ClaimsPrincipal principal);
    }
}
