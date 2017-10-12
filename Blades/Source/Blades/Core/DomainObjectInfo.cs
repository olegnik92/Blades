using System;

namespace Blades.Core
{
    /// <summary>
    /// Информация об объекте учета
    /// </summary>
    public class DomainObjectInfo
    {
        /// <summary>
        /// Идентификатор типа объекта учета
        /// </summary>
        public Guid TypeId { get; set; }

        /// <summary>
        /// Наименование типа объекта учета
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Идентификатор объекта учета
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Наименование объекта учета
        /// </summary>
        public string Name { get; set; }
    }
}