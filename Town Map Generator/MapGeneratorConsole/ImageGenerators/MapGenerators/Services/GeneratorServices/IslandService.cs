using System;
using System.Collections.Generic;
using System.Linq;
using TerrainGenerator.Helpers;
using TerrainGenerator.Models;
using SlimDX;
using MapGeneratorConsole.Utilities;

namespace TerrainGenerator.Services.GeneratorServices
{
    public interface IIslandService
    {
        void CreateIsland();
    }

    public class IslandService : IIslandService
    {
        private int counter;
        public double MapMaxHeight = 0.0d;
        public double MapDeepest = 1.0d;
        private int _mapX;
        private int _mapY;
        private int _mapZ;
        private MapGenService _mapGen;

        public IslandService(MapGenService mapgen, int mapX, int mapY, int mapZ)
        {
            _mapX = mapX;
            _mapZ = mapZ;
            _mapY = mapY;
            _mapGen = mapgen;
        }

        public void CreateIsland()
        {
            foreach (var c in _mapGen.Corners.Values.SelectMany(x => x))
            {
                c.Water = !InLand(c.Point); // calculate land&water Corners.Values.SelectMany(x => x)
            }

            FixCentersFloodFillOceans();

            foreach (var c in _mapGen.Corners.Values.SelectMany(x => x))
            {
                c.Coast = (c.Touches.Any(x => x.Water) && c.Touches.Any(x => x.Land)) ? true : false;

                c.Water = (c.Touches.Any(x => x.Land)) ? false : true;

                c.Ocean = (c.Touches.All(x => x.Ocean)) ? true : false;
                //coasts are water , island is smaller ->
            }

            CalculateElevations();
            

            
            CalculateCornerDownslopes();

            
            CalculateRivers();

            foreach (River river in _mapGen.Rivers)
            {
                Vector3 first, second, third, fourth;

                for (int x = 1; x < river.Corners.Count - 1; x++)
                {
                    second = river.Corners[x - 1].Point;

                    if (x == 1)
                        first = second;
                    else
                        first = river.Corners[x - 1].Point;

                    third = river.Corners[x + 1].Point;

                    if (x == river.Corners.Count - 2)
                        fourth = third;
                    else
                        fourth = river.Corners[x + 2].Point;


                    river.Corners[x].Point = Vector3.CatmullRom(
                        first,
                        second,
                        third,
                        fourth,
                        0.5f);

                    river.Corners[x].Elevation -= (river.Corners[x].River*river.Corners[x].Elevation)/3;
                }
                
            }

            CalculateCornerMoisture();
            
            foreach (Center center in _mapGen.Centers.Values)
            {
                center.Elevation = (float)(center.Corners.Sum(x => x.Elevation) / center.Corners.Count());
            }

            foreach (Corner c in _mapGen.Corners.Values.SelectMany(x => x))
            {
                foreach (Center ce in c.Touches)
                {
                    foreach (Corner co in ce.Corners)
                    {
                        if (c.Adjacents.Any(x => x.Point.X == co.Point.X && x.Point.Y == co.Point.Y && x.Point.Z == co.Point.Z))
                        {
                             var Norm = Vector3.Zero;

                            if (BMathHelper.Area(ce.Point, c.Point, co.Point) < 0)
                            {
                                Norm = BMathHelper.CalculateNormal(ce.Point, c.Point, co.Point);
                            }
                            else
                            {
                                Norm = BMathHelper.CalculateNormal(ce.Point, co.Point, c.Point);
                            }

                            c.Normal = new Vector3(c.Normal.X + Norm.X, c.Normal.Y + Norm.Y, c.Normal.Z + Norm.Z);
                        }
                    }
                }
            }

            for (int i = 0; i < 1; i++)
            {
                if (_mapGen.subdivide)
                    Subdivide();
            }

            foreach (Corner c in _mapGen.Corners.Values.SelectMany(x => x).Where(x => x.Coast))
            {
                var p1e = c.Protrudes.FirstOrDefault(x => x.Coast);
                var p2e = c.Protrudes.FirstOrDefault(x => x != p1e && x.Coast);

                if (p1e == null || p2e == null)
                    continue;

                var first = p1e.Corners.FirstOrDefault(x => x != c && x.Coast);
                var second = p2e.Corners.FirstOrDefault(x => x != c && x.Coast);

                if (first == null || second == null)
                    continue;

                c.Point = new Vector3(
                    (first.Point.X + second.Point.X) / 2,
                    c.Point.Y,
                    (first.Point.Z + second.Point.Z) / 2);

            }
            Console.WriteLine("Setting Biomes...");
            var islandprogress = new ProgressBar();
            counter = 0;
            foreach (Corner corner in _mapGen.Corners.Values.SelectMany(x => x))
            {
                corner.SetBiome();
                counter++;
                islandprogress.Report((double)counter / _mapGen.Corners.Values.Count);
            }

            Console.WriteLine("\rSetting Elevations...");
            counter = 0;
            foreach (var center in _mapGen.Centers.Values)
            {
                
                center.Elevation = (float)(center.Corners.Sum(x => x.Elevation) / center.Corners.Count());
                counter++;
                islandprogress.Report((double) counter / _mapGen.Centers.Values.Count);
            }


            Console.WriteLine("\rSetting Moistures and Polys...");
            counter = 0;
            foreach (var ct in _mapGen.Centers.Values)
            {

                ct.Moisture = (float) (ct.Corners.Sum(x => x.Moisture) / ct.Corners.Count());

                ct.SetBiome();

                ct.GetPolygons();
                counter++;
                islandprogress.Report((double)counter / _mapGen.Centers.Values.Count);

            }

            
        }

