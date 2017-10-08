using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blades.Interfaces
{
    public interface IDomainObject
    {
        Guid Id { get; set; }

        string Name { get; set; }
    }
}
