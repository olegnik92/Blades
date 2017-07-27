using Blades.Core;
using Blades.Es.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Es
{
    public abstract class AggregateRoot<TState>
    {
        public virtual byte MaxDetalizationLevel
        {
            get
            {
                return (byte)10;
            }
        }


        private IEsRepository repo;
        public Resource Resource { get; private set; }
        public TState State { get; private set; }
        public byte DetalizationLevel { get; private set; }

        public AggregateRootAttribute AggregateInfo { get; private set; }

        public List<ulong> Versions { get; private set; }

        public AggregateRoot(IEsRepository repo, Resource resource, byte detalizationLevel)
        {
            AggregateInfo = this.GetType().GetTypeInfo().GetCustomAttribute<AggregateRootAttribute>();
            if (AggregateInfo == null)
            {
                throw new Exception("AggregateRoot Attribute has not set");
            }

            this.repo = repo;
            this.Resource = resource;
            this.DetalizationLevel = detalizationLevel;

            List<ulong> versions;
            int mutationsCount;
            State = CreateState(detalizationLevel, out versions, out mutationsCount);
            Versions = versions;

            if (mutationsCount > AggregateInfo.SnapshotInterval)
            {
                MakeSnapshot();
            }
        }


        private ulong GetVersion(List<ulong> versions, byte detalizationLevel)
        {
            return versions.Count > detalizationLevel ? versions[detalizationLevel] : 0;
        }

        private void IncrementVersion(ref List<ulong> versions, byte detalizationLevel)
        {
            while(versions.Count <= detalizationLevel)
            {
                versions.Add(0);
            }

            versions[detalizationLevel]++;
        }

        public ulong GetVersion()
        {
            return GetVersion(Versions, DetalizationLevel);
        }

        public void Mutate(MutationEvent mutation)
        {
            mutation.BaseVersion = GetVersion();
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

        private TState ApplyMutation(TState state, MutationEvent mutation, byte detalizationLevel, ref List<ulong> versions)
        {
            if (mutation.DetalizationLevel > detalizationLevel)
            {
                throw new ArgumentException("Попытка применить событие с более высокой степенью детализации");
            }

            var version = GetVersion(versions, mutation.DetalizationLevel);
            if (mutation.BaseVersion != version)
            {
                throw new VersionConsistencyException(Resource, mutation, version);
            }

            var newState = (TState)((dynamic)this).Apply(State, (dynamic)mutation);
            IncrementVersion(ref versions, mutation.DetalizationLevel);
            return newState;
        }

        private void ApplyMutation(MutationEvent mutation)
        {
            var versions = Versions;
            State = ApplyMutation(State, mutation, DetalizationLevel, ref versions);
            Versions = versions;
        }

        private void MakeSnapshot()
        {
            Task.Run(() => {
                List<ulong> versions;
                int mutationCount;
                var snapshot = CreateState(MaxDetalizationLevel, out versions, out mutationCount);
                repo.PushSnapshot(Resource, snapshot, versions);
            });
        }

        private TState CreateState(byte detalizationLevel, out List<ulong> versions, out int mutationCount)
        {
            var snapshot = repo.GetLastSnapshot<TState>(Resource, out versions);
            var mutations = repo.GetEvents(Resource, detalizationLevel, versions);
            mutationCount = mutations.Count;
            foreach (var mutation in mutations)
            {
                snapshot = ApplyMutation(snapshot, mutation, detalizationLevel, ref versions);
            }

            return snapshot;
        }
    }
}
