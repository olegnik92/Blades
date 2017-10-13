using System;
using System.Diagnostics;
using System.Linq;
using Blades.Core.Services;

namespace Blades.Core.ServicesBase
{
    public class ConsoleLogger: ILogger
    {
        public string CodePlace
        {
            get
            {
                var stackTrace = new StackTrace(new Exception(), true);
                var method = stackTrace.GetFrames()?.Skip(2).FirstOrDefault()?.GetMethod()?.ToString() ?? "UNKNOWN METHOD";
                return $"{method}";
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

        public void Error(Error error)
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