using Microsoft.VisualStudio.TestTools.UnitTesting;
using CubesFortune.CubesFortune;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubesFortune.CubesFortune.Tests
{
    [TestClass()]
    public class VoronoiSegmentTests
    {
        [TestMethod()]
        public void CalculateSlopeAndInterceptTest()
        {
            var sut = new VoronoiSegment(-11048.960017837, 1.924694284, new Arc(new SiteEvent(new VoronoiPoint(1, 1))), new Arc(new SiteEvent(new VoronoiPoint(1, 1))), 7);
            sut.finish(-35559885627.6882, -10327767.6276044, 8);
            Assert.AreEqual(0.000290433, sut.m, .000000001);
            Assert.AreEqual(5.133679529, sut.b, .0000001);
        }

        [TestMethod()]
        public void SetXAndYTest_BiggerX_ReturnsNormalizedX()
        {
            var sut = new VoronoiSegment(-11048.960017837, 1.924694284, new Arc(new SiteEvent(new VoronoiPoint(1, 1))), new Arc(new SiteEvent(new VoronoiPoint(1, 1))), 7);
            sut.finish(35559885627.6882, 10327767.6276044, 8);
            Assert.AreEqual(-10000, sut.start.X);
            Assert.AreEqual(2.229347, sut.start.Y, .00001);
            Assert.AreEqual(10000, sut.end.X);
            Assert.AreEqual(8.038006, sut.end.Y,.00001);
        }

        [TestMethod()]
        public void SetXAndYTest_BiggerY_ReturnsNormalizedY()
        {
            var sut = new VoronoiSegment( 1.924694284, -11048.960017837, new Arc(new SiteEvent(new VoronoiPoint(1, 1))), new Arc(new SiteEvent(new VoronoiPoint(1, 1))), 7);
            sut.finish(10327767.6276044, 35559885627.6882,  8);
            Assert.AreEqual(-10000, sut.start.Y);
            Assert.AreEqual(2.229346837, sut.start.X, .00001);
            Assert.AreEqual(10000, sut.end.Y);
            Assert.AreEqual(8.038006, sut.end.X, .00001);
        }

        [TestMethod()]
        public void getlimitTest()
        {
            var sut = new VoronoiSegment(-11048.960017837, 1.924694284, new Arc(new SiteEvent(new VoronoiPoint(1, 1))), new Arc(new SiteEvent(new VoronoiPoint(1, 1))), 7);
            Assert.AreEqual(10000, sut.getlimit(1));
            Assert.AreEqual(-10000, sut.getlimit(-1));
        }

        [TestMethod()]
        public void GetXCoordTest()
        {
            var sut = new VoronoiSegment(-11048.960017837, 1.924694284, new Arc(new SiteEvent(new VoronoiPoint(1, 1))), new Arc(new SiteEvent(new VoronoiPoint(1, 1))), 7);
            sut.finish(35559885627.6882, 10327767.6276044, 8);
            Assert.AreEqual(34413679.00766, sut.GetXCoord(10000), .0001);

        }

        [TestMethod()]
        public void GetYCoordTest()
        {
            var sut = new VoronoiSegment(-11048.960017837, 1.924694284, new Arc(new SiteEvent(new VoronoiPoint(1, 1))), new Arc(new SiteEvent(new VoronoiPoint(1, 1))), 7);
            sut.finish(35559885627.6882, 10327767.6276044, 8);
            Assert.AreEqual(8.038006, sut.GetYCoord(10000),.0001);
        }
    }
}