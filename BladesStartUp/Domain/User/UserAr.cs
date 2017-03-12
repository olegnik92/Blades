using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Es;
using Blades.Es.Interfaces;
using Blades.Core;

namespace BladesStartUp.Domain.User
{
    [AggregateRoot(ResourceTypeId, ResourceTypeDescription)]
    public class UserAr : AggregateRoot<UserState>
        , IMutationHandler<UserState, CreateEvent>
        , IMutationHandler<UserState, ChangeEvent>
        , IMutationHandler<UserState, ResourcePermissionsMutationEvent>
    {
        public const string ResourceTypeId = "{0EDD15E0-A712-49C1-A7F2-7C400AB92349}";
        public const string ResourceTypeDescription = "Пользователь системы";

        public static Resource CreateBaseResource()
        {
            var resource = new Resource
            {
                TypeId = Guid.Parse(ResourceTypeId),
                TypeDescription = ResourceTypeDescription,
            };

            return resource;
        }

        public UserAr(IEsRepository repo, Resource resource) : base(repo, resource, 0)
        {
        }



        public UserState Apply(UserState state, CreateEvent mutation)
        {
            return new UserState
            {
                Id = mutation.UserId,
                Name = mutation.BaseInfo.Name,
                Email = mutation.BaseInfo.Email,
                Login = mutation.BaseInfo.Login,
                Password = mutation.Password,
                Roles = mutation.Roles,
                PermissionedObjects = mutation.PermissionedObjects
            };
        }

        public UserState Apply(UserState state, ChangeEvent mutation)
        {
            state.Name = mutation.BaseInfo.Name;
            state.Email = mutation.BaseInfo.Email;
            state.Login = mutation.BaseInfo.Login;
            state.Password = mutation.Password;
            state.Roles = mutation.Roles;
            state.PermissionedObjects = mutation.PermissionedObjects;

            return state;
        }


        public UserState Apply(UserState state, ResourcePermissionsMutationEvent mutation)
        {
            state.ResourcePermissions = mutation.NewPermissions;
            return state;
        }
    }
}
