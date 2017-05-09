using System;
using System.Collections.Generic;
using WpfApplication1.Models;
using SlimDX;

namespace TerrainGenerator.Models
{
    public class Edge : IEquatable<Edge> , IMapItem
    {
        public int Index { get; set; }

        public int Key { get; set; }
        public Center DelaunayStart { get; set; }
        public Center DelaunayEnd { get; set; }// Delaunay edge
        public Corner VoronoiStart { get; set; }
        public Corner VoronoiEnd { get; set; }// Voronoi edge
        public Vector3 Midpoint { get; set; }  // halfway between v0,v1
        public Vector3 FreshMidpoint
        {
            get { return Vector3.Lerp(VoronoiStart.Point, VoronoiEnd.Point, 0.5f); }
        }
        public int River { get; set; }  // volume of water, or 0
        public bool MapEdge = false;
        public Vector3 Vector3 { get { return VoronoiStart.Point; } }

        public Edge(Corner begin, Corner end, Center left, Center right)
        {
            River = 0;
            VoronoiStart = begin;
            VoronoiEnd = end;

            DelaunayStart = left;
            DelaunayEnd = right;

            Midpoint = new Vector3((VoronoiStart.Point.X + VoronoiEnd.Point.X) / 2, (VoronoiStart.Point.Y + VoronoiEnd.Point.Y) / 2, (VoronoiStart.Point.Z + VoronoiEnd.Point.Z) / 2);
            Key = Midpoint.GetHashCode();
            Index = Midpoint.GetHashCode();
        }

        public Edge(int index, Corner begin, Corner end)
        {
            Index = index;
            River = 0;
            VoronoiStart = begin;
            VoronoiEnd = end;
            Midpoint = new Vector3((VoronoiStart.Point.X + VoronoiEnd.Point.X) / 2, (VoronoiStart.Point.Y + VoronoiEnd.Point.Y) / 2, (VoronoiStart.Point.Z + VoronoiEnd.Point.Z) / 2);
            Key = Midpoint.GetHashCode();
        }

        public bool Coast
        {
            get
            {
                if (DelaunayStart != null && DelaunayEnd != null)
                    return ((VoronoiStart.Coast) && (VoronoiEnd.Coast)
                        && !(DelaunayStart.Water && DelaunayEnd.Water)
                        && !(DelaunayStart.Land && DelaunayEnd.Land));
                return false;
            }

            set {  }
        }

        public Corner[] Corners { get { return new Corner[] {VoronoiStart, VoronoiEnd}; } }

        public float DiffX { get { return VoronoiEnd.Point.X - VoronoiStart.Point.X; } }
        public float DiffY { get { return VoronoiEnd.Point.Y - VoronoiStart.Point.Y; } }
        
        public bool Equals(Edge other)
        {
            return this.VoronoiStart.Equals(other.VoronoiStart) && 
                this.VoronoiEnd.Equals(other.VoronoiEnd);
        }
    }
}
