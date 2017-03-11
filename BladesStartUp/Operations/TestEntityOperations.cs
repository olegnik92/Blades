using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BladesStartUp.Domain.TestEntity;
using Blades.DataStore.Es;
using Blades.Core;
using Blades.Auth.Interfaces;
using Blades.DataStore.Interfaces;
using BladesStartUp.Domain.Commands;
using Blades.Commands.Interfaces;

namespace BladesStartUp.Operations
{
    [Operation("SaveTestEntity", OperationType.Command, "CreateTestEntity")]
    public class SaveTestEntity : TransactOperation<TestEntityState, Guid, EsRepository>
    {
        public SaveTestEntity(IPermissionRequirementChecker checker, ITransactRepositoryFactory repoFactory) : base(checker, repoFactory)
        {
        }

        public override Guid Transaction(EsRepository repo, out OperationExecutionReport executionReport)
        {
            var resource = TestEntityAr.CreateBaseResource();
            resource.InstanceDescription = Data.Name;
            executionReport = new OperationExecutionReport();
            executionReport.AssociatedResources.Add(resource);
            if (Data.Id == Guid.Empty)
            {                
                resource.InstanceId = Guid.NewGuid();
                executionReport.ReportStrings.Add("Create");

                var entity = new TestEntityAr(repo, resource);
                Data.Id = resource.InstanceId;
                var createEvent = new CreateEvent() { State = Data };
                entity.Mutate(createEvent);
            }
            else
            {
                resource.InstanceId = Data.Id;
                executionReport.ReportStrings.Add("Modify");

                var entity = new TestEntityAr(repo, resource);
                var changeEvent = new ChangeEvent() { NewState = Data };
                entity.Mutate(changeEvent);
            }

            return resource.InstanceId;
        }
    }

    [Operation("AlterTestEntity", OperationType.Command, "AlterTestEntity")]
    public class AlterTestEntity : TransactOperation<Guid, int, EsRepository>
    {
        public AlterTestEntity(IPermissionRequirementChecker checker, ITransactRepositoryFactory repoFactory) : base(checker, repoFactory)
        {
        }

        public override int Transaction(EsRepository repo, out OperationExecutionReport executionReport)
        {
            var resource = TestEntityAr.CreateBaseResource();
            resource.InstanceId = Data;
            executionReport = new OperationExecutionReport();
            executionReport.AssociatedResources.Add(resource);
            var entity = new TestEntityAr(repo, resource);

            var alterEvent = new AlterNumEvent();
            entity.Mutate(alterEvent);

            return entity.State.Num;
        }
    }

    [Operation("TestEntityTestCommand", OperationType.Command, "TestEntityTestCommand")]
    public class TestEntityTestCommand : CommandOperation<SaveTestEntityCommand>
    {
        public TestEntityTestCommand(ICommandEmitter emitter, IPermissionRequirementChecker checker) : base(emitter, checker)
        {
            isAsync = true;
        }

        public override List<Error> GetDataValidationErrors()
        {
            return base.GetDataValidationErrors();
        }
    }

}
