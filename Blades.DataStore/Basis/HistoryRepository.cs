using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Core;
using Blades.Interfaces;
using Blades.Commands.Interfaces;
using MongoDB.Driver;
using Blades.Commands;

namespace Blades.DataStore.Basis
{
    public class HistoryRepository : IOperationsHistory, ICommandsHistory
    {
        private IMongoDatabase db;

        private string operationsHistoryCollectionName = "OperationsHistory";
        private string commandsHistoryCollectionName = "CommandsHistory";

        public HistoryRepository(IMongoDatabase db)
        {
            this.db = db;
        }

        void ICommandsHistory.Put(Command item)
        {
            db.GetCollection<Command>(commandsHistoryCollectionName).InsertOne(item);
        }

        void IOperationsHistory.Put(OperationsHistoryItem item)
        {
            if (item.OperationType == OperationType.Command
                || item.ExecutionStatus == OperationExecutionStatus.ExecutionCrushErrors
                || item.ExecutionStatus == OperationExecutionStatus.ExecutionProcessedErrors
                || item.ExecutionStatus == OperationExecutionStatus.InfrastructureErrors)
            {
                db.GetCollection<OperationsHistoryItem>(operationsHistoryCollectionName).InsertOne(item);
            }
        }

        IQueryable<Command> ICommandsHistory.Query()
        {
            return db.GetCollection<Command>(commandsHistoryCollectionName).AsQueryable();
        }

        IQueryable<OperationsHistoryItem> IOperationsHistory.Query()
        {
            return db.GetCollection<OperationsHistoryItem>(operationsHistoryCollectionName).AsQueryable();
        }
    }
}
