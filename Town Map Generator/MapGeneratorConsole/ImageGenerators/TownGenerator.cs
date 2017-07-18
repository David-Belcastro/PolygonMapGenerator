using MapGeneratorConsole.ImageGenerators;
using MapGeneratorConsole.ImageGenerators.Graph;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Town_Map_Generator
{
    internal class TownGenerator
    {
        private PolyMap _basemap;

        internal void GenerateCivilization(PolyMap polymap)
        {
            _basemap = polymap;
            GenerateVilliagePaths();


        }

        public void GenerateVilliagePaths()
        {
            var camefrom = new Dictionary<Centers, Centers>();
            var costsofar = new Dictionary<Centers, int>();
            var landlist = _basemap.centerlist.FindAll(x => x.mapData.Water == false);
            foreach (Centers cnt in landlist)
            {
                var mapdata = cnt.mapData;
                foreach (Centers othernode in landlist) {
                    camefrom.Clear();
                    costsofar.Clear();
                    Priority_Queue.FastPriorityQueue<FastTownNode> frontier = new Priority_Queue.FastPriorityQueue<FastTownNode>(landlist.Count());

                    costsofar[cnt] = 0;
                    frontier.Enqueue(new FastTownNode(cnt, 0),0);
                    while (frontier.Count != 0)
                    {
                        var currentnode = frontier.Dequeue();
                        var current = currentnode.cnt;
                        if (current == othernode){ break; }
                        foreach (Centers next in current.neigbors)
                        {
                            var nextdata = next.mapData;
                            var new_cost = costsofar[current] + nextdata.Biome.MovementCost();
                            if (! costsofar.ContainsKey(next) || new_cost < costsofar[next])
                            {
                                costsofar[next] = new_cost;
                                frontier.Enqueue(new FastTownNode(next, new_cost),new_cost);
                                camefrom[next] = current;                                    
                            }
                        }

                    }
                    mapdata.Villiage.AddTownToList(costsofar[othernode], othernode, camefrom);
                }

            }
        }
    }
}