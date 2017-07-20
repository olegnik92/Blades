using Blades.Auth.Interfaces;
using Blades.Interfaces;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dispatcher;

namespace BladesEx.Startup
{
    public static class Application
    {

        public static List<Assembly> AssembliesForceLoad(Func<List<string>, List<string>> filesFilter = null)
        {
            //from https://stackoverflow.com/questions/2384592/is-there-a-way-to-force-all-referenced-assemblies-to-be-loaded-into-the-app-doma
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();

            var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();
            if (filesFilter != null)
            {
                toLoad = filesFilter(toLoad);
            }
            toLoad.ForEach(path => TryLoadAssembly(path, ref loadedAssemblies));
            return loadedAssemblies;
        }

        private static void TryLoadAssembly(string path, ref List<Assembly> loadedAssemblies)
        {
            try
            {
                loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path)));
            }
            catch(Exception err)
            {
                Console.WriteLine(err);
            }
        }


        public static ApplicationInfo AppInfo { get; private set; }

        public static void Run(IAppBuilder appBuilder, ApplicationInfo appInfo, Action<Di.Activator> registerDependencies = null)
        {
            AppInfo = appInfo;

            var activator = new Di.Activator(appInfo);
            registerDependencies?.Invoke(activator);

            if (appInfo.HostType == HostType.ConsoleSelfHost)
            {
                FileServerConfig(appBuilder, appInfo.FileServer);
            }

            var authManager = ((IBladesServiceLocator)activator).GetInstance<IAuthManager>();
            Blades.Auth.StartupHelper.OAuthConfiguration(appBuilder, authManager, accessTokenExpireTimeSpan: appInfo.AccessTokenExpireTimeSpan);

            var config = Blades.Web.StartupHelper.InitWebApiConfiguration(appBuilder, activator);
            config.Services.Replace(typeof(IHttpControllerActivator), activator);
            appBuilder.UseWebApi(config);

            Blades.Web.StartupHelper.InitClientsConnection(appBuilder, activator);
        }

        private static void FileServerConfig(IAppBuilder appBuilder, FileServerConfig config)
        {
            var physicalFileSystem = new PhysicalFileSystem(config.PublicRootPath);
            var options = new FileServerOptions
            {
                EnableDefaultFiles = true,
                FileSystem = physicalFileSystem
            };
            options.StaticFileOptions.FileSystem = physicalFileSystem;
            options.StaticFileOptions.ServeUnknownFileTypes = true;
            options.DefaultFilesOptions.DefaultFileNames = new[] { config.DefaultPublicFileName };
            appBuilder.UseFileServer(options);
        }
    }
}
