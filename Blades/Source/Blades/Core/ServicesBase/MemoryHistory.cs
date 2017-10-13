using System;
using System.Collections.Concurrent;
using System.Linq;
using Blades.Core.Services;

namespace Blades.Core.ServicesBase
{
    public class MemoryHistory: IHistory
    {
        private readonly BlockingCollection<HistoryItem> _history = new BlockingCollection<HistoryItem>();
        
        public void Put<TItem>(TItem item) where TItem : HistoryItem
        {
            _history.Add(item);           
        }

        public IQueryable<TItem> Query<TItem>() where TItem : HistoryItem
        {
            return _history.Where(item => item is TItem).Cast<TItem>().AsQueryable();
        }
    }
}