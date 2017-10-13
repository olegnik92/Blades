using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Blades.Core.Errors;
using Blades.Core.Extensions;
using Blades.Core.Services;

namespace Blades.Core.ServicesBase
{
    public class TypeMap : ITypeMap
    {
        private readonly Dictionary<Guid, Type> _typesMap = new Dictionary<Guid, Type>();
        
        public TypeMap()
        {
            var types = LoadAllAvailableTypes(null ,null);
            foreach (var type in types)
            {
                var id = type.GetTypeLabelId();
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


        public static List<Type> LoadAllAvailableTypes(Action<Assembly> successLog, Action<Assembly, Exception> logError)
        {
            //Могут возникнуть ошибки загрузки сборки.
            //Все типы из корректно загруженных сборок должны попасть в результат функции.
            var loadedTypes = new List<Type>();

            var asmbls = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asmbl in asmbls)
            {
                try
                {
                    loadedTypes.AddRange(asmbl.GetTypes());
                    successLog?.Invoke(asmbl);
                }
                catch (Exception e)
                {
                    logError?.Invoke(asmbl, e);
                }
            }

            return loadedTypes;
        }
        
        public Type Get(Guid typeId)
        {
            return _typesMap[typeId];
        }


        public IEnumerable<Type> GetAll()
        {
            return _typesMap.Values;
        }
    }
}