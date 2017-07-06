using BladesEx.Startup;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BladesExStartup
{
    class Program
    {
        static void Main(string[] args)
        {
            using (WebApp.Start<Startup>("http://localhost:9000"))
            {
                Console.ReadLine();
            }

        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var appInfo = new ApplicationInfo
            {
                HostType = HostType.ConsoleSelfHost,
            };

            Application.Run(app, appInfo);
        }
    }
}
