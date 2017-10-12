using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Blades.Core.Errors;
using Blades.Core.Services;

namespace Blades.Core.ServicesBase
{
    public class TypeMap : ITypeMap
    {
        private readonly Dictionary<Guid, Type> _typesMap = new Dictionary<Guid, Type>();
        
        public TypeMap()
        {
            var types = GetAllLoadedTypes();
            foreach (var type in types)
            {
                var id = Get(type);
                if (id.Equals(Guid.Empty))
                {
                    continue;
                }
                
                if (_typesMap.ContainsKey(id))
                {
                    throw new TypeIdDuplicatedException(id);
                }

                _typesMap[id] = type;
            }
        }


        public static IEnumerable<Type> GetAllLoadedTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes());
        }
        
        public Type Get(Guid typeId)
        {
            return _typesMap[typeId];
        }

        public Guid Get(Type type)
        {
            return type?.GetTypeInfo().GetCustomAttribute<TypeIdAttribute>()?.Id ?? Guid.Empty;
        }

        public IEnumerable<Type> GetAllTypes()
        {
            return _typesMap.Values;
        }
    }
}