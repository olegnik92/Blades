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

        public List<Error> Errors { get; set; }

        public OperationExecutionException(Error error, OperationExecutionStatus status)
            : base(error.Message)
        {
            Errors = new List<Error> { error };
            Status = status;
        }

        public OperationExecutionException(List<Error> errors, OperationExecutionStatus status)
            :base(string.Join("\n", errors.Select(e => $"[ {e.Message} ]")))
        {
            Errors = errors;
            Status = status;
        }
    }
}
