namespace MapGeneratorConsole.ImageGenerators.Graph
{
    public class BasicMapData
    {
        public bool Water { get; set; }
        public double Elevation { get; set; }
        public bool Ocean { get; internal set; }
        public bool Coast { get; internal set; }
    }

    public class CornerMapData : BasicMapData
    {
    }

    public class CenterMapData : BasicMapData
    {
    }

    public class EdgeMapData : BasicMapData
    {
    }
}