using System;
using System.Collections.Generic;
using ceometric.DelaunayTriangulator;

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

        public List<Point> Givemepoints(int points)
        {
            var finallist = new List<Point>();
            for (int i = 0; i < points; i++)
            {
                finallist.Add(new Point(Math.Abs(randomizer.NextDouble() * 10), Math.Abs(randomizer.NextDouble() * 10), 0));
            }
            return finallist;
        }
    }
}