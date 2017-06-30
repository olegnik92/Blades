using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Owin.WebSocket;
using Owin.WebSocket.Extensions;
using Microsoft.Practices.ServiceLocation;
using Blades.Interfaces;
using Blades.Web.Interfaces;

namespace Blades.Web
{
    public static class StartupHelper
    {
        public static HttpConfiguration InitWebApiConfiguration(IAppBuilder appBuilder, IBladesServiceLocator locator)
        {
            var converter = locator.GetInstance<IDataConverter>();
            var webApiConfig = new HttpConfiguration();
            webApiConfig.Routes.MapHttpRoute(
                name: "BladesWebApiRoute",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            webApiConfig.Formatters.JsonFormatter.SerializerSettings = converter.GetSerializerSettings();
            webApiConfig.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;

            return webApiConfig;
        }



        public static void InitClientsConnection(IAppBuilder appBuilder, IBladesServiceLocator locator)
        {
            appBuilder.MapWebSocketRoute<ClientConnection>("/ws", new WebSocketServiceLocator(locator));
        }
    }


    internal class WebSocketServiceLocator : ServiceLocatorImplBase
    {
        private IBladesServiceLocator locator;
        public WebSocketServiceLocator(IBladesServiceLocator locator)
        {
            this.locator = locator;
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return new object[] { DoGetInstance(serviceType, null) };
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (serviceType.Equals(typeof(ClientConnection)))
            {
                return new ClientConnection(locator.GetInstance<IDataConverter>(), locator.GetInstance<IOperationsExecutor>(), locator.GetInstance<ILogger>());
            }
            throw new ArithmeticException("Unknown service Type in WebSocketServiceLocator");
        }
    }
}
