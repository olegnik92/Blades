using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Commands.Interfaces;

namespace Blades.Commands.Basis
{
    public class CommandReceiverActivator: ICommandReceiverActivator
    {
        public ICommandReceiver Create(Type receiverType)
        {
            return (ICommandReceiver)Activator.CreateInstance(receiverType);
        }
    }
}
