using System;
using System.Collections.Generic;
using Blades.Core.Services;

namespace Blades.Auth.Services
{
    public interface IUserPermissionsProvider: IBladesService
    {
        IEnumerable<PermissionsAsset> GetUserPermissions(Guid userId);
    }
}