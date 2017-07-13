using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using ceometric.DelaunayTriangulator;
using CubesFortune;
using MapGeneratorConsole.ImageGenerators.Graph;
using System;

namespace Town_Map_Generator
{
    internal class ImageDrawer
    {

        Random randomizer;
        Bitmap b;
        public ImageDrawer()
        {
            randomizer = new Random();
            b = new Bitmap(1000, 1000);
        }

        public Bitmap DrawVoronoi(List<VoronoiPoint> pointlist, VoronoiMap voronoimap)
        {
            //var b = new Bitmap(1000, 1000);
            var g = Graphics.FromImage(b);
            var lrnodes = new List<ceometric.DelaunayTriangulator.Point>();
            foreach (VoronoiSegment seg in voronoimap.graph)
            {
                lrnodes.Add(seg.LeftNode.Point()); lrnodes.Add(seg.RightNode.Point());
            }
            var dlpoints = new DelaunayTriangulation2d().Triangulate(lrnodes);
            foreach (VoronoiSegment seg in voronoimap.graph)
            {
                float imageratio = 100f;
                var pen = new SolidBrush(Color.Red);
                var randomizer = new System.Random();
                var cornerpen = new Pen(Color.Green);
                g.FillRectangle(pen, (float)seg.start.SafeX * imageratio, (float)seg.start.SafeY * imageratio, 2, 2);
                if (seg.completed)
                {
                    g.DrawLine(cornerpen, (float)seg.start.SafeX * imageratio, (float)seg.start.SafeY * imageratio, (float)seg.end.SafeX * imageratio, (float)seg.end.SafeY * imageratio);
                }

            }
            foreach (VoronoiPoint pt in pointlist)
            {
                float imageratio = 100f;
                var pen = new SolidBrush(Color.Blue);
                var randomizer = new System.Random();
                var cornerpen = new SolidBrush(Color.Blue);
                g.FillRectangle(pen, (float)pt.X * imageratio, (float)pt.Y * imageratio, 2, 2);
            }

            foreach (ceometric.DelaunayTriangulator.Triangle tri in dlpoints)
            {
                tri.Draw(g, Color.Red);
            }
            return b;
        }

        internal Bitmap DrawMapGraph(List<VoronoiPoint> pointlist, List<DualGraphVertex> mapGraph)
        {
            b = new Bitmap(1000, 1000);
            var g = Graphics.FromImage(b);
            var centerlist = new List<Centers>();
            float imageratio = 100f;
            var blackpen = new Pen(Color.Black,1);
            var blackbrusy = new SolidBrush(Color.Black);
            foreach (DualGraphVertex poly in mapGraph)
            {
                
                if (poly.d0 != null)
                {
                    centerlist.Add(poly.d0);
                }
                if (poly.d1 != null)
                {
                    centerlist.Add(poly.d1);
                }
            }
            foreach ( Centers cnt in centerlist)
            {if (cnt.corners.Count <3)
                {
                    var pen = new SolidBrush(Color.FromArgb(randomizer.Next(0, 255), randomizer.Next(0, 255), randomizer.Next(0, 255)));
                    var polypoints = new List<PointF>();
                    foreach (Corners crn in cnt.corners)
                    {
                        polypoints.Add(new PointF((float)crn.location.X * imageratio, (float)crn.location.Y * imageratio));
                    }
                    polypoints.Add(new PointF((float)cnt.center.X * imageratio, (float)cnt.center.Y * imageratio));
                    if (polypoints.Count > 1)
                    {
                        //  g.DrawPolygon(blackpen, polypoints.ToArray());
                        g.FillPolygon(pen, polypoints.ToArray());
                    }
                }
            }
            return b;
        }
    }
}
