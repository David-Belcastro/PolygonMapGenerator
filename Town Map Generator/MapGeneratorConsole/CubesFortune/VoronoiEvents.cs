using System;
using MapGeneratorConsole.ImageGenerators.Graph;

namespace CubesFortune
{
    public interface IVoronoiPoint
    {
        double X { get; set; }
        double Y { get; set; }
        Arc primaryArc { get; set; }
        bool isNodeValid { get; }
        SiteEvent Center { get; set; }
        double circleLength { get; set; }


        bool IsCircleEvent { get; }

        bool IsSiteEvent { get; }
    }

    public class SiteEvent : IVoronoiPoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        public bool isNodeValid { get; set; }
        public Arc primaryArc { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public SiteEvent Center { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double circleLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
        public Arc primaryArc { get; set; }
        public bool isNodeValid { get; set; }
        public SiteEvent Center { get; set; }
        public double circleLength { get; set; }

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

        public double SafeX;
        public double SafeY;

        public VoronoiPoint(double x, double y)
        {
            X = x;
            Y = y;
            SetSafePoints();
        }

        public void SetSafePoints()
        {
            SafeX = X;
            SafeY = Y;
        }

        public ceometric.DelaunayTriangulator.Point Point()
        {
            return new ceometric.DelaunayTriangulator.Point(SafeX, SafeY, 0);
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
            LeftNode = new VoronoiPoint(lefts0.arcpoint.X, lefts0.arcpoint.Y);
            RightNode = new VoronoiPoint(rights1.arcpoint.X, rights1.arcpoint.Y);
            System.Diagnostics.Debug.WriteLine("Creating point at ({0},{1}) from code {2}", startX, startY, cp);
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
            SetSafeXAndY();
            System.Diagnostics.Debug.WriteLine("Creating point at ({0},{1}) from code {2}", startX, startY, cp);
        }

        public void SetSafeXAndY()
        {
            var largestpointstart = Math.Max(Math.Abs((start.X)), Math.Abs(start.Y));
            var largestpointend = Math.Max(Math.Abs(end.X), Math.Abs(end.Y));
            if (largestpointstart > 10000)
            {
                if (largestpointstart == Math.Abs(start.X))
                {
                    start.SafeX = getlimit(start.X);
                    start.SafeY = GetYCoord(start.SafeX);
                }
                else
                {
                    start.SafeY = getlimit(start.Y);
                    start.SafeX = GetXCoord(start.SafeY);
                }
            }
            else { start.SafeX = start.X; start.SafeY = start.Y; }

            if (largestpointend > 10000)
            {
                if (largestpointend == Math.Abs(end.X))
                {
                    end.SafeX = getlimit(end.X);
                    end.SafeY = GetYCoord(end.SafeX);
                }
                else
                {
                    end.SafeY = getlimit(end.Y);
                    end.SafeX = GetXCoord(end.SafeY);
                }
            }
            else { end.SafeX = end.X; end.SafeY = end.Y; }
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
            return new Line(LeftNode, RightNode);
        }

        public Line DelaunayLine()
        {
            return new Line(start, end);
        }
    }
}
