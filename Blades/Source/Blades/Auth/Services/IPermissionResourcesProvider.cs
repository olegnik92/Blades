using System.Collections.Generic;
using Blades.Core.Services;

namespace Blades.Auth.Services
{
    public interface IPermissionResourcesProvider: IBladesService
    {
        IEnumerable<IPermissionResource> GetAllPermissionResources();
    }
}