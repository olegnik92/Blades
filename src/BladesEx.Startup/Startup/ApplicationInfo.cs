using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.PlatformAbstractions;

namespace BladesEx.Startup
{
    public class ApplicationInfo
    {
        public string Name { get; set; } = PlatformServices.Default.Application.ApplicationName;

        public string Title { get; set; } = PlatformServices.Default.Application.ApplicationName;

        public string Configuration { get; set; } = "Development";

        public string Version { get; set; } = PlatformServices.Default.Application.ApplicationVersion;

        public TimeSpan AccessTokenExpireTimeSpan { get; set; } = TimeSpan.FromDays(1);

        public Server Server { get; set; } = Server.Kestrel;

        public string HostUrl { get; set; } = "http://localhost:5000/";

        public DataBaseConfig DataBase { get; set; }

        public FileServerConfig FileServer { get; set; } = new FileServerConfig();

        public List<Assembly> ApplicationAssemblies { get; set; } = new List<Assembly>();

        public string ToClientJson()
        {
            return $"{{\"name\":\"{Name}\", \"title\":\"{Title}\",  \"configuration\":\"{Configuration}\",  \"version\":\"{Version}\"}}";
        }
    }
}
