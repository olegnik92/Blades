using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Commands.Interfaces;
using Blades.Core;
using BladesStartUp.Domain.Commands;

namespace BladesStartUp.Domain.TestEntity
{
    public class TestEntityCommandsReceiver : ICommandReceiver<SaveTestEntityCommand>
    {
        public OperationExecutionReport OnReceive(SaveTestEntityCommand command)
        {
            return new OperationExecutionReport();
        }
    }
}
