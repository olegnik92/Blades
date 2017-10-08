using Blades.Core;
using Blades.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Blades.Extensions
{
    public static class DomainObjectExtensions
    {
        public static DomainObjectInfo GetResourceInfo(this IDomainObject resource)
        {
            var resourceAttr = resource.GetType().GetTypeInfo().GetCustomAttribute<DomainObjectAttribute>();
            if(resourceAttr == null)
            {
                throw new ArgumentException($"Тип аргумента не помечен атрибутом {nameof(DomainObjectAttribute)}", "resource");
            }

            var info = new DomainObjectInfo
            {
                InstanceId = resource.Id,
                InstanceName = resource.Name,
                TypeId = resourceAttr.TypeId,
                TypeName = resourceAttr.TypeName
            };
            return info;
        } 
    }
}
