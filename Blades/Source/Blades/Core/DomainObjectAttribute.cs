using System;

namespace Blades.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DomainObjectAttribute : TypeIdAttribute
    {
        /// <summary>
        /// Наименование типа объекта учета.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="typeId">Идентификатор типа объекта учета</param>
        /// <param name="typeName">Наименование типа объекта учета</param>
        public DomainObjectAttribute(string typeId, string typeName) : base(typeId)
        {
            Name = typeName;
        }
    }
}