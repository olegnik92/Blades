using Blades.Core;
using Blades.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Auth.Interfaces
{
    public interface IPermissionRequirementChecker: IBladesService
    {
        bool HasRequirement(UserInfo user, PermissionRequirement requirement);

        bool HasRequirement(UserInfo user, PermissionType permission, Resource resource);

        bool HasRequirement(UserInfo user, PermissionType permission, Guid resourceTypeId, Guid resourceInstanceId);

        bool HasRequirement(UserInfo user, PermissionType permission, Guid resourceTypeId); //resourceInstanceId = guid.Empty
    }
}
