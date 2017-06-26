using System.Collections.Generic;
using CubesFortune;
namespace Town_Map_Generator
{
    public class Corners
    {
        public double index;
            public VoronoiPoint location;
        public bool border;
            public List<Centers> touches;
            public List<VoronoiSegment> protrudes;
            public List<Corners> adjacents;

        public Corners(int ind, VoronoiPoint pointloc)
        {
            index = ind;
            location = pointloc;
            border = (pointloc.SafeX <= 0 || pointloc.SafeX >= 1000 || pointloc.SafeY <= 0 || pointloc.SafeY > 1000);
            touches = new List<Centers>();
            protrudes = new List<VoronoiSegment>();
            adjacents = new List<Corners>();
        }
    }
}