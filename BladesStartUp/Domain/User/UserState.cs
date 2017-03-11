using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Es;

namespace BladesStartUp.Domain.User
{
    public class UserState : UserBase
    {
        public string Password { get; set; }

        public List<RoleBase> Roles { get; set; }

        public List<ResourceTypePermission> ResourcePermissions { get; set; }

        public List<Guid> PermissionedObjects { get; set; }
    }
}
