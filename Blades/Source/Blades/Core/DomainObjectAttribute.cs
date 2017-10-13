using System;

namespace Blades.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DomainObjectAttribute : TypeLabelAttribute
    {

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="typeId">Идентификатор типа объекта учета (Guid)</param>
        /// <param name="typeName">Наименование типа объекта учета</param>
        public DomainObjectAttribute(string typeId, string typeName) : base(typeId, typeName)
        {
        }
    }
}