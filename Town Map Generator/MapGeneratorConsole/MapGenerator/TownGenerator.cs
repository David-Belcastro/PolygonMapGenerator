using MapGeneratorConsole.ImageGenerators;
using MapGeneratorConsole.ImageGenerators.Graph;
using System;
using System.Linq;
using System.Collections.Generic;
using CubesFortune;

namespace Town_Map_Generator
{
    internal class TownGenerator
    {
        private PolyMap _basemap;
        private List<Centers> landlist;

        private Priority_Queue.FastPriorityQueue<FastTownNode> frontier; 

        internal void GenerateCivilization(PolyMap polymap)
        {
            _basemap = polymap;
            landlist = _basemap.centerlist.FindAll(x => x.mapData.Water == false);
            GenerateVilliagePaths();
            GenerateCities();

        }

        public void GenerateVilliagePaths()
        {
            var camefrom = new Dictionary<Centers, Centers>();
            var costsofar = new Dictionary<Centers, int>();
            frontier = new Priority_Queue.FastPriorityQueue<FastTownNode>(_basemap.centerlist.Count());
            foreach (Centers cnt in landlist)
            {
                var mapdata = cnt.mapData;
                var radiuslist = landlist.FindAll(x => heuristic(cnt.center, x.center) < .1);
                  foreach (Centers othernode in radiuslist)

                      //  foreach (Centers othernode in cnt.neigbors)
                        {
                            {
                                if (!mapdata.Villiage.TownAlreadyMeasured(othernode) && !othernode.mapData.Water )
                                {
                                    MeasureTownDistance(camefrom, costsofar, landlist, cnt, mapdata, othernode);
                                }
                            }
                        }


            }
        }

        private void MeasureTownDistance(Dictionary<Centers, Centers> camefrom, Dictionary<Centers, int> costsofar, List<Centers> landlist, Centers cnt, CenterMapData mapdata, Centers othernode)
        {
            camefrom.Clear();
            costsofar.Clear();
            frontier.Clear();
            costsofar[cnt] = 0;
            frontier.Enqueue(new FastTownNode(cnt, 0), 0);
            while (frontier.Count != 0)
            {
                var currentnode = frontier.Dequeue();
                var current = currentnode.cnt;
                if (current == othernode) { break; }

                            foreach (Centers next in current.neigbors) {
                                MeasureNeigbors(camefrom, costsofar, cnt, mapdata, othernode, current, next); 
                }

            }
            mapdata.Villiage.AddTownToList(costsofar[othernode], othernode, camefrom);
            othernode.mapData.Villiage.AddTownToListReversed(costsofar[othernode], cnt, camefrom);
        }

        private void MeasureNeigbors(Dictionary<Centers, Centers> camefrom, Dictionary<Centers, int> costsofar, Centers cnt, CenterMapData mapdata, Centers othernode, Centers current, Centers next)
        {
            var nextdata = next.mapData;
            var new_cost = costsofar[current] + nextdata.Biome.MovementCost();
            if (!costsofar.TryGetValue(next, out var x) || new_cost < costsofar[next])
            {
                costsofar[next] = new_cost;
                var priority = new_cost + heuristic(othernode.center, next.center);
                frontier.Enqueue(new FastTownNode(next, new_cost), priority*10);
                camefrom[next] = current;
                mapdata.Villiage.AddTownToList(costsofar[next], next, camefrom);
                next.mapData.Villiage.AddTownToListReversed(costsofar[next], cnt, camefrom);
            }
        }

        private static float heuristic(VoronoiPoint a, VoronoiPoint b)
        {
            return (float)Math.Sqrt(Math.Pow((b.X - a.X), 2) + Math.Pow((b.Y - b.Y), 2));
        }

        public void GenerateCities()
        {
            AssignVilliageAgents();
            RedistributeToTowns();
            GrowToCities();
        }

        public void AssignVilliageAgents()
        {
            System.Threading.Tasks.Parallel.ForEach(landlist, cnt => cnt.mapData.CalculateTopDestinations());
        }

        public void RedistributeToTowns()
        {
            foreach (Centers cnt in landlist)
            {
                cnt.mapData.Villiage.DistributeMoneyToChoices();
            }
            var sortedlandlist = new List<Centers>(landlist);
            sortedlandlist.Sort((a, b) => b.mapData.Villiage.CurrentEconPull.CompareTo(a.mapData.Villiage.CurrentEconPull));
            foreach (Centers cnt in landlist)
            {
                cnt.mapData.Villiage.CheckifTownCenter();
            }
        }

        public void GrowToCities()
        {
            var Townlist = landlist.Where(x => x.mapData.Villiage.CurrentEconPull != 0);
            System.Threading.Tasks.Parallel.ForEach(Townlist, cnt => cnt.mapData.CalculateTopCityDestinations());
            foreach (Centers cnt in Townlist)
            {
                cnt.mapData.Villiage.DistributeMoneyToTownChoices();
            }
        }

    }
}