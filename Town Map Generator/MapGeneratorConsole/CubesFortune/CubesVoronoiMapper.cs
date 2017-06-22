using System;
using System.Collections.Generic;
using ceometric.DelaunayTriangulator;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGeneratorConsole.CubesFortune
{
    public class VoronoiMap {
        public List<VoronoiSegment> graph;

        public VoronoiMap(List<VoronoiSegment> unfinishedgraph)
        {
            graph = unfinishedgraph;
        }
    }

    public class CubesVoronoiMapper
    {
        public EventQueue eventQ;
        public SweepTable regionsT;

        public VoronoiMap GimmesomeVeoroiois(List<Point> sites)
        {
            eventQ = new EventQueue();
            regionsT = new SweepTable();
            
            foreach (Point pt in sites)
            {
                var pointasevent = new SiteEvent(pt.X, pt.Y);
                eventQ.AddNode(pointasevent);
            }

            while (!eventQ.IsEmpty())
            {   
                var currentPoint = eventQ.PeekAndRemoveSmallest();
                if (currentPoint.IsSiteEvent)
                {
                    ProcessSiteEvent(currentPoint);
                }
                if (currentPoint.IsCircleEvent && currentPoint.isNodeValid)
                {
                    ProcessVertexEvent(currentPoint);
                }
            }
            var finalmap = regionsT.FinishEdges();
            return finalmap;
        }

        public void ProcessVertexEvent(IVoronoiPoint p)
        {
            regionsT.ProcessCircleEvent(p);
            AddCreatedCircleEvents();
        }

        public void ProcessSiteEvent(IVoronoiPoint p)
        {
            var pointAsEvent = new SiteEvent(p.X, p.Y);
            regionsT.AddToBeachLine(pointAsEvent);
            AddCreatedCircleEvents();
        }

        private void AddCreatedCircleEvents()
        {
            foreach (VrnEvent circ in regionsT.GetCurrentCircleEvents())
            {
                eventQ.AddNode(new CircleEvent(circ.creatingArc, circ.circleCenter, circ.circleLength));
            }
        }
    }
}
