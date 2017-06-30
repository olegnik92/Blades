using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Web
{
    public enum DataFromat
    {
        Json = 0
    }

    public static class DataFromatExt
    {
        public static DataFromat ToDataFormatEnum(this string value)
        {
            if(value == "json")
            {
                return DataFromat.Json;
            }

            throw new ArgumentException("Неизвестной имя формата данных");
        }
    }
}