        private void Subdivide()
        {
            var fact = new DataFactory(_mapGen);
            var org = new Corner[_mapGen.Corners.Values.SelectMany(x => x).Count()];
            _mapGen.Corners.Values.SelectMany(x => x).ToArray().CopyTo(org, 0);

            var bck = new Dictionary<int, Corner>();

            foreach (Center c in _mapGen.Centers.Values)
            {
                foreach (Corner co in c.Corners)
                {
                    if (bck.ContainsKey(co.Key))
                        continue;

                    var back = new Corner(0, 0, 0);
                    back.Key = co.Key;

                    var qx = co.Touches.Sum(x => x.Point.X) / co.Touches.Count;
                    var qy = co.Touches.Sum(x => x.Point.Y) / co.Touches.Count;
                    var qz = co.Touches.Sum(x => x.Point.Z) / co.Touches.Count;

                    var rx = co.Protrudes.Sum(x => x.FreshMidpoint.X) / co.Protrudes.Count;
                    var ry = co.Protrudes.Sum(x => x.FreshMidpoint.Y) / co.Protrudes.Count;
                    var rz = co.Protrudes.Sum(x => x.FreshMidpoint.Z) / co.Protrudes.Count;

                    var div = co.Protrudes.Count;
                    var newp = ((new Vector3(qx, qy, qz) / div) +
                                (2 * new Vector3(rx, ry, rz) / div) +
                                ((div - 3) * new Vector3(co.Point.X, co.Point.Y, co.Point.Z) / div));

                    //var newp = new Point((qx / div) + (2 * rx / div) + (co.Point.X / div),
                    //    (qy / div) + (2 * ry / div) + (co.Point.Y / div));

                    back.Point = new Vector3(newp.X, newp.Y, newp.Z);
                    back.Moisture = co.Moisture;
                    back.Watershed = co.Watershed;

                    bck.Add(co.Key, back);
                }
            }

            foreach (var t in bck)
            {
                _mapGen.Corners[t.Key].FirstOrDefault(x => x.Moisture == t.Value.Moisture).Point = t.Value.Point;
            }

            foreach (Center c in _mapGen.Centers.Values)
            {
                var buff = new Edge[c.Borders.Count];
                c.Borders.CopyTo(buff);
                c.Borders.Clear();

                foreach (Edge edge in buff)
                {
                    if (edge.DelaunayEnd == null || edge.DelaunayStart == null)
                        continue;


                    var r1 = new Vector3((edge.VoronoiStart.Point.X + edge.VoronoiEnd.Point.X + edge.DelaunayStart.Point.X) / 3,
                            (edge.VoronoiStart.Point.Y + edge.VoronoiEnd.Point.Y + edge.DelaunayStart.Point.Y) / 3,
                            (edge.VoronoiStart.Point.Z + edge.VoronoiEnd.Point.Z + edge.DelaunayStart.Point.Z) / 3);

                    var r2 = new Vector3((edge.VoronoiStart.Point.X + edge.VoronoiEnd.Point.X + edge.DelaunayEnd.Point.X) / 3,
                            (edge.VoronoiStart.Point.Y + edge.VoronoiEnd.Point.Y + edge.DelaunayEnd.Point.Y) / 3,
                            (edge.VoronoiStart.Point.Z + edge.VoronoiEnd.Point.Z + edge.DelaunayEnd.Point.Z) / 3);

                    var po = new Vector3((edge.VoronoiStart.Point.X + edge.VoronoiEnd.Point.X + r1.X + r2.X) / 4,
                                       (edge.VoronoiStart.Point.Y + edge.VoronoiEnd.Point.Y + r1.Y + r2.Y) / 4,
                                       (edge.VoronoiStart.Point.Z + edge.VoronoiEnd.Point.Z + r1.Z + r2.Z) / 4);

                    fact.RemoveEdge(edge);

                    var co = fact.CornerFactory(po.X, po.Y, po.Z);
                    co.Water = (edge.DelaunayStart.Water && edge.DelaunayEnd.Water) ? true : false;
                    co.Coast = (edge.DelaunayStart.Coast && edge.DelaunayEnd.Coast) || ((edge.DelaunayStart.Water && !edge.DelaunayEnd.Water) || (!edge.DelaunayStart.Water && edge.DelaunayEnd.Water)) ? true : false;
                    co.Ocean = (edge.DelaunayStart.Ocean && edge.DelaunayEnd.Ocean) ? true : false;
                    //co.Coast = ((edge.DelaunayStart.Water && !edge.DelaunayEnd.Water) || (!edge.DelaunayStart.Water && edge.DelaunayEnd.Water)) ? true : false;
                    co.Elevation = (edge.VoronoiStart.Elevation + edge.VoronoiEnd.Elevation) / 2;
                    co.Moisture = (edge.VoronoiStart.Moisture + edge.VoronoiEnd.Moisture) / 2;
                    co.Normal = (edge.VoronoiStart.Normal + edge.VoronoiEnd.Normal) / 2;
                    co.Normal.Normalize();

                    c.Corners.Add(co);
                    var e1 = fact.EdgeFactory(edge.VoronoiStart, co, edge.DelaunayStart, edge.DelaunayEnd);
                    c.Borders.Add(e1);
                    var e2 = fact.EdgeFactory(co, edge.VoronoiEnd, edge.DelaunayStart, edge.DelaunayEnd);
                    c.Borders.Add(e2);

                    e1.River = edge.River;
                    e2.River = edge.River;

                    co.Protrudes.Add(e1);
                    co.Protrudes.Add(e2);
                    co.Touches.Add(edge.DelaunayStart);
                    co.Touches.Add(edge.DelaunayEnd);

                    edge.VoronoiStart.Protrudes.Remove(edge);
                    edge.VoronoiStart.Protrudes.Add(e1);

                    edge.VoronoiEnd.Protrudes.Remove(edge);
                    edge.VoronoiEnd.Protrudes.Add(e2);

                }
            }

            
        }

