using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Extensions
{
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(this List<T> list)
        {
            return list == null || list.Count == 0;
        }

        public static bool IsNullOrEmpty(this Array arr)
        {
            return arr == null || arr.Length == 0;
        }
    }
}
