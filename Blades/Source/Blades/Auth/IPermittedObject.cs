using Blades.Core;

namespace Blades.Auth
{   
    public interface IPermittedObject: IDomainObject
    {       
        bool IsUserHasRequiredPermissions(UserInfo user, PermissionsAsset permissionsAsset, PermissionType demandAction);
    }
}