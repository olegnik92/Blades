﻿using Blades.Basis;
using Blades.Interfaces;
using Blades.Web;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Blades.Auth.Interfaces;
using Blades.Auth.Basis;
using Blades.Core;

namespace BladesStartUp
{
    public class WindsorActivator: IOperationsActivator
    {
        public static IPermissionRequirementChecker PermissionsChecker { get; private set; }

        public IWindsorContainer Container { get; private set; }

        public WindsorActivator()
        {
            Container = new WindsorContainer();
            RegisterTypes(Container);
            PermissionsChecker = Container.Resolve<IPermissionRequirementChecker>();
        }

        private void RegisterTypes(IWindsorContainer container)
        {
            container.Register(Component.For<ILogger>().ImplementedBy<ConsoleLogger>());
            container.Register(Component.For<IOperationsHistory>().ImplementedBy<MemoryHistory>());
            container.Register(Component.For<IOperationMetaInfoProvider>().ImplementedBy<OperationMetaInfoProvider>());
            container.Register(Component.For<IOperationsExecutor>().ImplementedBy<OperationsExecutor>());

            container.Register(Component.For<IOperationsActivator>().ImplementedBy<WindsorActivator>());
            RegisterOperationTypes(container);


            container.Register(Component.For(new[] { typeof(IAuthManager), typeof(IPermissionRequirementChecker) }).ImplementedBy<BasisForTests>());

            container.Register(Component.For<OperationController>().ImplementedBy<OperationController>().LifeStyle.Transient);
        }


        private void RegisterOperationTypes(IWindsorContainer container)
        {
            var asms = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .OrderBy(a => a.FullName)
                .ToList();

            asms.ForEach(a =>
            {
                container.Register(
                    Classes.FromAssembly(a)
                    .BasedOn<Operation>()
                    .If(type => !type.IsAbstract)
                    .LifestyleTransient()
                );
            });
        }

        public void InitControllerActivator(HttpConfiguration config)
        {
            config.Services.Replace(typeof(IHttpControllerActivator), new WindsorCompositionRoot(Container));
        }

        public Operation Create(Type operationType)
        {
            return (Operation)Container.Resolve(operationType);
        }
    }

    public class WindsorCompositionRoot : IHttpControllerActivator
    {
        private readonly IWindsorContainer container;

        public WindsorCompositionRoot(IWindsorContainer container)
        {
            this.container = container;
        }

        public IHttpController Create(
            HttpRequestMessage request,
            HttpControllerDescriptor controllerDescriptor,
            Type controllerType)
        {
            var controller =
                (IHttpController)this.container.Resolve(controllerType);

            request.RegisterForDispose(
                new Release(
                    () => this.container.Release(controller)));

            return controller;
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
