using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrnVoronoi.Handlers;
using BrnVoronoi.Models;

namespace BrnVoronoi.Helpers
{
    class Methods
    {
        public static VCircleEvent CircleCheckDataNode(VDataNode n, double ys)
        {
            VDataNode l = NodeHandler.LeftDataNode(n);
            VDataNode r = NodeHandler.RightDataNode(n);
            if (l == null || r == null || l.DataPoint == r.DataPoint || l.DataPoint == n.DataPoint || n.DataPoint == r.DataPoint)
                return null;
            if (Ccw(l.DataPoint[0], l.DataPoint[1], n.DataPoint[0], n.DataPoint[1], r.DataPoint[0], r.DataPoint[1], false) <= 0)
                return null;
            var center = CircumCircleCenter(l.DataPoint, n.DataPoint, r.DataPoint);
            var vc = new VCircleEvent();
            vc.NodeN = n;
            vc.NodeL = l;
            vc.NodeR = r;
            vc.Center = center;
            vc.Valid = true;
            return vc.Y >= ys ? vc : null;
        }

        public static void CleanUpTree(VNode Root)
        {
            if (Root is VDataNode)
                return;
            VEdgeNode VE = Root as VEdgeNode;
            while (VE.Edge.VVertexB == Constants.VVUnkown)
            {
                VE.Edge.AddVertex(Constants.VVInfinite);
                //				VE.Flipped = !VE.Flipped;
            }
            if (VE.Flipped)
            {
                Vector T = VE.Edge.LeftData;
                VE.Edge.LeftData = VE.Edge.RightData;
                VE.Edge.RightData = T;
            }
            VE.Edge.Done = true;
            CleanUpTree(Root.Left);
            CleanUpTree(Root.Right);
        }

        public static double ParabolicCut(double x1, double y1, double x2, double y2, double ys)
        {
            if (Math.Abs(x1 - x2) < 1e-10 && Math.Abs(y1 - y2) < 1e-10)
            {
                throw new Exception("Identical datapoints are not allowed!");
            }

            if (Math.Abs(y1 - ys) < 1e-10 && Math.Abs(y2 - ys) < 1e-10)
                return (x1 + x2) / 2;
            if (Math.Abs(y1 - ys) < 1e-10)
                return x1;
            if (Math.Abs(y2 - ys) < 1e-10)
                return x2;
            double a1 = 1 / (2 * (y1 - ys));
            double a2 = 1 / (2 * (y2 - ys));
            if (Math.Abs(a1 - a2) < 1e-10)
                return (x1 + x2) / 2;
            double xs1 = 0.5 / (2 * a1 - 2 * a2) * (4 * a1 * x1 - 4 * a2 * x2 + 2 * Math.Sqrt(-8 * a1 * x1 * a2 * x2 - 2 * a1 * y1 + 2 * a1 * y2 + 4 * a1 * a2 * x2 * x2 + 2 * a2 * y1 + 4 * a2 * a1 * x1 * x1 - 2 * a2 * y2));
            double xs2 = 0.5 / (2 * a1 - 2 * a2) * (4 * a1 * x1 - 4 * a2 * x2 - 2 * Math.Sqrt(-8 * a1 * x1 * a2 * x2 - 2 * a1 * y1 + 2 * a1 * y2 + 4 * a1 * a2 * x2 * x2 + 2 * a2 * y1 + 4 * a2 * a1 * x1 * x1 - 2 * a2 * y2));
            xs1 = Math.Round(xs1, 10);
            xs2 = Math.Round(xs2, 10);
            if (xs1 > xs2)
            {
                double h = xs1;
                xs1 = xs2;
                xs2 = h;
            }
            if (y1 >= y2)
                return xs2;
            return xs1;
        }

        private static int Ccw(double P0x, double P0y, double P1x, double P1y, double P2x, double P2y, bool PlusOneOnZeroDegrees)
        {
            var dx1 = P1x - P0x; 
            var dy1 = P1y - P0y;
            var dx2 = P2x - P0x; 
            var dy2 = P2y - P0y;

            if (dx1 * dy2 > dy1 * dx2) return +1;
            if (dx1 * dy2 < dy1 * dx2) return -1;
            if ((dx1 * dx2 < 0) || (dy1 * dy2 < 0)) return -1;
            if ((dx1 * dx1 + dy1 * dy1) < (dx2 * dx2 + dy2 * dy2) && PlusOneOnZeroDegrees)
                return +1;
            return 0;
        }

        private static Vector CircumCircleCenter(Vector A, Vector B, Vector C)
        {
            if (A == B || B == C || A == C)
                throw new Exception("Need three different points!");
            double tx = (A[0] + C[0]) / 2;
            double ty = (A[1] + C[1]) / 2;

            double vx = (B[0] + C[0]) / 2;
            double vy = (B[1] + C[1]) / 2;

            double ux, uy, wx, wy;

            if (A[0] == C[0])
            {
                ux = 1;
                uy = 0;
            }
            else
            {
                ux = (C[1] - A[1]) / (A[0] - C[0]);
                uy = 1;
            }

            if (B[0] == C[0])
            {
                wx = -1;
                wy = 0;
            }
            else
            {
                wx = (B[1] - C[1]) / (B[0] - C[0]);
                wy = -1;
            }

            double alpha = (wy * (vx - tx) - wx * (vy - ty)) / (ux * wy - wx * uy);

            return new Vector(tx + alpha * ux, ty + alpha * uy);
        }

        public static double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }

    }
}
