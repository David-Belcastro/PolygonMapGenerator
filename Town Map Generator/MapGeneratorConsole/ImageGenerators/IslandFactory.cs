using CubesFortune;
using System;
using System.Security.Cryptography;


namespace Town_Map_Generator
{
     public class IslandFactory
    {
         public double islandfactor = 1.75;
        private int seed;
        private int bumps;
        private double startAngle;
        private double dipAngle;
        private double dipWidth;
        private double dipdepth;
        private Random random;

        private VoronoiPoint[,] _gradient;
        public double IslandSize = 1.2;
        public int TendrilFactor = 4;

        public IslandFactory(int seed)
        {
            this.seed = seed;
            random = new Random(seed);
            bumps = random.Next(1,TendrilFactor);
            startAngle = random.NextDouble() * 2.0 * Math.PI;
            dipAngle   = random.NextDouble() * 2.0 * Math.PI;
            dipWidth   = random.NextDouble() * .5 + .2;
            dipdepth = random.NextDouble() * .3;
            _gradient = GenerateNoiseGradient(255);
          //  System.Diagnostics.Debug.WriteLine("Bumps: {0}, startangle: {1}, dipangle: {2}, dipwidth: {3}", bumps, startAngle, dipAngle, dipWidth);
        }

        private VoronoiPoint[,] GenerateNoiseGradient(int v)
        {
            var gradient = new VoronoiPoint[v, v];
            for (int i = 0; i < v; i++)
            {
                for (int f = 0; f < v; f++)
                {
                    var angle = random.NextDouble() * 2 * Math.PI;
                    gradient[i, f] = new VoronoiPoint(Math.Cos(angle), Math.Sin(angle));
                }
            }
            return gradient;
        }

        public bool RadialLand(double stdX, double stdY, double variant)
        {
            var angle = Math.Atan2(stdY, stdX);
            var length = 
                1 * (Math.Max(Math.Abs(stdX), Math.Abs(stdY)) + DistanceFromCenter(stdX, stdY));

            var r1 = IslandSize + .4 * Math.Sin(startAngle + bumps * angle + Math.Cos((bumps + 3) * angle));
            var r2 = (IslandSize +.2) - .2 * Math.Sin(startAngle + bumps * angle - Math.Sin((bumps + 2) * angle));
            if (Math.Abs(angle-dipAngle) < dipWidth || Math.Abs(angle-dipAngle+(2*Math.PI)) < dipWidth || Math.Abs(angle - dipAngle - (2 * Math.PI)) < dipWidth)
            {
                r1 = r2 = dipdepth;
            }
           return ((length < r1 )|| (length > r1 * islandfactor && length < r2)
                );
        }

        private  double DistanceFromCenter(double stdX, double stdY)
        {
            return Math.Sqrt(Math.Pow(stdX, 2) + Math.Pow(stdY, 2));
        }


        public double PerlinLand(double stdX, double stdY)
        {
            double lerp(double a0, double a1, double w)
            {
                return (1.0 - w) * a0 + w * a1;
            }

            double dotGridGradient(int ix, int iy, double x, double y)
            {
                // Compute the distance vector
                double dx = x - (double)ix;
                double dy = y - (double)iy;

                // Compute the dot-product
                return (dx * _gradient[iy,ix].X + dy * _gradient[iy,ix].Y);
            }

            var length = DistanceFromCenter(stdX, stdY);

            // Determine grid cell coordinates
            int x0 = Math.Min(Math.Max((int)Math.Floor(stdX),0),254);
            int x1 = Math.Min(x0 + 1,254);
            int y0 = Math.Min(Math.Max((int)Math.Floor(stdY), 0),254);
            int y1 = Math.Min(x0 + 1, 254); ;

            // Determine interpolation weights
            // Could also use higher order polynomial/s-curve here
            double sx = stdX - (double)x0;
            double sy = stdY - (double)y0;

            // Interpolate between grid point gradients
            double n0, n1, ix0, ix1, value;
            n0 = dotGridGradient(x0, y0, stdX, stdY);
            n1 = dotGridGradient(x1, y0, stdX, stdY);
            ix0 = lerp(n0, n1, sx);
            n0 = dotGridGradient(x0, y1, stdX, stdY);
            n1 = dotGridGradient(x1, y1, stdX, stdY);
            ix1 = lerp(n0, n1, sx);
            value = lerp(ix0, ix1, sy);
            return value;
            //return value > (.3+.3*length*length);
        }
    }
}