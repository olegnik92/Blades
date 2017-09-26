using Blades.DataStore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blades.DataStore
{
    public abstract class Journal<TState, TListState> : AggregateRoot<TState>, IMutationHandler<TState, CreateJournalItem>
        where TListState : IStoreItem
    {

        private ICollectionsRepository collectionsRepo;
        public Journal(Guid instanceId, ICollectionsRepository collectionsRepo, IEsRepository esRepo) : base(instanceId, esRepo)
        {
            this.collectionsRepo = collectionsRepo;

            this.OnMutationEventPushed += (e => UpdateJournalState());
        }

        /// <summary>
        /// From TState to TListState map function
        /// </summary>
        public abstract TListState GetListState(TState state);

        private TListState GetListStateInternal()
        {
            var state = GetListState(State);
            return state;
        }

        protected void UpdateJournalState()
        {
            var state = GetListStateInternal();
            collectionsRepo.AddOrUpdate(state);
        }

        public void RemoveRecord()
        {
            var state = GetListStateInternal();
            RemoveRecord(collectionsRepo, state);
        }

        public static void RemoveRecord(ICollectionsRepository collectionsRepo, TListState state)
        {
            collectionsRepo.Delete(state);
        }

        public static Guid CreateRecord(ICollectionsRepository collectionsRepo, TListState state)
        {
            collectionsRepo.AddOrUpdate(state);
            return state.Id;
        }

        public static IQueryable<TListState> Query(ICollectionsRepository collectionsRepo)
        {
            var data = collectionsRepo.Query<TListState>();
            return data;
        }

        public TState Apply(TState state, CreateJournalItem mutation)
        {
            var newState = (TState)mutation.InitialState;
            return newState;
        }
    }
}
