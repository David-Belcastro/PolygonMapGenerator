using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XnaMapGenerator3D.Models
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
                                drawTextures ? center.Biome.Color : Color.White
                                );

                vertices[1] = new VertexPositionNormalColor(
                    new Vector3(second.Point.X, second.Point.Y, second.Point.Z),
                    second.Normal,
                    drawTextures ? ((center.Water) ? center.Biome.Color : second.Biome.Color) : Color.White);

                vertices[2] = new VertexPositionNormalColor(
                    new Vector3(third.Point.X, third.Point.Y, third.Point.Z),
                    third.Normal,
                    drawTextures ? ((center.Water) ? center.Biome.Color : third.Biome.Color) : Color.White);
            }
            else
            {
                vertices[0] = new VertexPositionNormalColor(
                            new Vector3(center.Point.X, center.Point.Y, center.Point.Z),
                            center.Normal,
                            drawTextures ? center.Biome.Color : Color.White);

                vertices[1] = new VertexPositionNormalColor(
                                    new Vector3(third.Point.X, third.Point.Y, third.Point.Z),
                                    third.Normal,
                                    drawTextures ? ((center.Water) ? center.Biome.Color : third.Biome.Color) : Color.White);

                vertices[2] = new VertexPositionNormalColor(
                                    new Vector3(second.Point.X, second.Point.Y, second.Point.Z),
                                    second.Normal,
                                    drawTextures ? ((center.Water) ? center.Biome.Color : second.Biome.Color) : Color.White);
            }
        }

        public float Area(Vector3 A, Vector3 B, Vector3 C)
        {
            return ((A.X - C.X) * (B.Z - C.Z) - (A.Z - C.Z) * (B.X - C.X)) / 2;
        }

        public void Draw(GraphicsDevice device)
        {
            device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 1);    
        }
    }

    public interface ITriangleHolder
    {
        void Draw(GraphicsDevice device);
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

        public void Draw(GraphicsDevice device)
        {
            foreach (Triangle triangle in tris)
            {
                triangle.Draw(device);
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

        public void Draw(GraphicsDevice device)
        {
            foreach (Triangle triangle in tris)
            {
                triangle.Draw(device);
            }
        }
    }
}
