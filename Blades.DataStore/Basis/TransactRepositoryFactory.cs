using Blades.DataStore.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blades.DataStore.Basis
{
    public class TransactRepositoryFactory : ITransactRepositoryFactory
    {
        private IMongoDatabase db;

        private ConcurrentDictionary<Guid, Es.EsRepository> esRepositories = new ConcurrentDictionary<Guid, Es.EsRepository>();

        public TransactRepositoryFactory(IMongoDatabase db)
        {
            this.db = db;
        }

        public TRepo GetRepository<TRepo>(Guid transactionId) where TRepo : ITransactRepository
        {
            if (typeof(TRepo).IsAssignableFrom(typeof(Es.EsRepository)))
            {
                if (Guid.Empty.Equals(transactionId))
                {
                    return (TRepo)(object)(new Es.EsRepository(db));
                }

                var repo = esRepositories.GetOrAdd(transactionId, CreateEsRepository);
                var result = (TRepo)(object)(repo);
                return result;
            }

            throw new NotImplementedException();
        }


        private Es.EsRepository CreateEsRepository(Guid transactionId)
        {
            return new Es.EsRepository(db, transactionId, DestroyEsRepository);
        }

        private void DestroyEsRepository(Es.EsRepository repo)
        {
            esRepositories.TryRemove(repo.TransactionId, out repo);
        }
    }
}
