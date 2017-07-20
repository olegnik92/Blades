using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace BladesEx.Startup
{
    public class DataBaseConfig
    {
        public const string MongoDriver = "MongoDB";

        public string Driver { get; set; } = MongoDriver;

        public string ConnectionString { get; set; }

        public string Name { get; set; }
    }


    internal class DataBaseConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("Driver")]
        public string Driver
        {
            get
            {
                return (string)this["Driver"];
            }
        }

        [ConfigurationProperty("ConnectionString")]
        public string ConnectionString
        {
            get
            {
                return (string)this["ConnectionString"];
            }
        }

        [ConfigurationProperty("Name")]
        public string Name
        {
            get
            {
                return (string)this["Name"];
            }
        }
    }
}
