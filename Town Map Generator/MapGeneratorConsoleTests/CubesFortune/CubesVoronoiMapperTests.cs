using Microsoft.VisualStudio.TestTools.UnitTesting;
using CubesFortune.CubesFortune;
using System;
using System.Collections.Generic;
using ceometric.DelaunayTriangulator;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubesFortune.CubesFortune.Tests
{
    [TestClass()]
    public class CubesVoronoiMapperTests
    {
        [TestMethod()]
        public void GimmesomeVeoroioisTest_No_input_ReturnsVeronoiMap()
        {
            var mockedMapper = new CubesVoronoiMapper() { };
            Assert.IsInstanceOfType(mockedMapper.GimmesomeVeoroiois(new List<VoronoiPoint> { new VoronoiPoint(1, 1), new VoronoiPoint(2, 2) }), typeof(VoronoiMap));

        }


        [TestMethod()]
        public void GimmesomeVeoroioisTest_4VerticalPointsInALine_ReturnsLineAtMid()
        {
            var mockedMapper = new CubesVoronoiMapper() { };
            var points = new List<VoronoiPoint> { new VoronoiPoint(100, 100), new VoronoiPoint(100, 200), new VoronoiPoint(100, 300), new VoronoiPoint(100, 400) };
            var finalpoints = mockedMapper.GimmesomeVeoroiois(points);
            Assert.AreEqual(3,finalpoints.graph.Count);
            
            for (int i = 0; i < finalpoints.graph.Count; i++)
            {
                Assert.AreEqual((i+1)*100+50, finalpoints.graph[i].start.Y);
                Assert.AreEqual((i + 1) * 100 + 50, finalpoints.graph[i].end.Y);
            }

        }

        [TestMethod()]
        public void GimmesomeVeoroioisTest_4SquarePoints_ReturnsDividedatQuadrents()
        {
            var mockedMapper = new CubesVoronoiMapper() { };
            var points = new List<VoronoiPoint> { new VoronoiPoint(2.5, 2.5), new VoronoiPoint(7.5, 2.5), new VoronoiPoint(7.5, 7.5), new VoronoiPoint(2.5, 7.5) };
            var finalpoints = mockedMapper.GimmesomeVeoroiois(points);
            foreach (VoronoiSegment seg in finalpoints.graph)
            Assert.AreEqual(7, finalpoints.graph.Count);
           // Assert.AreEqual(5.0, finalpoints.graph[0].start.Y);
            //Assert.AreEqual(5.0, finalpoints.graph[0].end.Y);
            

        }
        [TestMethod()]
        public void GimmesomeVoronois_ComplexPoints_ReturnscorrectGrapg()
        {
            var mockedMapper = new CubesVoronoiMapper();
            var points = new List<VoronoiPoint>
            {
                new VoronoiPoint(2.78,.48),
                new VoronoiPoint(5.1, 0.62),
                new VoronoiPoint(6.96, 0.39),
                new VoronoiPoint(7.87, 0.32),
                new VoronoiPoint(8.66, 5.02),
                new VoronoiPoint(8.8, 5.46),
                new VoronoiPoint(8.69, 6.89),
                new VoronoiPoint(9.12, 8.5),
                new VoronoiPoint(8.05, 6.79),
                new VoronoiPoint(5.92, 5.99),
                new VoronoiPoint(5.19, 6.3),
                new VoronoiPoint(3.3, 8.44),
                new VoronoiPoint(3.75, 6.25),
                new VoronoiPoint(1.45, 6.19),
                new VoronoiPoint(1.92, 4.74)

            };
            var expsXlist = new List<double> { -0.551702127659575, -0.551702127659575, -8.20093023255814, -8.20093023255814, -22.7649337923729, 1.00675675675676, 1.00675675675676, 2.59921739130435, 2.59921739130435, 2.61185834919125, 2.57574052132701, 3.93577586206897, 3.93577586206897, 4.46913194444444, 4.46913194444444, 4.42641588740909, 5.48917808219178, 5.48917808219178, 3.79804910232316, 6.01577956989247, 6.01577956989247, 4.55618243942845, 4.51601932045303, 4.516333414813, 7.41230769230769, 7.41230769230769, 6.83476525821596, 6.83476525821596, 6.36267419432613, 7.11830291970803, 7.11830291970803, 8.3621875, 8.3621875, 7.30329219604988, 8.03857142857144, 8.03857142857144, 7.53172026346378, 7.21859813084113, 7.21859813084113, 6.18509153480451, 6.20314016990291, 6.36018316170112, 8.47712589073634, 8.20728985213693, 6.47259947541715, 7.60165374050735, 4.62726070528967, 13.2877777777778, 26.3730219072164, 3.87835548172758 };
            var expsYlist = new List<double> { 4.74, 4.74, 0.48, 0.48, -2.46015095338983, 8.44, 8.44, 6.25, 6.25, 5.76542994766889, 7.14994668246445, 0.62, 0.62, 6.3, 6.3, 7.5302224426183, 5.99, 5.99, 2.90232916150186, 0.39, 0.39, 3.79294574446055, 3.4577381745503, 3.45673307259839, 0.32, 0.32, 6.79, 6.79, 8.04694245760668, 5.02, 5.02, 6.89, 6.89, 5.54254702801719, 5.46, 5.46, 5.62127082526153, 8.5, 8.5, 9.08344532746754, 9.13540351941747, 3.17517687288735, 6.15439429928741, 7.88134494632368, 3.19605418829176, 2.78149862659557, -10.8388916876574, 6.52444444444445, -0.373688788659778, -45.6213787375415 };
            var expeXlist = new List<double> { -22.7649337923729, 2.61185834919125, -22.7649337923729, 3.79804910232316, -10000, 2.57574052132701, -5080.81104569194, 2.61185834919125, 2.57574052132701, 4.51601932045303, 4.42641588740909, 4.62726070528967, 3.79804910232316, 4.55618243942845, 4.42641588740909, 6.18509153480451, 4.55618243942845, 6.36267419432613, 4.516333414813, 4.62726070528967, 6.36018316170112, 4.51601932045303, 4.516333414813, 6.36018316170112, 3.87835548172758, 7.60165374050735, 7.30329219604988, 6.36267419432613, 6.18509153480451, 6.47259947541715, 7.30329219604988, 8.47712589073634, 8.20728985213693, 7.53172026346378, 26.3730219072164, 7.53172026346378, 8.47712589073634, 8.20728985213693, 6.20314016990291, 6.20314016990291, -9.34944784663338, 6.47259947541715, 13.2877777777778, 13.2877777777778, 7.60165374050735, 26.3730219072164, 3.87835548172758, 752.425431267184, 748.159075892528, -43.1589289593671 };
            var expeYlist = new List<double> { -2.46015095338983, 5.76542994766889, -2.46015095338983, 2.90232916150186, -2326.40457092833, 7.14994668246445, 4186.82352645784, 5.76542994766889, 7.14994668246445, 3.4577381745503, 7.5302224426183, -10.8388916876574, 2.90232916150186, 3.79294574446055, 7.5302224426183, 9.08344532746754, 3.79294574446055, 8.04694245760668, 3.45673307259839, -10.8388916876574, 3.17517687288735, 3.4577381745503, 3.45673307259839, 3.17517687288735, -45.6213787375415, 2.78149862659557, 5.54254702801719, 8.04694245760668, 9.08344532746754, 3.19605418829176, 5.54254702801719, 6.15439429928741, 7.88134494632368, 5.62127082526153, -0.373688788659778, 5.62127082526153, 6.15439429928741, 7.88134494632368, 9.13540351941747, 9.13540351941747, 1517.73644112219, 3.19605418829176, 6.52444444444445, 6.52444444444445, 2.78149862659557, -0.373688788659778, -45.6213787375415, -71.2795190807851, -130.969219957175, -1541.99499001867 };



            var finalpoints = mockedMapper.GimmesomeVeoroiois(points);
            var starXlist = new List<double>();
            var starYlist = new List<double>();
            var endyXlist = new List<double>();
            var endyYlist = new List<double>();
            foreach (VoronoiSegment seg in finalpoints.graph)
            {
                starXlist.Add(seg.start.X);
                starYlist.Add(seg.start.Y);
                endyXlist.Add(seg.end.X);
                endyYlist.Add(seg.end.Y);
            }
            for (int i = 0; i< finalpoints.graph.Count; i++)
            {
            Assert.AreEqual(expsXlist[i], starXlist[i],0.0001);
            Assert.AreEqual(expsYlist[i], starYlist[i],0.0001);
            Assert.AreEqual(expeXlist[i], endyXlist[i],0.0001);
            Assert.AreEqual(expeYlist[i], endyYlist[i],0.0001);

            }


        }
    }
}