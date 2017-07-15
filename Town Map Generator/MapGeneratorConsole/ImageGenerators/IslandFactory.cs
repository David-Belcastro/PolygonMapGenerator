using System;
using System.Security.Cryptography;


namespace Town_Map_Generator
{
     public class IslandFactory
    {
         public double islandfactor = 1.07;
        private int seed;

        public IslandFactory(int seed)
        {
            this.seed = seed;

        }

        public bool CheckForLand(double stdX, double stdY)
        {
            var random = new Random(seed);
            var bumps = random.Next(1, 7);
            var startAngle = random.NextDouble() * 2 * Math.PI;
            var dipAngle = random.NextDouble() * 2 * Math.PI;
            var dipWidth = random.NextDouble() * .5 + .2;

            var angle = Math.Atan2(stdY, stdX);
            var length = 0.5 * (Math.Max(Math.Abs(stdX), Math.Abs(stdY)) + DistanceFromCenter(stdX, stdY));

            var r1 = .5 + .4 * Math.Sin(startAngle + bumps * angle + Math.Cos((bumps + 3) * angle));
            var r2 = .7 * .2 * Math.Sin(startAngle + bumps * angle - Math.Sin((bumps + 2) * angle));
            if (Math.Abs(angle-dipAngle) < dipWidth || Math.Abs(angle-dipAngle+(2*Math.PI)) < dipWidth || Math.Abs(angle - dipAngle - (2 * Math.PI)) < dipWidth)
            {
                r1 = r2 = .2;
            }
            return (length < r1 || length > r1 * islandfactor && length < r2
                );
        }

        private  double DistanceFromCenter(double stdX, double stdY)
        {
            return Math.Sqrt(Math.Pow(stdX, 2) + Math.Pow(stdY, 2));
        }
    }
}