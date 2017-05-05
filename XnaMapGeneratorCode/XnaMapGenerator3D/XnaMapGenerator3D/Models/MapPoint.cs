using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication1.Models
{
    public class MapPoint : IEquatable<MapPoint>
    {
        public MapPoint(float d, float d1)
        {
            this.X = d;
            this.Y = d1;
        }

        public float X { get; set; }
        public float Y { get; set; }

        public bool Equals(MapPoint other)
        {
            return (this.X == other.X && this.Y == other.Y);
        }
    }

}
