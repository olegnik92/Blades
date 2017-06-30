using Blades.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Web.Basis
{
    public class TypeIdBinder : SerializationBinder
    {
        private const string UsedAssemblyName = "guid";

        private ITypeIdMap map;
        public TypeIdBinder(ITypeIdMap map)
        {
            this.map = map;
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            if (UsedAssemblyName.Equals(assemblyName))
            {
                return map.Get(Guid.Parse(typeName));
            }

            throw new ArgumentException($"TypeIdBinder: assemblyName is '{assemblyName}' but should be '{UsedAssemblyName}'");
        }

        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            var guid = map.Get(serializedType);
            if (guid.Equals(Guid.Empty))
            {
                assemblyName = null;
                typeName = null;
            }
            else
            {
                assemblyName = UsedAssemblyName;
                typeName = guid.ToString();
            }
        }
    }
}
