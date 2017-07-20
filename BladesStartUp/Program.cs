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


        class Item: Blades.DataStore.StoreItem
        {
            public int A { get; set; }
        }


        static void FastTests()
        {
            var dbClient = new MongoClient("mongodb://localhost:27017");
            var db = dbClient.GetDatabase("TestDataStore");
            var repo = new Blades.DataStore.Basis.SimpleCollectionsRepository(db);

            var addedItems = repo.Query<Item>().ToList();
            foreach(var item in addedItems)
            {
                item.A += 100;
            }
            repo.AddOrUpdate((IEnumerable<Item>)addedItems);
            addedItems = repo.Query<Item>().ToList();
        }

        static void Main(string[] args)
        {
            var a = new Blades.WebClient.TestOperations.EchoOperation();
            FastTests();

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
