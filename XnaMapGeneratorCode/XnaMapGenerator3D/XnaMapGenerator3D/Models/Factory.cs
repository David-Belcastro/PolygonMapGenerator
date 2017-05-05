using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using XnaMapGenerator3D.Services;

namespace XnaMapGenerator3D.Models
{
    class CenterComparer : IEqualityComparer<Center>
    {
        public CenterComparer() { }
        public bool Equals(Center x, Center y)
        {
            //return (x.Vector3.X == y.Vector3.X) && (x.Vector3.Y == y.Vector3.Y);
            return GetHashCode(x) == GetHashCode(y);
        }

        public int GetHashCode(Center obj)
        {
            return obj.Point.GetHashCode();
        }
    }

    class CornerComparer : IEqualityComparer<Corner>
    {
        public CornerComparer() { }
        public bool Equals(Corner x, Corner y)
        {
            return x.Point.Equals(y.Point);
        }

        public int GetHashCode(Corner obj)
        {
            return obj.Point.GetHashCode();
        }
    }

    class EdgeComparer : IEqualityComparer<Edge>
    {
        public EdgeComparer() { }
        public bool Equals(Edge x, Edge y)
        {
            return x.Midpoint.Z == y.Midpoint.Z && x.Midpoint.X == y.Midpoint.X && x.Midpoint.Y == y.Midpoint.Y;
        }

        public int GetHashCode(Edge obj)
        {
            return obj.Midpoint.GetHashCode();
        }
    }

    public interface IFactory
    {
        Center CenterFactory(BGame game, float ax, float ay, float az);
        Edge EdgeFactory(Corner begin, Corner end, Center Left, Center Right);
        Corner CornerFactory(float ax, float ay, float az);
    }

    public class DataFactory : IFactory
    {
        private MapGenService _mapGen;

        public DataFactory(MapGenService mapGen)
        {
            _mapGen = mapGen;
        }

        #region Implementation of IFactory

        public Center CenterFactory(BGame game, float ax, float ay, float az)
        {
            Vector3 p = new Vector3( ax, ay , az);
            int hash = p.GetHashCode();
            if(_mapGen.Centers.ContainsKey(hash))
            {
                return _mapGen.Centers[hash];
            }
            else
            {
                var nc = new Center(game, ax, ay, az);
                _mapGen.Centers.Add(nc.Key, nc);
                return nc;
            }
        }

        public Edge EdgeFactory(Corner begin, Corner end, Center Left, Center Right)
        {
            Vector3 p = new Vector3((begin.Point.X + end.Point.X) / 2, (begin.Point.Y + end.Point.Y) / 2, (begin.Point.Z + end.Point.Z) / 2);
            int hash = p.GetHashCode();
            if (_mapGen.Edges.ContainsKey(hash))
            {
                var a = _mapGen.Edges[hash].FirstOrDefault(x => x.VoronoiStart.Point == begin.Point && x.VoronoiEnd.Point == end.Point);

                if (a != null)
                {
                    return a;
                }
                else
                {
                    var nc = new Edge(begin, end, Left, Right);
                    _mapGen.Edges[hash].Add(nc);
                    return nc;
                }
            }
            else
            {
                var b = new List<Edge>();
                var nc = new Edge(begin, end, Left, Right);
                b.Add(nc);
                _mapGen.Edges.Add(nc.Key, b);
                return nc;
            }
        }

        public Corner CornerFactory(float ax, float ay, float az)
        {
            Vector3 p = new Vector3( ax,  ay , az);
            int hash = p.GetHashCode();
            if (_mapGen.Corners.ContainsKey(hash))
            {
                var a = _mapGen.Corners[hash].FirstOrDefault(x => x.Point.X == p.X && x.Point.Y == p.Y && x.Point.Z == p.Z);

                if (a != null)
                {
                    if(a.Point.X == ax && a.Point.Y == ay && a.Point.Z == az)
                    {
                        return a;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    var nc = new Corner(ax, ay, az);
                    _mapGen.Corners[hash].Add(nc);
                    return nc;
                }
            }
            else
            {
                var a = new List<Corner>();
                var nc = new Corner(ax, ay, az);
                a.Add(nc);
                _mapGen.Corners.Add(nc.Key, a);
                return nc;
            }
        }

        public void RemoveEdge(Edge e)
        {
            _mapGen.Edges.Remove(e.Midpoint.GetHashCode());
        }

        public void RemoveCorner(Corner e)
        {
            _mapGen.Corners.Remove(e.Point.GetHashCode());
        }

        #endregion
    }
}
