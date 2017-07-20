using Blades.DataStore.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.DataStore.Basis
{
    public class SimpleCollectionsRepository : ICollectionsRepository
    {
        private IMongoDatabase db;
        public SimpleCollectionsRepository(IMongoDatabase db)
        {
            this.db = db;
        }

        public void AddOrUpdate<T>(IEnumerable<T> items) where T : StoreItem
        {
            var itemsForUpdate = new List<T>();
            var itemsForInsert = new List<T>();
            foreach (var item in items)
            {
                if (Guid.Empty.Equals(item.Id))
                {
                    itemsForInsert.Add(item);
                }
                else
                {
                    itemsForUpdate.Add(item);
                }
            }

            if (itemsForInsert.Count > 0)
            {
                InsertItems(itemsForInsert);
            }

            if (itemsForUpdate.Count > 0)
            {
                UpdateItems(itemsForUpdate);
            }

        }

        public void AddOrUpdate<T>(T item) where T : StoreItem
        {
            if (Guid.Empty.Equals(item.Id))
            {
                InsertItem(item);
            }
            else
            {
                UpdateItem(item);
            }
        }

        public void Delete<T>(IEnumerable<T> items) where T : StoreItem
        {
            var ids = items.Select(item => item.Id);
            var collection = GetCollection<T>();
            collection.DeleteMany(it => ids.Contains(it.Id));
        }

        public void Delete<T>(T item) where T : StoreItem
        {
            var collection = GetCollection<T>();
            collection.DeleteOne(it => it.Id == item.Id);
        }

        public IQueryable<T> Query<T>() where T : StoreItem
        {
            return GetCollection<T>().AsQueryable();
        }

        private IMongoCollection<T> GetCollection<T>() where T : StoreItem
        {
            return db.GetCollection<T>(typeof(T).FullName);
        }

        private void InsertItem<T>(T item) where T : StoreItem
        {
            var collection = GetCollection<T>();
            item.Id = Guid.NewGuid();
            collection.InsertOne(item);
        }

        private void InsertItems<T>(IEnumerable<T> items) where T : StoreItem
        {
            var collection = GetCollection<T>();
            Parallel.ForEach(items, item =>
            {
                item.Id = Guid.NewGuid();
            });
            collection.InsertMany(items);
        }

        private void UpdateItem<T>(T item) where T : StoreItem
        {
            var collection = GetCollection<T>();
            collection.FindOneAndReplace((storeItem) => storeItem.Id == item.Id, item);
        }

        private void UpdateItems<T>(IEnumerable<T> items) where T : StoreItem
        {
            var collection = GetCollection<T>();
            Parallel.ForEach(items, item =>
            {
                collection.FindOneAndReplace((storeItem) => storeItem.Id == item.Id, item);
            });
        }
    }
}
