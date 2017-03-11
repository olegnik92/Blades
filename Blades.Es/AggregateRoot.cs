using Blades.Core;
using Blades.Es.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Es
{
    public class AggregateRoot<TState>
    {
        private IEsRepository repo;
        public Resource Resource { get; private set; }
        public TState State { get; private set; }

        public AggregateRootAttribute AggregateInfo { get; private set; }

        public ulong Version { get; private set; }

        public AggregateRoot(IEsRepository repo, Resource resource)
        {
            AggregateInfo = Attribute.GetCustomAttribute(this.GetType(), typeof(AggregateRootAttribute)) as AggregateRootAttribute;
            if (AggregateInfo == null)
            {
                throw new Exception("AggregateRoot Attribute has not set");
            }

            this.repo = repo;
            this.Resource = resource;

            ulong version = 0;
            State = repo.GetLastSnapshot<TState>(resource, out version);
            this.Version = version;

            var mutations = repo.GetEvents(resource, Version).OrderBy(e => e.BaseVersion);
            ulong mutationsCount = 0;
            foreach(var mutation in mutations)
            {
                Apply(mutation);
                mutationsCount++;
            }

            if(mutationsCount > AggregateInfo.SnapshotInterval)
            {
                var state = State;
                version = Version;
                Task.Run(() => repo.PushSnapshot(Resource, state, version));
            }
        }

        public void Apply(MutationEvent mutation)
        {
            if(Version != mutation.BaseVersion)
            {
                throw new VersionConsistencyException(Resource, mutation, Version);
            }
            State = ((dynamic)this).Apply(State, (dynamic)mutation);
            Version++;
        }

        public void Mutate(MutationEvent mutation)
        {
            mutation.BaseVersion = Version;
            Apply(mutation);
            if (Guid.Empty.Equals(mutation.Id))
            {
                repo.PushEvent(Resource, mutation);
            }
        }

        public void Mutate(List<MutationEvent> mutations)
        {
            mutations.ForEach(Mutate);
        }
    }
}
