using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Es
{
    public class AggregateRootAttribute: Attribute
    {
        public Guid ResourceTypeId { get; private set; }
        public string ResourceTypeDescription { get; private set; }
        public ulong DefaultSnapshotInterval { get; private set; }
        private ulong[] DetalizationSnapshotIntervals { get; set;}

        public AggregateRootAttribute(string resourceTypeId, string resourceTypeDescription, ulong defaultSnapshotInterval = 100, ulong[] detalizationSnapshotIntervals = null)
        {
            ResourceTypeId = Guid.Parse(resourceTypeId);
            ResourceTypeDescription = resourceTypeDescription;
            DefaultSnapshotInterval = defaultSnapshotInterval;
            DetalizationSnapshotIntervals = detalizationSnapshotIntervals;
        }

        public ulong GetSnapshotInterval(byte detalizationLevel)
        {
            if(DetalizationSnapshotIntervals == null || DetalizationSnapshotIntervals.Length < (detalizationLevel - 1))
            {
                return DefaultSnapshotInterval;
            }

            return DetalizationSnapshotIntervals[detalizationLevel];
        }
    }
}
