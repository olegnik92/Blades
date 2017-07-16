using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BladesEx.Startup
{
    public class FileServerConfig
    {
        public string PublicRootPath { get; set; } = @".\public";

        public string DefaultPublicFileName { get; set; } = "index.html";
    }


    internal class FileServerConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("PublicRootPath")]
        public string PublicRootPath
        {
            get
            {
                return (string)this["PublicRootPath"];
            }
        }

        [ConfigurationProperty("DefaultPublicFileName")]
        public string DefaultPublicFileName
        {
            get
            {
                return (string)this["DefaultPublicFileName"];
            }
        }
    }
}
