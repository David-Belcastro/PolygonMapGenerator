using System;

namespace MapGeneratorConsole.ImageGenerators.Graph
{
    public class BasicMapData
    {
        public bool Water { get; set; }
        public double Elevation { get; set; }
        public bool Ocean { get; internal set; }
        public bool Coast { get; internal set; }
        public int? River { get; set; }
        public double Moisture { get; set; }
    }

    public class CornerMapData : BasicMapData
    {
        internal Corners downslope;
    }

    public class CenterMapData : BasicMapData
    {
        public IBiomes Biome { get; set; }
        public Town Villiage;
        public int BaseEconPull;

        public CenterMapData(Centers location)
        {
            Villiage = new Town(location);
        }

        public void EconomicPullValue(Centers MapCenter)
        {
            var pullvalue = Biome.EconomicPullValue();
            switch (MapCenter.mapData.Biome.BiomeType())
            {
                case BiomeTypes.Bare:
                case BiomeTypes.Snow:
                case BiomeTypes.Ice:
                case BiomeTypes.Scorched:
                    break;

                default:

            foreach(Corners crn in MapCenter.corners)
            {
                pullvalue += crn.mapdata.River ?? 0;
            }

                    foreach (Centers ngbh in MapCenter.neigbors)
                    {
                        if (Econimicboon(ngbh.mapData.Biome.BiomeType()))
                        {
                            pullvalue++;
                        }
                    }
                    break;
            }

            BaseEconPull = pullvalue;
            Villiage.SetBaseEconomics(pullvalue);
        }

        private bool Econimicboon(BiomeTypes biomeTypes)
        {
            switch (biomeTypes)
            {
                case BiomeTypes.Forest:
                case BiomeTypes.TropicalRainForest:
                case BiomeTypes.TemperateRainForest:
                case BiomeTypes.Bare:
                case BiomeTypes.Beach:
                case BiomeTypes.Taiga:
                    return true;
                default:
                    return false;
            }
        }

        internal void CalculateTopDestinations()
        {
            Villiage.CalculateTopDestinations(Biome.EconimicPushValue());
        }

        internal void CalculateTopCityDestinations()
        {
            Villiage.CalculateTopCityDestination();
        }
    }

    public class EdgeMapData : BasicMapData
    {
    }
}