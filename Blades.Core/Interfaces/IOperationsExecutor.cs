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
        TResult Execute<TData, TResult>(string operationName, TData data, UserInfo user);

        TResult Execute<TData, TResult>(Operation<TData, TResult> operation);

        TResult Execute<TData, TResult>(Operation<TData, TResult> operation, Operation parentOperation, OperationExecutionReport parentOperationReport);

        object Execute(string operationName, object data, UserInfo user);

        object Execute(Operation operation);
    }
}
