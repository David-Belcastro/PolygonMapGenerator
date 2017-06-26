using System;
using System.Collections.Generic;
using CubesFortune;
using ceometric.DelaunayTriangulator;

namespace Town_Map_Generator
{
    public class PolyMap
    {
        private VoronoiMap voronoimap;
        public List<Centers> polys;
        public List<Corners> polycorns;
        private List<ceometric.DelaunayTriangulator.Point> basepoints;
        private Dictionary<Point, Centers> _centerLookup;
        private Dictionary<int, List<Corners>> _cornerLookup;

        public PolyMap(VoronoiMap voronoimap, List<ceometric.DelaunayTriangulator.Point> basepoints)
        {
            this.voronoimap = voronoimap;
            this.basepoints = basepoints;
            MakeCenters();
        }

        public void MakeCenters()
        {
            _centerLookup = new Dictionary<Point, Centers>();
            _cornerLookup = new Dictionary<int, List<Corners>>();
            polys = new List<Centers>();
            polycorns = new List<Corners>();
            foreach (Point pt in basepoints)
            {
                var poly = new Centers(polys.Count, new VoronoiPoint(pt.X, pt.Y);
                polys.Add(poly);
                _centerLookup[pt] = poly;
            }

        }

        public Corners MakeCorner(VoronoiPoint point)
        {
            for (var i = (int)(point.SafeX)-1; i <= (int)(point.SafeX)+1; i++)
            {
                foreach(Corners crn in _cornerLookup[i])
                {
                    var dx = point.SafeX - crn.location.SafeX;
                    var dy = point.SafeY - crn.location.SafeY;
                    if (Math.Pow(dx,2)+Math.Pow(dx,2) < Math.Pow(1, -6))
                    {
                        return crn;
                    }
                }
            }
            var bucket = (int)point.SafeX;
            if (!_cornerLookup.ContainsKey(bucket))
            {
                _cornerLookup[bucket] = new List<Corners>();
            }
            var newcrn = new Corners(polycorns.Count, point);
            polycorns.Add(newcrn);
            _cornerLookup[bucket].Add(newcrn);
            return newcrn;
        }
    }
}