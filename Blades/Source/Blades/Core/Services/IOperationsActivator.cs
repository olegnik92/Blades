using System;

namespace Blades.Core.Services
{
    /// <summary>
    /// Сервис инстанцирования экземпляров операций.
    /// Служит для внедрения DI зависимостей в операции
    /// </summary>
    public interface IOperationsActivator: IBladesService
    {
        /// <summary>
        /// Создает экземпляр операции с внедренными зависимостями
        /// </summary>
        /// <param name="operationTypeId">Идентификатор типа операции</param>
        /// <param name="data">Входные данные</param>
        /// <param name="user">Пользователь, от имени которого выполняется операция</param>
        /// <returns>Экземпляр операции </returns>
        Operation Create(Guid operationTypeId, object data, UserInfo user);
    }
}