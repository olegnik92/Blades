using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.FileSystems;
using System.Web.Http.Dispatcher;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Cors;
using Microsoft.AspNet.SignalR;

namespace BladesStartUp
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var di = new WindsorActivator();

            FileServerConfig(appBuilder);

            Blades.Auth.StartupHelper.OAuthConfiguration(appBuilder, di.GetInstance<Blades.Auth.Interfaces.IAuthManager>(), accessTokenExpireTimeSpan: TimeSpan.FromDays(1));


            var config = Blades.Web.StartupHelper.InitWebApiConfiguration(appBuilder);
            di.InitControllerActivator(config);
            appBuilder.UseWebApi(config);

            Blades.Web.StartupHelper.InitClientsConnection(appBuilder, di);
        }

        public void StaticFilesConfig(IAppBuilder appBuilder)
        {
            var contentFileSystem = new PhysicalFileSystem("public");
            var staticOptions = new StaticFileOptions
            {
                FileSystem = contentFileSystem
            };
            appBuilder.UseStaticFiles(staticOptions);
        }

        public void FileServerConfig(IAppBuilder appBuilder)
        {
            var physicalFileSystem = new PhysicalFileSystem(@"./public");
            var options = new FileServerOptions
            {
                EnableDefaultFiles = true,
                FileSystem = physicalFileSystem
            };
            options.StaticFileOptions.FileSystem = physicalFileSystem;
            options.StaticFileOptions.ServeUnknownFileTypes = true;
            options.DefaultFilesOptions.DefaultFileNames = new[] { "index.html" };
            appBuilder.UseFileServer(options);
        }
    }
}
