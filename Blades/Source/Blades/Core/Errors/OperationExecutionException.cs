using System;
using System.Collections.Generic;
using System.Linq;

namespace Blades.Core.Errors
{
    public class OperationExecutionException : Exception
    {
        public ExecutionStatus Status { get; set; }

        public List<Error> Errors { get; set; }

        public OperationExecutionException(Error error, ExecutionStatus status)
            : base(error.Message)
        {
            Errors = new List<Error> { error };
            Status = status;
        }

        public OperationExecutionException(List<Error> errors, ExecutionStatus status)
            :base(string.Join("\n", errors.Select(e => $"[ {e.Message} ]")))
        {
            Errors = errors;
            Status = status;
        }
    }
}