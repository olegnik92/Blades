using Blades.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Auth.Interfaces;
using Blades.Commands;
using Blades.Core;
using Blades.Commands.Interfaces;

namespace BladesStartUp.Operations
{
    public abstract class CommandOperation<TCommand> : PermissionedOperation<TCommand, Guid> where TCommand : Command
    {
        protected bool isAsync = false;
        protected ICommandEmitter emitter;
        public CommandOperation(ICommandEmitter emitter, IPermissionRequirementChecker checker) : base(checker)
        {
            this.emitter = emitter;
        }

        public override Guid Execute(out OperationExecutionReport executionReport)
        {
            if (isAsync)
            {
                executionReport = new OperationExecutionReport($"Асинхронное выполнение команды {Data.CommandName}");
                emitter.Emit(Data, User);
                executionReport.ReportStrings.Add($"Идентификатор команды: {Data.Id}");
            }
            else
            {
                executionReport = new OperationExecutionReport($"Синхронное выполнение команды {Data.CommandName}");
                emitter.Execute(Data, User);
                executionReport.ReportStrings.Add($"Идентификатор команды: {Data.Id}");
                executionReport.SubReports.AddRange(Data.ExecutionReports);
            }
            return Data.Id;
        }
    }
}
