using System;
using CubesFortune;
using Town_Map_Generator;

namespace MapGeneratorConsole.ImageGenerators.Graph
{
    public class DualGraphVertex
    {

        public int Index;
        //DelaunayEdge;
        public Centers d0;
        public Centers d1;

        //VoronoiEdge;
        public Corners v0;
        public Corners v1;
        public VoronoiPoint midpoint;
        
        public DualGraphVertex(int index, Centers cnt0, Centers cnt1, Corners crn0, Corners crn1, VoronoiPoint mdpt)
        {
            Index = index;
            d0 = cnt0;
            d1 = cnt1;
            v0 = crn0;
            v1 = crn1;
            midpoint = mdpt;
            BordersAndProtrudes();
           
        }

        private void BordersAndProtrudes()
        {
            if (d0 != null) { d0.BordersandCorners(this, v0, v1); }
            if (d1 != null) { d1.BordersandCorners(this, v0, v1); }
            if (v0 != null) { v0.ProtrudesandTouches(this, d0, d1); }
            if (v1 != null) { v1.ProtrudesandTouches(this, d0, d1); }
            if (d1 != null && d0 != null)
            {
                d0.AddToNeighbors(d1);
                d1.AddToNeighbors(d0);
            }
            if (v1 != null && v0 != null)
            {
                v0.AddToAdjacents(v1);
                v1.AddToAdjacents(v0);
            }
        }
    }


}
