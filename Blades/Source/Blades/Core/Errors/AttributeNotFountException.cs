using System;

namespace Blades.Core.Errors
{
    public class AttributeNotFountException: Exception
    {
        public AttributeNotFountException(Type attrType, object target)
            : base($"Объект {target} не помечен атрибутом {attrType.FullName}.")
        {
            
        }
    }
}