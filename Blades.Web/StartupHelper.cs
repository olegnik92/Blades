using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Blades.Web
{
    public static class StartupHelper
    {
        public static HttpConfiguration InitWebApiConfiguration(IAppBuilder appBuilder)
        {
            var webApiConfig = new HttpConfiguration();
            webApiConfig.Routes.MapHttpRoute(
                name: "BladesWebApiRoute",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            webApiConfig.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            webApiConfig.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
            return webApiConfig;
        }
    }
}
