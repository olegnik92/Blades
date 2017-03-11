using Blades.Es;
using Blades.Es.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blades.Core;

namespace BladesStartUp.Domain.TestEntity
{
    [AggregateRoot(ResourceTypeId, ResourceTypeDescription, 5)]
    public class TestEntityAr: AggregateRoot<TestEntityState>
        , IMutationHandler<TestEntityState, CreateEvent>
        , IMutationHandler<TestEntityState, ChangeEvent>
        , IMutationHandler<TestEntityState, AlterNumEvent>
    {
        public const string ResourceTypeId = "{00DDACD0-8498-44E6-A50F-9A7FEAB7E546}";
        public const string ResourceTypeDescription = "Test entity";

        public static Resource CreateBaseResource()
        {
            var resource = new Resource
            {
                TypeId = Guid.Parse(ResourceTypeId),
                TypeDescription = ResourceTypeDescription,
            };

            return resource;
        }

        public TestEntityAr(IEsRepository repo, Resource resource) : base(repo, resource)
        {
        }

        public TestEntityState Apply(TestEntityState state, AlterNumEvent mutation)
        {
            state.Num = -state.Num;
            return state;
        }

        public TestEntityState Apply(TestEntityState state, ChangeEvent mutation)
        {
            mutation.NewState.Id = state.Id;
            return mutation.NewState;
        }

        public TestEntityState Apply(TestEntityState state, CreateEvent mutation)
        {
            return mutation.State;
        }


    }
}
