using Blades.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Commands.Interfaces
{
    public interface ICommandsHistory : IBladesService
    {
        void Put(Command item);

        IQueryable<Command> Query();
    }
}
