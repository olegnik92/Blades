using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Blades.Core;

namespace Blades.Tests.Core
{
    [TestFixture]
    public class TypeIdMapTests
    {

        private void SearchTest(Type type, string guid)
        {
            var id = Guid.Parse(guid);
            var typesMap = new Blades.Basis.TypeIdMap();
            var actualType = typesMap.Get(id);
            Assert.AreEqual(type.FullName, actualType.FullName);

            var actualId = typesMap.Get(type);
            Assert.AreEqual(id, actualId);
        }


        [Test]
        public void BasicSearchTest()
        {
            SearchTest(typeof(SomeClass), SomeClass.Guid);
        }


        [Test]
        public void DomainObjectSearchTest()
        {
            SearchTest(typeof(SomeDomainObject), SomeDomainObject.Guid);
        }

        [Test]
        public void OperationSearchTest()
        {
            SearchTest(typeof(SomeOperation), SomeOperation.Guid);
        }


        [TypeId(Guid)]
        public class SomeClass
        {
            public const string Guid = "{4D6EE994-2C02-4FAB-AFC9-B813A193945A}";

        }

        [DomainObject(Guid, "Доменный объект")]
        public class SomeDomainObject
        {
            public const string Guid = "{CA710F10-683E-4466-86F3-C2A321AEB18A}";
        }

        [Operation(Guid, "Операция")]
        public class SomeOperation : Operation<int, int>
        {
            public const string Guid = "{F0FCB5C2-3144-4140-8491-5570ADA2D3F3}";

            public override int Execute(out OperationExecutionReport executionReport)
            {
                executionReport = new OperationExecutionReport();
                return Data;
            }
        }
    }

}
