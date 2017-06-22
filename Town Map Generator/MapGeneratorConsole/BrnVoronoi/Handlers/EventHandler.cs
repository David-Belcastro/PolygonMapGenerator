using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrnVoronoi.Models;

namespace BrnVoronoi.Handlers
{
    public class EventHandler
    {
        public static VNode ProcessDataEvent(VDataEvent vertexevent, VNode root, VoronoiGraph veronoigraphbox, double vertexeventycoord, out VDataNode[] circleCheckList)
        {
            if (root == null)
            {
                root = new VDataNode(vertexevent.DataPoint);
                circleCheckList = new VDataNode[] { (VDataNode)root };
                return root;
            }
            //1. Find the node to be replaced
            VNode C = NodeHandler.FindDataNode(root, vertexeventycoord, vertexevent.DataPoint[0]);
            //2. Create the subtree (ONE Edge, but two VEdgeNodes)
            VoronoiEdge VE = new VoronoiEdge();
            VE.LeftData = ((VDataNode)C).DataPoint;
            VE.RightData = vertexevent.DataPoint;
            VE.VVertexA = Constants.VVUnkown;
            VE.VVertexB = Constants.VVUnkown;
            veronoigraphbox.Edges.Add(VE);

            VNode SubRoot;
            if (Math.Abs(VE.LeftData[1] - VE.RightData[1]) < 1e-10)
            {
                if (VE.LeftData[0] < VE.RightData[0])
                {
                    SubRoot = new VEdgeNode(VE, false);
                    SubRoot.Left = new VDataNode(VE.LeftData);
                    SubRoot.Right = new VDataNode(VE.RightData);
                }
                else
                {
                    SubRoot = new VEdgeNode(VE, true);
                    SubRoot.Left = new VDataNode(VE.RightData);
                    SubRoot.Right = new VDataNode(VE.LeftData);
                }
                circleCheckList = new VDataNode[] { (VDataNode)SubRoot.Left, (VDataNode)SubRoot.Right };
            }
            else
            {
                SubRoot = new VEdgeNode(VE, false);
                SubRoot.Left = new VDataNode(VE.LeftData);
                SubRoot.Right = new VEdgeNode(VE, true);
                SubRoot.Right.Left = new VDataNode(VE.RightData);
                SubRoot.Right.Right = new VDataNode(VE.LeftData);
                circleCheckList = new VDataNode[] { (VDataNode)SubRoot.Left, (VDataNode)SubRoot.Right.Left, (VDataNode)SubRoot.Right.Right };
            }

            //3. Apply subtree
            if (C.Parent == null)
                return SubRoot;
            C.Parent.Replace(C, SubRoot);
            return root;
        }

        public static VNode ProcessCircleEvent(VCircleEvent e, VNode Root, VoronoiGraph VG, double ys, out VDataNode[] CircleCheckList)
        {
            VDataNode a, b, c;
            VEdgeNode eu, eo;
            b = e.NodeN;
            a = NodeHandler.LeftDataNode(b);
            c = NodeHandler.RightDataNode(b);
            if (a == null || b.Parent == null || c == null || !a.DataPoint.Equals(e.NodeL.DataPoint) || !c.DataPoint.Equals(e.NodeR.DataPoint))
            {
                CircleCheckList = new VDataNode[] { };
                return Root; // Abbruch da sich der Graph verändert hat
            }
            eu = (VEdgeNode)b.Parent;
            CircleCheckList = new VDataNode[] { a, c };
            //1. Create the new Vertex
            Vector VNew = new Vector(e.Center[0], e.Center[1]);
            //			VNew[0] = Fortune.ParabolicCut(a.DataPoint[0],a.DataPoint[1],c.DataPoint[0],c.DataPoint[1],ys);
            //			VNew[1] = (ys + a.DataPoint[1])/2 - 1/(2*(ys-a.DataPoint[1]))*(VNew[0]-a.DataPoint[0])*(VNew[0]-a.DataPoint[0]);
            VG.Vertizes.Add(VNew);
            //2. Find out if a or c are in a distand part of the tree (the other is then b's sibling) and assign the new vertex
            if (eu.Left == b) // c is sibling
            {
                eo = NodeHandler.EdgeToRightDataNode(a);

                // replace eu by eu's Right
                eu.Parent.Replace(eu, eu.Right);
            }
            else // a is sibling
            {
                eo = NodeHandler.EdgeToRightDataNode(b);

                // replace eu by eu's Left
                eu.Parent.Replace(eu, eu.Left);
            }
            eu.Edge.AddVertex(VNew);
            //			///////////////////// uncertain
            //			if(eo==eu)
            //				return Root;
            //			/////////////////////
            eo.Edge.AddVertex(VNew);
            //2. Replace eo by new Edge
            VoronoiEdge VE = new VoronoiEdge();
            VE.LeftData = a.DataPoint;
            VE.RightData = c.DataPoint;
            VE.AddVertex(VNew);
            VG.Edges.Add(VE);

            VEdgeNode VEN = new VEdgeNode(VE, false);
            VEN.Left = eo.Left;
            VEN.Right = eo.Right;
            if (eo.Parent == null)
                return VEN;
            eo.Parent.Replace(eo, VEN);
            return Root;
        }

       
    }
}
