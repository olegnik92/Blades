using System;

namespace Blades.Core
{
    /// <summary>
    /// Объект учета.
    /// (Еденица бизнес-логики)
    /// </summary>
    public interface IDomainObject
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Наименование
        /// </summary>
        string Name { get; }
    }
}