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
            var OceanPen = new SolidBrush(Color.DarkBlue);
            var MarshPen = new SolidBrush(Color.Teal);
            var IcePen = new SolidBrush(Color.GhostWhite);
            var LakePen = new SolidBrush(Color.Blue);
            var BeachPen = new SolidBrush(Color.AntiqueWhite);
            var SnowPen = new SolidBrush(Color.White);
            var TundraPen = new SolidBrush(Color.Snow);
            var BarePen = new SolidBrush(Color.LightGray);
            var ScorchedPen = new SolidBrush(Color.Sienna);
            var TaigaPen = new SolidBrush(Color.CadetBlue);
            var ShrublandPen = new SolidBrush(Color.Tan);
            var PlainsPen = new SolidBrush(Color.Goldenrod);
            var TemperateRainForestPen = new SolidBrush(Color.DarkGreen);
            var ForestPen = new SolidBrush(Color.Green);
            var GrasslandPen = new SolidBrush(Color.LawnGreen);
            var TropicalRainForestPen = new SolidBrush(Color.YellowGreen);
            var DesertPen = new SolidBrush(Color.Moccasin);

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
                    switch (cnt.mapData.Biome)
                    {
                        case MapGeneratorConsole.ImageGenerators.Biomes.Ocean:
                            g.FillPolygon(OceanPen, polypoints.ToArray());
                            break;
                        case MapGeneratorConsole.ImageGenerators.Biomes.Marsh:
                            g.FillPolygon(MarshPen, polypoints.ToArray());
                            break;
                        case MapGeneratorConsole.ImageGenerators.Biomes.Ice:
                            g.FillPolygon(IcePen, polypoints.ToArray());
                            break;
                        case MapGeneratorConsole.ImageGenerators.Biomes.Lake:
                            g.FillPolygon(LakePen, polypoints.ToArray());
                            break;
                        case MapGeneratorConsole.ImageGenerators.Biomes.Beach:
                            g.FillPolygon(BeachPen, polypoints.ToArray());
                            break;
                        case MapGeneratorConsole.ImageGenerators.Biomes.Snow:
                            g.FillPolygon(SnowPen, polypoints.ToArray());
                            break;
                        case MapGeneratorConsole.ImageGenerators.Biomes.Tundra:
                            g.FillPolygon(TundraPen, polypoints.ToArray());
                            break;
                        case MapGeneratorConsole.ImageGenerators.Biomes.Bare:
                            g.FillPolygon(BarePen, polypoints.ToArray());
                            break;
                        case MapGeneratorConsole.ImageGenerators.Biomes.Scorched:
                            g.FillPolygon(ScorchedPen, polypoints.ToArray());
                            break;
                        case MapGeneratorConsole.ImageGenerators.Biomes.Taiga:
                            g.FillPolygon(TaigaPen, polypoints.ToArray());
                            break;
                        case MapGeneratorConsole.ImageGenerators.Biomes.Shrubland:
                            g.FillPolygon(ShrublandPen, polypoints.ToArray());
                            break;
                        case MapGeneratorConsole.ImageGenerators.Biomes.Plains:
                            g.FillPolygon(PlainsPen, polypoints.ToArray());
                            break;
                        case MapGeneratorConsole.ImageGenerators.Biomes.TemperateRainForest:
                            g.FillPolygon(TemperateRainForestPen, polypoints.ToArray());
                            break;
                        case MapGeneratorConsole.ImageGenerators.Biomes.Forest:
                            g.FillPolygon(ForestPen, polypoints.ToArray());
                            break;
                        case MapGeneratorConsole.ImageGenerators.Biomes.Grassland:
                            g.FillPolygon(GrasslandPen, polypoints.ToArray());
                            break;
                        case MapGeneratorConsole.ImageGenerators.Biomes.TropicalRainForest:
                            g.FillPolygon(TropicalRainForestPen, polypoints.ToArray());
                            break;
                        case MapGeneratorConsole.ImageGenerators.Biomes.Desert:
                            g.FillPolygon(DesertPen, polypoints.ToArray());
                            break;
                        default:
                            g.FillPolygon(new SolidBrush(Color.Magenta), polypoints.ToArray());
                            break;
                    }
                }
                foreach (Edges edge in cnt.borders)
                {
                    if (edge.mapdata.River > 0)
                    {
                        g.DrawLine(new Pen(Color.Aqua,(float)Math.Sqrt((int)edge.mapdata.River)), new PointF((float)edge.voronoiCorner1.location.X * imageratio, (float)edge.voronoiCorner1.location.Y * imageratio), new PointF((float)edge.voronoiCorner2.location.X * imageratio, (float)edge.voronoiCorner2.location.Y * imageratio));
                    }
                }
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
