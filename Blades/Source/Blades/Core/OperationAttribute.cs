using System;

namespace Blades.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class OperationAttribute: TypeLabelAttribute
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="typeId">Идентификатор операции (Guid)</param>
        /// <param name="typeName">Наименование операции</param>
        public OperationAttribute(string typeId, string typeName) : base(typeId, typeName)
        {
        }
    }
}