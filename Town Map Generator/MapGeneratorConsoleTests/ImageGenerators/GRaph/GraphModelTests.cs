using Microsoft.VisualStudio.TestTools.UnitTesting;
using Town_Map_Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ceometric.DelaunayTriangulator;
using System.Threading.Tasks;
using CubesFortune;
using MapGeneratorConsole.ImageGenerators.Graph;

namespace MapGeneratorConsoleTests.ImageGenerators.GRaphTests
{
    [TestClass()]
   public class GraphModelTests
    {
        [TestMethod()]
        public void CornerEquality_SameCorners_HashTheSameasLocation()
        {
            var mainlocation = new VoronoiPoint(5, 7);
            var sut = new Corners(2, mainlocation);
            Assert.IsTrue(mainlocation.GetHashCode() == sut.GetHashCode());
        }

        [TestMethod()]
        public void CornerEquality_SameCorners_SameLocObjectsEqual_ReturnsTrue()
        {
            var mainlocation = new VoronoiPoint(5, 7);
            var sut = new Corners(2, mainlocation);
            var test = new Corners(7, mainlocation);
            Assert.IsTrue(sut.Equals(test));
        }
        [TestMethod()]
        public void CornerEquality_SameCorners_DiffLocObjectsEqual_ReturnsTrue()
        {
            var mainlocation = new VoronoiPoint(5, 7);
            var sut = new Corners(2, mainlocation);
            var test = new Corners(7, new VoronoiPoint(5, 7));
            Assert.IsTrue(sut.Equals(test));
        }

        [TestMethod()]
        public void CornerEquality_SameCorners_DiffLocObjectsNotEqual_ReturnsFalse()
        {
            var mainlocation = new VoronoiPoint(5, 7);
            var sut = new Corners(2, mainlocation);
            var test = new Corners(7, new VoronoiPoint(7, 5));
            Assert.IsFalse(sut.Equals(test));
        }
        [TestMethod()]
        public void CornerEquality_SameCorners_SameLocObjectsNotEqual_ReturnsTrue()
        {
            var mainlocation = new VoronoiPoint(5, 7);
            var sut = new Corners(2, mainlocation);
            mainlocation.X = 6;
            var test = new Corners(7, mainlocation);
            Assert.IsTrue(sut.Equals(test));
        }


        [TestMethod()]
        public void CenterEquality_SameCenters_HashTheSameasLocation()
        {
            var mainlocation = new VoronoiPoint(5, 7);
            var sut = new Centers(2, mainlocation);
            Assert.IsTrue(mainlocation.GetHashCode() == sut.GetHashCode());
        }

        [TestMethod()]
        public void CenterEquality_SameCenters_SameLocObjectsEqual_ReturnsTrue()
        {
            var mainlocation = new VoronoiPoint(5, 7);
            var sut = new Centers(2, mainlocation);
            var test = new Centers(7, mainlocation);
            Assert.IsTrue(sut.Equals(test));
        }
        [TestMethod()]
        public void CenterEquality_SameCenters_DiffLocObjectsEqual_ReturnsTrue()
        {
            var mainlocation = new VoronoiPoint(5, 7);
            var sut = new Centers(2, mainlocation);
            var test = new Centers(7, new VoronoiPoint(5, 7));
            Assert.IsTrue(sut.Equals(test));
        }

        [TestMethod()]
        public void CenterEquality_SameCenters_DiffLocObjectsNotEqual_ReturnsFalse()
        {
            var mainlocation = new VoronoiPoint(5, 7);
            var sut = new Centers(2, mainlocation);
            var test = new Centers(7, new VoronoiPoint(7, 5));
            Assert.IsFalse(sut.Equals(test));
        }
        [TestMethod()]
        public void CenterEquality_SameCenters_SameLocObjectsNotEqual_ReturnsTrue()
        {
            var mainlocation = new VoronoiPoint(5, 7);
            var sut = new Centers(2, mainlocation);
            mainlocation.X = 6;
            var test = new Centers(7, mainlocation);
            Assert.IsTrue(sut.Equals(test));
        }
    }

}

