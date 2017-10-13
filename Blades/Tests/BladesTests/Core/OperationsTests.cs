using System;
using Blades.Core;
using Blades.Core.ServicesBase;
using Xunit;

namespace BladesTests.Core
{
    public class OperationsTests
    {
        [Fact]
        public void OperationActivatorTest()
        {
            var activator = new OperationsActivator(new TypeMap());
            var operation = activator.Create(Guid.Parse(EchoOperation.TypeId), "Hello", new UserInfo());
            Assert.Equal(typeof(EchoOperation), operation.GetType());
        }


        [Fact]
        public void OperationExecutionTest()
        {
            var executor = new OperationsExecutor(new ConsoleLogger(), new MemoryHistory());
            var operation = new EchoOperation() {Data = "OperationsExecutor"};
            var (result, report) = executor.Execute(operation);
            Assert.Equal("OperationsExecutor", result);
            Assert.Equal(EchoOperation.TypeName, report.ReportStrings[0]);
        }
        
        [Operation(TypeId, TypeName)]
        public class EchoOperation : Operation<string, string>
        {
            public const string TypeId = "{8EF565DE-084D-4694-BFE8-58FE6840C13D}";
            public const string TypeName = "EchoOperation";
             
            
            public override (string Result, ExecutionReport Report) Execute()
            {
                return (Data, new ExecutionReport(TypeName));
            }
        }
    }
}