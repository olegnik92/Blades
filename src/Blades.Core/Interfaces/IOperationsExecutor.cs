using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Core;

namespace Blades.Interfaces
{
    public interface IOperationsExecutor: IBladesService
    {
        TResult Execute<TData, TResult>(Operation<TData, TResult> operation, Operation parentOperation = null, OperationExecutionReport parentOperationReport = null);

        object Execute(Operation operation, Operation parentOperation = null, OperationExecutionReport parentOperationReport = null);
    }
}
