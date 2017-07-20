using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Core;

namespace Blades.Interfaces
{
    public interface IOperationsHistory: IBladesService
    {
        void Put(OperationsHistoryItem item);

        IQueryable<OperationsHistoryItem> Query();
    }
}
