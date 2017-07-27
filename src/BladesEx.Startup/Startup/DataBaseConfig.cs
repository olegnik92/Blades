using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BladesEx.Startup
{
    public class DataBaseConfig
    {
        public const string MongoDriver = "MongoDB";

        public string Driver { get; set; } = MongoDriver;

        public string ConnectionString { get; set; }

        public string Name { get; set; }
    }
}
