using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.DataStore
{
    public class MutationEvent
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Mutation reason command
        /// </summary>
        public Guid CommandId { get; set; }

        public ulong BaseVersion { get; set; }

        public override string ToString()
        {
            return $"Cобытие {Id} (для версии: {BaseVersion})";
        }
    }
}
