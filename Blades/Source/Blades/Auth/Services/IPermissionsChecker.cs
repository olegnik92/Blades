using Blades.Core;
using Blades.Core.Services;

namespace Blades.Auth.Services
{
    public interface IPermissionsChecker: IBladesService
    {
        bool IsUserHasRequiredPermissions(UserInfo user, PermissionRequirement requirement);
        
        bool IsUserHasRequiredPermissions(UserInfo user, PermissionType demandAction, DomainObjectInfo resource);
    }
}