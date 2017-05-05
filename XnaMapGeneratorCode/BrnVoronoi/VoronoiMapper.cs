using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BrnVoronoi.Helpers;
using BrnVoronoi.Models;
using EventHandler = BrnVoronoi.Handlers.EventHandler;

namespace BrnVoronoi
{
    public class VoronoiMapper
    {
        public IEnumerable<Vector> Datapoints { get; set; }
        public BinaryPriorityQueue DataPq { get; set; }

        public VoronoiMapper(IEnumerable<Vector> datapoints)
        {
            Datapoints = datapoints;
            DataPq = new BinaryPriorityQueue();
        }

        public VoronoiGraph ComputeVoronoiGraph()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var vg = new VoronoiGraph();
            //Hashtable CurrentCircles = new Hashtable();
            var CurrentCircles = new Dictionary<VDataNode, VCircleEvent>();
            VNode RootNode = null;

            foreach (Vector v in Datapoints)
            {
                DataPq.Push(new VDataEvent(v));
            }

            while (DataPq.Count > 0)
            {
                var vevent = DataPq.Pop() as VEvent;
                VDataNode[] circleCheckList;

                if (vevent is VDataEvent)
                {
                    RootNode = EventHandler.ProcessDataEvent(vevent as VDataEvent, RootNode, vg, vevent.Y, out circleCheckList);
                }
                else if (vevent is VCircleEvent)
                {
                    CurrentCircles.Remove(((VCircleEvent)vevent).NodeN);
                    if (!((VCircleEvent)vevent).Valid)
                        continue;
                    RootNode = EventHandler.ProcessCircleEvent(vevent as VCircleEvent, RootNode, vg, vevent.Y, out circleCheckList);
                }
                else throw new Exception("Got event of type " + vevent.GetType() + "!");

                foreach (var vd in circleCheckList)
                {

                    if (CurrentCircles.ContainsKey(vd))
                    {
                        CurrentCircles[vd].Valid = false;
                        CurrentCircles.Remove(vd);
                    }

                    var vce = Methods.CircleCheckDataNode(vd, vevent.Y);

                    if (vce != null)
                    {
                        DataPq.Push(vce);
                        CurrentCircles[vd] = vce;
                    }
                }

                if (vevent is VDataEvent)
                {
                    var dp = ((VDataEvent)vevent).DataPoint;
                    foreach (VCircleEvent vce in CurrentCircles.Values)
                    {
                        if (Methods.Distance(dp[0], dp[1], vce.Center[0], vce.Center[1]) < vce.Y - vce.Center[1] && Math.Abs(Methods.Distance(dp[0], dp[1], vce.Center[0], vce.Center[1]) - (vce.Y - vce.Center[1])) > 1e-10)
                            vce.Valid = false;
                    }
                }
            }

            Methods.CleanUpTree(RootNode);
            foreach (VoronoiEdge ve in vg.Edges.Where(x => !x.Done && x.VVertexB == Constants.VVUnkown))
            {
                ve.AddVertex(Constants.VVInfinite);
                if (Math.Abs(ve.LeftData[1] - ve.RightData[1]) < 1e-10 && ve.LeftData[0] < ve.RightData[0])
                {
                    Vector T = ve.LeftData;
                    ve.LeftData = ve.RightData;
                    ve.RightData = T;
                }

            }

            var minuteEdges = new List<VoronoiEdge>();

            foreach (VoronoiEdge ve in vg.Edges.Where(ve => !ve.IsPartlyInfinite && ve.VVertexA.Equals(ve.VVertexB)))
            {
                minuteEdges.Add(ve);
                // prevent rounding errors from expanding to holes
                foreach (VoronoiEdge ve2 in vg.Edges)
                {
                    if (ve2.VVertexA.Equals(ve.VVertexA))
                        ve2.VVertexA = ve.VVertexA;
                    if (ve2.VVertexB.Equals(ve.VVertexA))
                        ve2.VVertexB = ve.VVertexA;
                }
            }
            foreach (VoronoiEdge ve in minuteEdges)
                vg.Edges.Remove(ve);


            stopWatch.Stop();


            return vg;
        }
    }
}
