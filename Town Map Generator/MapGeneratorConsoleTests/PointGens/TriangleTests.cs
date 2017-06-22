using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino;
using ceometric.DelaunayTriangulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ceometric.DelaunayTriangulator.Tests
{
    [TestClass()]
    public class TriangleTests
    {
        [TestMethod()]
        public void SharesVertexWith_TrianglewithNoSharedVertecies_ReturnsFalse()
        {
            var triange = new Triangle(new Point(1, 1, 1), new Point(2, 2, 2), new Point(3, 3, 3));
            var comparetriangle = new Triangle(new Point(4,4,4), new Point(5,5,5), new Point(6,6,6));
            var result = triange.SharesVertexWith(comparetriangle);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void SharesVertexWith_TrianglewithSharedVertecies_Returns3Trues()
        {
            var triange = new Triangle(new Point(1, 1, 1), new Point(2, 2, 2), new Point(3, 3, 3));
            var triangles = new List<Triangle>();
            triangles.Add(new Triangle(new Point(1,1,1), new Point(5, 5, 5), new Point(6, 6, 6)));
            triangles.Add(new Triangle(new Point(4,4,4), new Point(1, 1, 1), new Point(6, 6, 6)));
            triangles.Add(new Triangle(new Point(4,4,4), new Point(5, 5, 5), new Point(1, 1, 1)));

            foreach (Triangle comparertri in triangles)
            {
                var result = triange.SharesVertexWith(comparertri);
                Assert.IsTrue(result);
            }


        }

        [TestMethod()]
        public void ContainsInCircumcircle_PointInsideCircumcircle_returnspositive()
        {
            var triange = new Triangle(new Point(1, 1, 1), new Point(2, 1, 1), new Point(2,2, 3));
            var testpoint = new Point(1.5,1,3);
            var result = triange.ContainsInCircumcircle(testpoint);
            Assert.IsTrue(result > 0);
        }

        [TestMethod()]
        public void ContainsInCircumcircle_PointOutsideCircumcircle_returnsnegatice()
        {
            var triange = new Triangle(new Point(1, 1, 1), new Point(2, 1, 1), new Point(2, 2, 3));
            var testpoint = new Point(5, 5, 5);

            var result = triange.ContainsInCircumcircle(testpoint);
            Assert.IsTrue(result < 0);
        }

        [TestMethod()]
        public void ContainsInCircumcircle_PointOnCircumcircle_returnszero()
        {
            var triange = new Triangle(new Point(1, 1, 1), new Point(1, 2, 1), new Point(2, 2, 3));
            var testpoint = new Point(1, 1, 1);


            var result = triange.ContainsInCircumcircle(testpoint);
            Assert.IsTrue(result == 0);
        }
    }
}