using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Es
{
    public class MutationEvent
    {
        public Guid Id { get; set; }

        public ulong BaseVersion { get; set; }

        public override string ToString()
        {
            return $"Cобытие {Id} (для версии: {BaseVersion})";
        }
    }
}
