using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Core
{
    public class OperationExecutionException : Exception
    {
        public OperationExecutionStatus Status { get; set; }

        public List<string> Errors { get; set; }

        public OperationExecutionException(string message, OperationExecutionStatus status)
            : base(message)
        {
            Errors = new List<string> { message };
            Status = status;
        }

        public OperationExecutionException(List<string> errors, OperationExecutionStatus status)
            :base(string.Join("\n", errors.Select(e => $"[ {e} ]")))
        {
            Errors = errors;
            Status = status;
        }
    }
}
