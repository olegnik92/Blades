using Blades.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Interfaces
{
    public interface IOperationsActivator: IBladesService
    {
        Operation Create(string operationName, object data, UserInfo user);
    }
}
