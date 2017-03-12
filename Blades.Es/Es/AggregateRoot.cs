using Blades.Core;
using Blades.Es.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Es
{
    public abstract class AggregateRoot<TState>
    {
        private IEsRepository repo;
        public Resource Resource { get; private set; }
        public TState State { get; private set; }
        public byte DetalizationLevel { get; private set; }

        public AggregateRootAttribute AggregateInfo { get; private set; }

        public ulong Version { get; private set; }

        public AggregateRoot(IEsRepository repo, Resource resource, byte detalizationLevel)
        {
            AggregateInfo = Attribute.GetCustomAttribute(this.GetType(), typeof(AggregateRootAttribute)) as AggregateRootAttribute;
            if (AggregateInfo == null)
            {
                throw new Exception("AggregateRoot Attribute has not set");
            }

            this.repo = repo;
            this.Resource = resource;
            this.DetalizationLevel = detalizationLevel;

            ulong version = 0;
            State = repo.GetLastSnapshot<TState>(Resource, detalizationLevel, out version);

            var mutations = repo.GetEvents(Resource, detalizationLevel, version).OrderBy(e => e.BaseVersion);
            ulong mutationsCount = 0;
            foreach(var mutation in mutations)
            {
                Apply(mutation);
                mutationsCount++;
            }
            Version = repo.GetVersion(Resource);

            if (mutationsCount > AggregateInfo.GetSnapshotInterval(detalizationLevel))
            {
                var state = State;
                version = Version;
                Task.Run(() => repo.PushSnapshot(Resource, state, detalizationLevel, version));
            }
        }

        public void Apply(MutationEvent mutation)
        {
            if(AggregateInfo == null)
            {
                throw new Exception("Агрегат не загружен");
            }

            if(mutation.DetalizationLevel > DetalizationLevel)
            {
                throw new ArgumentException("Попытка применить событие с более высокой степенью детализации");
            }

            if(mutation.BaseVersion < Version)
            {
                throw new VersionConsistencyException(Resource, mutation, Version);
            }
            State = ((dynamic)this).Apply(State, (dynamic)mutation);
            Version = mutation.BaseVersion + 1;
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
