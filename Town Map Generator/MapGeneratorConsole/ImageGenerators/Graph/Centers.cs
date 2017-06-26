using System.Collections.Generic;
using CubesFortune;
namespace Town_Map_Generator
{
    public class Centers
    {
        public int index;
        public VoronoiPoint center;
        public List<Centers> neigbors;
        public List<VoronoiSegment> borders;
        public List<Corners> corners; 

        public Centers(int indx, VoronoiPoint polycenter)
        {
            index = indx;
            center = polycenter;
            neigbors = new List<Centers>();
            borders = new List<VoronoiSegment>();
            corners = new List<Corners>();
        }
    }
}