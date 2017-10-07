using Blades.Core;
using Blades.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Blades.Core.System;

namespace Blades.Basis
{
    public class TypeIdMap : ITypeIdMap
    {
        private Dictionary<Guid, Type> typesMap;

        public TypeIdMap()
        {
            typesMap = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetTypeInfo().GetCustomAttributes<TypeIdAttribute>(false).Count() > 0)
                .ToDictionary(t => Get(t), t => t);
        }


        public Guid Get(Type type)
        {
            var typeIdAttr = type.GetTypeInfo().GetCustomAttribute<TypeIdAttribute>(false);
            if(typeIdAttr == null)
            {
                return Guid.Empty;
            }

            return typeIdAttr.TypeId;
        }

        public Type Get(Guid id)
        {
            Type type = null;
            typesMap.TryGetValue(id, out type);
            return type;
        }

    }
}
