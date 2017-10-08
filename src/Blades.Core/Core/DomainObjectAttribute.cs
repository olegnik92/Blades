using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blades.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class DomainObjectAttribute : TypeIdAttribute
    {
        public DomainObjectAttribute(string typeId, string typeName) : base(typeId)
        {
            TypeName = typeName;
        }


        public string TypeName { get; private set; }
    }
}
