﻿using Blades.Es;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BladesStartUp.Domain.Role
{
    public class RoleState : RoleBase
    {
        public List<ResourceTypePermission> ResourcePermissions { get; set; }
    }
}
