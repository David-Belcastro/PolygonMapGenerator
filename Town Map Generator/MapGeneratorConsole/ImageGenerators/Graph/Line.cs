using CubesFortune;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGeneratorConsole.ImageGenerators.Graph
{
    public class Line
    {
        public VoronoiPoint p0;
        public VoronoiPoint p1;

        public Line(VoronoiPoint leftpoint, VoronoiPoint rightpoint)
        {
            this.p0 = leftpoint;
            this.p1 = rightpoint;
        }

        public VoronoiPoint MidPoint()
        {
            var x = (p0.X + p1.X) / 2;

            var y = (p0.Y + p1.Y) / 2;

            return new VoronoiPoint(x, y);
        }
    }
}
