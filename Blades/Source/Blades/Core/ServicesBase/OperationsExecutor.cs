using System;
using System.Collections.Generic;
using System.Reflection;
using Blades.Core.Errors;
using Blades.Core.Extensions;
using Blades.Core.Services;

namespace Blades.Core.ServicesBase
{
    public class OperationsExecutor: IOperationsExecutor
    {

        private readonly ILogger _log;
        private readonly IHistory _history;
        public OperationsExecutor(ILogger log, IHistory history)
        {
            _log = log;
            _history = history;
        }
        
        
        public (TResult Result, ExecutionReport Report) Execute<TData, TResult>(Operation<TData, TResult> operation, Operation parentOperation = null)
        {
            var rawResult = Execute((Operation)operation, parentOperation);
            return ((TResult) rawResult.Result, rawResult.Report);
        }

        public (object Result, ExecutionReport Report) Execute(Operation operation, Operation parentOperation = null)
        {
            try
            {
                if (parentOperation != null)
                {
                    operation.ParentOpearionId = parentOperation.Id;
                    operation.User = parentOperation.User;
                }
                
                return ExecuteOperation(operation);
            }
            catch (OperationExecutionException)
            {
                throw;
            }
            catch (Exception error)
            {
                _log.Error(error);
                throw new OperationExecutionException(new Error(error), ExecutionStatus.InfrastructureErrors);
            }
        }


        private (object result, ExecutionReport report) ExecuteOperation(Operation operation)
        {
            _log.Info($"Begin execution of operation. Id: {operation.Id} TypeId: {operation.GetTypeLabelId()}");
            var histItem = new OperationsHistoryItem(operation);
            List<Error> errors;
            
            
            
            histItem.InvokedTime = DateTime.Now;
            try
            {
                errors = operation.GetPermissionsValidationErrors();
            }
            catch (UnauthorizedAccessException error)
            {
                _log.Error(error);
                throw new OperationExecutionException(new Error(error), ExecutionStatus.AuthorizationErrors);
            }
            catch (Exception error)
            {
                _log.Error(error);
                errors = new List<Error> { new Error(error) };
            }
            histItem.PermissionsValidationVinishedTime = DateTime.Now;
                     
            if (!errors.IsNullOrEmpty())
            {
                histItem.Status = ExecutionStatus.PermissionsValidationErrors;
                histItem.Report = new ExecutionReport() { Errors = errors };
                PutToHistory(operation, histItem);
                throw new OperationExecutionException(errors, ExecutionStatus.PermissionsValidationErrors);
            }
            
            
            
            try
            {
                errors = operation.GetDataValidationErrors();
            }
            catch (Exception error)
            {
                _log.Error(error);
                errors = new List<Error> { new Error(error) };
            }
            histItem.DataValidationVinishedTime = DateTime.Now;

            if (!errors.IsNullOrEmpty())
            {
                histItem.Status = ExecutionStatus.DataValidationErrors;
                histItem.Report = new ExecutionReport() { Errors = errors };
                PutToHistory(operation, histItem);
                throw new OperationExecutionException(errors, ExecutionStatus.DataValidationErrors);
            }
            
            
            
            object result = null;
            ExecutionReport report = null;
            try
            {
                var executeMethod = operation.GetType().GetTypeInfo().GetMethod("Execute");
                var tuple = executeMethod.Invoke(operation, null);
                (result, report) = MakeResultTuple(tuple);
            }
            catch (TargetInvocationException error)
            {
                if (error.InnerException != null)
                {
                    _log.Error(error.InnerException);
                    errors = new List<Error> { new Error(error.InnerException) };
                }
            }
            catch (Exception error)
            {
                _log.Error(error);
                errors = new List<Error> { new Error(error) };
            }
            histItem.ExecutionVinishedTime = DateTime.Now;
            
            if (report == null)
            {               
                report = new ExecutionReport();
            }
            
            histItem.Report = report;
            if (!errors.IsNullOrEmpty())
            {
                histItem.Report.Errors.AddRange(errors);
                histItem.Status = ExecutionStatus.ExecutionCrushErrors;
                PutToHistory(operation, histItem);
                throw new OperationExecutionException(errors, ExecutionStatus.ExecutionCrushErrors);
            }
                      
            if (!histItem.Report.Errors.IsNullOrEmpty())
            {
                histItem.Status = ExecutionStatus.ExecutionProcessedErrors;
            }
            else
            {
                histItem.Status = ExecutionStatus.ExecutionWithoutErrors;
            }
            
            PutToHistory(operation, histItem);
                        
            _log.Info($"End execution of operation. Id: {operation.Id} TypeId: {operation.GetTypeLabelId()}");
            return (result, report);
        }


        private (object Result, ExecutionReport Report) MakeResultTuple(object result)
        {
            var typeInfo = result.GetType().GetTypeInfo();
            var resultField = typeInfo.GetField("Item1");
            var reportField = typeInfo.GetField("Item2");

            return (resultField.GetValue(result), (ExecutionReport) reportField.GetValue(result));
        }
        
        private void PutToHistory(Operation operation, OperationsHistoryItem histItem)
        {
            histItem.RecordDate = DateTime.Now;
            if (operation.SaveInHistory)
            {
                _history.Put(histItem);
            }
        }
    }
}