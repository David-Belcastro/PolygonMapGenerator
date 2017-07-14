using System;
using System.Collections.Generic;
using ceometric.DelaunayTriangulator;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubesFortune
{
    public class VoronoiMap {
        public List<VoronoiSegment> graph;

        public VoronoiMap(List<VoronoiSegment> unfinishedgraph)
        {
            graph = unfinishedgraph;
        }

        public List<VoronoiSegment> FinishedGraph()
        {
            var newgraph = graph.FindAll(x => x.completed == true);
            return newgraph;
        }
    }

    public class CubesVoronoiMapper
    {
        public EventQueue eventQ;
        public CircleQueue circleQ;
        public SweepTable regionsT;

        public CubesVoronoiMapper()
        {
            eventQ = new EventQueue();
            circleQ = new CircleQueue();
            regionsT = new SweepTable();
        }

        public VoronoiMap GimmesomeVeoroiois(List<VoronoiPoint> sites)
        {

            foreach (VoronoiPoint pt in sites)
            {
                var pointasevent = new SiteEvent(pt);
                eventQ.AddNode(pointasevent);
            }

            while (!eventQ.IsEmpty())
            {   if (!circleQ.IsEmpty() && circleQ.PeekSmallest().circleLength <= eventQ.PeekSmallest().X)
                {
                    ProcessCircleEvent();
                }
                else
                {
                    ProcessPointEvent();
                }
            }

            while (!circleQ.IsEmpty())
            {
                ProcessCircleEvent();
            }

            var finalmap = regionsT.FinishEdges();
            return finalmap;
        }

        public void ProcessCircleEvent()
        {
            var p = circleQ.PeekAndRemoveSmallest();
            regionsT.ProcessCircleEvent(p);
            AddCreatedCircleEvents();
        }

        public void ProcessPointEvent()
        {
            var p = eventQ.PeekAndRemoveSmallest();
            var pointAsEvent = new SiteEvent(p.basepoint);
            regionsT.AddToBeachLine(pointAsEvent);
            AddCreatedCircleEvents();
        }

        private void AddCreatedCircleEvents()
        {
            foreach (CircleEvent circ in regionsT.GetCurrentCircleEvents())
            {
                circleQ.AddNode(circ);
            }
        }
    }
}
