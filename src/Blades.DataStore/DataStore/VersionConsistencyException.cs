using Blades.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.DataStore
{
    public class VersionConsistencyException : Exception
    {
        public ResourceInfo Resource { get; private set; }
        public MutationEvent Mutation { get; private set; }
        public ulong AggregateVersion { get; private set; }

        public VersionConsistencyException(ResourceInfo resource, MutationEvent mutation, ulong aggregateVersion)
            :base($"Версия агрегата для ресурса {resource.ToFullString()} {aggregateVersion} не соответствует базовой версии для события ${mutation}")
        {
            Resource = resource;
            Mutation = mutation;
            AggregateVersion = aggregateVersion;
        }
    }
}
