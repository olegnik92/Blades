using System;
using System.Collections.Generic;
using System.Linq;
using Blades.Auth.Errors;
using Blades.Auth.Services;
using Blades.Core;
using Blades.Core.Extensions;

namespace Blades.Auth
{
    public abstract class PermissionedOperation<TData, TResult> : Operation<TData, TResult>
        , IPermissionResource
    {

        Guid IPermissionResource.Id => this.GetTypeLabelId();

        string IPermissionResource.Name => this.GetTypeLabelName();

        Guid IPermissionResource.ParentResourceId => Guid.Empty;

        PermissionType IPermissionResource.ActualPermissions => PermissionType.Execute;
 
        Dictionary<Guid, PermissionType> IPermissionResource.GetAssociatedPermissions(PermissionType userPermissions)
        {
            return null;
        }
        
        protected IPermissionsChecker _checker;

        protected PermissionedOperation(IPermissionsChecker checker)
        {
            _checker = checker;
        }
        
        public override List<Error> GetPermissionsValidationErrors()
        {
            if(User?.IsGuest() ?? true)
            {
                throw new UnauthorizedAccessException();
            }

            foreach(var req in GetPermissionRequirements())
            {
                if(!_checker.IsUserHasRequiredPermissions(User, req))
                {
                    return new List<Error> { new Error(new PermissionException(User, req)) };
                }
            }

            return null;
        }
        
        protected virtual IEnumerable<PermissionRequirement> GetPermissionRequirements()
        {
            return Enumerable.Range(0, 0).Select(i => new PermissionRequirement());
        }
    }
}