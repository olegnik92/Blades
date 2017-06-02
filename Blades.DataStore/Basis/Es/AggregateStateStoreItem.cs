using Blades.Es;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.DataStore.Basis.Es
{
    [BsonIgnoreExtraElements]
    class AggregateStateStoreItem<TState>
    {
        public Guid ResourceTypeId { get; set; }
        public Guid ResourceInstanceId { get; set; }
        public TState AggregateState { get; set; }
        public ulong Version { get; set; }
        public DateTime SaveTime { get; set; } = DateTime.Now;
    }
}
