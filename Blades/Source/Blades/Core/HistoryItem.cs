using System;
using System.Reflection;
using Blades.Core.Extensions;

namespace Blades.Core
{
    /// <summary>
    /// Базовый тип для записей журнала истории
    /// </summary>
    public abstract class HistoryItem
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор типа записи
        /// </summary>
        public Guid ItemTypeId { get; set; }

        /// <summary>
        /// Дата и время записи
        /// </summary>
        public DateTime RecordDate { get; set; }

        protected HistoryItem()
        {
            Id = Guid.NewGuid();
            ItemTypeId = this.GetTypeLabelId();
            
            RecordDate = DateTime.Now;
        }
    }
}