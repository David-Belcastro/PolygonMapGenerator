﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Town_Map_Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ceometric.DelaunayTriangulator;
using System.Threading.Tasks;
using CubesFortune;
using MapGeneratorConsole.ImageGenerators.Graph;

namespace Town_Map_Generator.Tests
{
    [TestClass()]
    public class PolyMapTests
    {
        [TestMethod()]
        public void MakeCentersTest()
        {
            var points = new List<VoronoiPoint> { new VoronoiPoint(2.5, 2.5), new VoronoiPoint(7.5, 2.5), new VoronoiPoint(7.5, 7.5), new VoronoiPoint(2.5, 7.5), new VoronoiPoint(5, 5) };
            var vmap = new CubesFortune.CubesVoronoiMapper().GimmesomeVeoroiois(points);
            var sut = new PolyMap(vmap, points);
            Assert.AreEqual(6, sut.edgelist.Count);
        }

        [TestMethod()]
        public void MakeCentersTest_Ensure_eachoneisinlist()
        {
            var points = new List<VoronoiPoint> { new VoronoiPoint(2.5, 2.5), new VoronoiPoint(7.5, 2.5), new VoronoiPoint(7.5, 7.5), new VoronoiPoint(2.5, 7.5), new VoronoiPoint(5, 5) };
            var vmap = new CubesFortune.CubesVoronoiMapper().GimmesomeVeoroiois(points);
            var sut = new PolyMap(vmap, points);
            foreach (Edges cnt in sut.edgelist)
            {
                Assert.IsTrue(points.Contains(cnt.delaunayCenter1.center));

                Assert.IsTrue(points.Contains(cnt.delaunayCenter2.center));
            }
        }


    }
}