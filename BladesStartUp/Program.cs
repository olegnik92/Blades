using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Core;
using System.Threading;
using Blades.Basis;
using Blades.Interfaces;
using Castle.Windsor;
using Castle.MicroKernel.Registration;

namespace BladesStartUp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start up project fired");
            string baseAddress = "http://localhost:9000/";

            // Start OWIN host 
            WebApp.Start<Startup>(baseAddress);

            Console.WriteLine("OWIN started at:");
            Console.WriteLine($"{baseAddress}");
            Console.ReadLine();           
        }


    }
}