        private void SmoothCoast()
        {
            DataFactory fact = new DataFactory(_mapGen);

            var newl = new List<Edge>();

            foreach (Edge edge in _mapGen.Edges.Values.SelectMany(x => x).Where(x => x.Coast))
            {
                newl.Add(edge);
            }

            foreach (Edge edge in newl)
            {
                var first = edge.VoronoiStart;
                var second = edge.VoronoiEnd;

                var p1e = first.Protrudes.FirstOrDefault(x => x != edge && x.Coast);
                var p2e = second.Protrudes.FirstOrDefault(x => x != edge && x.Coast);

                if (p1e == null || p2e == null)
                    continue;

                var firsttang = p1e.Corners.First(x => x != first);
                var secondtang = p2e.Corners.First(x => x != second);

                fact.RemoveEdge(edge);
                edge.DelaunayStart.Borders.Remove(edge);
                edge.DelaunayEnd.Borders.Remove(edge);
                
                first.Protrudes.Remove(edge);
                second.Protrudes.Remove(edge);
                
                first.Adjacents.Remove(second);
                second.Adjacents.Remove(first);

                var previous = first;

                for (int i = 1; i < 3; i++)
                {
                    var n = Vector3.CatmullRom(firsttang.Point, first.Point, second.Point, secondtang.Point, (float)i / 3);
                    
                    var newCorner = fact.CornerFactory(n.X, n.Y, n.Z);
                    newCorner.Elevation = previous.Point.Y;
                    newCorner.Coast = true;
                    var e1 = fact.EdgeFactory(previous, newCorner, edge.DelaunayStart, edge.DelaunayEnd);
                    //var e2 = fact.EdgeFactory(nc, p2, edge.DelaunayStart, edge.DelaunayEnd);
                    e1.Coast = true;
                    newCorner.Protrudes.Add(e1);
                    newCorner.Adjacents.Add(previous);

                    edge.DelaunayStart.Corners.Add(newCorner);
                    edge.DelaunayEnd.Corners.Add(newCorner);

                    edge.DelaunayEnd.Borders.Add(e1);
                    edge.DelaunayStart.Borders.Add(e1);

                    previous.Protrudes.Add(e1);

                    previous = newCorner;
                }

                var e2 = fact.EdgeFactory(previous, second, edge.DelaunayStart, edge.DelaunayEnd);
                //var e2 = fact.EdgeFactory(nc, p2, edge.DelaunayStart, edge.DelaunayEnd);

                edge.DelaunayEnd.Borders.Add(e2);
                edge.DelaunayStart.Borders.Add(e2);

                previous.Protrudes.Add(e2);
                second.Protrudes.Add(e2);

            }

            foreach (Corner c in _mapGen.Corners.Values.SelectMany(x => x).Where(x => x.Coast))
            {
                var p1e = c.Protrudes.FirstOrDefault(x => x.Coast);
                var p2e = c.Protrudes.FirstOrDefault(x => x != p1e && x.Coast);

                if (p1e == null || p2e == null)
                    continue;

                var first = p1e.Corners.FirstOrDefault(x => x != c && x.Coast);
                var second = p2e.Corners.FirstOrDefault(x => x != c && x.Coast);

                if (first == null || second == null)
                    continue;

                c.Point = new Vector3(
                    (first.Point.X + second.Point.X)/2,
                    c.Point.Y,
                    (first.Point.Z + second.Point.Z)/2);
            }
        }
        

