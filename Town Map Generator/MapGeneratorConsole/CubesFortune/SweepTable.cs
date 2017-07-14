using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubesFortune
{
    public class IntersectionFlag
    {
        public bool flag;
        public SiteEvent siteEvent;

        public IntersectionFlag(bool fg, SiteEvent se)
        {
            flag = fg; siteEvent = se;
        }
    }

    public class SweepTable
    {

        class PointComparer<SiteEvent> : IComparer<SiteEvent>
        {
            private readonly Comparison<SiteEvent> comparison;
            public PointComparer(Comparison<SiteEvent> comparison)
            {
                this.comparison = comparison;
            }
            public int Compare(SiteEvent a, SiteEvent b)
            {
                return comparison(a, b);
            }
        }
        public Arc knownSite;
        private List<VoronoiSegment> VoronoiGraph;


        public Arc BeachPoints;
        private List<CircleEvent> createdCircleEvents;
        private double _boundx0 = -150.0;
        private double _boundy0 = -150.0;


        private double _boundx1 = 150.0;
        private double _boundy1 = 150.0;


        public SweepTable()
        {
            VoronoiGraph = new List<VoronoiSegment>();
            createdCircleEvents = new List<CircleEvent>();
        }

        public SweepTable(double boundrysize)
        {
            VoronoiGraph = new List<VoronoiSegment>();
            createdCircleEvents = new List<CircleEvent>();
            _boundx1 = boundrysize * 1.5;
            _boundy1 = boundrysize * 1.5;
        }

        public void AddToBeachLine(SiteEvent newarcpoint)
        {
            if (BeachPoints is null)
            {
                BeachPoints = new Arc(newarcpoint);
            }
            else
            {
                knownSite = BeachPoints;
                while(!(knownSite is null))
                {
                    var intersection = FindIntersectedArc(newarcpoint, knownSite);
                    if (intersection.flag)
                    {
                        var nextpointcheckZ = FindIntersectedArc(newarcpoint, knownSite.nextpoint);
                        if (!(knownSite.nextpoint is null) && !nextpointcheckZ.flag)
                        {
                            knownSite.nextpoint.previouspoint = new Arc(knownSite.arcpoint, knownSite, knownSite.nextpoint);
                            knownSite.nextpoint = knownSite.nextpoint.previouspoint;
                        }
                        else
                        {
                            knownSite.nextpoint = new Arc(knownSite.arcpoint, knownSite);
                        }
                        knownSite.nextpoint.s1 = knownSite.s1;

                        knownSite.nextpoint.previouspoint = new Arc(newarcpoint, knownSite, knownSite.nextpoint);
                        knownSite.nextpoint = knownSite.nextpoint.previouspoint;

                        knownSite = knownSite.nextpoint;
                        
                        var vifSeg = new VoronoiSegment(intersection.siteEvent.X, intersection.siteEvent.Y, knownSite, knownSite.previouspoint, 3);
                        knownSite.previouspoint.s1 = vifSeg;
                        knownSite.s0 = vifSeg;
                        VoronoiGraph.Add(vifSeg);
                        vifSeg = new VoronoiSegment(intersection.siteEvent.X, intersection.siteEvent.Y,knownSite.nextpoint,knownSite, 7);
                        knownSite.s1 = vifSeg;
                        knownSite.nextpoint.s0 = vifSeg;
                        VoronoiGraph.Add(vifSeg); //Yes this needs to be added twice
                        CheckForCircleEventsInTriple(newarcpoint.X);
                        return;
                    }
                    knownSite = knownSite.nextpoint ?? null;
                }
                knownSite = BeachPoints;
                while (!(knownSite.nextpoint is null))
                {
                    knownSite = knownSite.nextpoint;
                }
                knownSite.nextpoint = new Arc(newarcpoint, knownSite);
                var x = _boundx0;
                var y = (knownSite.nextpoint.arcpoint.Y + knownSite.arcpoint.Y) / 2.0;
                var segmentEvent = new SiteEvent(new VoronoiPoint(x,y));
                var vSeg = new VoronoiSegment(segmentEvent.X, segmentEvent.Y,knownSite.nextpoint,knownSite, 1);
                VoronoiGraph.Add(vSeg);
                knownSite.s1 = vSeg;
                knownSite.nextpoint.s0 = vSeg;
            }
        }

        private void CheckForCircleEventsInTriple(double x)
        {
            CheckForCircleEvent(knownSite, x);
            CheckForCircleEvent(knownSite.previouspoint, x);
            CheckForCircleEvent(knownSite.nextpoint, x);
        }

        public void CheckForCircleEvent(Arc knowncircleSite, double x)
        {
            if(!(knowncircleSite.CreatingEventE is null) && knowncircleSite.CreatingEventE.circleLength != _boundx0)
            {
                knowncircleSite.CreatingEventE.isNodeValid = false;
            }
            knowncircleSite.CreatingEventE = null;

            if (knowncircleSite.previouspoint is null || knowncircleSite.nextpoint is null)
            {
                return;
            }

            var circleeventtest = CalculateCircleEventCheck(knowncircleSite.previouspoint.arcpoint, knowncircleSite.arcpoint, knowncircleSite.nextpoint.arcpoint);
            if (circleeventtest.isCircle && circleeventtest.x > _boundx0)
            {
                knowncircleSite.CreatingEventE = new CircleEvent(knowncircleSite, circleeventtest.o, circleeventtest.x);
                createdCircleEvents.Add(knowncircleSite.CreatingEventE);
            }
        
        }

        public CircleeventCheck CalculateCircleEventCheck(SiteEvent a, SiteEvent b, SiteEvent c)
        {
            if ((b.X - a.X) * (c.Y - a.Y) - (c.X - a.X) * (b.Y - a.Y) > 0)
                return new CircleeventCheck();

            // Joseph O'Rourke, Computational Geometry in C (2nd ed.) p.189
            var A = b.X - a.X;
            var B = b.Y - a.Y;
            var C = c.X - a.X;
            var D = c.Y - a.Y;
            var E = A * (a.X + b.X) + B * (a.Y + b.Y);
            var F = C * (a.X + c.X) + D * (a.Y + c.Y);
            var G = 2 * (A * (c.Y - b.Y) - B * (c.X - b.X));

            if (G == 0)
            {
                return new CircleeventCheck();
            }

            var ox = 1.0 * (D * E - B * F) / G;
            var oy = 1.0 * (A * F - C * E) / G;

            var x = ox + Math.Sqrt(Math.Pow((a.X - ox),2) + Math.Pow((a.Y - oy),2));
            var o = new SiteEvent(new VoronoiPoint(ox, oy));

            return new CircleeventCheck(x, o);
            
        }

        public void ProcessCircleEvent(IVoronoiPoint p)
        {
            if(!p.isNodeValid)
            {
                return;
            }
            var assocatiedArcA = p.primaryArc.CreatingEventE.primaryArc;
            var segment = new VoronoiSegment(p.primaryArc.CreatingEventE.Center.X, p.primaryArc.CreatingEventE.Center.Y, assocatiedArcA.nextpoint, assocatiedArcA.previouspoint ,2);
            VoronoiGraph.Add(segment);            
            if (!(assocatiedArcA.previouspoint is null))
            {
                assocatiedArcA.previouspoint.nextpoint = assocatiedArcA.nextpoint;
                assocatiedArcA.previouspoint.s1 = segment;
            }
            if (!(assocatiedArcA.nextpoint is null))
            {
                assocatiedArcA.nextpoint.previouspoint = assocatiedArcA.previouspoint;
                assocatiedArcA.nextpoint.s0 = segment;
            }
            if(!(assocatiedArcA.s0 is null))
            {
                assocatiedArcA.s0.finish(p.primaryArc.CreatingEventE.Center.X, p.primaryArc.CreatingEventE.Center.Y,4);
            }
            if (!(assocatiedArcA.s1 is null))
            {
                assocatiedArcA.s1.finish(p.primaryArc.CreatingEventE.Center.X, p.primaryArc.CreatingEventE.Center.Y,5);
            }


            CheckForCircleEvent(assocatiedArcA.previouspoint, p.primaryArc.CreatingEventE.circleLength);
            CheckForCircleEvent(assocatiedArcA.nextpoint, p.primaryArc.CreatingEventE.circleLength);
        }

        private void InsertPointAfterCurrent(SiteEvent arcfocus)
        {
            knownSite.nextpoint.previouspoint = new Arc(arcfocus, knownSite, knownSite.nextpoint);
            knownSite.nextpoint = knownSite.nextpoint.previouspoint;
        }



        public IntersectionFlag FindIntersectedArc(SiteEvent newPoint, Arc intersectedKnownSite)
        {
            var nomatch = new IntersectionFlag(false, null);

            if (intersectedKnownSite is null)
            {
                return nomatch;
            }
            if (intersectedKnownSite.arcpoint.X == newPoint.X)
            {
                return nomatch;
            }
            var priorpoint = 0.0;
            var nextpoint = 0.0;

            if (!(intersectedKnownSite.previouspoint is null))
            {
                priorpoint = ParabolaIntersection(intersectedKnownSite.previouspoint.arcpoint, intersectedKnownSite.arcpoint, newPoint.X).Y;
            }
            if (!(intersectedKnownSite.nextpoint is null))
            {
                nextpoint = ParabolaIntersection(intersectedKnownSite.arcpoint, intersectedKnownSite.nextpoint.arcpoint, newPoint.X).Y;
            }
            if ((intersectedKnownSite.previouspoint is null || priorpoint <= newPoint.Y) && (intersectedKnownSite.nextpoint is null || newPoint.Y <= nextpoint))
            {
                var py = newPoint.Y;
                double px = CalculatePX(newPoint, intersectedKnownSite, py);
                return new IntersectionFlag(true, new SiteEvent(new VoronoiPoint(px, py)));
            }
            return nomatch;
        }

        private static double CalculatePX(SiteEvent newPoint, Arc intersectedKnownSite, double py)
        {
            var xsq = Math.Pow(intersectedKnownSite.arcpoint.X, 2);
            var difysq = Math.Pow((intersectedKnownSite.arcpoint.Y - py), 2);
            var npXSq = Math.Pow(newPoint.X, 2) ;
            var difXdoub = ((2 * intersectedKnownSite.arcpoint.X) - (2 * newPoint.X));
            var result = (xsq + difysq - npXSq) / difXdoub;
            return result;
        }

        public SiteEvent ParabolaIntersection(SiteEvent p0, SiteEvent p1, double newpointX)
        {
            var p = p0;
            var py = new double();
            if (p0.X == p1.X)
                py = (p0.Y + p1.Y) / 2.0;
            else if (p1.X == newpointX)
                py = p1.Y;
            else if (p0.X == newpointX)
            {
                py = p0.Y;
                p = p1;
            }
            else
            {
                var z0 = 2.0 * (p0.X - newpointX);
                var z1 = 2.0 * (p1.X - newpointX);

                var a = 1.0 / z0 - 1.0 / z1;
                var b = -2.0 * (p0.Y / z0 - p1.Y / z1);
                var c = 1.0 * (Math.Pow(p0.Y,2) + Math.Pow(p0.X,2) - Math.Pow(newpointX,2)) / z0 - 1.0 * (Math.Pow(p1.Y,2) + Math.Pow(p1.X,2) - Math.Pow(newpointX,2)) / z1;

                py = 1.0 * (-b - Math.Sqrt(b * b - 4 * a * c)) / (2 * a);

            }
            var px = 1.0 * (Math.Pow(p.X, 2) + Math.Pow((p.Y - py), 2) - Math.Pow(newpointX, 2)) / (2 * p.X - 2 * newpointX);
            var res = new SiteEvent(new VoronoiPoint(px, py));
            return res;
        }

        public List<CircleEvent> GetCurrentCircleEvents()
        {
            var currentevents = createdCircleEvents.ToList();
            createdCircleEvents.Clear();
            return currentevents;
        }

        public VoronoiMap FinishEdges()
        {
            var infinitypoint = _boundx1 + (_boundx1 - _boundx0) + (_boundy1 - _boundy0);
            var workingSite = BeachPoints;
            while (!(workingSite.nextpoint is null))
            {
                if (!(workingSite.s1 is null))
                {
                    var p = ParabolaIntersection(workingSite.arcpoint, workingSite.nextpoint.arcpoint, infinitypoint * 2);
                    workingSite.s1.finish(p.X, p.Y,6);
                }
                workingSite = workingSite.nextpoint;
            }
            return new VoronoiMap(VoronoiGraph);
        }
    }

    public class CircleeventCheck
    {
        public bool isCircle;
        public double x;
        public SiteEvent o;
        
        public CircleeventCheck()
        {
            isCircle = false;
        }
        public CircleeventCheck(double rad, SiteEvent cnt)
        {
            isCircle = true;
            x = rad;
            o = cnt;
        }
    }
}
