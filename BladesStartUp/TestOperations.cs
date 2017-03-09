using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Core;
using Blades.Auth;
using Blades.Auth.Interfaces;
using Blades.Interfaces;

namespace BladesStartUp
{

    [Operation("TestOperation.EchoOperation", OperationType.Query, "Тестовая операция: echo")]
    public class EchoOperation : Operation<int, int>
    {
        public override int Execute(out OperationExecutionReport executionReport)
        {
            executionReport = new OperationExecutionReport("OK");
            return this.Data;
        }
    }




    public class ComplexDataType
    {
        public int SomeInt { get; set; }

        public string SomeString { get; set; }

        public bool SomeBool { get; set; }

        public DateTime Time { get; set; }

        public List<Tuple<int, string, bool>> ListData { get; set; }
    }


    [Operation("TestOperation.ComplexDataOperation", OperationType.Query, "Тестовая операция: ComplexData")]
    public class ComplexDataOperation : Operation<ComplexDataType, ComplexDataType>
    {
        public override ComplexDataType Execute(out OperationExecutionReport executionReport)
        {
            executionReport = new OperationExecutionReport("OK");
            var result = new ComplexDataType
            {
                SomeInt = 2 * Data.SomeInt,
                SomeString = Data.SomeString.ToUpper(),
                SomeBool = !Data.SomeBool,
                Time = Data.Time.AddYears(1),
                ListData = Data.ListData.Select(t => Tuple.Create(t.Item1 - 1, t.Item2.ToLower(), !t.Item3)).ToList()
            };
            return result;
        }
    }


    [Operation("TestOperation.FailedOperation", OperationType.Query, "Тестовая операция: FailedOperation")]
    public class FailedOperation : Operation<int, int>
    {
        public override int Execute(out OperationExecutionReport executionReport)
        {
            if(Data < 0)
            {
                throw new ArgumentException("Data < 0");
            }

            executionReport = new OperationExecutionReport("Partial Complite");
            if(Data > 0)
            {
                throw new ArgumentException("Data > 0");
            }

            executionReport.ReportStrings[0] = "Full Complite";
            return 0;
        }
    }

    [Operation("TestOperation.AuthFailedOperation", OperationType.Query, "Тестовая операция: AuthFailedOperation")]
    public class AuthFailedOperation : PermissionedOperation<int, int>
    {
        public AuthFailedOperation(IPermissionRequirementChecker checker) : base(checker)
        {
        }

        public override int Execute(out OperationExecutionReport executionReport)
        {
            executionReport = new OperationExecutionReport("OK");
            return this.Data;
        }
    }

    [Operation("TestOperation.PermissionedFailedOperation", OperationType.Command, "Тестовая операция: PermissionedFailedOperation")]
    public class PermissionedFailedOperation : PermissionedOperation<int, int>
    {
        public PermissionedFailedOperation(IPermissionRequirementChecker checker) : base(checker)
        {
        }

        protected override IEnumerable<PermissionRequirement> GetPermissionRequirements()
        {
            return new PermissionRequirement[]
            {
                new PermissionRequirement
                {
                    Recource = new Resource { TypeId = Guid.Empty, TypeDescription = "Тестовый тип", InstanceDescription="Тестовый объект"},
                    Requirement = PermissionType.Delete | PermissionType.Update
                }
            };
        }

        public override int Execute(out OperationExecutionReport executionReport)
        {
            executionReport = new OperationExecutionReport("OK");
            return this.Data;
        }
    }

    [Operation("TestOperation.WebSocketOperation", OperationType.Query, "Тестовая операция: WebSocketOperation")]
    public class WebSocketOperation : PermissionedOperation<Tuple<int, string, DateTime>, object>
    {
        private IUsersNotifier notifier;
        public WebSocketOperation(IPermissionRequirementChecker checker, IUsersNotifier notifier) : base(checker)
        {
            this.notifier = notifier;
        }

        public override object Execute(out OperationExecutionReport executionReport)
        {
            executionReport = new OperationExecutionReport("OK");
            var data = Tuple.Create(Data.Item1 * 2, Data.Item2.ToLower(), Data.Item3.AddYears(5));
            var message = new NotifyMessage<Tuple<int, string, DateTime>>() { Data = data, Name = "TestWsOp" };
            notifier.SendMessage(User.Id, message);
            return null;
        }
    }

}
