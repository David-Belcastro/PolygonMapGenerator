using System;
using System.Collections.Generic;
using Priority_Queue;
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
        public FastPriorityQueue<FastSiteQueueNode> eventQ;
        public FastPriorityQueue<FastCircleQueueNode> circleQ;
        public SweepTable regionsT;

        public CubesVoronoiMapper()
        {
            regionsT = new SweepTable();
        }

        public VoronoiMap GimmesomeVeoroiois(List<VoronoiPoint> sites)
        {
            eventQ = new FastPriorityQueue<FastSiteQueueNode>(sites.Count+1);
            circleQ = new FastPriorityQueue<FastCircleQueueNode>(sites.Count * sites.Count);

            foreach (VoronoiPoint pt in sites)
            {
                var pointasevent = new SiteEvent(pt);
                eventQ.Enqueue(new FastSiteQueueNode(pointasevent), pointasevent.NodePriority);
            }

            while (eventQ.Count != 0)
            {   if (circleQ.Count != 0 && circleQ.First().point.circleLength <= eventQ.First().point.X)
                {
                    ProcessCircleEvent();
                }
                else
                {
                    ProcessPointEvent();
                }
            }

            while (circleQ.Count != 0)
            {
                ProcessCircleEvent();
            }

            var finalmap = regionsT.FinishEdges();
            return finalmap;
        }

        public void ProcessCircleEvent()
        {
            var p = circleQ.Dequeue().point;
            regionsT.ProcessCircleEvent(p);
            AddCreatedCircleEvents();
        }

        public void ProcessPointEvent()
        {
            var p = eventQ.Dequeue().point;
            var pointAsEvent = new SiteEvent(p.basepoint);
            regionsT.AddToBeachLine(pointAsEvent);
            AddCreatedCircleEvents();
        }

        private void AddCreatedCircleEvents()
        {
            foreach (CircleEvent circ in regionsT.GetCurrentCircleEvents())
            {
                circleQ.Enqueue(new FastCircleQueueNode(circ), circ.NodePriority);
            }
        }
    }
}
