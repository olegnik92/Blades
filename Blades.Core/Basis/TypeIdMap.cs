using Blades.Core;
using Blades.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Basis
{
    public class TypeIdMap : ITypeIdMap
    {
        private Dictionary<Guid, Type> typesMap;

        public TypeIdMap()
        {
            typesMap = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetCustomAttributes(typeof(TypeIdAttribute), false).Length > 0)
                .ToDictionary(t => Get(t), t => t);
        }


        public Guid Get(Type type)
        {
            var typeIdAttr = Attribute.GetCustomAttribute(type, typeof(TypeIdAttribute)) as TypeIdAttribute;
            if(typeIdAttr == null)
            {
                return Guid.Empty;
            }

            return typeIdAttr.ClassTypeId;
        }

        public Type Get(Guid id)
        {
            Type type = null;
            typesMap.TryGetValue(id, out type);
            return type;
        }

    }
}
