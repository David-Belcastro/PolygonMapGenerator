using Microsoft.VisualStudio.TestTools.UnitTesting;
using Town_Map_Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ceometric.DelaunayTriangulator;
using System.Threading.Tasks;

namespace Town_Map_Generator.Tests
{
    [TestClass()]
    public class PolyMapTests
    {
        [TestMethod()]
        public void MakeCentersTest()
        {
            var points = new List<Point> { new Point(2.5, 2.5, 0), new Point(7.5, 2.5, 0), new Point(7.5, 7.5, 0), new Point(2.5, 7.5, 0), new Point(5,5,0) };
            var vmap = new CubesFortune.CubesVoronoiMapper().GimmesomeVeoroiois(points);
            var sut = new PolyMap(vmap);
            Assert.AreEqual(5, sut.polys.Count);
            Assert.AreEqual(4, sut.polys.Find(x => x.center.X == 5 && x.center.Y == 5).borders.Count);
        }
    }
}