        private void FixCentersFloodFillOceans()
        {
            foreach (var ct in _mapGen.Centers.Values)
            {
                FixBorders(ct); //Fix Edges.Values at map border , set borders and oceans at borders
                
                //if it touches any water corner , it's water ; there will be leftovers tho
                ct.Water = (ct.Corners.Any(x => x.Water)) ? true : false;
            }

            var Oceans = new Queue<Center>();
            //start with oceans at the borders
            foreach (Center c in _mapGen.Centers.Values.Where(c => c.Ocean))
            {
                Oceans.Enqueue(c);
            }

            //floodfill oceans
            while (Oceans.Count > 0)
            {
                Center c = Oceans.Dequeue();

                foreach (Center n in c.Neighbours.Where(x => !x.Ocean))
                {
                    if (n.Corners.Any(x => x.Water))
                    {
                        n.Ocean = true;
                        if (!Oceans.Contains(n))
                            Oceans.Enqueue(n);
                    }
                    else
                    {
                        n.Coast = true;
                    }
                }
            }
        }

        public void FixBorders(Center ct)
        {
            var ms = from p in ct.Borders.SelectMany(x => x.Corners)
                     group p by p.Point
                         into grouped
                         select new { Point = grouped.Key, count = grouped.Count() };

            var fpoint = ms.FirstOrDefault(x => x.count == 1);
            var spoint = ms.LastOrDefault(x => x.count == 1);

            if (fpoint != null & spoint != null)
            {
                Corner p1 = ct.Corners.FirstOrDefault(x => x.Point == fpoint.Point);
                Corner p2 = ct.Corners.FirstOrDefault(x => x.Point == spoint.Point);

                if (p1 == null || p2 == null)
                    return;

                IFactory fact = new DataFactory(_mapGen);
                Edge e = fact.EdgeFactory(
                    p1,
                    p2,
                    ct, null);

                e.MapEdge = true;

                p1.Protrudes.Add(e);
                p2.Protrudes.Add(e);

                ct.Border = ct.Ocean = ct.Water = true;
                e.VoronoiStart.Border = e.VoronoiEnd.Border = true;
                e.VoronoiStart.Elevation = e.VoronoiEnd.Elevation = 0.0f;

                ct.Borders.Add(e);
            }
        }

