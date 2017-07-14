using System;
using System.Collections.Generic;
using CubesFortune;
using System.Linq;
namespace MapGeneratorConsole.ImageGenerators.Graph
{
    public class Corners : IEquatable<Corners>
    {
        public double index;
        public VoronoiPoint location;
        public bool border;
        public List<Centers> touches;
        public List<Edges> protrudes;
        public List<Corners> adjacents;

        public Corners(int ind, VoronoiPoint pointloc)
        {
            index = ind;
            location = pointloc;
            border = (pointloc.X <= 0 || pointloc.X >= 1000 || pointloc.Y <= 0 || pointloc.Y > 1000);
            touches = new List<Centers>();
            protrudes = new List<Edges>();
            adjacents = new List<Corners>();
        }

        internal void AddToAdjacents(Corners d1)
        {
            if (d1 != null && !adjacents.Any(x => x.Equals(d1)))
            {
                adjacents.Add(d1);
            }
        }

        internal void ProtrudesandTouches(Edges dualGraphVertex, Centers d0, Centers d1)
        {
            protrudes.Add(dualGraphVertex);
            AddToTouches(d0);
            AddToTouches(d1);
        }

        internal void AddToTouches(Centers d1)
        {
            if (d1 != null && !touches.Any(x => x.Equals(d1)))
            {
                touches.Add(d1);
            }
        }

        public override int GetHashCode()
        {
            return location.GetHashCode();
        }

        public bool Equals(Corners other)
        {
            if (CoordsCloseEnough(location.X, other.location.X) + CoordsCloseEnough(location.Y,other.location.Y) < Math.Pow(10,-6))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private double CoordsCloseEnough(double listpoint, double comparisonpoint)
        {
            return Math.Pow(listpoint - comparisonpoint,2);
        }

        internal Guid GetGuid()
        {
            return location.guid;
        }
    }
}