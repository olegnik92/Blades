using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Commands.Interfaces;

namespace Blades.Commands.Basis
{
    public class MemoryCommandsHistory: ICommandsHistory
    {
        private static readonly BlockingCollection<Command> history = new BlockingCollection<Command>();

        public void Put(Command item)
        {
            history.Add(item);
        }

        public IQueryable<Command> Query()
        {
            return history.ToList().AsQueryable();
        }

    }
}
