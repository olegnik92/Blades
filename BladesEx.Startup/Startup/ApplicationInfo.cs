using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BladesEx.Startup
{
    public class ApplicationInfo
    {
        public string Name { get; set; } = "BladesEx";

        public string Title { get; set; } = "BladesEx";

        public string Configuration { get; set; } = "Development";

        public string Version { get; set; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public TimeSpan AccessTokenExpireTimeSpan { get; set; } = TimeSpan.FromDays(1);

        public HostType HostType { get; set; }

        public DataBaseConfig DataBase { get; set; }

        public FileServerConfig FileServer { get; set; } = new FileServerConfig();

        public string ToClientJson()
        {
            return $"{{\"name\":\"{Name}\", \"title\":\"{Title}\",  \"configuration\":\"{Configuration}\",  \"version\":\"{Version}\"}}";
        }
    }
}
