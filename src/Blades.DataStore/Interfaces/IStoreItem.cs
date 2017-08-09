using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blades.DataStore.Interfaces
{
    public interface IStoreItem
    {
        Guid Id { get; set; }
    }
}
