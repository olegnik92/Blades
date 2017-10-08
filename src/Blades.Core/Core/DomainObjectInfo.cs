using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Core
{
    public class DomainObjectInfo
    {
        public Guid TypeId { get; set; }

        public Guid InstanceId { get; set; }

        public string TypeName { get; set; }

        public string InstanceName { get; set; }

        public override string ToString()
        {
            return Guid.Empty.Equals(InstanceId) ? $"{TypeName}[{TypeId}]" 
                : $"{TypeName}[{TypeId}] --- {InstanceName}[{InstanceId}]";
        }

        public string Description()
        {
            return Guid.Empty.Equals(InstanceId) ? TypeName : $"{TypeName} --- {InstanceName}";
        }
    }
}
