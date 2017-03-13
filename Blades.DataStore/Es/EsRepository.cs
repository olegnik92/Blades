using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Core;
using Blades.Es;
using MongoDB.Driver;
using MongoDB.Bson;
using Blades.DataStore.Interfaces;

namespace Blades.DataStore.Es
{
    public class EsRepository : IEsRepository, ITransactRepository
    {

        private List<MutationEventStoreItem> unsavedEvents;

        private IMongoDatabase db;
        public EsRepository(IMongoDatabase db)
        {
            this.db = db;
        }

        private IMongoCollection<MutationEventStoreItem> GetEventsCollection(Guid resourceTypeId)
        {
            return db.GetCollection<MutationEventStoreItem>($"MutationEvents_{resourceTypeId}");
        }

        private IMongoCollection<AggregateStateStoreItem<TState>> GetSnapshotsCollection<TState>(Guid resourceTypeId, byte detalizationLevel)
        {
            return db.GetCollection<AggregateStateStoreItem<TState>>($"Snapshots_{resourceTypeId}_{detalizationLevel}");
        }

        public List<MutationEvent> GetEvents(Resource resource, ulong startVersion)
        {
            var result = GetEventsCollection(resource.TypeId).AsQueryable()
                .Where(item => item.ResourceInstanceId == resource.InstanceId)
                .Where(item => item.MutationEvent.BaseVersion >= startVersion)
                .OrderBy(item => item.MutationEvent.BaseVersion)
                .Select(item => item.MutationEvent);

            return result.ToList();
        }

        public List<MutationEvent> GetEvents(Resource resource, byte detalizationLevel, ulong startVersion)
        {
            var result = GetEventsCollection(resource.TypeId).AsQueryable()
                .Where(item => item.ResourceInstanceId == resource.InstanceId)
                .Where(item => item.MutationEvent.DetalizationLevel <= detalizationLevel)
                .Where(item => item.MutationEvent.BaseVersion >= startVersion)
                .OrderBy(item => item.MutationEvent.BaseVersion)
                .Select(item => item.MutationEvent);

            return result.ToList();
        }

        public TState GetLastSnapshot<TState>(Resource resource, byte detalizationLevel, out ulong version)
        {
            var snapshots = GetSnapshotsCollection<TState>(resource.TypeId, detalizationLevel).AsQueryable()
                .Where(item => item.ResourceInstanceId == resource.InstanceId);

            var ver = snapshots.Any() ? snapshots.Max(s => s.Version) : 0;
            var lastSnapshot = snapshots.FirstOrDefault(snapshot => snapshot.Version == ver);

            version = ver;
            return lastSnapshot == null ? default(TState): lastSnapshot.AggregateState;
        }

        public ulong GetVersion(Resource resource)
        {
            var maxBaseVer = GetEventsCollection(resource.TypeId).AsQueryable()
                .Where(item => item.ResourceTypeId == resource.TypeId)
                .Where(item => item.ResourceInstanceId == resource.InstanceId)
                .Max(item => item.MutationEvent.BaseVersion);

            return maxBaseVer + 1;
        }


        public List<Guid> GetAllInstanceIds(Guid reosurceTypeId)
        {
            return GetEventsCollection(reosurceTypeId).AsQueryable()
                .Select(e => e.ResourceInstanceId)
                .Distinct().ToList();
        }

        public void PushEvent(Resource resource, MutationEvent mutation)
        {
            if(mutation.Id == Guid.Empty)
            {
                mutation.Id = Guid.NewGuid();
            }

            var storeItem = new MutationEventStoreItem()
            {
                ResourceTypeId = resource.TypeId,
                ResourceInstanceId = resource.InstanceId,
                MutationEvent = mutation
            };

            if(unsavedEvents == null)
            {
                unsavedEvents = new List<MutationEventStoreItem>();
            }
            unsavedEvents.Add(storeItem);
        }


        public void PushSnapshot<TState>(Resource resource, TState state, byte detalizationLevel, ulong version)
        {
            var storeItem = new AggregateStateStoreItem<TState>()
            {
                ResourceTypeId = resource.TypeId,
                ResourceInstanceId = resource.InstanceId,
                AggregateState = state,
                Version = version
            };

            GetSnapshotsCollection<TState>(resource.TypeId, detalizationLevel).InsertOne(storeItem);
        }


        private static object locker = new object();
        public void Commit()
        {
            if(unsavedEvents == null)
            {
                return;
            }

            var instanceEventsChains = unsavedEvents.GroupBy(e => e.ResourceInstanceId)
                .Select(group => group.OrderBy(e => e.MutationEvent.BaseVersion)).ToList();

            lock (locker)
            {
                foreach (var instanceEventsChain in instanceEventsChains)
                {
                    var firstEvent = instanceEventsChain.First();
                    var resource = new Resource() { TypeId = firstEvent.ResourceTypeId, InstanceId = firstEvent.ResourceInstanceId };
                    var version = GetVersion(resource);
                    if(version != firstEvent.MutationEvent.BaseVersion)
                    {
                        throw new VersionConsistencyException(resource, firstEvent.MutationEvent, version);
                    }
                    GetEventsCollection(firstEvent.ResourceTypeId).InsertMany(instanceEventsChain);
                }
            }

            unsavedEvents = null;
        }


    }
}
