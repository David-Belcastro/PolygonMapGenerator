using Microsoft.VisualStudio.TestTools.UnitTesting;
using Town_Map_Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Town_Map_Generator.Tests
{
    [TestClass()]
    public class PointGeneratorTests
    {
        [TestMethod()]
        public void GivemepointsTest_ReturnsListSameSizeAsPointsGiven()
        {
            var sut = new PointGenerator(12345);
            Assert.AreEqual(10, sut.Givemepoints(10).Count);
        }
    }
}