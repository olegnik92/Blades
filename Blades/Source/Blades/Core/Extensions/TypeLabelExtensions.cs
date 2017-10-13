using System;
using System.Reflection;

namespace Blades.Core.Extensions
{
    public static class TypeLabelExtensions
    {
        public static Guid GetTypeLabelId(this object obj)
        {
            return GetTypeId(GetObjectType(obj));
        }

        public static string GetTypeLabelName(this object obj)
        {
            return GetTypeName(GetObjectType(obj));
        }

        private static Type GetObjectType(object obj)
        {     
            return (obj as Type) ?? obj?.GetType();
        }
        
        private static Guid GetTypeId(Type type)
        {
            return type?.GetTypeInfo().GetCustomAttribute<TypeLabelAttribute>()?.Id ?? Guid.Empty;
        }

        private static string GetTypeName(Type type)
        {
            return type?.GetTypeInfo().GetCustomAttribute<TypeLabelAttribute>()?.Name;
        }
    }
}