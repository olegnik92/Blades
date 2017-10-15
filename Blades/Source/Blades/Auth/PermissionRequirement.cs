using System.Collections.Generic;
using System.Linq;
using Blades.Core;
using Blades.Core.Extensions;

namespace Blades.Auth
{
    public class PermissionRequirement
    {
        public PermissionType DemandAction { get; }
        
        public DomainObjectInfo ResourcesTypeInfo { get; }

        public bool IsResourcesCollection { get; }

        public IDomainObject Resource { get; }

        public IEnumerable<IDomainObject> Resources { get; }


        public PermissionRequirement()
        {            
            IsResourcesCollection = false;
            DemandAction = 0;
            Resources = null;
            Resource = null;
            ResourcesTypeInfo = null;            
        }
        
        public PermissionRequirement(PermissionType demandAction, IDomainObject resource)
        {
            IsResourcesCollection = false;
            DemandAction = demandAction;
            Resource = resource;
            ResourcesTypeInfo = Resource?.GetResourceInfo();
        }

        public PermissionRequirement(PermissionType demandAction, IEnumerable<IDomainObject> resources)
        {
            IsResourcesCollection = true;
            DemandAction = demandAction;
            Resources = resources;
            Resource = Resources?.FirstOrDefault();
            ResourcesTypeInfo = Resource?.GetResourceInfo();
        }

        public string GetResourceDescription()
        {
            if (ResourcesTypeInfo == null)
            {
                return "Неопределенный ресурс";
            }
            
            if (IsResourcesCollection)
            {
                return $"Коллекция объектов [{ResourcesTypeInfo.TypeId} - {ResourcesTypeInfo.TypeName}]";
            }

            return $"{Resource.Id} - {Resource.Name}";
        }

        public bool IsEmpty => ResourcesTypeInfo == null;
    }
}