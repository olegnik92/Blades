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

        public Guid ParentOperationId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public UserInfo User { get; set; }

        public DateTime InvokedTime { get; set; }

        public DateTime PermissionsValidationVinishedTime { get; set; }

        public DateTime DataValidationVinishedTime { get; set; }

        public DateTime ExecutionVinishedTime { get; set; }

        public OperationExecutionStatus ExecutionStatus { get; set; }

        public OperationExecutionReport Report { get; set; }

        public OperationsHistoryItem() { }


        public OperationsHistoryItem(Operation operation)
        {
            Id = operation.Id;
            ParentOperationId = operation.ParentOpearionId;
            Name = operation.Name;
            User = operation.User;
            Title = operation.Title;  
        }
    }
}
