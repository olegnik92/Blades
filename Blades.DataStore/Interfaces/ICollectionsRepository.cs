using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.DataStore.Interfaces
{
    /// <summary>
    /// Execute collections operations immediately
    /// </summary>
    public interface ICollectionsRepository
    {
        IQueryable<T> Query<T>() where T : StoreItem;

        void AddOrUpdate<T>(T item) where T : StoreItem;

        void AddOrUpdate<T>(IEnumerable<T> items) where T : StoreItem;

        void Delete<T>(T item) where T : StoreItem;

        void Delete<T>(IEnumerable<T> items) where T : StoreItem;
    }

    /// <summary>
    /// Execute collections operations in transaction
    /// </summary>
    public interface ITransactCollectionsRepository: ICollectionsRepository, ITransactRepository
    {

    }
}
