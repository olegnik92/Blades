using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Core;

namespace Blades.Interfaces
{
    public interface IOperationMetaInfoProvider
    {
        OperationMetaInfo Get(string operationName);
    }
}
