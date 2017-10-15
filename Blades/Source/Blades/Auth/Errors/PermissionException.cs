using System;
using Blades.Core;

namespace Blades.Auth.Errors
{
    public class PermissionException : Exception
    {
        public PermissionException(UserInfo user, PermissionRequirement requirement)
            :base($"Пользователь {user.Login} не имеет прав: {requirement.DemandAction}, на ресурс: [{requirement.GetResourceDescription()}]")
        {

        }
    }
}