using Blades.Es;
using Blades.Es.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Core;
using Blades.Auth;

namespace BladesStartUp.Domain.Role
{
    public class RoleAr : AggregateRoot<RoleState>
        , IMutationHandler<RoleState, CreateEvent>
        , IMutationHandler<RoleState, ResourcePermissionsMutationEvent>
    {
        public const string ResourceTypeId = "{AE5830F2-D075-4487-9344-30717A6EAA52}";

        public static Resource CreateTypeResource()
        {
            var resource = new Resource
            {
                TypeId = Guid.Parse(ResourceTypeId),
                TypeDescription = "Роль",
            };

            return resource;
        }

        public RoleAr(IEsRepository repo, Resource resource) : base(repo, resource)
        {
        }

        public RoleState Apply(RoleState state, CreateEvent mutation)
        {
            return mutation.Role;
        }

        public RoleState Apply(RoleState state, ResourcePermissionsMutationEvent mutation)
        {
            state.ResourcePermissions = mutation.NewPermissions;
            return state;
        }


        public static List<ResourceTypePermission> GetResourcePermissions(IEsRepository repo, IEnumerable<Guid> roleIds)
        {
            var dict = new Dictionary<Guid, PermissionType>();
            foreach(var roleId in roleIds)
            {
                var resource = CreateTypeResource();
                resource.InstanceId = roleId;
                var role = new RoleAr(repo, resource);

                if(role.State.ResourcePermissions == null)
                {
                    continue;
                }

                foreach(var res in role.State.ResourcePermissions)
                {
                    PermissionType permission = 0;
                    dict.TryGetValue(res.TypeId, out permission);
                    permission = permission | res.Permission;
                    dict[res.TypeId] = permission;
                }
            }

            return dict.Select(pair => new ResourceTypePermission { TypeId = pair.Key, Permission = pair.Value }).ToList();
        }

    }
}
