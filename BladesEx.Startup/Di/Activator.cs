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
        public static Dictionary<string, DiRule> SingletonServicesRules { get; private set; } = CreateDefaultSingletonServicesRules();

        private static Dictionary<string, DiRule> CreateDefaultSingletonServicesRules()
        {
            var rules = new List<DiRule>()
            {
                new DiRule { Service = typeof(ITypeIdMap), Realization = typeof(TypeIdMap)  },
                new DiRule { Service = typeof(ILogger), Realization = typeof(ConsoleLogger)  },
                new DiRule { Service = typeof(IOperationMetaInfoProvider), Realization = typeof(OperationMetaInfoProvider)  },
                new DiRule { Service = typeof(IOperationsExecutor), Realization = typeof(OperationsExecutor)  },
                new DiRule { Service = typeof(IUsersNotifier), Realization = typeof(UsersNotifier)  },
                new DiRule { Service = typeof(ICommandEmitter), Realization = typeof(CommandEmiter)  },
                new DiRule { Service = typeof(IDataConverter), Realization = typeof(DataConverter)  },
                new DiRule { Service = typeof(IOperationsHistory), Realization = typeof(HistoryRepository)  },
                new DiRule { Service = typeof(ICommandsHistory), Realization = typeof(HistoryRepository)  },
                new DiRule { Service = typeof(IAuthManager), Realization = typeof(BasisForTests)  },
                new DiRule { Service = typeof(IPermissionRequirementChecker), Realization = typeof(BasisForTests)  },

                //Data storagte (works with mongodb) 
                new DiRule { Service = typeof(ITransactRepositoryFactory), Realization = typeof(TransactRepositoryFactory)  },
                new DiRule { Service = typeof(IEsRepository), Realization = typeof(EsRepository)  },
                new DiRule { Service = typeof(ICollectionsRepository), Realization = typeof(SimpleCollectionsRepository)  },
            };

            return rules.ToDictionary(r => r.Service.FullName, r => r);
        }


        public IWindsorContainer Container { get; private set; }

        public Activator(ApplicationInfo appInfo)
        {
            Container = new WindsorContainer();

            ReristerOwnRoles();
            RegisterSingletonServices();
            RegisterTransientTypes(typeof(Operation), typeof(ICommandReceiver), typeof(IHttpController));

            appInfo.DataBase = appInfo.DataBase ?? GetDataBaseConfig();
            if (appInfo.DataBase != null)
            {
                RegisterDatabase(appInfo.DataBase);
            }
        }

        private void ReristerOwnRoles()
        {
            Container.Register(Component.For(new[] { typeof(IOperationsActivator), typeof(ICommandReceiverActivator), typeof(IBladesServiceLocator) }).Instance(this));
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


        private void RegisterSingletonServices()
        {
            foreach(var rule in SingletonServicesRules.Values)
            {
                Container.Register(Component.For(rule.Service).Named(rule.Service.FullName).ImplementedBy(rule.Realization));
            }
        }


        private void RegisterDatabase(DataBaseConfig config)
        {
            if(config.Driver != DataBaseConfig.MongoDriver)
            {
                throw new NotSupportedException("По умолчанию поддерживается только MongoDB");
            }
            var dbClient = new MongoClient(config.ConnectionString);
            var db = dbClient.GetDatabase(config.Name);

            Container.Register(Component.For<IMongoDatabase>().Instance(db));
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


        public class DiRule
        {
            public Type Service { get; set; }

            public Type Realization { get; set; }
        }
    }
}
