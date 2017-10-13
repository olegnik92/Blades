using System;
using Blades.Core;
using Blades.Core.ServicesBase;
using Xunit;

namespace BladesTests.Core
{
    public class TypeMapTests
    {        
        [Fact]
        public void TypeSearchTest()
        {
            TestTypeRegistration(typeof(TypeSearchTest_Class), TypeSearchTest_Class.TypeId);
        }

        [Fact]
        public void DomainObjectSearchTest()
        {
            TestTypeRegistration(typeof(DomainObjectSearchTest_Class), DomainObjectSearchTest_Class.TypeId);
        }


        private void TestTypeRegistration(Type type, string typeId)
        {
            var typeMap = new TypeMap();
            var id = Guid.Parse(typeId);

            var actualType = typeMap.Get(id);
            Assert.Equal(type.FullName, actualType.FullName);
        }

        
        
        
        [TypeLabel(TypeId)]
        public class TypeSearchTest_Class
        {
            public const string TypeId = "{EAC1DF8B-3F5D-4FE6-89FB-B180163C7086}";
        }
        
        [DomainObject(TypeId, "Some name")]
        public class DomainObjectSearchTest_Class : IDomainObject
        {
            public const string TypeId = "{5EF21F92-53EB-4EB7-B8A7-0C0FEF3C1DC3}";
            
            public Guid Id { set; get; }
            
            public string Name { set; get; }
        }
        
        
    }
}