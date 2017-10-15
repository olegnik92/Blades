using System;
using System.Collections.Generic;
using Blades.Core;

namespace Blades.Auth
{
    
    /// <summary>
    /// Набор прав пользователя
    /// </summary>
    public class PermissionsAsset
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Разрешения, выданные пользователю
        /// </summary>
        public Dictionary<Guid, PermissionType> Permissions { get; set; }
    }
}