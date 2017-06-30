using Blades.Core;
using Blades.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.WebClient.TestOperations
{
    public class ObjectInheritanceTests
    {
    }

    public class BaseTestType
    {
        public int Field1 { get; set; }
    }

    [TypeId("{38ADA025-A94F-486E-AEEF-55E61F951CF8}")]
    public class ChildType1: BaseTestType
    {
        public int Field2 { get; set; }
    }

    [TypeId("{A670EBC8-29D8-44DD-8D23-C08BAE68F218}")]
    public class ChildType2 : BaseTestType
    {
        public int Field2 { get; set; }
    }

    [Operation("TestOperation.ChildrenTypesData", OperationType.Query, "Тестовая операция: ChilderTypesData")]
    public class ChilderTypesData : Operation<BaseTestType, int>
    {
        public override int Execute(out OperationExecutionReport executionReport)
        {
            executionReport = new OperationExecutionReport();
            var type1 = Data as ChildType1;
            if (type1 != null)
            {
                return type1.Field1 + type1.Field2;
            }

            var type2 = Data as ChildType2;
            if(type2 != null)
            {
                return type2.Field1 - type2.Field2;
            }

            return Data.Field1;
        }
    }


    [Operation("TestOperation.ListOfChildrenTypes", OperationType.Query, "Тестовая операция: ListOfChildrenTypes")]
    public class ListOfChildrenTypes : Operation<List<BaseTestType>, List<int>>
    {
        private IOperationsExecutor executor;
        public ListOfChildrenTypes(IOperationsExecutor executor)
        {
            this.executor = executor;
        }

        public override List<int> Execute(out OperationExecutionReport executionReport)
        {
            var report = executionReport = new OperationExecutionReport();

            var result = Data.Select(item =>
            {
                var dataItemOperation = new ChilderTypesData() { Data = item };
                return executor.Execute(dataItemOperation, this, report);
            }).ToList();
            return result;
        }
    }
}
