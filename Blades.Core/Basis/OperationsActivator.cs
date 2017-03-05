using Blades.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Core;

namespace Blades.Basis
{
    public class OperationsActivator : IOperationsActivator
    {
        public Operation Create(Type operationType)
        {
            return (Operation)Activator.CreateInstance(operationType);
        }
    }
}
