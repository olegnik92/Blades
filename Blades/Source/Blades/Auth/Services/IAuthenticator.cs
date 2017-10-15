using System.Security.Claims;
using Blades.Core.Services;

namespace Blades.Auth.Services
{
    public interface IAuthenticator: IBladesService
    {
        bool TryAuthenticateUser(string login, string password, out ClaimsPrincipal principal);
    }
}