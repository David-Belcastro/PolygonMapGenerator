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
            var point = NodeList[0];
            RemoveSmallest();
            return point;
        }

        public void SortNodeList()
        {
            NodeList.Sort((a, b) =>
            {
                var result = a.Y.CompareTo(b.Y);
                if (result == 0)
                {
                    result = a.X.CompareTo(b.X);
                }
                return result;
            });
        }

        internal bool IsEmpty()
        {
            return NodeList.Count == 0;
        }
    }
}
