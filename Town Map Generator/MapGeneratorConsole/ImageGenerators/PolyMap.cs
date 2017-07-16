using System;
using System.Collections.Generic;
using CubesFortune;
using ceometric.DelaunayTriangulator;
using MapGeneratorConsole.ImageGenerators.Graph;
using System.Linq;

namespace Town_Map_Generator
{
    public class PolyMap
    {
        private VoronoiMap voronoimap;
        public List<Edges> edgelist;
        public List<Corners> cornerlist;
        public List<Centers> centerlist;
        private List<VoronoiPoint> basepoints;
        private Dictionary<Guid, Centers> _centerLookup;
        private Dictionary<int, List<Corners>> _cornerLookup;
        private Corners startCorner;
        private Corners endCorner;
        private Centers leftCenters;
        private Centers rightCenters;


        public PolyMap(VoronoiMap voronoimap, List<VoronoiPoint> basepoints)
        {
            this.voronoimap = voronoimap;
            this.basepoints = basepoints;
            GenerateMainGraph();
        }

        public void GenerateMainGraph()
        {
            edgelist = new List<Edges>();
            cornerlist = new List<Corners>();
            centerlist = new List<Centers>();
            _centerLookup = new Dictionary<Guid, Centers>();
            _cornerLookup = new Dictionary<int, List<Corners>>();
            foreach (VoronoiSegment vrnSeg in voronoimap.FinishedGraph())
            {
                ClearCornerCenterEdgeCache();
                GenerateandCheckCenters(vrnSeg);
                GenerateandCheckCorners(vrnSeg);
                GenerateEdge(vrnSeg.VoronoiLine());

            }
        }

        public void GenerateandCheckCenters(VoronoiSegment vrnSeg)
        {
            var delaunayline = vrnSeg.DelaunayLine();
            leftCenters = CenterWarehouse(delaunayline.startleft);
            rightCenters = CenterWarehouse(delaunayline.endright);
        }

        public void GenerateandCheckCorners(VoronoiSegment vrnSeg)
        {
            var voronoiline = vrnSeg.VoronoiLine();
            startCorner = MakeCorner(voronoiline.startleft);
            endCorner = MakeCorner(voronoiline.endright);
        }

        private Corners MakeCorner(VoronoiPoint cornerPnt)
        {
            if (cornerPnt == null)
            {
                return null;
            }
            for (var bck = (int)cornerPnt.X - 1; bck <= (int)cornerPnt.X + 1; bck++)
            {
                if (_cornerLookup.ContainsKey(bck)) {
                    foreach (Corners q in _cornerLookup[bck])
                    {
                        var dx = cornerPnt.X - q.location.X;
                        var dy = cornerPnt.Y - q.location.Y;
                        if (Math.Pow(dx, 2) + Math.Pow(dy, 2) < Math.Pow(10, -5))
                        {
                            return q;
                        }
                    }
                }
            }
            var bucket = (int)cornerPnt.X;
            if (!_cornerLookup.ContainsKey(bucket))
            {
                _cornerLookup[bucket] = new List<Corners>();
            }
            var newcorner = new Corners(_cornerLookup.Count, cornerPnt);
            _cornerLookup[bucket].Add(newcorner);
            cornerlist.Add(newcorner);
            return newcorner;

        }

    

        private Centers CenterWarehouse(VoronoiPoint centerPoint)
        {
            if (_centerLookup.ContainsKey(centerPoint.guid))
            {
                return _centerLookup[centerPoint.guid];
            }
            else
            {
                var newcenter = new Centers(_centerLookup.Count, centerPoint);
                _centerLookup[newcenter.GetGuid()] = newcenter;
                centerlist.Add(newcenter);
                return newcenter;
            }
        }

        public void GenerateEdge(Line vrnMidPoint)
        {
            edgelist.Add(new Edges(_centerLookup.Count, leftCenters, rightCenters, startCorner, endCorner, vrnMidPoint.MidPoint()));
        }


        private void ClearCornerCenterEdgeCache()
        {
            leftCenters = null;
            rightCenters = null;
            startCorner = null;
            endCorner = null;
        }
    }

}