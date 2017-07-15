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
        private Dictionary<Guid, Corners> _cornerLookup;
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
            _cornerLookup = new Dictionary<Guid, Corners>();
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
            if (_cornerLookup.ContainsKey(cornerPnt.guid))
            {
                return _cornerLookup[cornerPnt.guid];
            }
            else
            {
                var newcorner = new Corners(_cornerLookup.Count, cornerPnt);
                _cornerLookup[newcorner.GetGuid()] = newcorner;
                cornerlist.Add(newcorner);
                return newcorner;
            }

            
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