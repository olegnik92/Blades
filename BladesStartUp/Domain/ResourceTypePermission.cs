using Blades.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BladesStartUp.Domain
{
    public class ResourceTypePermission
    {
        public Guid TypeId { get; set; }

        public PermissionType Permission { get; set; }

    }
}
