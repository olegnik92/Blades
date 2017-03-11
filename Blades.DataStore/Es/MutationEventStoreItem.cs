using Blades.Core;
using Blades.Es;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.DataStore.Es
{
    class MutationEventStoreItem
    {
        public Guid ResourceTypeId { get; set; }
        public Guid ResourceInstanceId { get; set; }
        public MutationEvent MutationEvent { get; set; }
        public DateTime SaveTime { get; set; } = DateTime.Now;
    }
}
