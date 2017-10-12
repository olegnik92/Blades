using System;

namespace Blades.Core
{
    /// <summary>
    /// Позваляет присвоить типу уникальный идентификатор.
    /// Тип можно будет получать через ITypeMap сервис.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TypeIdAttribute : Attribute
    {
        /// <summary>
        /// Присвоенный типу идентификатор.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="id">Идентификатор типа. Должен быть Guid.</param>
        public TypeIdAttribute(string id)
        {
            Id = Guid.Parse(id);
        }
    }
}