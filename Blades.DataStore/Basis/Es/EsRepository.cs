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
using System.Collections.Concurrent;

namespace Blades.DataStore.Basis.Es
{
    public class EsRepository : IEsRepository, ITransactRepository
    {
        public Guid TransactionId { get { return transactionId; } }
        private ConcurrentBag<MutationEventStoreItem> unsavedEvents;

        private IMongoDatabase db;
        private Guid transactionId;
        private Action<EsRepository> onDispose;

        private bool disposed = false;

        //Create for write
        //should create only with Transact Repo factory
        internal EsRepository(IMongoDatabase db, Guid transactionId, Action<EsRepository> onDispose)
        {
            if (Guid.Empty.Equals(transactionId))
            {
                throw new ArgumentException("transactionId should not be empty");
            }

            this.db = db;
            this.transactionId = transactionId;
            this.onDispose = onDispose;
            this.unsavedEvents = new ConcurrentBag<MutationEventStoreItem>();
        }

        //Create for read
        public EsRepository(IMongoDatabase db)
        {
            this.db = db;
            this.transactionId = Guid.Empty;
        }

        private IMongoCollection<MutationEventStoreItem> GetEventsCollection(Guid resourceTypeId)
        {
            return db.GetCollection<MutationEventStoreItem>($"MutationEvents_{resourceTypeId}");
        }

        private IMongoCollection<AggregateStateStoreItem<TState>> GetSnapshotsCollection<TState>(Guid resourceTypeId)
        {
            return db.GetCollection<AggregateStateStoreItem<TState>>($"Snapshots_{resourceTypeId}");
        }

        public List<MutationEvent> GetEvents(Resource resource, byte detalizationLevel, List<ulong> startVersions)
        {
            var result = GetEventsCollection(resource.TypeId).AsQueryable()
                .Where(item => item.ResourceInstanceId == resource.InstanceId)
                .Where(item => item.MutationEvent.DetalizationLevel <= detalizationLevel)
                .Where(item => item.MutationEvent.BaseVersion >= startVersions[item.MutationEvent.DetalizationLevel])
                .OrderBy(item => item.MutationEvent.BaseVersion)
                .Select(item => item.MutationEvent);

            var events = result.ToList();
            events.AddRange(unsavedEvents
                .Where(e => e.ResourceTypeId == resource.TypeId && e.ResourceInstanceId == resource.InstanceId)
                .Select(e => e.MutationEvent)
                .Where(e => e.DetalizationLevel <= detalizationLevel)
                .Where(e => e.BaseVersion >= startVersions[e.DetalizationLevel])
                .OrderBy(e => e.BaseVersion));

            return events;
        }


        public TState GetLastSnapshot<TState>(Resource resource, out List<ulong> versions)
        {
            var snapshots = GetSnapshotsCollection<TState>(resource.TypeId).AsQueryable()
                .Where(item => item.ResourceInstanceId == resource.InstanceId);

            var lastSnapshot = snapshots.FirstOrDefault(snapshot => snapshot.SaveTime == snapshots.Max(s => s.SaveTime));

            if (lastSnapshot == null)
            {
                versions = new List<ulong>();
                return default(TState);
            }

            versions = lastSnapshot.Versions;
            return lastSnapshot.AggregateState;
        }


        public List<Guid> GetAllInstanceIds(Guid reosurceTypeId)
        {
            return GetEventsCollection(reosurceTypeId).AsQueryable()
                .Select(e => e.ResourceInstanceId)
                .Distinct().ToList();
        }

        public void PushEvent(Resource resource, MutationEvent mutation)
        {
            if (Guid.Empty.Equals(transactionId))
            {
                throw new Exception("Es repositore was created without transaction Id, so it can not be used for mutations");
            }

            if(mutation.Id == Guid.Empty)
            {
                mutation.Id = Guid.NewGuid();
            }

            var storeItem = new MutationEventStoreItem()
            {
                ResourceTypeId = resource.TypeId,
                ResourceInstanceId = resource.InstanceId,
                TransactionId = transactionId,
                MutationEvent = mutation
            };

            unsavedEvents.Add(storeItem);
        }


        public void PushSnapshot<TState>(Resource resource, TState state, List<ulong> versions)
        {
            if (!Guid.Empty.Equals(transactionId))
            {
                return; //Add snapshots only for readable only repositiory, because of possible unsaved events otherwise
            }

            var storeItem = new AggregateStateStoreItem<TState>()
            {
                ResourceTypeId = resource.TypeId,
                ResourceInstanceId = resource.InstanceId,
                AggregateState = state,
                Versions = versions
            };

            GetSnapshotsCollection<TState>(resource.TypeId).InsertOne(storeItem);
        }


        private static object locker = new object();
        public void Commit()
        {
            if (disposed)
            {
                return;
            }

            if (Guid.Empty.Equals(transactionId))
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
                    GetEventsCollection(firstEvent.ResourceTypeId).InsertMany(instanceEventsChain);
                }
            }

            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //Вызов метода Despose без предварительного комита - некий эквивалент transaction rollback
        //так как все хранится в памяти, то и откатывать особо нечего.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            // release unmanaged resources
            // nothing to release
            if (disposing)
            { // release other disposable objects
                unsavedEvents = null;
            }

            onDispose(this);
            disposed = true;
        }


        ~EsRepository()
        {
            Dispose(false);
        }
    }
}
