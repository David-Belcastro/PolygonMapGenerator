using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrnVoronoi.Models
{
    public class VoronoiGraph
    {
        public HashSet<Vector> Vertizes = new HashSet<Vector>();
        public HashSet<VoronoiEdge> Edges = new HashSet<VoronoiEdge>();
    }
}
