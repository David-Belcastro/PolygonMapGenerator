using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrnVoronoi.Models;

namespace BrnVoronoi.Handlers
{
    public class NodeHandler
    {
        public static VDataNode FirstDataNode(VNode Root)
        {
            VNode C = Root;
            while (C.Left != null)
                C = C.Left;
            return (VDataNode)C;
        }

        public static VDataNode LeftDataNode(VDataNode Current)
        {
            VNode C = Current;
            //1. Up
            do
            {
                if (C.Parent == null)
                    return null;
                if (C.Parent.Left == C)
                {
                    C = C.Parent;
                    continue;
                }
                else
                {
                    C = C.Parent;
                    break;
                }
            } while (true);
            //2. One Left
            C = C.Left;
            //3. Down
            while (C.Right != null)
                C = C.Right;
            return (VDataNode)C; // Cast statt 'as' damit eine Exception kommt
        }

        public static VDataNode RightDataNode(VDataNode Current)
        {
            VNode C = Current;
            //1. Up
            do
            {
                if (C.Parent == null)
                    return null;
                if (C.Parent.Right == C)
                {
                    C = C.Parent;
                    continue;
                }
                else
                {
                    C = C.Parent;
                    break;
                }
            } while (true);
            //2. One Right
            C = C.Right;
            //3. Down
            while (C.Left != null)
                C = C.Left;
            return (VDataNode)C; // Cast statt 'as' damit eine Exception kommt
        }

        public static VEdgeNode EdgeToRightDataNode(VDataNode Current)
        {
            VNode C = Current;
            //1. Up
            do
            {
                if (C.Parent == null)
                    throw new Exception("No Left Leaf found!");
                if (C.Parent.Right == C)
                {
                    C = C.Parent;
                    continue;
                }
                else
                {
                    C = C.Parent;
                    break;
                }
            } while (true);
            return (VEdgeNode)C;
        }

        public static VDataNode FindDataNode(VNode Root, double ys, double x)
        {
            VNode C = Root;
            do
            {
                if (C is VDataNode)
                    return (VDataNode)C;
                if (((VEdgeNode)C).Cut(ys, x) < 0)
                    C = C.Left;
                else
                    C = C.Right;
            } while (true);
        }
    }
}
