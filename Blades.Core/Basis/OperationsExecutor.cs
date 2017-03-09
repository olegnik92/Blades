﻿using System;
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
            log = logger;
            this.activator = activator;
        }

        public object Execute(string operationName, object data, UserInfo user)
        {
            try
            {
                var meta = metaInfoProvider.Get(operationName);
                var operationInstance = activator.Create(meta.ClassType);
                operationInstance.User = user;
                var dataProperty = meta.ClassType.GetProperty("Data");
                dataProperty.SetValue(operationInstance, data);

                return ExecuteOperation(operationInstance, meta);
            }
            catch(OperationExecutionException error)
            {
                throw error;
            }
            catch(Exception error)
            {
                log.Error(error);
                throw new OperationExecutionException(error.Message, OperationExecutionStatus.InfrastructureErrors);
            }
        }


        public object Execute(Operation operation)
        {
            return ExecuteOperation(operation);
        }

        public TResult Execute<TData, TResult>(string operationName, TData data, UserInfo user)
        {
            try
            {
                return (TResult)Execute(operationName, data, user);
            }
            catch (OperationExecutionException error)
            {
                throw error;
            }
            catch (Exception error)
            {
                log.Error(error);
                throw new OperationExecutionException(error.Message, OperationExecutionStatus.InfrastructureErrors);
            }          
        }

        public TResult Execute<TData, TResult>(Operation<TData, TResult> operation)
        {
            try
            {
                return (TResult)ExecuteOperation(operation);
            }
            catch (OperationExecutionException error)
            {
                throw error;
            }
            catch (Exception error)
            {
                log.Error(error);
                throw new OperationExecutionException(error.Message, OperationExecutionStatus.InfrastructureErrors);
            }           
        }

        private object ExecuteOperation(Operation operation, OperationMetaInfo metaInfo = null)
        {
            log.Info($"Begin execution of {operation.Name} {operation.Id}");
            if(metaInfo == null)
            {
                metaInfo = metaInfoProvider.Get(operation.Name);
            }

            var histItem = new OperationsHistoryItem(operation);
            List<string> errors;             
                             
            try
            {
                errors = operation.GetPermissionsValidationErrors();
            }
            catch(UnauthorizedAccessException error)
            {
                log.Error(error);
                throw new OperationExecutionException(error.Message, OperationExecutionStatus.AuthorizationErrors);
            }
            catch(Exception error)
            {
                log.Error(error);
                errors = new List<string> { error.Message };
            }
            if (!errors.IsNullOrEmpty())
            {
                histItem.ExecutionStatus = OperationExecutionStatus.PermissionsValidationErrors;
                histItem.Report = new OperationExecutionReport() { Errors = errors };
                operationsHistory.Put(histItem);
                throw new OperationExecutionException(errors, OperationExecutionStatus.PermissionsValidationErrors);
            }


            try
            {
                errors = operation.GetDataValidationErrors();
            }
            catch (Exception error)
            {
                log.Error(error);
                errors = new List<string> { error.Message };
            }
            if (!errors.IsNullOrEmpty())
            {
                histItem.ExecutionStatus = OperationExecutionStatus.DataValidationErrors;
                histItem.Report = new OperationExecutionReport() { Errors = errors };
                operationsHistory.Put(histItem);
                throw new OperationExecutionException(errors, OperationExecutionStatus.DataValidationErrors);
            }

            OperationExecutionReport report = null;
            object result = null;
            try
            {
                var executeMethod = metaInfo.ClassType.GetMethod("Execute");
                var parameters = new object[] { null };
                result = executeMethod.Invoke(operation, parameters);
                report = (OperationExecutionReport)parameters[0];
            }
            catch(TargetInvocationException error)
            {
                if(error.InnerException != null)
                {
                    log.Error(error.InnerException);
                    errors = new List<string> { error.InnerException.Message };
                }
            }
            catch (Exception error)
            {
                log.Error(error);
                errors = new List<string> { error.Message };
            }

            if(report == null)
            {
                histItem.Report = new OperationExecutionReport() { Errors = errors };
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
                operationsHistory.Put(histItem);
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

            operationsHistory.Put(histItem);
            log.Info($"End execution of {operation.Name} {operation.Id}");
            return result;
        }

    }
}