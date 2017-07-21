using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGeneratorConsole.ImageGenerators
{
    public interface IBiomes
    {
        SolidBrush MapColor();
        int MovementCost();
        BiomeTypes BiomeType();
        int EconomicPullValue();
        int EconimicPushValue();
    }



    public abstract class Biome : IBiomes
    {
        abstract protected int Movement { get; }
        abstract protected BiomeTypes Type { get; }
        abstract protected SolidBrush mapcolor { get; }
        abstract protected int ResourcePush { get; }
        abstract protected int ResourcePull { get; }

        public SolidBrush MapColor()
        {
            return mapcolor;
        }

        public BiomeTypes BiomeType()
        {
            return Type;
        }

        public int MovementCost()
        {
            return Movement;
        }

        public int EconomicPullValue()
        {
            return ResourcePull;
        }

        public int EconimicPushValue()
        {
            return ResourcePush;
        }
    }

    public class Ocean : Biome
    {
        protected override int Movement { get { return 3; } }
        protected override BiomeTypes Type { get { return BiomeTypes.Ocean; } }
        protected override SolidBrush mapcolor { get { return new SolidBrush(Color.DarkBlue); } }
        protected override int ResourcePush { get {return 3;} }
        protected override int ResourcePull { get { return 0; } }
    }
    public class Marsh : Biome
    {
        protected override int Movement { get { return  10;}}
        protected override BiomeTypes Type { get { return   BiomeTypes.Marsh;}}
        protected override SolidBrush mapcolor { get { return    new SolidBrush(Color.Teal);} }
        protected override int ResourcePush { get { return 2; } }
        protected override int ResourcePull { get { return 0; } }

    }
    public class Ice : Biome
    {
        protected override int Movement { get { return  50;}}
        protected override BiomeTypes Type { get { return   BiomeTypes.Ice;}}
        protected override SolidBrush mapcolor { get { return  new SolidBrush(Color.GhostWhite);} }
        protected override int ResourcePush { get { return 0; } }
        protected override int ResourcePull { get { return 0; } }
    }
    public class Lake : Biome
    {
        protected override int Movement { get { return  3;}}
        protected override BiomeTypes Type { get { return   BiomeTypes.Lake;}}
        protected override SolidBrush mapcolor { get { return  new SolidBrush(Color.Blue);} }
        protected override int ResourcePush { get { return 3; } }
        protected override int ResourcePull { get { return 0; } }
    }
    public class Beach : Biome
    {
        protected override int Movement { get { return  2;}}
        protected override BiomeTypes Type { get { return   BiomeTypes.Beach;}}
        protected override SolidBrush mapcolor { get { return  new SolidBrush(Color.AntiqueWhite);} }
        protected override int ResourcePush { get { return 3; } }
        protected override int ResourcePull { get { return 3; } }
    }
    public class Snow : Biome
    {
        protected override int Movement { get { return  6;}}
        protected override BiomeTypes Type { get { return   BiomeTypes.Snow;}}
        protected override SolidBrush mapcolor { get { return  new SolidBrush(Color.White);} }
        protected override int ResourcePush { get { return 0; } }
        protected override int ResourcePull { get { return 0; } }
    }
    public class Tundra : Biome
    {
        protected override int Movement { get { return  5;}}
        protected override BiomeTypes Type { get { return   BiomeTypes.Tundra;}}
        protected override SolidBrush mapcolor { get { return  new SolidBrush(Color.Snow);} }
        protected override int ResourcePush { get { return 1; } }
        protected override int ResourcePull { get { return 0; } }
    }
    public class Bare : Biome
    {
        protected override int Movement { get { return  15;}}
        protected override BiomeTypes Type { get { return   BiomeTypes.Bare;}}
        protected override SolidBrush mapcolor { get { return  new SolidBrush(Color.LightGray);} }
        protected override int ResourcePush { get { return 1; } }
        protected override int ResourcePull { get { return 0; } }
    }
    public class Scorched : Biome
    {
        protected override int Movement { get { return  7;}}
        protected override BiomeTypes Type { get { return   BiomeTypes.Scorched;}}
        protected override SolidBrush mapcolor { get { return  new SolidBrush(Color.Sienna);} }
        protected override int ResourcePush { get { return 0; } }
        protected override int ResourcePull { get { return 0; } }
    }
    public class Taiga : Biome
    {
        protected override int Movement { get { return  5;}}
        protected override BiomeTypes Type { get { return   BiomeTypes.Taiga;}}
        protected override SolidBrush mapcolor { get { return  new SolidBrush(Color.CadetBlue);} }
        protected override int ResourcePush { get { return 2; } }
        protected override int ResourcePull { get { return 1; } }
    }
    public class Shrubland : Biome
    {
        protected override int Movement { get { return  4;}}
        protected override BiomeTypes Type { get { return   BiomeTypes.Shrubland;}}
        protected override SolidBrush mapcolor { get { return  new SolidBrush(Color.Tan);} }
        protected override int ResourcePush { get { return 2; } }
        protected override int ResourcePull { get { return 1; } }
    }
    public class Plains : Biome
    {
        protected override int Movement { get { return  2;}}
        protected override BiomeTypes Type { get { return   BiomeTypes.Plains;}}
        protected override SolidBrush mapcolor { get { return  new SolidBrush(Color.Goldenrod);} }
        protected override int ResourcePush { get { return 2; } }
        protected override int ResourcePull { get { return 2; } }
    }
    public class TemperateRainForest : Biome
    {
        protected override int Movement { get { return  4;}}
        protected override BiomeTypes Type { get { return   BiomeTypes.TemperateRainForest;}}
        protected override SolidBrush mapcolor { get { return  new SolidBrush(Color.DarkGreen);} }
        protected override int ResourcePush { get { return 2; } }
        protected override int ResourcePull { get { return 1; } }
    }
    public class Forest : Biome
    {
        protected override int Movement { get { return  3;}}
        protected override BiomeTypes Type { get { return   BiomeTypes.Forest;}}
        protected override SolidBrush mapcolor { get { return  new SolidBrush(Color.Green);} }
        protected override int ResourcePush { get { return 3; } }
        protected override int ResourcePull { get { return 2; } }
    }
    public class Grassland : Biome
    {
        protected override int Movement { get { return  2;}}
        protected override BiomeTypes Type { get { return   BiomeTypes.Grassland;}}
        protected override SolidBrush mapcolor { get { return  new SolidBrush(Color.LawnGreen);} }
        protected override int ResourcePush { get { return 5; } }
        protected override int ResourcePull { get { return 3; } }
    }
    public class TropicalRainForest : Biome
    {
        protected override int Movement { get { return  8;}}
        protected override BiomeTypes Type { get { return   BiomeTypes.TropicalRainForest;}}
        protected override SolidBrush mapcolor { get { return  new SolidBrush(Color.YellowGreen);} }
        protected override int ResourcePush { get { return 2; } }
        protected override int ResourcePull { get { return 0; } }
    }
    public class Desert : Biome
    {
        protected override int Movement { get { return  6;}}
        protected override BiomeTypes Type { get { return   BiomeTypes.Desert;}}
        protected override SolidBrush mapcolor { get { return  new SolidBrush(Color.Moccasin);} }
        protected override int ResourcePush { get { return 1; } }
        protected override int ResourcePull { get { return 1; } }
    }
    

    public enum BiomeTypes
{   Ocean,
    Marsh,
    Ice,
    Lake,
    Beach,
    Snow,
    Tundra,
    Bare,
    Scorched,
    Taiga,
    Shrubland,
    Plains,
    TemperateRainForest,
    Forest,
    Grassland,
    TropicalRainForest,
    Desert
}
}
