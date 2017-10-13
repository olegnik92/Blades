using System;
using Blades.Core;
using Blades.Core.ServicesBase;

namespace ConsoleNetCore
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello");
            var activator = new OperationsActivator(new TypeMap());
            var executor = new OperationsExecutor(new ConsoleLogger(), new MemoryHistory());

            var operation = activator.Create(Guid.Parse(EchoOperation.TypeId), "Hello Blades", new UserInfo());
            var (result, report) = executor.Execute(operation);
            Console.WriteLine(result);
        }
    }


    [Operation(TypeId, TypeName)]
    public class EchoOperation : Operation<string, string>
    {
        public const string TypeId = "{E5EB06BA-2407-487A-8E5D-6081BC46E28A}";
        public const string TypeName = "EchoOperation";
        
        public override (string Result, ExecutionReport Report) Execute()
        {
            return (Data, null);
        }
    }
    
}