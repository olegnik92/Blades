using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Interfaces;
using System.Diagnostics;

namespace Blades.Basis
{
    public class ConsoleLogger : ILogger
    {
        public string CodePlace
        {
            get
            {
                var method = (new StackTrace()).GetFrame(2).GetMethod();
                return $"{method.ReflectedType} : {method}";
            }
        }

        public void Debug(string message)
        {
            Console.WriteLine($"DEBUG : {DateTime.Now} : {CodePlace} : {message}");
        }

        public void Error(Exception error)
        {
            Console.WriteLine($"ERROR : {DateTime.Now} : {CodePlace} : {error.Message}");
            Console.WriteLine(error.StackTrace);
        }

        public void Info(string message)
        {
            Console.WriteLine($"INFO : {DateTime.Now} : {CodePlace} : {message}");
        }

        public void Warning(string message)
        {
            Console.WriteLine($"WARNING : {DateTime.Now} : {CodePlace} : {message}");
        }
    }
}
