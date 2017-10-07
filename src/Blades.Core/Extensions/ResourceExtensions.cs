using Blades.Core;
using Blades.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Blades.Extensions
{
    public static class ResourceExtensions
    {
        public static ResourceInfo GetResourceInfo(this IResource resource)
        {
            var resourceAttr = resource.GetType().GetTypeInfo().GetCustomAttribute<ResourceTypeAttribute>();
            if(resourceAttr == null)
            {
                throw new ArgumentException($"Тип аргумента не помечен атрибутом {nameof(ResourceTypeAttribute)}", "resource");
            }

            var info = new ResourceInfo
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
