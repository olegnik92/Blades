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

        [Test]
        public void BasicGetTest()
        {
            var typesMap = new Blades.Basis.TypeIdMap();
            var someClassId = Guid.Parse(SomeClass.Guid);
            var someClassType = typesMap.Get(someClassId);
            Assert.AreEqual(someClassType.FullName, typeof(SomeClass).FullName);

            var id = typesMap.Get(typeof(SomeClass));
            Assert.AreEqual(id, someClassId);
        }


        [Test]
        public void DomainObjectSearchTest()
        {
            var typesMap = new Blades.Basis.TypeIdMap();
            var someDomainObjectId = Guid.Parse(SomeDomainObject.Guid);
            var someDomainObjectType = typesMap.Get(someDomainObjectId);
            Assert.AreEqual(someDomainObjectType.FullName, typeof(SomeDomainObject).FullName);
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
    }

}