        private bool InLand(Vector3 p)
        {
            return IsLandShape(new Vector3((float) (2 * (p.X / _mapX - 0.5)), 0, (float) (2 * (p.Z / _mapZ - 0.5))));
        }

        private bool IsLandShape(Vector3 Vector3)
        {
            double ISLAND_FACTOR = 1.02;
            var islandRandom = new Random(_mapGen.mapseed);
            int bumps = islandRandom.Next(1, 6);
            double startAngle = islandRandom.NextDouble() * 2 * Math.PI;
            double dipAngle = islandRandom.NextDouble() * 2 * Math.PI;
            double dipWidth = islandRandom.Next(2, 7) / 10;

            double angle = Math.Atan2(Vector3.Z, Vector3.X);
            double length = 0.5 * (Math.Max(Math.Abs(Vector3.X), Math.Abs(Vector3.Z)) + BMathHelper.GetPointLength(Vector3));

            double r1 = 0.5 + 0.40 * Math.Sin(startAngle + bumps * angle + Math.Cos((bumps + 3) * angle));
            double r2 = 0.7 - 0.20 * Math.Sin(startAngle + bumps * angle - Math.Sin((bumps + 2) * angle));
            if (Math.Abs(angle - dipAngle) < dipWidth
                || Math.Abs(angle - dipAngle + 2 * Math.PI) < dipWidth
                || Math.Abs(angle - dipAngle - 2 * Math.PI) < dipWidth)
            {
                r1 = r2 = 0.2;
            }
            return (length < r1 || (length > r1 * ISLAND_FACTOR && length < r2));
        }

