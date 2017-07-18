using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using ceometric.DelaunayTriangulator;
using CubesFortune;
using MapGeneratorConsole.ImageGenerators.Graph;
using System;
using System.Linq;

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


        internal Bitmap DrawIsland(List<VoronoiPoint> pointlist, List<Edges> mapGraph)
        {
            b = new Bitmap(1000, 1000);
            var g = Graphics.FromImage(b);
            var centerlist = new List<Centers>();
            float imageratio = 100f;
            var blackpen = new Pen(Color.Black, 1);
            var blackbrusy = new SolidBrush(Color.Black);

            foreach (Edges poly in mapGraph)
            {

                if (poly.delaunayCenter1 != null)
                {
                    centerlist.Add(poly.delaunayCenter1);
                }
                if (poly.delaunayCenter2 != null)
                {
                    centerlist.Add(poly.delaunayCenter2);
                }

            }

            var maxelev = centerlist.Max(x => x.mapData.Elevation);
           foreach (Centers cnt in centerlist)

            {
                
                var polypoints = new List<PointF>();
                foreach (Corners crn in cnt.SortedCorners())
                {
                    polypoints.Add(new PointF((float)crn.location.X * imageratio, (float)crn.location.Y * imageratio));
                }
                if (polypoints.Count > 1)
                {
                    g.DrawPolygon(blackpen, polypoints.ToArray());
                            g.FillPolygon(cnt.mapData.Biome.MapColor(), polypoints.ToArray());
 
                    
                }
                foreach (Edges edge in cnt.borders)
                {
                    if (edge.mapdata.River > 0)
                    {
                        g.DrawLine(new Pen(Color.Aqua,(float)Math.Sqrt((int)edge.mapdata.River)), new PointF((float)edge.voronoiCorner1.location.X * imageratio, (float)edge.voronoiCorner1.location.Y * imageratio), new PointF((float)edge.voronoiCorner2.location.X * imageratio, (float)edge.voronoiCorner2.location.Y * imageratio));
                    }
                }
            }
            var landlist = centerlist.FindAll(x => x.mapData.Water == false);
            var rnd = new Random();
            var font = new Font("Arial", 15);
            foreach (KeyValuePair<Centers,int> node in landlist[rnd.Next(landlist.Count())].mapData.Villiage.DistanceToAllTowns())
            {
                g.DrawString(node.Value.ToString(),font,blackbrusy, new PointF((float)node.Key.center.X * imageratio, (float)node.Key.center.Y * imageratio));
            }

                return b;
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
                g.FillRectangle(pen, (float)seg.start.X * imageratio, (float)seg.start.Y * imageratio, 2, 2);
                if (seg.completed)
                {
                    g.DrawLine(cornerpen, (float)seg.start.X * imageratio, (float)seg.start.Y * imageratio, (float)seg.end.X * imageratio, (float)seg.end.Y * imageratio);
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

        internal Bitmap DrawMapGraph(List<VoronoiPoint> pointlist, List<Edges> mapGraph)
        {
            b = new Bitmap(1000, 1000);
            var g = Graphics.FromImage(b);
            var centerlist = new List<Centers>();
            float imageratio = 100f;
            var blackpen = new Pen(Color.Black,1);
            var blackbrusy = new SolidBrush(Color.Black);
            foreach (Edges poly in mapGraph)
            {
                
                if (poly.delaunayCenter1 != null)
                {
                    centerlist.Add(poly.delaunayCenter1);
                }
                if (poly.delaunayCenter2 != null)
                {
                    centerlist.Add(poly.delaunayCenter2);
                }
            }
            foreach ( Centers cnt in centerlist)
            
                {
                    var pen = new SolidBrush(Color.FromArgb(randomizer.Next(0, 255), randomizer.Next(0, 255), randomizer.Next(0, 255)));
                    var polypoints = new List<PointF>();
                    foreach (Corners crn in cnt.SortedCorners())
                    {
                        polypoints.Add(new PointF((float)crn.location.X * imageratio, (float)crn.location.Y * imageratio));
                    }
     //               polypoints.Add(new PointF((float)cnt.center.X * imageratio, (float)cnt.center.Y * imageratio));
                    if (polypoints.Count > 1)
                    {
                        //  g.DrawPolygon(blackpen, polypoints.ToArray());
                        g.FillPolygon(pen, polypoints.ToArray());
                    }
                }
            
            return b;
        }
    }
}
