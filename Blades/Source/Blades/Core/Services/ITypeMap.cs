using System;
using System.Collections;
using System.Collections.Generic;

namespace Blades.Core.Services
{
    /// <summary>
    /// Сопоставляет информацию о типе объекта и уникальный идентификатор, которым был этот тип помечен при объявлении
    /// (Значение атрибута TypeId)
    /// </summary>
    public interface ITypeMap : IBladesService
    {
        /// <summary>
        /// Возвращает тип по его идентификатору (значение атрибута TypeId)
        /// </summary>
        /// <param name="typeId">Значение атрибута TypeId</param>
        /// <returns>Тип помеченный атрибутом TypeId</returns>
        Type Get(Guid typeId);
        
        
        /// <summary>
        /// Возвращает все зарегестрированные в сервисе типы
        /// </summary>
        /// <returns>Зарегестрированные в сервисе типы</returns>
        IEnumerable<Type> GetAll();
    }
}