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
    public class IslandGeneratorTests
    {
        [TestMethod()]
        public void CoordtoCenterTest_Send10_Get1()
        {
            var sut = new IslandGenerator(1);
            Assert.AreEqual(1, sut.GetStdCoord(10.0));
        }

        [TestMethod()]
        public void CoordtoCenterTest_Send5_Get0()
        {
            var sut = new IslandGenerator(1);
            Assert.AreEqual(0, sut.GetStdCoord(5.0));
        }

        [TestMethod()]
        public void CoordtoCenterTest_Send0_Getnegative1()
        {
            var sut = new IslandGenerator(1);
            Assert.AreEqual(-1, sut.GetStdCoord(0.0));
        }
    }
}