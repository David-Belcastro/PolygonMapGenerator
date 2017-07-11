using System;
using System.Collections.Generic;
using CubesFortune;
namespace MapGeneratorConsole.ImageGenerators.Graph
{
    public class Centers
    {
        public int index;
        public VoronoiPoint center;
        public List<Centers> neigbors;
        public List<DualGraphVertex> borders;
        public List<Corners> corners; 

        public Centers(int indx, VoronoiPoint polycenter)
        {
            index = indx;
            center = polycenter;
            neigbors = new List<Centers>();
            borders = new List<DualGraphVertex>();
            corners = new List<Corners>();
        }

        internal void AddToNeighbors(Centers d1)
        {
            if (d1 != null && !neigbors.Contains(d1))
            {
                neigbors.Add(d1);
            }
        }

        internal void BordersandCorners(DualGraphVertex dualGraphVertex, Corners v0, Corners v1)
        {
            borders.Add(dualGraphVertex);
            AddToCorners(v0);
            AddToCorners(v1);
        }

        internal void AddToCorners(Corners d1)
        {
            if (d1 != null && !corners.Contains(d1))
            {
                corners.Add(d1);
            }
        }
    }
}