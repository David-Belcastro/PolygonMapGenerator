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
        public VoronoiPoint startleft;
        public VoronoiPoint endright;

        public Line(VoronoiPoint leftpoint, VoronoiPoint rightpoint)
        {
            startleft = leftpoint;
            endright = rightpoint;
        }

        public VoronoiPoint MidPoint()
        {
            var x = (startleft.X + endright.X) / 2;

            var y = (startleft.Y + endright.Y) / 2;

            return new VoronoiPoint(x, y);
        }
    }
}
