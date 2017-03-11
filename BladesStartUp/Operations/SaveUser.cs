using Blades.Core;
using Blades.DataStore.Interfaces;
using Blades.DataStore.Es;
using Blades.Es.Interfaces;
using BladesStartUp.Domain.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Auth.Interfaces;
using BladesStartUp.Domain;
using RoleAr = BladesStartUp.Domain.Role.RoleAr;


namespace BladesStartUp.Operations
{
    [Operation("SaveUser", OperationType.Command, "Сохранение Пользователя")]
    public class SaveUser : TransactOperation<UserState, UserState, EsRepository>
    {

        public SaveUser(IPermissionRequirementChecker checker, ITransactRepositoryFactory repoFactory) : base(checker, repoFactory)
        {
        }

        public override UserState Transaction(EsRepository repo, out OperationExecutionReport executionReport)
        {
            if (Guid.Empty.Equals(Data.Id))
            {
                executionReport = new OperationExecutionReport("Регистрация пользователя");
                var resource = UserAr.CreateBaseResource();
                resource.InstanceId = Guid.NewGuid();
                resource.InstanceDescription = Data.Login;
                executionReport.AssociatedResources.Add(resource);
                var mutation = new CreateEvent()
                {
                    UserId = resource.InstanceId,
                    BaseInfo = new UserBase
                    {
                        Name = Data.Name,
                        Login = Data.Login,
                        Email = Data.Email,
                        IsActive = Data.IsActive
                    },
                    Password = Data.Password,
                    Roles = Data.Roles,
                    PermissionedObjects = Data.PermissionedObjects
                };

                var user = new UserAr(repo, resource);
                user.Mutate(mutation);

                var addUserEvent = new Domain.UsersList.AddUserEvent
                {
                    NewItem = Data
                };
                var usersList = new Domain.UsersList.UsersListAr(repo);
                usersList.Mutate(addUserEvent);

                Data.Id = resource.InstanceId;
                return Data;
            }
            else
            {
                executionReport = new OperationExecutionReport("Изменение данных пользователя");
                var resource = UserAr.CreateBaseResource();
                resource.InstanceId = Data.Id;
                resource.InstanceDescription = Data.Login;
                executionReport.AssociatedResources.Add(resource);

                var mutation = new ChangeEvent
                {
                    BaseInfo = new UserBase
                    {
                        Name = Data.Name,
                        Login = Data.Login,
                        Email = Data.Email,
                        IsActive = Data.IsActive
                    },
                    Password = Data.Password,
                    Roles = Data.Roles,
                    PermissionedObjects = Data.PermissionedObjects
                };

                var user = new UserAr(repo, resource);
                user.Mutate(mutation);

                var newResourcePermissions = RoleAr.GetResourcePermissions(repo, Data.Roles.Select(r => r.Id));
                var premissionMutation = new ResourcePermissionsMutationEvent() { NewPermissions = newResourcePermissions };
                user.Mutate(premissionMutation);

                var changeUserEvent = new Domain.UsersList.ChangeUserEvent()
                {
                    User = Data
                };
                var usersList = new Domain.UsersList.UsersListAr(repo);
                usersList.Mutate(changeUserEvent);

                return Data;
            }
        }

    }
}
