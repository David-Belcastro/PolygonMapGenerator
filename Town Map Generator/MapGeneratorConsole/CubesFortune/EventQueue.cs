using System;
using System.Collections.Generic;

namespace MapGeneratorConsole.CubesFortune
{
    public class EventQueue {

        public List<IVoronoiPoint> NodeList = new List<IVoronoiPoint>();

        public void AddNode(IVoronoiPoint newPoint) {
            NodeList.Add(newPoint);
            SortNodeList();
        }
        public void RemoveSmallest() {
            NodeList.RemoveAt(0);
        }

        public IVoronoiPoint PeekAndRemoveSmallest() {
            var point = PeekSmallest();
            RemoveSmallest();
            return point;
        }

        public IVoronoiPoint PeekSmallest()
        {
            var point = NodeList[0];
            return point;
        }

        public virtual void SortNodeList()
        {
            NodeList.Sort((a, b) =>
            {
                var result = a.X.CompareTo(b.X);
                if (result == 0)
                {
                    result = a.Y.CompareTo(b.Y);
                }
                return result;
            });
        }

        internal bool IsEmpty()
        {
            return NodeList.Count == 0;
        }
    }

    public class CircleQueue :EventQueue
    {
        public override void SortNodeList()
        {
            NodeList.Sort((a, b) =>
            {
                var result = a.circleLength.CompareTo(b.circleLength);
                if (result == 0)
                {
                    result = a.X.CompareTo(b.X);
                }
                return result;
            });
        }

    }
}
