using Blades.Core;
using Blades.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Commands.Interfaces
{
    public interface ICommandEmitter : IBladesService
    {
        void Execute(Command command, UserInfo user);

        Task Emit(Command command, UserInfo user);
    }
}
