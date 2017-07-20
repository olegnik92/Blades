using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Core
{
    public class Resource
    {
        public Guid TypeId { get; set; }

        public Guid InstanceId { get; set; }

        public string TypeDescription { get; set; }

        public string InstanceDescription { get; set; }

        public override string ToString()
        {
            return $"{TypeDescription} --- {InstanceDescription}";
        }

        public string ToFullString()
        {
            return $"{this} -:- ({TypeId} --- {InstanceId})";
        }
    }
}
