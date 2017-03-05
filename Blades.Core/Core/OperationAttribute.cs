using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Core
{
    public class OperationAttribute : Attribute
    {
        public string Name { get; private set; }

        public string Title { get; set; }

        public OperationType Type { get; private set; }

        public OperationAttribute(string name, OperationType type, string title)
        {
            Name = name;
            Type = type;
            Title = title;
        }
    }
}
