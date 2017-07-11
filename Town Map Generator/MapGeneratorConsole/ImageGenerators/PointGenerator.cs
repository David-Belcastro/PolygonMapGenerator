using System;
using System.Collections.Generic;
using ceometric.DelaunayTriangulator;
using CubesFortune;

namespace Town_Map_Generator
{
    public class PointGenerator
    {
        private int seed;
        private Random randomizer;

        public PointGenerator(int seed)
        {
            this.seed = seed;
            randomizer = new Random(seed);
        }

        public List<VoronoiPoint> Givemepoints(int points)
        {
            var finallist = new List<VoronoiPoint>();
            for (int i = 0; i < points; i++)
            {
                finallist.Add(new VoronoiPoint(Math.Abs(randomizer.NextDouble() * 10), Math.Abs(randomizer.NextDouble() * 10)));
            }
            return finallist;
        }
    }
}