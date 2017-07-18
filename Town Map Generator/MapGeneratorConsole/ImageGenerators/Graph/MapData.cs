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

        public CenterMapData()
        {
            Villiage = new Town();
        }
    }

    public class EdgeMapData : BasicMapData
    {
    }
}