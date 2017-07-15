using System;
using System.Collections.Generic;
using CubesFortune;
using System.Linq;
namespace MapGeneratorConsole.ImageGenerators.Graph
{
    public class Centers : IEquatable<Centers>
    {
        public int index;
        public VoronoiPoint center;
        public List<Centers> neigbors;
        public List<Edges> borders;
        public List<Corners> corners;
        public CenterMapData mapData;
        internal bool border = false;

        public Centers(int indx, VoronoiPoint polycenter)
        {
            index = indx;
            center = polycenter;
            neigbors = new List<Centers>();
            borders = new List<Edges>();
            corners = new List<Corners>();
            mapData = new CenterMapData();
        }
        public List<Corners> SortedCorners()
        {
            return corners.OrderBy(x => Math.Atan2(x.location.Y-center.Y, x.location.X-center.X)).ToList();
        }   
    

        internal void AddToNeighbors(Centers d1)
        {
            if (d1 != null && !neigbors.Any(x => x.Equals(d1)))
            {
                neigbors.Add(d1);
            }
        }

        internal void BordersandCorners(Edges dualGraphVertex, Corners v0, Corners v1)
        {
            borders.Add(dualGraphVertex);
            AddToCorners(v0);
            AddToCorners(v1);
        }

        internal void AddToCorners(Corners d1)
        {
            if (d1 != null && !corners.Any(x => x.Equals(d1)))
            {
                corners.Add(d1);
            }
        }

        public override int GetHashCode()
        {
            return center.GetHashCode();
        }

        public bool Equals(Centers other)
        {
            if (other.center.X == center.X && other.center.Y == center.Y)
            {
                return true;
            }
            else
                return false;
        }

        internal Guid GetGuid()
        {
            return center.guid;
        }
    }
}