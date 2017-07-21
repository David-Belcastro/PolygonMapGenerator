using MapGeneratorConsole.ImageGenerators;
using MapGeneratorConsole.ImageGenerators.Graph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Town_Map_Generator
{
    public class IslandGenerator
    {
        private PolyMap _basegraph;
        private double mapsize = 10.0;
        private Queue<Corners> cornerQueue;
        private Queue<Centers> centerQueue;
        private double lake_threshold = .1;
        private IslandFactory _islandFactory;
        private double  mountainscale = 1.1;
        private Random randomizer;

        public IslandGenerator(int seed)
        {
            _islandFactory = new IslandFactory(seed);
            randomizer = new Random();
        }

        public double GetStdCoord(double oneDcoord)
        {
            return 2 * ((oneDcoord / mapsize) - .5);
        }

        internal void GenerateIsland(PolyMap voronoigraph, double variant)
        {
            cornerQueue = new Queue<Corners>();
            _basegraph = voronoigraph;
            foreach (Corners crn in _basegraph.cornerlist)
            {
                AssignLandandBaseElevations(crn, variant);
            }
            while (cornerQueue.Count > 0)
            {
                RaisetheIsland();
            }
            RedistributeElevations(LandCorners(_basegraph.cornerlist));
            AssignOceanCoastandLandToCenter();
            CalculateDownslopes();
            CreateRivers();
            Mmmmoist();
            AssignBiomes();
        }

        private void AssignBiomes()
        {
            foreach (Centers cnt in _basegraph.centerlist)
            {
                cnt.mapData.Biome = getBiome(cnt.mapData);
            }
            foreach (Centers cnt in _basegraph.centerlist)
            {
                cnt.mapData.EconomicPullValue(cnt);
            }
        }

        public IBiomes getBiome(CenterMapData cnt)
        {
            if (cnt.Ocean)
            {
                return new Ocean();
            }
            else if (cnt.Water)
            {
                if (cnt.Elevation < 0.1) return new Marsh();
                if (cnt.Elevation > 0.8) return new Ice();
                return new Lake();
            }
            else if (cnt.Coast)
            {
                return new Beach();
            }
            else if (cnt.Elevation > 0.8)
            {
                if (cnt.Moisture > 0.50) return new Snow();
                else if (cnt.Moisture > 0.33) return new Tundra();
                else if (cnt.Moisture > 0.16) return new Bare();
                else return new Scorched();
            }
            else if (cnt.Elevation > 0.6)
            {
                if (cnt.Moisture > 0.66) return new Taiga();
                else if (cnt.Moisture > 0.33) return new Shrubland();
                else return new Plains();
            }
            else if (cnt.Elevation > 0.3)
            {
                if (cnt.Moisture > 0.83) return new TemperateRainForest();
                else if (cnt.Moisture > 0.50) return new Forest();
                else if (cnt.Moisture > 0.16) return new Grassland();
                else return new Plains();
            }
            else
            {
                if (cnt.Moisture > 0.66) return new TropicalRainForest();
                else if (cnt.Moisture > 0.33) return new Forest();
                else if (cnt.Moisture > 0.16) return new Grassland();
                else return new Desert();
            }
        }

        private void Mmmmoist()
        {
            var moistqueue = new Queue<Corners>();
            BaseFreshwaterMoist(moistqueue);
            while (moistqueue.Count > 0)
            {
                var crn = moistqueue.Dequeue();
                SpreadMoisture(moistqueue, crn);
            }
            foreach (Corners crn in _basegraph.cornerlist)
            {
                if (crn.mapdata.Ocean || crn.mapdata.Coast)
                {
                    crn.mapdata.Moisture += 1.0;
                }
            }
            RedistributeMoisture();
            AssignCenterMoisture();

        }

        private void AssignCenterMoisture()
        {
            foreach (Centers cnt in _basegraph.centerlist)
            {
                var moistsum = 0.0;
                foreach (Corners crn in cnt.corners)
                {
                    moistsum += crn.mapdata.Moisture;
                }
                cnt.mapData.Moisture = moistsum / cnt.corners.Count;
            }
        }

        private void RedistributeMoisture()
        {
            var landcorners = LandCorners(_basegraph.cornerlist);
            var maxmoisture = landcorners.Max(x => x.mapdata.Moisture);
            foreach (Corners crn in landcorners)
            {
                crn.mapdata.Moisture = crn.mapdata.Moisture / maxmoisture;
            }
            //landcorners.Sort((x, y) => y.mapdata.Moisture.CompareTo(x.mapdata.Moisture));
            //for (var i = 0; i < landcorners.Count; i++)
            //{
            //    landcorners[i].mapdata.Moisture = (double)i / (landcorners.Count - 1);
            //}
        }

        private static void SpreadMoisture(Queue<Corners> moistqueue, Corners crn)
            {
                foreach (Corners adjCrn in crn.adjacents)
                {
                    var mewMoist = crn.mapdata.Moisture * 0.9;
                    if (mewMoist > adjCrn.mapdata.Moisture)
                    {
                        adjCrn.mapdata.Moisture = mewMoist;
                        moistqueue.Enqueue(adjCrn);
                    }
                }
            }

        private void BaseFreshwaterMoist(Queue<Corners> moistqueue)
        {
            foreach (Corners crn in _basegraph.cornerlist)
            {
                if ((crn.mapdata.Water || crn.mapdata.River > 0) && !crn.mapdata.Ocean)
                {
                    crn.mapdata.Moisture = crn.mapdata.River > 0 ? Math.Min(3.0, (crn.mapdata.River ?? 0) * .2) : 1.0;
                    moistqueue.Enqueue(crn);
                }
                else
                {
                    crn.mapdata.Moisture = 0.0;
                }
            }
        }

        private void CreateRivers()
        {
            var riverrandomizer = randomizer.Next(3, 750);
            for (var i = 0; i < riverrandomizer; i++)
            {
                var randomCorner = _basegraph.cornerlist[randomizer.Next(0, _basegraph.cornerlist.Count - 1)];
                if (randomCorner.mapdata.Ocean || randomCorner.mapdata.Elevation < .3 || randomCorner.mapdata.Elevation > 0.9) continue;
                while (!randomCorner.mapdata.Coast)
                {
                    if (randomCorner == randomCorner.mapdata.downslope) { break; }
                    var edge = lookupEdgefromCorner(randomCorner, randomCorner.mapdata.downslope);
                    edge.mapdata.River = (edge.mapdata.River ?? 0) + 1;
                    randomCorner.mapdata.River = (randomCorner.mapdata.River ?? 0) + 1;
                    randomCorner.mapdata.downslope.mapdata.River = (randomCorner.mapdata.downslope.mapdata.River ?? 0) + 1;
                    randomCorner = randomCorner.mapdata.downslope;
                }
            }
        }

        private Edges lookupEdgefromCorner(Corners q, Corners downslope)
        {
            foreach (Edges edg in q.protrudes)
            {
                if (edg.voronoiCorner1 == downslope || edg.voronoiCorner2 == downslope)
                {
                    return edg;
                }
            }
                return null;
        }

        private void CalculateDownslopes()
        {
            foreach(Corners q in _basegraph.cornerlist)
            {
                var r = q;
                foreach (Corners s in q.adjacents)
                {
                    if (s.mapdata.Elevation <= r.mapdata.Elevation)
                    {
                        r = s;
                    }
                }
                q.mapdata.downslope = r;
            }
        }

        private void RedistributeElevations(List<Corners> crnList)
        {
            crnList.Sort((x, y) => x.mapdata.Elevation.CompareTo(y.mapdata.Elevation));
            var maxelevation = crnList.Last().mapdata.Elevation;
            foreach (Corners crn in crnList)
            {
                crn.mapdata.Elevation = crn.mapdata.Elevation / maxelevation;
            }
            //for (var i = 0; i < crnList.Count; i++)
            //{
            //    var y = i / (crnList.Count - 1);
            //    var x = Math.Sqrt(mountainscale) - Math.Sqrt(mountainscale * (1 - y));
            //    if (x > 1)
            //    {
            //        x = 1;
            //    }
            //    crnList[i].mapdata.Elevation = x;
            //}
            //crnList.Sort((x, y) => x.mapdata.Elevation.CompareTo(y.mapdata.Elevation));
            //maxelevation = crnList.Last().mapdata.Elevation;
            //foreach (Corners crn in crnList)
            //{
            //    crn.mapdata.Elevation = crn.mapdata.Elevation / maxelevation;
            //}
        }

        private List<Corners> LandCorners(List<Corners> cornerlist)
        {
            var landcorners = cornerlist.FindAll(x => x.mapdata.Water == false && x.mapdata.Coast == false);
            return landcorners;
        }

        private void AssignLandandBaseElevations(Corners crn, double variant)
        {
            var stdx = GetStdCoord(crn.location.X);
            var stdy = GetStdCoord(crn.location.Y);
            crn.mapdata.Water = !_islandFactory.RadialLand(stdx, stdy, variant);
            if (crn.border)
            {
                crn.mapdata.Elevation = 0.0;
                cornerQueue.Enqueue(crn);
            }
            else
            {
                crn.mapdata.Elevation = 255;
            }
        }

        private void RaisetheIsland()
        {
            var crn = cornerQueue.Dequeue();
            foreach (Corners adjcrn in crn.adjacents)
            {
                var newelevation = 0.01 + crn.mapdata.Elevation;
                if (!crn.mapdata.Water && !adjcrn.mapdata.Water)
                {
                    newelevation += 1;
                }
                if (newelevation < adjcrn.mapdata.Elevation)
                {
                    adjcrn.mapdata.Elevation = newelevation;
                    cornerQueue.Enqueue(adjcrn);
                }
            }
        }

        private void AssignOceanCoastandLandToCenter()
        {
            centerQueue = new Queue<Centers>();
            foreach (Centers cnt in _basegraph.centerlist)
            {
                var WaterCornerCount = 0;
                foreach (Corners crn in cnt.corners)
                {
                    if (crn.border)
                    {
                        cnt.border = true;
                        cnt.mapData.Ocean = true;
                        crn.mapdata.Water = true;
                        centerQueue.Enqueue(cnt);
                    }
                    if (crn.mapdata.Water)
                    {
                        WaterCornerCount++;
                    }
                }
                cnt.mapData.Water = (cnt.mapData.Ocean || WaterCornerCount >= cnt.corners.Count * lake_threshold);
            }
            while (centerQueue.Count > 0)
            {
                var cnt = centerQueue.Dequeue();
                foreach (Centers ngbhcnt in cnt.neigbors)
                {
                    if(ngbhcnt.mapData.Water && !ngbhcnt.mapData.Ocean)
                    {
                        ngbhcnt.mapData.Ocean = true;
                        centerQueue.Enqueue(ngbhcnt);
                    }
                }
            }

            foreach(Centers cnt in _basegraph.centerlist)
            {
                var OceanTiles = 0;
                var LandTiles = 0;
                foreach (Centers ngbh in cnt.neigbors)
                {
                    OceanTiles += ngbh.mapData.Ocean ? 1 : 0;
                    LandTiles += ngbh.mapData.Water ? 0 : 1;
                }
                cnt.mapData.Coast = (OceanTiles > 0) && (LandTiles > 0);
                cnt.mapData.Elevation = AverageCornerElevation(cnt);
            }

            foreach (Corners crn in _basegraph.cornerlist)
            {
                var OceanTiles = 0;
                var LandTiles = 0;
                foreach (Centers tch in crn.touches)
                {
                    OceanTiles += tch.mapData.Ocean ? 1 : 0;
                    LandTiles  += tch.mapData.Water ? 0 : 1;
                }
                crn.mapdata.Ocean = (OceanTiles == crn.touches.Count);
                crn.mapdata.Coast = (OceanTiles > 0) && (LandTiles > 0);
                crn.mapdata.Water = crn.border || ((LandTiles != crn.touches.Count) && !crn.mapdata.Coast);
            }
        }

        private double AverageCornerElevation(Centers cnt)
        {
            var Elevationsum = 0.0;
            foreach(Corners crn in cnt.corners)
            {
                if (crn.mapdata.Elevation != double.MaxValue)
                {
                    Elevationsum += crn.mapdata.Elevation;
                }
            }
            return Elevationsum / (double)cnt.corners.Count;

        }
    }
}