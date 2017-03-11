using Blades.Es;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BladesStartUp.Domain.User
{
    public class ChangeEvent : MutationEvent
    {
        public UserBase BaseInfo { get; set; }

        public string Password { get; set; }

        public List<Guid> PermissionedObjects { get; set; }

        public List<RoleBase> Roles { get; set; }
    }
}
