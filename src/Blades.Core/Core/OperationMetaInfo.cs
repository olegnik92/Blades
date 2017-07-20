using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Core
{
    public class OperationMetaInfo
    {
        public string OperationName { get; set; }

        public Type ClassType { get; set; }

        public Type[] ExecuteTypes { get; set; }

        public Type DataType
        {
            get
            {
                return (ExecuteTypes?.Length ?? 0) > 0 ? ExecuteTypes[0] : null;
            }
        }

        public Type ResultType
        {
            get
            {
                return (ExecuteTypes?.Length ?? 0) > 1 ? ExecuteTypes[1] : null;
            }
        }
    }
}
