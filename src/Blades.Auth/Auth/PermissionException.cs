using Blades.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Auth
{
    public class PermissionException : Exception
    {
        public PermissionException(UserInfo user, PermissionRequirement reason)
            :base($"Пользователь {user.Login} не имеет прав: {reason.Requirement}, на ресурс: {reason.Recource}")
        {

        }
    }
}
