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
        internal Biomes Biome { get; set; }
    }

    public class EdgeMapData : BasicMapData
    {
    }
}