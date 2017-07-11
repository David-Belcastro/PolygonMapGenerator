using System;
using System.Collections.Generic;
using CubesFortune;
namespace MapGeneratorConsole.ImageGenerators.Graph
{
    public class Corners
    {
        public double index;
            public VoronoiPoint location;
        public bool border;
            public List<Centers> touches;
            public List<DualGraphVertex> protrudes;
            public List<Corners> adjacents;

        public Corners(int ind, VoronoiPoint pointloc)
        {
            index = ind;
            location = pointloc;
            border = (pointloc.SafeX <= 0 || pointloc.SafeX >= 1000 || pointloc.SafeY <= 0 || pointloc.SafeY > 1000);
            touches = new List<Centers>();
            protrudes = new List<DualGraphVertex>();
            adjacents = new List<Corners>();
        }

        internal void AddToAdjacents(Corners d1)
        {
            if (d1 != null && !adjacents.Contains(d1))
            {
                adjacents.Add(d1);
            }
        }

        internal void ProtrudesandTouches(DualGraphVertex dualGraphVertex, Centers d0, Centers d1)
        {
            protrudes.Add(dualGraphVertex);
            AddToTouches(d0);
            AddToTouches(d1);
        }

        internal void AddToTouches(Centers d1)
        {
            if (d1 != null && !touches.Contains(d1))
            {
                touches.Add(d1);
            }
        }
    }
}