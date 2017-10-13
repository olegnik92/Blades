using System;
using Blades.Core.Extensions;

namespace Blades.Core
{
    [TypeLabel("{44693434-8272-4695-926C-16A9B01CFC61}")]
    public class OperationsHistoryItem : HistoryItem
    {
        public Guid OperationId { get; set; }
        
        public Guid ParentOperationId { get; set; }

        public Guid OperationTypeId { get; set; }

        public string OperationTypeName { get; set; }
        
        public UserInfo User { get; set; }
        
        public DateTime InvokedTime { get; set; }

        public DateTime PermissionsValidationVinishedTime { get; set; }

        public DateTime DataValidationVinishedTime { get; set; }

        public DateTime ExecutionVinishedTime { get; set; }
        
        public ExecutionStatus Status { get; set; }

        public ExecutionReport Report { get; set; }
        
        public OperationsHistoryItem(Operation operation)
        {
            OperationId = operation.Id;
            ParentOperationId = operation.ParentOpearionId;
            OperationTypeId = operation.GetTypeLabelId();
            OperationTypeName = operation.GetTypeLabelName();
            User = operation.User; 
        }
    }
}