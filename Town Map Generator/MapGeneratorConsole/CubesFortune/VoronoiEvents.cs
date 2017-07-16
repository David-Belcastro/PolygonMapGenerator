using System;
using MapGeneratorConsole.ImageGenerators.Graph;

namespace CubesFortune
{
    public interface IVoronoiPoint
    {
        double X { get; }
        double Y { get; }
        VoronoiPoint basepoint { get; set; }
        Arc primaryArc { get; set; }
        bool isNodeValid { get; }
        SiteEvent Center { get; set; }
        double circleLength { get; set; }


        bool IsCircleEvent { get; }

        bool IsSiteEvent { get; }
    }

    public class SiteEvent : IVoronoiPoint
    {
        public double X { get { return basepoint.X; } }
        public double Y { get { return basepoint.Y; } }

        public VoronoiPoint basepoint { get; set; }
        public bool isNodeValid { get; set; }
        public Arc primaryArc { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public SiteEvent Center { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double circleLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SiteEvent(VoronoiPoint point)
        {
            basepoint = point;
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
        public Arc primaryArc { get; set; }
        public bool isNodeValid { get; set; }
        public SiteEvent Center { get; set; }
        public double circleLength { get; set; }

        public VoronoiPoint basepoint { get; set; }
        public double Y
        {
            get
            {
                return Math.Round(Center.Y + Distance(primaryArc.arcpoint.X, primaryArc.arcpoint.Y, Center.X, Center.Y), 10);
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
            primaryArc = node;
            Center = center;
            circleLength = radius;
            isNodeValid = true;
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
        public SiteEvent Center;
        public Arc primaryArc;
        public bool valid = true;

        public VrnEvent(double xloc, SiteEvent circlecenter, Arc CreatingArc)
        {
            circleLength = xloc; Center = circlecenter; primaryArc = CreatingArc; valid = true;
        }
    }

    public class Arc
    {
        public SiteEvent arcpoint;
        public Arc previouspoint;
        public Arc nextpoint;
        public CircleEvent CreatingEventE;
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
        public Guid guid;

        public VoronoiPoint(double x, double y)
        {
            X = x;
            Y = y;
            guid = Guid.NewGuid();
        }


        public ceometric.DelaunayTriangulator.Point Point()
        {
            return new ceometric.DelaunayTriangulator.Point(X, Y, 0);
        }

        public override int GetHashCode()
        {
            return guid.GetHashCode();
        }
    }

    public class VoronoiSegment
    {
        public VoronoiPoint start;
        public VoronoiPoint end;
        public VoronoiPoint LeftNode;
        public VoronoiPoint RightNode;
        public bool completed = false;
        public int creationpoint = 0;

        public double m;
        public double b;


        //known should be left, pprev right (i?)
        public VoronoiSegment(double startX, double startY, Arc lefts0, Arc rights1, int cp)
        {
            start = new VoronoiPoint(startX, startY);
            creationpoint = cp;
            LeftNode = lefts0.arcpoint.basepoint;
            RightNode = rights1.arcpoint.basepoint;
            //System.Diagnostics.Debug.WriteLine("Creating point at ({0},{1}) from code {2}", startX, startY, cp);
        }

        public void CalculateSlopeAndIntercept()
        {
            m = (end.Y - start.Y) / (end.X - start.X);
            b = start.Y - (m * start.X);
        }

        public void finish(double startX, double startY, int cp)
        {
            if (completed)
            {
                return;
            }
            creationpoint = cp;
            end = new VoronoiPoint(startX, startY);
            completed = true;
            CalculateSlopeAndIntercept();
            SetXAndY();
            //System.Diagnostics.Debug.WriteLine("Creating point at ({0},{1}) from code {2}", startX, startY, cp);
        }

        public void SetXAndY()
        {
            var largestpointstart = Math.Max(Math.Abs((start.X)), Math.Abs(start.Y));
            var largestpointend = Math.Max(Math.Abs(end.X), Math.Abs(end.Y));
            if (largestpointstart > 10000)
            {
                if (largestpointstart == Math.Abs(start.X))
                {
                    start.X = getlimit(start.X);
                    start.Y = GetYCoord(start.X);
                }
                else
                {
                    start.Y = getlimit(start.Y);
                    start.X = GetXCoord(start.Y);
                }
            }

            if (largestpointend > 10000)
            {
                if (largestpointend == Math.Abs(end.X))
                {
                    end.X = getlimit(end.X);
                    end.Y = GetYCoord(end.X);
                }
                else
                {
                    end.Y = getlimit(end.Y);
                    end.X = GetXCoord(end.Y);
                }
            }
        }

        public double getlimit(double testednumber)
        {
            return testednumber >= 0 ? 10000 : -10000;
        }

        public double GetYCoord(double X)
        {
            return m * X + b;
        }

        public double GetXCoord(double Y) { return (Y - b) / m; }

        public Line VoronoiLine()
        {
            return new Line(start, end);
        }

        public Line DelaunayLine()
        {
            return new Line(LeftNode, RightNode);
        }
    }
}
