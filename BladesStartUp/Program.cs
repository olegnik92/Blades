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
using MongoDB.Driver;

namespace BladesStartUp
{
    class Program
    {
        private static void M1()
        {
            throw new ArgumentException("Some bad Argument");
        }


        private static void M2()
        {
            M1();
        }

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
