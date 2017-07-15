using System;
using CubesFortune;
using Town_Map_Generator;

namespace MapGeneratorConsole.ImageGenerators.Graph
{
    public class Edges
    {

        public int Index;
        //DelaunayEdge;
        public Centers delaunayCenter1;
        public Centers delaunayCenter2;

        //VoronoiEdge;
        public Corners voronoiCorner1;
        public Corners voronoiCorner2;
        public VoronoiPoint midpoint;
        public EdgeMapData mapdata;
        
        public Edges(int index, Centers leftCenter, Centers rightCenter, Corners startCorner, Corners endCorner, VoronoiPoint mdpt)
        {
            Index = index;
            delaunayCenter1 = leftCenter;
            delaunayCenter2 = rightCenter;
            voronoiCorner1 = startCorner;
            voronoiCorner2 = endCorner;
            midpoint = mdpt;
            BordersAndProtrudes();
            mapdata = new EdgeMapData();
           
        }
        

        private void BordersAndProtrudes()
        {
            if (delaunayCenter2 != null && delaunayCenter1 != null)
            {
                delaunayCenter1.AddToNeighbors(delaunayCenter2);
                delaunayCenter2.AddToNeighbors(delaunayCenter1);
            }
            if (voronoiCorner2 != null && voronoiCorner1 != null)
            {
                voronoiCorner1.AddToAdjacents(voronoiCorner2);
                voronoiCorner2.AddToAdjacents(voronoiCorner1);
            }
            if (delaunayCenter1 != null) { delaunayCenter1.BordersandCorners(this, voronoiCorner1, voronoiCorner2); }
            if (delaunayCenter2 != null) { delaunayCenter2.BordersandCorners(this, voronoiCorner1, voronoiCorner2); }
            if (voronoiCorner1 != null) { voronoiCorner1.ProtrudesandTouches(this, delaunayCenter1, delaunayCenter2); }
            if (voronoiCorner2 != null) { voronoiCorner2.ProtrudesandTouches(this, delaunayCenter1, delaunayCenter2); }
        }
    }


}
