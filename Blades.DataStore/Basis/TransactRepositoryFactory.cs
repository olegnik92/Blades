using Blades.DataStore.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.DataStore.Basis
{
    public class TransactRepositoryFactory : ITransactRepositoryFactory
    {
        private IMongoDatabase db;

        public TransactRepositoryFactory(IMongoDatabase db)
        {
            this.db = db;
        }

        public TRepo GetRepository<TRepo>() where TRepo : ITransactRepository
        {
            if (typeof(TRepo).IsAssignableFrom(typeof(Es.EsRepository)))
            {
                var repo = new Es.EsRepository(db); ;
                var result = (TRepo)(object)(repo);
                return result;
            }

            throw new NotImplementedException();
        }
    }
}
