using System.Linq;

namespace Blades.Core.Services
{
    /// <summary>
    /// Сервис взаимодействия с журналом истории
    /// </summary>
    public interface IHistory: IBladesService
    {
        /// <summary>
        /// Добавить запись в журнал
        /// </summary>
        /// <param name="item">Запись в журнале истории</param>
        /// <typeparam name="TItem">Тип записи</typeparam>
        void Put<TItem>(TItem item) where TItem : HistoryItem;
        
        /// <summary>
        /// Прочитать записи из журнала
        /// </summary>
        /// <typeparam name="TItem">Тип записи</typeparam>
        /// <returns>Записи искомого типа TItem</returns>
        IQueryable<TItem> Query<TItem>() where TItem : HistoryItem;
    }
}