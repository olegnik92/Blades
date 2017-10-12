using System;
using Blades.Core;
using Blades.Core.Services.Basis;
using Xunit;

namespace BladesTests.Core
{

    public class TypeIdMapTests
    {        
        [Fact]
        public void TypeSearchTest()
        {
            TestTypeRegistration(typeof(TypeSearchTest_Class), TypeSearchTest_Class.TypeId);
        }



        private void TestTypeRegistration(Type type, string typeId)
        {
            var typeMap = new TypeMap();
            var id = Guid.Parse(typeId);

            var actualType = typeMap.Get(id);
            Assert.Equal(type.FullName, actualType.FullName);

            var actualId = typeMap.Get(type);
            Assert.Equal(id, actualId);
        }

        
        [TypeId(TypeId)]
        public class TypeSearchTest_Class
        {
            public const string TypeId = "{EAC1DF8B-3F5D-4FE6-89FB-B180163C7086}";
        }
    }
}