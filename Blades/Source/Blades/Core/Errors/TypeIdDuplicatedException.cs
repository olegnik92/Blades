using System;

namespace Blades.Core.Errors
{
    public class TypeIdDuplicatedException: Exception
    {
        public TypeIdDuplicatedException(Guid id)
            :base($"Идентификатор {id} был присвоен более чем одному типу.")
        {
            
        }
    }
}