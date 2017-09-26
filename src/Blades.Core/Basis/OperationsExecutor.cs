using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Core;
using Blades.Interfaces;
using Blades.Extensions;
using System.Reflection;

namespace Blades.Basis
{
    public class OperationsExecutor : IOperationsExecutor
    {
        private IOperationMetaInfoProvider metaInfoProvider;
        private IOperationsHistory operationsHistory;
        private ILogger log;
        private IOperationsActivator activator;

        public OperationsExecutor(
            IOperationMetaInfoProvider metaInfoProvider, 
            IOperationsHistory operationsHistory,
            ILogger logger,
            IOperationsActivator activator)
        {
            this.metaInfoProvider = metaInfoProvider;
            this.operationsHistory = operationsHistory;
            this.log = logger;
            this.activator = activator;
        }

        public TResult Execute<TData, TResult>(Operation<TData, TResult> operation, Operation parentOperation = null, OperationExecutionReport parentOperationReport = null)
        {
            return (TResult)Execute((Operation)operation, parentOperation, parentOperationReport);
        }

        public object Execute(Operation operation, Operation parentOperation = null, OperationExecutionReport parentOperationReport = null)
        {
            try
            {
                OperationExecutionReport report;
                if (parentOperation == null)
                {
                    return ExecuteOperation(operation, out report);
                }

                operation.ParentOpearionId = parentOperation.Id;
                operation.User = parentOperation.User;

                var result = ExecuteOperation(operation, out report);
                if (report != null && parentOperationReport != null)
                {
                    parentOperationReport.SubReports.Add(report);
                }

                return result;
            }
            catch (OperationExecutionException error)
            {
                throw error;
            }
            catch (Exception error)
            {
                log.Error(error);
                throw new OperationExecutionException(new Error(error), OperationExecutionStatus.InfrastructureErrors);
            }
        }

        private object ExecuteOperation(Operation operation, out OperationExecutionReport report)
        {
            log.Info($"Begin execution of {operation.Name} {operation.Id}");
            var histItem = new OperationsHistoryItem(operation);
            List<Error> errors;

            histItem.InvokedTime = DateTime.Now;
            try
            {
                errors = operation.GetPermissionsValidationErrors();
            }
            catch (UnauthorizedAccessException error)
            {
                log.Error(error);
                throw new OperationExecutionException(new Error(error), OperationExecutionStatus.AuthorizationErrors);
            }
            catch (Exception error)
            {
                log.Error(error);
                errors = new List<Error> { new Error(error) };
            }
            histItem.PermissionsValidationVinishedTime = DateTime.Now;

            if (!errors.IsNullOrEmpty())
            {
                histItem.ExecutionStatus = OperationExecutionStatus.PermissionsValidationErrors;
                histItem.Report = new OperationExecutionReport() { Errors = errors };
                PutToHistory(operation, histItem);
                throw new OperationExecutionException(errors, OperationExecutionStatus.PermissionsValidationErrors);
            }


            try
            {
                errors = operation.GetDataValidationErrors();
            }
            catch (Exception error)
            {
                log.Error(error);
                errors = new List<Error> { new Error(error) };
            }
            histItem.DataValidationVinishedTime = DateTime.Now;

            if (!errors.IsNullOrEmpty())
            {
                histItem.ExecutionStatus = OperationExecutionStatus.DataValidationErrors;
                histItem.Report = new OperationExecutionReport() { Errors = errors };
                PutToHistory(operation, histItem);
                throw new OperationExecutionException(errors, OperationExecutionStatus.DataValidationErrors);
            }

            report = null;
            object result = null;
            try
            {
                var executeMethod = operation.GetType().GetMethod("Execute");
                var parameters = new object[] { null };
                result = executeMethod.Invoke(operation, parameters);
                report = (OperationExecutionReport)parameters[0];
            }
            catch (TargetInvocationException error)
            {
                if (error.InnerException != null)
                {
                    log.Error(error.InnerException);
                    errors = new List<Error> { new Error(error.InnerException) };
                }
            }
            catch (Exception error)
            {
                log.Error(error);
                errors = new List<Error> { new Error(error) };
            }
            histItem.ExecutionVinishedTime = DateTime.Now;

            if (report == null)
            {               
                histItem.Report = report = new OperationExecutionReport() { Errors = errors };
            }
            else
            {
                histItem.Report = report;
                if (!errors.IsNullOrEmpty())
                {
                    histItem.Report.Errors.AddRange(errors);
                }
            }


            if (!errors.IsNullOrEmpty())
            {
                histItem.ExecutionStatus = OperationExecutionStatus.ExecutionCrushErrors;
                PutToHistory(operation, histItem);
                throw new OperationExecutionException(errors, OperationExecutionStatus.ExecutionCrushErrors);
            }

            if (!histItem.Report.Errors.IsNullOrEmpty())
            {
                histItem.ExecutionStatus = OperationExecutionStatus.ExecutionProcessedErrors;
            }
            else
            {
                histItem.ExecutionStatus = OperationExecutionStatus.ExecutionWithoutErrors;
            }

            PutToHistory(operation, histItem);
            log.Info($"End execution of {operation.Name} {operation.Id}");
            return result;
        }

        private void PutToHistory(Operation operation, OperationsHistoryItem histItem)
        {
            if (operation.SaveInHistory)
            {
                operationsHistory.Put(histItem);
            }
        }
    }
}
