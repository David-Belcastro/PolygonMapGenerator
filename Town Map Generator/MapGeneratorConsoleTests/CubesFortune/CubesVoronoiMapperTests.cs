using Microsoft.VisualStudio.TestTools.UnitTesting;
using MapGeneratorConsole.CubesFortune;
using System;
using System.Collections.Generic;
using ceometric.DelaunayTriangulator;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGeneratorConsole.CubesFortune.Tests
{
    [TestClass()]
    public class CubesVoronoiMapperTests
    {
        [TestMethod()]
        public void GimmesomeVeoroioisTest_No_input_ReturnsVeronoiMap()
        {
            var mockedMapper = new CubesVoronoiMapper() { };
            Assert.IsInstanceOfType(mockedMapper.GimmesomeVeoroiois(new List<Point> { new Point(1, 1,0), new Point(2, 2,0) }), typeof(VoronoiMap));

        }
    }
}