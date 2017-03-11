using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Core
{

    public abstract class Operation
    {
        public Guid Id { get; } = Guid.NewGuid();

        public Guid ParentOpearionId { get; set; }

        public string Name { get; protected set; }

        public string Title { get; protected set; }

        public OperationType Type { get; private set; }

        public Operation()
        {
            var opAttr = Attribute.GetCustomAttribute(this.GetType(), typeof(OperationAttribute)) as OperationAttribute;
            if (opAttr == null)
            {
                throw new Exception("Operation Attribute has not set");
            }

            Name = opAttr.Name;
            Type = opAttr.Type;
            Title = opAttr.Title;
        }

        public virtual UserInfo User { get; set; }

        public virtual List<Error> GetDataValidationErrors()
        {
            return null;
        }

        public virtual List<Error> GetPermissionsValidationErrors()
        {
            return null;
        }
    }

    public abstract class Operation<TData, TResult> : Operation
    {
        public virtual TData Data { get; set; }

        public abstract TResult Execute(out OperationExecutionReport executionReport);
    }
}
