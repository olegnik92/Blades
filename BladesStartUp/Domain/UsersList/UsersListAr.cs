using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Core;
using Blades.Es;
using Blades.Es.Interfaces;

namespace BladesStartUp.Domain.UsersList
{
    [AggregateRoot(ResourceTypeId, ResourceTypeDescription)]
    public class UsersListAr : AggregateRoot<UsersListState>
        , IMutationHandler<UsersListState, AddUserEvent>
        , IMutationHandler<UsersListState, ChangeUserEvent>
    {

        public const string ResourceTypeId = "{AE2F5431-ECEA-4567-B01D-0BE62A5D544B}";
        public const string ResourceInstanceId = "{BCA8627F-3D77-4C57-AECE-DC55B4632757}";
        public const string ResourceTypeDescription = "Список пользователей системы";
        public static Resource UsersListResource { get; private set; } = new Resource
        {
            TypeId = Guid.Parse(ResourceTypeId),
            InstanceId = Guid.Parse(ResourceInstanceId),
            TypeDescription = ResourceTypeDescription
        };

        public UsersListAr(IEsRepository repo) : base(repo, UsersListResource, 0)
        {
        }

        public UsersListState Apply(UsersListState state, ChangeUserEvent mutation)
        {
            state.AllUsers = state.AllUsers.Where(u => u.Id != mutation.User.Id).ToList();
            state.AllUsers.Add(mutation.User);

            state.ActiveUsers = state.ActiveUsers.Where(u => u.Id != mutation.User.Id).ToList();
            if (mutation.User.IsActive)
            {
                state.ActiveUsers.Add(mutation.User);
            }

            return state;
        }

        public UsersListState Apply(UsersListState state, AddUserEvent mutation)
        {
            state.AllUsers.Add(mutation.NewItem);
            if(mutation.NewItem.IsActive)
            {
                state.ActiveUsers.Add(mutation.NewItem);
            }
            return state;
        }
    }
}
