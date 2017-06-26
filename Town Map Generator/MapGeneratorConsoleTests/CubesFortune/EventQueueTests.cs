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
    public class EventQueueTests
    {
        [TestMethod()]
        public void AddNodeTest_AddToEmpty_ListHasOneObject()
        {
            var mockedQueue = new EventQueue();
            mockedQueue.AddNode(new SiteEvent(1, 1));
            Assert.AreEqual(1, mockedQueue.NodeList.Count());
        }

        [TestMethod()]
        public void AddNodeTest_AddSmaller_NewPointisatBeginning()
        {
            var mockedQueue = new EventQueue { NodeList = new List<IVoronoiPoint> { new SiteEvent(5, 5) } };
            mockedQueue.AddNode(new SiteEvent(1, 1));
            Assert.AreEqual(1, mockedQueue.NodeList[0].Y);
        }

        [TestMethod()]
        public void AddNodeTest_AddLarger_NewPointisatEnd()
        {
            var mockedQueue = new EventQueue { NodeList = new List<IVoronoiPoint> { new SiteEvent(5, 5) } };
            mockedQueue.AddNode(new SiteEvent(10, 10));
            Assert.AreEqual(10, mockedQueue.NodeList[1].Y);
        }

        [TestMethod()]
        public void RemoveSmallestTest_YTest_Listof3TurnsIntoListOf2()
        {

            var mockedQueue = new EventQueue();

            mockedQueue.AddNode(new SiteEvent(3, 5));
            mockedQueue.AddNode(new SiteEvent(7, 5));
            mockedQueue.AddNode(new SiteEvent(1, 2));
            mockedQueue.RemoveSmallest();
            Assert.AreEqual(2, mockedQueue.NodeList.Count());
            Assert.AreEqual(5, mockedQueue.NodeList[0].Y);
        }

        [TestMethod()]
        public void RemoveSmallestTest_XTest_Listof3TurnsIntoListOf2()
        {

            var mockedQueue = new EventQueue();

            mockedQueue.AddNode(new SiteEvent(3, 5));
            mockedQueue.AddNode(new SiteEvent(7, 5));
            mockedQueue.AddNode(new SiteEvent(1, 5));
            mockedQueue.RemoveSmallest();
            Assert.AreEqual(2, mockedQueue.NodeList.Count());
            Assert.AreEqual(3, mockedQueue.NodeList[0].X);
        }
    }
}