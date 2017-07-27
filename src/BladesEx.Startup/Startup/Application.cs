using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace BladesEx.Startup
{
    public static class Application
    {

        //public static List<Assembly> AssembliesForceLoad(Func<List<string>, List<string>> filesFilter = null)
        //{
        //    //from https://stackoverflow.com/questions/2384592/is-there-a-way-to-force-all-referenced-assemblies-to-be-loaded-into-the-app-doma
        //    var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
        //    var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();

        //    var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
        //    var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();
        //    if (filesFilter != null)
        //    {
        //        toLoad = filesFilter(toLoad);
        //    }
        //    toLoad.ForEach(path => TryLoadAssembly(path, ref loadedAssemblies));
        //    return loadedAssemblies;
        //}

        //private static void TryLoadAssembly(string path, ref List<Assembly> loadedAssemblies)
        //{
        //    try
        //    {
        //        loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path)));
        //    }
        //    catch(Exception err)
        //    {
        //        Console.WriteLine(err);
        //    }
        //}


        public static ApplicationInfo AppInfo { get; private set; }

        public static void Run(ApplicationInfo appInfo)
        {
            AppInfo = appInfo;
            RunHost();
        }


        private static void RunHost()
        {
            var hostBuilder = new WebHostBuilder()
                .UseKestrel()
                .UseIISIntegration()
                .UseEnvironment(AppInfo.Configuration)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseWebRoot(Path.Combine(Directory.GetCurrentDirectory(), AppInfo.FileServer?.PublicRootPath ?? "wwwroot"))
                .UseUrls(AppInfo.HostUrl)
                .UseStartup<Startup>();


            var host = hostBuilder.Build();
            host.Run();
        }

    }
}
