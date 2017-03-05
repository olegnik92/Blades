using Blades.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Blades.Auth.Interfaces;

namespace Blades.Auth
{
    public abstract class PermissionedOperation<TData, TResult> : Operation<TData, TResult>
    {
        protected IPermissionRequirementChecker checker;
        public PermissionedOperation(IPermissionRequirementChecker checker)
        {
            this.checker = checker;
        }

        public override List<string> GetPermissionsValidationErrors()
        {
            if(User?.IsGuest() ?? true)
            {
                throw new UnauthorizedAccessException("Пользователь не авторизован");
            }

            foreach(var req in GetPermissionRequirements())
            {
                if(!checker.HasRequirement(User, req))
                {
                    return new List<string> { $"Пользователь не имеет прав: {req.Requirement}, на ресурс: {req.Recource.Description}" };
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
