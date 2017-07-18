using MapGeneratorConsole.ImageGenerators.Graph;
using System;
using System.Collections.Generic;
using Priority_Queue;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGeneratorConsole.ImageGenerators
{
    class PathfindingQueue
    {
        internal class CenterDistance
        {
            public int Distance;
            public Centers center;

            public CenterDistance(int dist, Centers cnt)
            {
                Distance = dist;
                center = cnt;
            }
        }
        public List<CenterDistance> NodeList = new List<CenterDistance>();

        public void AddNode(Centers newPoint, int Distance)
        {
            NodeList.Add(new CenterDistance(Distance, newPoint));
            SortNodeList();
        }
        public void RemoveSmallest()
        {
            NodeList.RemoveAt(0);
        }

        public Centers PeekAndRemoveSmallest()
        {
            var point = PeekSmallest();
            RemoveSmallest();
            return point;
        }

        public Centers PeekSmallest()
        {
            var point = NodeList[0];
            return point.center;
        }

        public virtual void SortNodeList()
            {
                NodeList.Sort((a, b) =>
                {
                    var result = a.Distance.CompareTo(b.Distance);
                    return result;
                });
            }

            public bool IsEmpty()
            {
                return NodeList.Count == 0;
            }
        
    }

    public class FastTownNode : FastPriorityQueueNode
    {
        public Centers cnt;
        public int v;

        public FastTownNode(Centers cnt, int v)
        {
            this.cnt = cnt;
            this.v = v;
        }

    }
}
