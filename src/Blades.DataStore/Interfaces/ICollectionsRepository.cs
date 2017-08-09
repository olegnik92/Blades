using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blades.DataStore.Interfaces
{
    public interface ICollectionsRepository
    {
        IQueryable<T> Query<T>() where T : IStoreItem;

        void AddOrUpdate<T>(T item) where T : IStoreItem;

        void AddOrUpdate<T>(IEnumerable<T> items) where T : IStoreItem;

        void Delete<T>(T item) where T : IStoreItem;

        void Delete<T>(IEnumerable<T> items) where T : IStoreItem;
    }
}
