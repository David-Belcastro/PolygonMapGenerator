using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGeneratorConsole.CubesFortune
{
    public interface IVoronoiPoint
    {
        double X { get; set; }
        double Y { get; set; }
        Arc _Node { get; set; }
        bool isNodeValid { get; }
        SiteEvent Center { get; set; }
        double o { get; set; }


        bool IsCircleEvent { get; }

        bool IsSiteEvent { get; }
    }

    public class SiteEvent : IVoronoiPoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        public bool isNodeValid { get { if (_Node.CreatingEventE is null) { return false; } else return _Node.CreatingEventE.valid; } }
        public Arc _Node { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public SiteEvent Center { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double o { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SiteEvent(double x, double y)
        {
            X = x;
            Y = y;
        }

        public bool IsCircleEvent
        {
            get
            {
                return false;
            }
        }

        public bool IsSiteEvent
        {
            get
            {
                return true;
            }
        }
    }


    public class CircleEvent : IVoronoiPoint
    {
        public Arc _Node { get; set; }
        public bool isNodeValid { get { if (_Node.CreatingEventE is null) { return false; } else return _Node.CreatingEventE.valid; } }
        public SiteEvent Center { get; set; }
        public double o { get; set; }

        public double Y
        {
            get
            {
                return Math.Round(Center.Y + Distance(_Node.arcpoint.X, _Node.arcpoint.Y, Center.X, Center.Y), 10);
            }
            set { }
        }

        public double X
        {
            get
            {
                return Center.X;
            }
            set { }

        }


        public CircleEvent(Arc node, SiteEvent center, double radius)
        {
            _Node = node;
            Center = center;
            o = radius;
        }

        private static double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }

        public bool IsCircleEvent
        {
            get
            {
                return true;
            }
        }

        public bool IsSiteEvent
        {
            get
            {
                return false;
            }
        }

    }


    public class RegionNode
    {
        public SiteEvent LeftNode;
        public SiteEvent RightNode;
        public SiteEvent CenterNode;

        public RegionNode(SiteEvent left, SiteEvent center, SiteEvent right)
        {
            LeftNode = left;
            RightNode = right;
            CenterNode = center;
        }
    }

    public class VrnEvent
    {
        public double circleLength = 0.0;
        public SiteEvent circleCenter;
        public Arc creatingArc;
        public bool valid = true;

        public VrnEvent(double xloc, SiteEvent circlecenter, Arc CreatingArc)
        {
            circleLength = xloc; circleCenter = circlecenter; creatingArc = CreatingArc; valid = true;
        }
    }

    public class Arc
    {
        public SiteEvent arcpoint;
        public Arc previouspoint;
        public Arc nextpoint;
        public VrnEvent CreatingEventE;
        public VoronoiSegment s0;
        public VoronoiSegment s1;

        public Arc() { }

        public Arc(SiteEvent point)
        {
            arcpoint = point;
        }

        public Arc(SiteEvent point, Arc ppoint)
        {
            arcpoint = point;
            previouspoint = ppoint;
        }

        public Arc(SiteEvent point, Arc npoint, bool next)
        {
            arcpoint = point;
            nextpoint = npoint;
        }

        public Arc(SiteEvent point, Arc ppoint, Arc npoint)
        {
            arcpoint = point;
            previouspoint = ppoint;
            nextpoint = npoint;
        }

        
    }
    public class VoronoiPoint
    {
        public double X;
        public double Y;

    }

    public class VoronoiSegment
    {
        public VoronoiPoint start;
        public VoronoiPoint end;
        public VoronoiPoint LeftNode;
        public VoronoiPoint RightNode;
        public bool completed = false;
        public int creationpoint = 0;

        public VoronoiSegment(double startX, double startY, int cp)
        {
            start = new VoronoiPoint { X = startX, Y = startY };
            creationpoint = cp;
            LeftNode = new VoronoiPoint { X= 0.0, Y=0.0 };
            RightNode = new VoronoiPoint { X = 0.0, Y = 0.0 };
        }

        public void finish(double startX, double startY, int cp)
        {
            creationpoint = cp;
            end = new VoronoiPoint { X = startX, Y = startY };
            completed = true;
        }
    }
}
