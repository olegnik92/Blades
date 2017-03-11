using Blades.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Es
{
    public class VersionConsistencyException : Exception
    {
        public Resource Resource { get; private set; }
        public MutationEvent Mutation { get; private set; }
        public ulong AggregateVersion { get; private set; }

        public VersionConsistencyException(Resource resource, MutationEvent mutation, ulong aggregateVersion)
            :base($"Версия агрегата для ресурса {resource.ToFullString()} {aggregateVersion} не соответствует базовой версии для события ${mutation}")
        {
            Resource = resource;
            Mutation = mutation;
            AggregateVersion = aggregateVersion;
        }
    }
}
