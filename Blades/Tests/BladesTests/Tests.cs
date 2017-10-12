using System;
using Xunit;

namespace BladesTests
{
    public class Tests
    {
        [Fact]
        public void Test1()
        {
            Assert.Equal(5, Blades.Class1.DoSmth());
        }
    }
}