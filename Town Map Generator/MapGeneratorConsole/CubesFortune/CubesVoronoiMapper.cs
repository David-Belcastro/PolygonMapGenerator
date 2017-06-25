﻿using System;
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
        public CircleQueue circleQ;
        public SweepTable regionsT;

        public CubesVoronoiMapper()
        {
            eventQ = new EventQueue();
            circleQ = new CircleQueue();
            regionsT = new SweepTable();
        }

        public VoronoiMap GimmesomeVeoroiois(List<Point> sites)
        {

            foreach (Point pt in sites)
            {
                var pointasevent = new SiteEvent(pt.X, pt.Y);
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
            var pointAsEvent = new SiteEvent(p.X, p.Y);
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
