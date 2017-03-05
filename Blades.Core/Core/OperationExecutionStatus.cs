using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Core
{
    public enum OperationExecutionStatus
    {
        InfrastructureErrors,
        AuthorizationErrors,
        PermissionsValidationErrors,
        DataValidationErrors,
        ExecutionCrushErrors,
        ExecutionProcessedErrors,
        ExecutionWithoutErrors
    }
}
