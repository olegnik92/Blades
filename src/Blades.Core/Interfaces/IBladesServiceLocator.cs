using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Interfaces
{
    public interface IBladesServiceLocator
    {
        T GetInstance<T>() where T : IBladesService;
    }
}
