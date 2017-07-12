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
        public List<Centers> polys;
        public List<Corners> polycorns;
        private List<VoronoiPoint> basepoints;
        private Dictionary<int,Centers> _centerLookup;
        private Dictionary<int, List<Corners>> _cornerLookup;
        public List<DualGraphVertex> MapGraph;

        public PolyMap(VoronoiMap voronoimap, List<VoronoiPoint> basepoints)
        {
            this.voronoimap = voronoimap;
            this.basepoints = basepoints;
            GenerateMainGraph();
        }

        public void GenerateMainGraph()
        {
            _centerLookup = new Dictionary<int, Centers>();
            _cornerLookup = new Dictionary<int, List<Corners>>();

            MapGraph = new List<DualGraphVertex>();
            foreach (VoronoiSegment vrnseg in voronoimap.graph)
            {
                var delaunayline = vrnseg.DelaunayLine();
                var voronoiline = vrnseg.VoronoiLine();
                var corner1 = MakeCorner(voronoiline.startleft);
                var corner2 = MakeCorner(voronoiline.endright);
                var CenterLeft = MakeCenter(delaunayline.startleft);
                var CenterRight = MakeCenter(delaunayline.endright);

                corner1.AddToAdjacents(corner2);
                corner2.AddToAdjacents(corner1);

                CenterRight.AddToCorners(corner1);
                CenterRight.AddToCorners(corner2);

                CenterLeft.AddToCorners(corner1);
                CenterRight.AddToCorners(corner2);

                var GraphVertex = new DualGraphVertex(0, CenterLeft, CenterRight, corner1, corner2, voronoiline.MidPoint());

                MapGraph.Add(GraphVertex);

                CenterLeft.borders.Add(GraphVertex);
                CenterRight.borders.Add(GraphVertex);
                CenterLeft.AddToNeighbors(CenterRight);
                CenterRight.AddToNeighbors(CenterLeft);

                corner1.protrudes.Add(GraphVertex);
                corner2.protrudes.Add(GraphVertex);
                corner1.AddToTouches(CenterLeft);
                corner1.AddToTouches(CenterRight);
                corner2.AddToTouches(CenterLeft);
                corner2.AddToTouches(CenterRight);
            }
        }

        public Corners MakeCorner(VoronoiPoint vrnPnt)
        {
            int hash = vrnPnt.GetHashCode();
            if (_cornerLookup.ContainsKey(hash))
            {
                var possibleCorner = _cornerLookup[hash].FirstOrDefault(crn => crn.location.SafeX == vrnPnt.SafeX && crn.location.SafeY == vrnPnt.SafeY);

                if (possibleCorner != null)
                {
                    if (possibleCorner.location.SafeX == vrnPnt.SafeX && possibleCorner.location.SafeY == vrnPnt.SafeY)
                    {
                        return possibleCorner;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    var newCorner = new Corners(_cornerLookup[hash].Count,vrnPnt);
                    _cornerLookup[hash].Add(newCorner);
                    return newCorner;
                }
            }
            else
            {
                var cornerlist = new List<Corners>();
                var newCorner = new Corners(0, vrnPnt);
                cornerlist.Add(newCorner);
                _cornerLookup.Add(hash , cornerlist);
                return newCorner;
            }
        }
        public Centers MakeCenter(VoronoiPoint dlnyPnt)
        {
            var hash = dlnyPnt.GetHashCode();
            if (_centerLookup.ContainsKey(hash))
            {
                return _centerLookup[hash];
            }
            else
            {
                var newCenter = new Centers(_centerLookup.Count, dlnyPnt);
                _centerLookup[hash] = newCenter;
                return newCenter;
            }
        }

    }

}