using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.DataStore
{
    public class AggregateRootAttribute: Attribute
    {
        public Guid ResourceTypeId { get; private set; }
        public string ResourceTypeDescription { get; private set; }
        public int SnapshotInterval { get; private set; }

        public AggregateRootAttribute(string resourceTypeId, string resourceTypeDescription, int snapshotInterval = 25)
        {
            ResourceTypeId = Guid.Parse(resourceTypeId);
            ResourceTypeDescription = resourceTypeDescription;
            SnapshotInterval = snapshotInterval;
        }
    }
}
