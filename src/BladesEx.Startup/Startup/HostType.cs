using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BladesEx.Startup
{
    [Flags]
    public enum Server
    {
        None = 0,
        Kestrel = 1,
        IIS = 2,
    }
}
