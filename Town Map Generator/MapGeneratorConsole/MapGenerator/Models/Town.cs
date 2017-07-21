using System;
using System.Collections.Generic;
using System.Linq;

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

        private Dictionary<Centers,TownDistance> distancetoalltowns;
        public int size;
        private Dictionary<Centers, Centers> reversecamefrom;
        private TownDistance choice1;
        private TownDistance choice2;
        private TownDistance choice3;
        private Centers location;

        private int BaseEconPull { get; set; }
        public int CurrentEconPull;
        public int NormalizedPull = 0;

        public Town(Centers loc)
        {
            location = loc;
            distancetoalltowns = new Dictionary<Centers, TownDistance>();
            reversecamefrom = new Dictionary<Centers, Centers>();
        }

        public void AddTownToList(int pathlenght, Centers towncenter, Dictionary<Centers, Centers> camefrom)
        {
            if (!TownAlreadyMeasured(towncenter) || (TownAlreadyMeasured(towncenter) && pathlenght < distancetoalltowns[towncenter].Distance))
            {
                distancetoalltowns[towncenter] = (new TownDistance { Distance = pathlenght, town = towncenter, pathway = camefrom });
            }
        }

        internal Dictionary<Centers,int> DistanceToAllTowns()
        {
            var result = new Dictionary<Centers, int>();
            foreach (KeyValuePair<Centers,TownDistance> villiage in distancetoalltowns)
            {
                result[villiage.Key] = villiage.Value.Distance;
            }
            return result;
        }

        internal void AddTownToListReversed(int pathlenght, Centers towncenter, Dictionary<Centers, Centers> camefrom)
        {

            if (!TownAlreadyMeasured(towncenter) || pathlenght < distancetoalltowns[towncenter].Distance)
            {
                reversecamefrom.Clear();
                foreach (KeyValuePair<Centers, Centers> node in camefrom.Reverse())
                {
                    reversecamefrom[node.Value] = node.Key;
                }
                distancetoalltowns[towncenter] = (new TownDistance { Distance = pathlenght, town = towncenter, pathway = reversecamefrom });
            }
        }

        internal void CalculateTopCityDestination()
        {
            var towndistances = new List<TownDistance>(distancetoalltowns.Values.Where(x => x.town.mapData.Villiage.CurrentEconPull != 0));
            choice1 = towndistances.Aggregate((a, b) => a.town.mapData.Villiage.CurrentEconPull / Math.Sqrt(a.Distance) > b.town.mapData.Villiage.CurrentEconPull / Math.Sqrt(b.Distance) ? a : b);
            towndistances.Remove(choice1);
            choice2 = towndistances.Aggregate((a, b) => a.town.mapData.Villiage.CurrentEconPull / Math.Sqrt(a.Distance) > b.town.mapData.Villiage.CurrentEconPull / Math.Sqrt(b.Distance) ? a : b);
            towndistances.Remove(choice2);
            choice3 = towndistances.Aggregate((a, b) => a.town.mapData.Villiage.CurrentEconPull / Math.Sqrt(a.Distance) > b.town.mapData.Villiage.CurrentEconPull / Math.Sqrt(b.Distance) ? a : b);
            towndistances.Remove(choice3);
        }

        internal TownSize TownCategory()
        {
            if(CurrentEconPull > 1000)
            {
                return TownSize.Metropolis;
            }
            else if (CurrentEconPull > 600)
            {
                return TownSize.BigCity;
            }
            else if (CurrentEconPull > 300)
            {
                return TownSize.City;
            }
            else if (CurrentEconPull > 100)
            {
                return TownSize.Town;
            }
            else if (CurrentEconPull > 25)
            {
                return TownSize.SmallTown;
            }
            else
            {
                return TownSize.Villiage;
            }
        }

        public void SetBaseEconomics(int value)
        {
            BaseEconPull = value;
            CurrentEconPull = value;
        }

        internal void CalculateTopDestinations(int v)
        {
            var towndistances = new List<TownDistance>(distancetoalltowns.Values);
            choice1 = towndistances.Aggregate((a, b) => a.town.mapData.BaseEconPull / Math.Sqrt( a.Distance) > b.town.mapData.BaseEconPull / Math.Sqrt(b.Distance) ? a : b);
            towndistances.Remove(choice1);
            choice2 = towndistances.Aggregate((a, b) => a.town.mapData.BaseEconPull / Math.Sqrt(a.Distance) > b.town.mapData.BaseEconPull / Math.Sqrt(b.Distance) ? a : b);
            towndistances.Remove(choice2);
            choice3 = towndistances.Aggregate((a, b) => a.town.mapData.BaseEconPull / Math.Sqrt(a.Distance) > b.town.mapData.BaseEconPull / Math.Sqrt(b.Distance) ? a : b);
            towndistances.Remove(choice3);
        }

        public bool TownAlreadyMeasured(Centers center)
        {
            return distancetoalltowns.ContainsKey(center);
        }

        internal void DistributeMoneyToChoices()
        {

            choice1.town.mapData.Villiage.GiveMoneyToTown(BaseEconPull);
            choice2.town.mapData.Villiage.GiveMoneyToTown(Math.Max(BaseEconPull-2,1));
            choice3.town.mapData.Villiage.GiveMoneyToTown(Math.Max(BaseEconPull-3,0));
        }

        internal void DistributeMoneyToTownChoices()
        {
            choice1.town.mapData.Villiage.GiveMoneyToTown(BaseEconPull);
            choice2.town.mapData.Villiage.GiveMoneyToTown(Math.Max(BaseEconPull/2, 20));
            choice3.town.mapData.Villiage.GiveMoneyToTown(Math.Max(BaseEconPull/3, 10));
        }

        private void GiveMoneyToTown(int givenmoney)
        {
            CurrentEconPull += givenmoney; 
        }

        internal void CheckifTownCenter()
        {
            if(CurrentEconPull == 0 && location.neigbors.Any(x => x.mapData.Villiage.CurrentEconPull >= CurrentEconPull)){
                return;
            }
            else
            {
                foreach (Centers cnt in location.neigbors)
                {
                    CurrentEconPull += cnt.mapData.Villiage.CurrentEconPull;
                    cnt.mapData.Villiage.CurrentEconPull = 0;
                }
            }
            
        }
    }

    public enum TownSize
    {
        Metropolis,
        BigCity,
        City,
        Town,
        SmallTown,
        Villiage
        
    }
}