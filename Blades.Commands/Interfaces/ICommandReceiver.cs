using Blades.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Commands.Interfaces
{
    public interface ICommandReceiver
    {

    }

    public interface ICommandReceiver<TCommand> : ICommandReceiver where TCommand: Command
    {
        OperationExecutionReport OnReceive(TCommand command);
    }
}