        private void CalculateElevations()
        {
            Queue<Corner> Lands = new Queue<Corner>();
            Queue<Corner> Sea = new Queue<Corner>();

            foreach (Corner c in _mapGen.Corners.Values.SelectMany(x => x))
            {
                if (c.Ocean)
                {
                    c.Elevation = -1000.0f;
                }
                else
                {
                    c.Elevation = float.MaxValue;
                }

                if (c.Coast && c.Touches.Any(x => x.Ocean))
                {
                    if (c.Land)
                        c.Elevation = 0.0f;
                    Lands.Enqueue(c);
                    Sea.Enqueue(c);
                }
            }

            while (Lands.Count() > 0)
            {
                Corner c = Lands.Dequeue();

                foreach (var a in c.Adjacents.Where(x => !x.Ocean))
                {
                    var newElevation = ((!a.Coast && !a.Water)
                        ? c.Elevation * 1.07 + 1
                        : c.Elevation);
                    
                    if (newElevation < a.Elevation)
                    {
                        a.Elevation = (float) newElevation;
                        if (newElevation > MapMaxHeight)
                            MapMaxHeight = newElevation;
                        Lands.Enqueue(a);
                    }
                }
            }

            while (Sea.Count() > 0)
            {
                Corner c = Sea.Dequeue();

                foreach (var a in c.Adjacents.Where(x => x.Water))
                {
                    float newElevation = (!a.Adjacents.Any(x => x.Coast && c.Land))
                        ? c.Elevation - 1
                        : c.Elevation;
                    if (newElevation > a.Elevation)
                    {
                        a.Elevation = newElevation;
                        if (newElevation < MapDeepest)
                            MapDeepest = newElevation;
                        Sea.Enqueue(a);
                    }
                }
            }

            foreach (Corner c in _mapGen.Corners.Values.SelectMany(x => x))
            {
                if (c.Land)
                    c.Elevation = (float) (c.Elevation / MapMaxHeight) * (EnvironmentService.MapY / 5) ;
                if (c.Water)
                    c.Elevation = (float) (-(c.Elevation / MapDeepest));
            }

        }

        private void CalculateCornerMoisture()
        {
            Queue<Corner> queue = new Queue<Corner>();

            foreach (Corner q in _mapGen.Corners.Values.SelectMany(x => x).Where(q => (q.Water || q.River > 0)))
            {
                q.Moisture = q.Ocean ? 1.5 : (q.Water ? 1.5 : (q.River > 0 ? Math.Max(1.0, (0.4 * q.River)) : 0.1));
                queue.Enqueue(q);
            }

            while (queue.Count() > 0)
            {
                var q = queue.Dequeue();

                foreach (Corner r in q.Adjacents)
                {
                    var newMoisture = q.Moisture * 0.85;
                    if (newMoisture > r.Moisture)
                    {
                        r.Moisture = newMoisture;
                        queue.Enqueue(r);
                    }
                }
            }
        }

        private void CalculateRivers()
        {
            Random rnd = new Random(_mapGen.mapseed);
            for (var i = 0; i < _mapX / 16; i++)
            {
                var q = _mapGen.Corners.Values.SelectMany(x => x).ElementAt(rnd.Next(_mapGen.Corners.Values.SelectMany(x => x).Count() - 1));
                if (q.Water || q.Coast || q.Elevation < 0.2 * EnvironmentService.MapY / 12 || q.Elevation > 0.6 * EnvironmentService.MapY / 12) continue;
                // Bias rivers to go west: if (q.downslope.x > q.x) continue;

                River nr = new River(q);

                while (!q.Coast)
                {
                    if (q == q.Downslope)
                    {
                        break;
                    }
                    Edge edge = q.Protrudes.FirstOrDefault(ed => ed.VoronoiStart == q.Downslope || ed.VoronoiEnd == q.Downslope);
                    if(edge != null)
                        edge.River = edge.River + 1;
                    q.River = (Math.Max(q.River, 0)) + 1;
                    q.Downslope.River = (Math.Max(q.Downslope.River, 0)) + 1;  // TODO: fix double count
                    nr.Add(q.Downslope);
                    q = q.Downslope;
                }

                _mapGen.Rivers.Add(nr);
            }
        }

        private void CalculateCornerDownslopes()
        {
            foreach (Corner c in _mapGen.Corners.Values.SelectMany(x => x))
            {
                var q = c;
                foreach (Corner cs in c.Adjacents)
                {
                    if (cs.Elevation < q.Elevation)
                        q = cs;
                }

                c.Downslope = q;
            }
        }
    }
}
