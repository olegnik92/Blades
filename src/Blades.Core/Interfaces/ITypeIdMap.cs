using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Interfaces
{
    public interface ITypeIdMap : IBladesService
    {
        Type Get(Guid id);

        Guid Get(Type type);
    }
}
