using Blades.Core;
using Blades.DataStore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Blades.DataStore
{
    public abstract class AggregateRoot<TState>
    {

        private IEsRepository repo;
        public Resource Resource { get; private set; }
        public TState State { get; private set; }

        public AggregateRootAttribute AggregateInfo { get; private set; }

        public ulong Version { get; private set; }

        public AggregateRoot(IEsRepository repo, Guid instanceId)
        {
            AggregateInfo = this.GetType().GetTypeInfo().GetCustomAttribute<AggregateRootAttribute>();
            if (AggregateInfo == null)
            {
                throw new Exception("AggregateRoot Attribute has not set");
            }

            this.repo = repo;
            this.Resource = new Resource
            {
                TypeId = AggregateInfo.ResourceTypeId,
                TypeDescription = AggregateInfo.ResourceTypeDescription,
                InstanceId = instanceId
            };

            int mutationsCount;
            ulong version;
            State = CreateState(out version, out mutationsCount);
            Version = version;

            if (mutationsCount > AggregateInfo.SnapshotInterval)
            {
                MakeSnapshot();
            }
        }


        public void Mutate(MutationEvent mutation)
        {
            mutation.BaseVersion = Version;
            ApplyMutation(mutation);
            if (Guid.Empty.Equals(mutation.Id))
            {
                repo.PushEvent(Resource, mutation);
            }
        }

        public void Mutate(List<MutationEvent> mutations)
        {
            mutations.ForEach(Mutate);
        }

        private TState ApplyMutation(TState state, MutationEvent mutation, ref ulong version)
        {
            if (mutation.BaseVersion != version)
            {
                throw new VersionConsistencyException(Resource, mutation, version);
            }

            var newState = (TState)((dynamic)this).Apply(State, (dynamic)mutation);
            version++;
            return newState;
        }

        private void ApplyMutation(MutationEvent mutation)
        {
            var version = Version;
            State = ApplyMutation(State, mutation, ref version);
            Version = version;
        }

        private void MakeSnapshot()
        {
            repo.PushSnapshot(Resource, State, Version);
        }

        private TState CreateState(out ulong version, out int mutationCount)
        {
            var snapshot = repo.GetLastSnapshot<TState>(Resource, out version);
            var mutations = repo.GetEvents(Resource, version);
            mutationCount = mutations.Count;
            foreach (var mutation in mutations)
            {
                snapshot = ApplyMutation(snapshot, mutation, ref version);
            }

            return snapshot;
        }
    }
}
