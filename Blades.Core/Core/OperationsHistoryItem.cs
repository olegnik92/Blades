using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Blades.Core
{
    public class OperationsHistoryItem
    {

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public UserInfo User { get; set; }

        public DateTime Time { get; set; }

        public OperationExecutionStatus ExecutionStatus { get; set; }

        public OperationExecutionReport Report { get; set; }

        public OperationType OperationType { get; set; }

        public OperationsHistoryItem() { }


        public OperationsHistoryItem(Operation operation)
        {
            Id = operation.Id;
            Name = operation.Name;
            User = operation.User;
            Title = operation.Title;
            OperationType = operation.Type;
            Time = DateTime.Now;         
        }
    }
}
