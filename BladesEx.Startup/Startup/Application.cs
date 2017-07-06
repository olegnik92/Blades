using Blades.Auth.Interfaces;
using Blades.Interfaces;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dispatcher;

namespace BladesEx.Startup
{
    public static class Application
    {

        public static ApplicationInfo AppInfo { get; private set; }

        public static void Run(IAppBuilder appBuilder, ApplicationInfo appInfo, Action<Di.Activator> registerDependencies = null)
        {
            AppInfo = appInfo;

            var activator = new Di.Activator(appInfo);
            registerDependencies?.Invoke(activator);

            if (appInfo.HostType == HostType.ConsoleSelfHost)
            {
                FileServerConfig(appBuilder);
            }

            var authManager = ((IBladesServiceLocator)activator).GetInstance<IAuthManager>();
            Blades.Auth.StartupHelper.OAuthConfiguration(appBuilder, authManager, accessTokenExpireTimeSpan: appInfo.AccessTokenExpireTimeSpan);

            var config = Blades.Web.StartupHelper.InitWebApiConfiguration(appBuilder, activator);
            config.Services.Replace(typeof(IHttpControllerActivator), activator);
            appBuilder.UseWebApi(config);

            Blades.Web.StartupHelper.InitClientsConnection(appBuilder, activator);
        }

        private static void FileServerConfig(IAppBuilder appBuilder)
        {
            var physicalFileSystem = new PhysicalFileSystem(@".\public");
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
