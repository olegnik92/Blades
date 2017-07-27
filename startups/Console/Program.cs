using BladesEx.Startup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StartupConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Blades Ex startup");
            var appInfo = new ApplicationInfo()
            {
                HostUrl = "http://localhost:9000/",
                FileServer = new FileServerConfig { PublicRootPath = "public" }
            };
            Application.Run(appInfo);
        }
    }
}
