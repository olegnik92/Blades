using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class OperationAttribute : Attribute
    {
        public string Name { get; private set; }

        public string Title { get; set; }

        public OperationAttribute(string name, string title)
        {
            Name = name;
            Title = title;
        }
    }
}
