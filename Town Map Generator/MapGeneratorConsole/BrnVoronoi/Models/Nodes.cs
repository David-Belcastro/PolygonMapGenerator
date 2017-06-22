using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrnVoronoi.Helpers;

namespace BrnVoronoi.Models
{
    public abstract class VNode
    {
        private VNode _left = null, _right = null;

        protected VNode()
        {
            Parent = null;
        }

        public VNode Left
        {
            get { return _left; }
            set
            {
                _left = value;
                value.Parent = this;
            }
        }

        public VNode Right
        {
            get { return _right; }
            set
            {
                _right = value;
                value.Parent = this;
            }
        }

        public VNode Parent { get; set; }

        public void Replace(VNode childOld, VNode childNew)
        {
            if (Left == childOld)
                Left = childNew;
            else if (Right == childOld)
                Right = childNew;
            else throw new Exception("Child not found!");
            childOld.Parent = null;
        }
    }

    public class VDataNode : VNode
    {
        public VDataNode(Vector dp)
        {
            this.DataPoint = dp;
        }
        public Vector DataPoint;
    }

    public class VEdgeNode : VNode
    {
        public VEdgeNode(VoronoiEdge e, bool flipped)
        {
            this.Edge = e;
            this.Flipped = flipped;
        }
        public VoronoiEdge Edge;
        public bool Flipped;

        public double Cut(double ys, double x)
        {
            return !Flipped 
                ? Math.Round(x - Methods.ParabolicCut(Edge.LeftData[0], Edge.LeftData[1], Edge.RightData[0], Edge.RightData[1], ys), 10) 
                : Math.Round(x - Methods.ParabolicCut(Edge.RightData[0], Edge.RightData[1], Edge.LeftData[0], Edge.LeftData[1], ys), 10);
        }
    }
}
