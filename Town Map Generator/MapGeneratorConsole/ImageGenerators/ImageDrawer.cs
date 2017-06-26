using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using ceometric.DelaunayTriangulator;
using CubesFortune;

namespace Town_Map_Generator
{
    internal class ImageDrawer
    {
        public ImageDrawer()
        {
        }

        public Bitmap DrawVoronoi(List<ceometric.DelaunayTriangulator.Point> pointlist, VoronoiMap voronoimap)
        {
            var b = new Bitmap(1000, 1000);
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
            foreach (ceometric.DelaunayTriangulator.Point pt in pointlist)
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
    }
}
