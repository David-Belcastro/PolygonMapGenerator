using Microsoft.VisualStudio.TestTools.UnitTesting;
using CubesFortune.CubesFortune;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubesFortune.CubesFortune.Tests
{
    public class intersectingpoints
    {
        public SiteEvent pt1 = new SiteEvent(1, 1);
        public SiteEvent pt2 = new SiteEvent(3.75, 10);
        public SiteEvent pt3 = new SiteEvent(4, 11);
        public SiteEvent pt4 = new SiteEvent(5, 11);
        public SiteEvent pt5 = new SiteEvent(3, 4);
        public SiteEvent pt6 = new SiteEvent(7, 4);

        public SiteEvent testsite = new SiteEvent(2.5, 9);

        public Arc arc1;
        public Arc arc2;
        public Arc arc3;
        public Arc arc4;
        public Arc arc5;
        public Arc arc6;

        public intersectingpoints()
        {

            arc1 = new Arc(pt1);
            arc2 = new Arc(pt2);
            arc3 = new Arc(pt3);
            arc4 = new Arc(pt4);
            arc5 = new Arc(pt5);
            arc6 = new Arc(pt6);
            arc1.nextpoint = arc2;
            arc2.previouspoint = arc1; arc2.nextpoint = arc3;
            arc3.previouspoint = arc2; arc3.nextpoint = arc4;
            arc4.previouspoint = arc3; arc4.nextpoint = arc5;
            arc5.previouspoint = arc4; arc5.nextpoint = arc6;
            arc6.previouspoint = arc5;
        }
    }

    [TestClass()]
    public class SweepTableTests
    {
        [TestMethod()]
        public void AddToBeachLineTest_AddPointToNonExistantBeachLine_PointisnowBeachline()
        {
            var sut = new SweepTable();
            var testevent = new SiteEvent(1, 1);
            sut.AddToBeachLine(testevent);
            Assert.AreSame(testevent, sut.BeachPoints.arcpoint);
            Assert.AreEqual(1, sut.BeachPoints.arcpoint.X);
        }
        [TestMethod()]
        public void AddToBeachLineTest_AddPointBeachLineWith6Points_NewBeachLinewith3Points()
        {
            var intpoints = new intersectingpoints();

            var sut = new SweepTable { BeachPoints = intpoints.arc3 };
            sut.AddToBeachLine(intpoints.testsite);
            Assert.AreSame(intpoints.testsite, intpoints.arc3.nextpoint.arcpoint);
        }

        [TestMethod()]
        public void AddToBeachLineTest_AddNewPoint_TwoDifferentPointRefs()
        {
            var intpoints = new intersectingpoints();
            var sut = new SweepTable { BeachPoints = intpoints.arc3 };
            sut.AddToBeachLine(intpoints.testsite);
            Assert.IsFalse(ReferenceEquals(sut.BeachPoints.nextpoint.s1, sut.BeachPoints.s1));
        }

        [TestMethod()]
        public void CheckCircleEventCheck_righthandcircle_returnsblankevent()
        {
            var pointa = new SiteEvent(10, 10);
            var pointb = new SiteEvent(20, 15);
            var pointc = new SiteEvent(15, 20);

            var sut = new SweepTable();
            var circlecheck = sut.CalculateCircleEventCheck(pointa, pointb, pointc);
            Assert.IsFalse(circlecheck.isCircle);            
        }

        [TestMethod()]
        public void CheckCircleEventCheck_sitesdifferent_returnscircleincenter()
        {
            var pointa = new SiteEvent(10, 10);
            var pointb = new SiteEvent(20, 15);
            var pointc = new SiteEvent(15, 20);

            var sut = new SweepTable();
            var circlecheck = sut.CalculateCircleEventCheck(pointc, pointb, pointa);
            Assert.AreEqual(14.1666, circlecheck.o.X, 0.0001);
            Assert.AreEqual(14.1666, circlecheck.o.Y, 0.0001);
            Assert.AreEqual(20.05922, circlecheck.x, 0.0001);
        }

        [TestMethod()]
        public void FindIntersectedArcTest_EmptyBreachList_ReturnsNull()
        {
            var sut = new SweepTable();
            Assert.IsFalse(sut.FindIntersectedArc(new SiteEvent(1, 1), null).flag);
        }

        [TestMethod()]
        public void FindIntersectedArcTest_PointIsSameAsInspectedPoint_ReturnsNull()
        {
            var sut = new SweepTable();
            Assert.IsFalse(sut.FindIntersectedArc(new SiteEvent(1, 1), new Arc(new SiteEvent(1, 1))).flag);
        }

        [TestMethod()]
        public void FindIntersectedArcTest_PointIsDifferentAsInspectedPoint_ReturnsSite()
        {
            var intpoints = new intersectingpoints();

            var sut = new SweepTable { BeachPoints = intpoints.arc3 };
            var newsite = sut.FindIntersectedArc(new SiteEvent(2.5, 9), intpoints.arc3);
            Assert.IsTrue(newsite.flag);
        }

        [TestMethod()]
        public void ParabolaIntersectionTest()
        {
            var sut = new SweepTable();
            var varparabolaresult = sut.ParabolaIntersection(new SiteEvent(3.75, 10), new SiteEvent(4, 11), 2.5);
            Assert.AreEqual(-.64579, varparabolaresult.Y, .0001);

            varparabolaresult = sut.ParabolaIntersection(new SiteEvent(4, 11), new SiteEvent(5, 11), 2.5);
            Assert.AreEqual(9.063508, varparabolaresult.Y, .0001);

        }

        [TestMethod()]
        public void CheckForCircleEventTest_pointsAreLinear_ReturnsFalseCheckFlag()
        {
            var sut = new SweepTable();
            var circlecheck = sut.CalculateCircleEventCheck(new SiteEvent(1, 1), new SiteEvent(1, 2), new SiteEvent(1, .5));
            Assert.IsFalse(circlecheck.isCircle);

        }

        [TestMethod()]
        public void CheckForCircleEventTest_PointsAReCircle_REturnsTrueFlagCircleDeets()
        {

            var sut = new SweepTable();
            var circlecheck = sut.CalculateCircleEventCheck(new SiteEvent(1, 1), new SiteEvent(1, 2), new SiteEvent(2, 2));
            Assert.IsTrue(circlecheck.isCircle);
        }

        [TestMethod()]
        public void CheckForCircleEventTest_PointsRightTurnCircle_REturnsFalseFlag()
        {

            var sut = new SweepTable();
            var circlecheck = sut.CalculateCircleEventCheck(new SiteEvent(3, 2), new SiteEvent(2, 2), new SiteEvent(2, 1));
            Assert.IsFalse(circlecheck.isCircle);
        }

        [TestMethod()]
        public void FinishEdgesTest_nowaitingpoints_returnsemptygraph()
        {
            var sut = new SweepTable { BeachPoints = new Arc() } ;
            Assert.IsInstanceOfType(sut.FinishEdges(), typeof(VoronoiMap));
            Assert.AreEqual(0, sut.FinishEdges().graph.Count);
        }

    }
}