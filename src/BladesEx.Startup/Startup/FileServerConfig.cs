using System;
using System.Collections.Generic;
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

}
