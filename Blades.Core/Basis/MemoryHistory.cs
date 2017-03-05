using Blades.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Core;
using System.Collections.Concurrent;

namespace Blades.Basis
{
    public class MemoryHistory : IOperationsHistory
    {
        private static readonly BlockingCollection<OperationsHistoryItem> history = new BlockingCollection<OperationsHistoryItem>();

        public void Put(OperationsHistoryItem item)
        {
            history.Add(item);
        }

        public IQueryable<OperationsHistoryItem> Query()
        {
            return history.ToList().AsQueryable();
        }
    }
}
