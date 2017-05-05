using System;
using System.Collections.Generic;
using System.Linq;

using System.Drawing;

using SlimDX;

namespace TerrainGenerator.Models
{
    public class Triangle
    {
        private VertexPositionNormalColor[] vertices = new VertexPositionNormalColor[3];

        public Triangle(Center center, Corner second, Corner third)
        {
            bool drawTextures = true;

            if (Area(center.Point, second.Point, third.Point) > 0)
            {
                vertices[0] = new VertexPositionNormalColor(
                                new Vector3(center.Point.X, center.Point.Y, center.Point.Z),
                                center.Normal,
                                drawTextures ? center.Biome.BiomeColor : Color.White
                                );

                vertices[1] = new VertexPositionNormalColor(
                    new Vector3(second.Point.X, second.Point.Y, second.Point.Z),
                    second.Normal,
                    drawTextures ? ((center.Water) ? center.Biome.BiomeColor : second.Biome.BiomeColor) : Color.White);

                vertices[2] = new VertexPositionNormalColor(
                    new Vector3(third.Point.X, third.Point.Y, third.Point.Z),
                    third.Normal,
                    drawTextures ? ((center.Water) ? center.Biome.BiomeColor : third.Biome.BiomeColor) : Color.White);
            }
            else
            {
                vertices[0] = new VertexPositionNormalColor(
                            new Vector3(center.Point.X, center.Point.Y, center.Point.Z),
                            center.Normal,
                            drawTextures ? center.Biome.BiomeColor : Color.White);

                vertices[1] = new VertexPositionNormalColor(
                                    new Vector3(third.Point.X, third.Point.Y, third.Point.Z),
                                    third.Normal,
                                    drawTextures ? ((center.Water) ? center.Biome.BiomeColor : third.Biome.BiomeColor) : Color.White);

                vertices[2] = new VertexPositionNormalColor(
                                    new Vector3(second.Point.X, second.Point.Y, second.Point.Z),
                                    second.Normal,
                                    drawTextures ? ((center.Water) ? center.Biome.BiomeColor : second.Biome.BiomeColor) : Color.White);
            }
        }

        public float Area(Vector3 A, Vector3 B, Vector3 C)
        {
            return ((A.X - C.X) * (B.Z - C.Z) - (A.Z - C.Z) * (B.X - C.X)) / 2;
        }

        public void Draw(Graphics finalimage)
        {
            var brush = new SolidBrush(vertices[0].Color);
            var points = new PointF[3];
            points[0] = new PointF(vertices[0].Position.X, vertices[0].Position.Z);
            points[1] = new PointF(vertices[1].Position.X, vertices[1].Position.Z);
            points[2] = new PointF(vertices[2].Position.X, vertices[2].Position.Z);

            finalimage.FillPolygon(brush, points);
        }
    }

    public interface ITriangleHolder
    {
        void Draw(Graphics finalimage);
    }

    public class Polygon : ITriangleHolder
    {
        private List<Triangle> tris = new List<Triangle>();

        public Polygon(HashSet<Corner> corners, Center center)
        {
            OrderCorners(center);
            CalculateNormals(center);

            var cnr = new Corner[corners.Count + 1];

            for (int i = 0; i < cnr.Count() - 1; i++)
            {
                cnr[i] = corners.ElementAt(i);
            }

            cnr[corners.Count] = cnr.First();

            for (int i = cnr.Count() - 1; i >= 1; i--)
            {
                tris.Add(new Triangle(center, cnr[i], cnr[i - 1]));
            }
        }

        public void Draw(Graphics finalimage)
        {
            foreach (Triangle triangle in tris)
            {
                triangle.Draw(finalimage);
            }
        }

        public void OrderCorners(Center center)
        {
            var currentCorner = center.Corners.First();
            var ordered = new List<Corner>(center.Corners.Count);
            Edge ed;

            ordered.Add(currentCorner);
            do
            {
                ed = currentCorner.Protrudes.FirstOrDefault(x => center.Borders.Contains(x) && !(ordered.Contains(x.VoronoiStart) && (ordered.Contains(x.VoronoiEnd))));

                if (ed != null)
                {
                    Corner newdot = ed.Corners.FirstOrDefault(x => !ordered.Contains(x) && x.Touches.Any(a => a == center));
                    if (newdot != null)
                    {
                        ordered.Add(newdot);
                        currentCorner = newdot;
                    }
                    else
                    {
                        ed = null;
                    }
                }
            } while (ed != null);

            if (ordered.Count != center.Corners.Count)
            {

            }

            center.Corners.Clear();

            foreach (var corner in ordered)
            {
                center.Corners.Add(corner);
            }
        }

        public void CalculateNormals(Center center)
        {
            var sx = center.Corners.Sum(x => x.Normal.X) / center.Corners.Count;
            var sy = center.Corners.Sum(x => x.Normal.Y) / center.Corners.Count;
            var sz = center.Corners.Sum(x => x.Normal.Z) / center.Corners.Count;

            center.Normal = new Vector3(sx, sy, sz);
        }
    }

    public class Hexagon : ITriangleHolder
    {
        private List<Triangle> tris = new List<Triangle>();

        public Hexagon(IEnumerable<Corner> corners, Center center)
        {
            var a = corners.ToArray();

            tris.Add(new Triangle(center, a[0], a[1]));
            tris.Add(new Triangle(center, a[1], a[2]));
            tris.Add(new Triangle(center, a[2], a[3]));
            tris.Add(new Triangle(center, a[3], a[4]));
            tris.Add(new Triangle(center, a[4], a[5]));
            tris.Add(new Triangle(center, a[5], a[0]));
        }

        public void Draw(Graphics finalimage)
        {
            foreach (Triangle triangle in tris)
            {
                triangle.Draw( finalimage);
            }
        }
    }
}
