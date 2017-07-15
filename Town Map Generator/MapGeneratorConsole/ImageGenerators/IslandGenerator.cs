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
        private double lake_threshold = .3;
        private IslandFactory _islandFactory;

        public IslandGenerator(int seed)
        {
            _islandFactory = new IslandFactory(seed);
        }

        public double GetStdCoord(double oneDcoord)
        {
            return 2 * ((oneDcoord / mapsize) - .5);
        }

        internal void GenerateIsland(PolyMap voronoigraph)
        {
            cornerQueue = new Queue<Corners>();
            _basegraph = voronoigraph;
            foreach (Corners crn in _basegraph.cornerlist)
            {
                AssignLandandBaseElevations(crn);
            }
            while (cornerQueue.Count > 0)
            {
                RaisetheIsland();
            }
            AssignOceanCoastandLandToCenter();
        }

        private void AssignLandandBaseElevations(Corners crn)
        {
            var stdx = GetStdCoord(crn.location.X);
            var stdy = GetStdCoord(crn.location.Y);
            crn.mapdata.Water = !_islandFactory.CheckForLand(stdx, stdy);
            if (crn.border)
            {
                crn.mapdata.Elevation = 0.0;
                cornerQueue.Enqueue(crn);
            }
            else
            {
                crn.mapdata.Elevation = double.MaxValue;
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


    }
}