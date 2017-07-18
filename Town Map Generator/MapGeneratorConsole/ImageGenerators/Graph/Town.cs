using System;
using System.Collections.Generic;

namespace MapGeneratorConsole.ImageGenerators.Graph
{
    public class Town
    {
        internal class TownDistance
        {
            public int Distance;
            public Centers town;
            public Dictionary<Centers,Centers> pathway;
        }

        private List<TownDistance> distancetoalltowns;
        public int size;

        public Town()
        {
            distancetoalltowns = new List<TownDistance>();
        }

        public void AddTownToList(int pathlenght, Centers towncenter, Dictionary<Centers, Centers> camefrom)
        {
            distancetoalltowns.Add(new TownDistance { Distance = pathlenght, town = towncenter, pathway = camefrom });
        }

        internal Dictionary<Centers,int> DistanceToAllTowns()
        {
            var result = new Dictionary<Centers, int>();
            foreach (TownDistance villiage in distancetoalltowns)
            {
                result[villiage.town] = villiage.Distance;
            }
            return result;
        }
    }
}