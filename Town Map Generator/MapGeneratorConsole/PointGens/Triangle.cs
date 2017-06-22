using System;
using System.Drawing;
using System.Collections.Generic;
using WpfApplication1.Models;
using SlimDX;
namespace ceometric.DelaunayTriangulator
{
    /// <summary>A class defining a triangle and some methods for the Delaunay algorithm.</summary>
    public class Triangle
    {
        /// <summary>The first vertex of the triangle.</summary>
        public Point Vertex1;
        /// <summary>The second vertex of the triangle.</summary>
        public Point Vertex2;
        /// <summary>The third vertex of the triangle.</summary>
        public Point Vertex3;

        public Point[] points { get { return new Point[] { Vertex1, Vertex2, Vertex3 }; } }
        
        #region Constructor

        /// <summary>Constructs a triangle from three points.</summary>
        /// <param name="vertex1">The first vertex of the triangle.</param>
        /// <param name="vertex2">The second vertex of the triangle.</param>
        /// <param name="vertex3">The third vertex of the triangle.</param>
        public Triangle(Point vertex1, Point vertex2, Point vertex3)
        {
            this.Vertex1 = vertex1;
            this.Vertex2 = vertex2;
            this.Vertex3 = vertex3;
        }

        #endregion

        #region Methods

        /// <summary>Tests if a point lies in the circumcircle of the triangle.</summary>
        /// <param name="point">A <see cref="Point"/>.</param>
        /// <returns>For a counterclockwise order of the vertices of the triangle, this test is 
        /// <list type ="bullet">
        /// <item>positive if <paramref name="point"/> lies inside the circumcircle.</item>
        /// <item>zero if <paramref name="point"/> lies on the circumference of the circumcircle.</item>
        /// <item>negative if <paramref name="point"/> lies outside the circumcircle.</item></list></returns>
        /// <remarks>The vertices of the triangle must be arranged in counterclockwise order or the result
        /// of this test will be reversed. This test ignores the z-coordinate of the vertices.</remarks>
        public double ContainsInCircumcircle(Point point)
        {
            double ax = this.Vertex1.X - point.X;
            double ay = this.Vertex1.Y - point.Y;
            double bx = this.Vertex2.X - point.X;
            double by = this.Vertex2.Y - point.Y;
            double cx = this.Vertex3.X - point.X;
            double cy = this.Vertex3.Y - point.Y;

            double det_ab = ax * by - bx * ay;
            double det_bc = bx * cy - cx * by;
            double det_ca = cx * ay - ax * cy;

            double a_squared = ax * ax + ay * ay;
            double b_squared = bx * bx + by * by;
            double c_squared = cx * cx + cy * cy;

            return a_squared * det_bc + b_squared * det_ca + c_squared * det_ab;
        }


        public bool SharesVertexWith(Triangle triangle)
        {
            foreach (Point tripoint in triangle.points)
            {
                if (Vertex1.X == tripoint.X && Vertex1.Y == tripoint.Y ||
                    Vertex2.X == tripoint.X && Vertex2.Y == tripoint.Y ||
                    Vertex3.X == tripoint.X && Vertex3.Y == tripoint.Y)
                { return true; }
            }
            return false;
        }

        public void Draw(Graphics finalimage, int basesize, int mapsize, Color cornercolor)
        {
            float imageratio = mapsize / basesize;
            var pen = new SolidBrush(Color.Red);
            var randomizer = new Random();
            var cornerpen = new SolidBrush(cornercolor);
            var points = new PointF[1];
            finalimage.FillRectangle(pen, (float)Vertex1.X * imageratio, (float)Vertex1.Y * imageratio, 2, 2);
            finalimage.FillRectangle(pen, (float)Vertex2.X * imageratio, (float)Vertex2.Y * imageratio, 2, 2);
            finalimage.FillRectangle(pen, (float)Vertex3.X * imageratio, (float)Vertex3.Y * imageratio, 2, 2);
           // var brush = new SolidBrush(vertices[0].Color);
            var linepen = new Pen(cornercolor, 1);
            var polypoints = new PointF[3];
            polypoints[0] = new PointF((float)Vertex1.X * imageratio, (float)Vertex1.Y * imageratio);
            polypoints[1] = new PointF((float)Vertex2.X * imageratio, (float)Vertex2.Y * imageratio);
            polypoints[2] = new PointF((float)Vertex3.X * imageratio, (float)Vertex3.Y * imageratio);
            finalimage.DrawPolygon(linepen, polypoints);

        }

        #endregion
    }
}
