using System;

namespace Blades.Core
{
    /// <summary>
    /// Позваляет присвоить типу уникальный идентификатор и описательное (необязательное) имя.
    /// Тип можно будет получать через ITypeMap сервис.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TypeLabelAttribute : Attribute
    {
        /// <summary>
        /// Присвоенный типу идентификатор
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Присвоенное типу наименование
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="id">Идентификатор типа. Должен быть Guid.</param>
        /// <param name="name">Наименование типа</param>
        public TypeLabelAttribute(string id, string name = null)
        {
            Id = Guid.Parse(id);
            Name = name;
        }
    }
}