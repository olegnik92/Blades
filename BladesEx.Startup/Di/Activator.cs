using Blades.Auth.Basis;
using Blades.Auth.Interfaces;
using Blades.Basis;
using Blades.Commands.Basis;
using Blades.Commands.Interfaces;
using Blades.Core;
using Blades.DataStore.Basis;
using Blades.DataStore.Basis.Es;
using Blades.DataStore.Interfaces;
using Blades.Interfaces;
using Blades.Web;
using Blades.Web.Basis;
using Blades.Web.Interfaces;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace BladesEx.Startup.Di
{
    public class Activator : IOperationsActivator, ICommandReceiverActivator, IBladesServiceLocator, IHttpControllerActivator
    {
        public IWindsorContainer Container { get; private set; }

        public Activator(ApplicationInfo appInfo)
        {
            Container = new WindsorContainer();

            RegisterBladesServices();
            RegisterTransientTypes(typeof(Operation), typeof(ICommandReceiver), typeof(IHttpController));

            appInfo.DataBase = appInfo.DataBase ?? GetDataBaseConfig();
            if (appInfo.DataBase != null)
            {
                RegisterDatabase(appInfo.DataBase);
            }
        }

        Operation IOperationsActivator.Create(Type operationType)
        {
            return (Operation)Container.Resolve(operationType);
        }

        ICommandReceiver ICommandReceiverActivator.Create(Type receiverType)
        {
            return (ICommandReceiver)Container.Resolve(receiverType);
        }

        T IBladesServiceLocator.GetInstance<T>()
        {
            return Container.Resolve<T>();
        }

        IHttpController IHttpControllerActivator.Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var controller = (IHttpController)Container.Resolve(controllerType);
            request.RegisterForDispose(new Release(() => Container.Release(controller)));
            return controller;
        }




        private void RegisterBladesServices()
        {
            Container.Register(Component.For<ITypeIdMap>().ImplementedBy<TypeIdMap>());
            Container.Register(Component.For<ILogger>().ImplementedBy<ConsoleLogger>());
            Container.Register(Component.For<IOperationMetaInfoProvider>().ImplementedBy<OperationMetaInfoProvider>());
            Container.Register(Component.For<IOperationsExecutor>().ImplementedBy<OperationsExecutor>());
            Container.Register(Component.For<IUsersNotifier>().ImplementedBy<UsersNotifier>());
            Container.Register(Component.For<ICommandEmitter>().ImplementedBy<CommandEmiter>());

            Container.Register(Component.For<IDataConverter>().ImplementedBy<DataConverter>());


            Container.Register(Component.For(new[] { typeof(IOperationsHistory), typeof(ICommandsHistory) }).ImplementedBy<HistoryRepository>());
            Container.Register(Component.For(new[] { typeof(IOperationsActivator), typeof(ICommandReceiverActivator), typeof(IBladesServiceLocator) }).Instance(this));

            Container.Register(Component.For(new[] { typeof(IAuthManager), typeof(IPermissionRequirementChecker) }).ImplementedBy<BasisForTests>());
        }


        private void RegisterDatabase(DataBaseConfig config)
        {
            var dbClient = new MongoClient(config.ConnectionString);
            var db = dbClient.GetDatabase(config.Name);

            Container.Register(Component.For<IMongoDatabase>().Instance(db));
            Container.Register(Component.For<ITransactRepositoryFactory>().ImplementedBy<TransactRepositoryFactory>());
            Container.Register(Component.For<IEsRepository>().ImplementedBy<EsRepository>());
        }


        private void RegisterTransientTypes(params Type[] types)
        {
            var asms = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .OrderBy(a => a.FullName)
                .ToList();

            foreach (var asm in asms)
            {
                foreach (var type in types)
                {
                    Container.Register(
                        Classes.FromAssembly(asm)
                            .Where(t => type.IsAssignableFrom(t))
                            .If(t => !t.IsAbstract)
                            .LifestyleTransient()
                    );
                }
            }
        }


        private DataBaseConfig GetDataBaseConfig()
        {
            var configElement = ConfigurationManager.GetSection("DataBase") as DataBaseConfigElement;
            if (configElement == null)
            {
                return null;
            }

            var config = new DataBaseConfig()
            {
                ConnectionString = configElement.ConnectionString,
                Name = configElement.Name
            };

            return config;
        }


        private class Release : IDisposable
        {
            private readonly Action release;

            public Release(Action release)
            {
                this.release = release;
            }

            public void Dispose()
            {
                this.release();
            }
        }
    }
}
