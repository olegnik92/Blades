using Blades.Es;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BladesStartUp.Domain.User
{
    public class ResourcePermissionsMutationEvent: MutationEvent
    {
        public List<ResourceTypePermission> NewPermissions { get; set; }
    }
}
