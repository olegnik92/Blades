using System;
using System.Reflection;
using Blades.Core.Errors;

namespace Blades.Core.Extensions
{
    public static class DomainObjectExtensions
    {
        public static DomainObjectInfo GetResourceInfo(this IDomainObject resource)
        {
            var resourceAttr = resource.GetType().GetTypeInfo().GetCustomAttribute<DomainObjectAttribute>();
            if(resourceAttr == null)
            {
                throw new AttributeNotFountException(typeof(DomainObjectAttribute), resource);
            }

            var info = new DomainObjectInfo
            {
                Id = resource.Id,
                Name = resource.Name,
                TypeId = resourceAttr.Id,
                TypeName = resourceAttr.Name
            };
            return info;
        } 
    }
}