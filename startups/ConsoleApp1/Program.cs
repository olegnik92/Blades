using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var types = Blades.Core.AppHelper.GetAllTypes().ToList();
            var types2 = Blades.Core.AppHelper.GetAllTypes().ToList();
        }
    }


    public class Hello
    {

    }
}
