using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using XnaMapGenerator3D.Services;
using Color = Microsoft.Xna.Framework.Color;

namespace XnaMapGenerator3D.Models
{
    public static class BiomeTypes
    {
        public static Biome Ocean = new Biome("Ocean", "363661");
        public static Biome ShallowWater = new Biome("ShallowWater", "364671");
        public static Biome Ice = new Biome("Ice", "364671");
        public static Biome Lake = new Biome("Lake", "364671");
        public static Biome Beach = new Biome("Beach", "ac9f8b");
        public static Biome Snow = new Biome("Snow", "FFFFFF");
        public static Biome Tundra = new Biome("Tundra", "c4ccbb"); //"c4ccbb"
        public static Biome Bare = new Biome("Beach", "bbbbbb");
        public static Biome Scorched = new Biome("Beach", "999999");
        public static Biome Marsh = new Biome("Marsh", "c4ccbb");
        public static Biome Cliff = new Biome("Cliff", Color.Brown);
        public static Biome Taiga = new Biome("Taiga", "ccd4bb"); //"ccd4bb"
        public static Biome Shrubland = new Biome("Shrubland", "99a68b");
        public static Biome TemperateDesert = new Biome("TemperateDesert", "e4e8ca");
        public static Biome TemperateRainForest = new Biome("TemperateRainForest", "a4c4a8"); //"a4c4a8"
        public static Biome TemperateDeciduousForest = new Biome("TemperateDeciduousForest", "b4c9a9"); //"b4c9a9"
        public static Biome Grassland = new Biome("Grassland", "99b470");
        public static Biome SubtropicalDesert = new Biome("SubtropicalDesert", "e9ddc7");
        public static Biome TropicalRainForest = new Biome("TropicalRainForest", "9cbba9"); //"9cbba9"
        public static Biome TropicalSeasonalForest = new Biome("TropicalSeasonalForest", "558b70"); //"558b70"

        public static Biome BiomeSelector(bool ocean, bool water, bool coast, float elevation, double moisture)
        {
            if (ocean && elevation < -0.1d )
            {
                return BiomeTypes.Ocean;
            }
            else if (ocean && elevation > -0.1d )
            {
                return BiomeTypes.ShallowWater;
            }
            else if (water)
            {
                //if (elevation < 0.1 * EnvironmentService.MapY / 6)
                //{
                //    return BiomeTypes.Marsh;
                //}
                if (elevation > 0.8 * EnvironmentService.MapY / 12)
                {
                    return BiomeTypes.Ice;
                }
                else
                {
                    return BiomeTypes.Lake;
                }
            }
            else if (coast)
            {
                return BiomeTypes.Beach;
            }
            else if (elevation > 0.9 * EnvironmentService.MapY / 12)
            {
                if (moisture > 0.50)
                {
                    return BiomeTypes.Snow;
                }
                else if (moisture > 0.33)
                {
                    return BiomeTypes.Tundra;
                }
                else if (moisture > 0.16)
                {
                    return BiomeTypes.Bare;
                }
                else
                {
                    return BiomeTypes.Scorched;
                }
            }
            else if (elevation > 0.5 * EnvironmentService.MapY / 12)
            {
                if (moisture > 0.66)
                {
                    return BiomeTypes.Taiga;
                }
                else if (moisture > 0.33)
                {
                    return BiomeTypes.Shrubland;
                }
                else
                {
                    return BiomeTypes.TemperateDesert;
                }
            }
            else if (elevation > 0.3 * EnvironmentService.MapY / 12)
            {
                if (moisture > 0.83)
                {
                    return BiomeTypes.TemperateRainForest;
                }
                else if (moisture > 0.50)
                {
                    return BiomeTypes.TemperateDeciduousForest;
                }
                else if (moisture > 0.16)
                {
                    return BiomeTypes.Grassland;
                }
                else
                {
                    return BiomeTypes.TemperateDesert;
                }
            }
            else
            {
                if (moisture > 0.66)
                {
                    return BiomeTypes.TropicalRainForest;
                }
                else if (moisture > 0.33)
                {
                    return BiomeTypes.TropicalSeasonalForest;
                }
                else if (moisture > 0.16)
                {
                    return BiomeTypes.Grassland;
                }
                else
                {
                    return BiomeTypes.SubtropicalDesert;
                }
            }
        }
    }

    public class Biome
    {
        public string Name { get; set; }
        public Color Color { get; set; }

        public Biome(string name, Color color)
        {
            Name = name;
            Color = color;
        }

        public Biome(string name, string color)
        {
            Name = name;
            Color = GetColor(color);
        }

        public static Color GetColor(string hex)
        {
            System.Drawing.Color cv = ColorTranslator.FromHtml("#" + hex);

            Color ret = new Color(cv.R, cv.G, cv.B, 255);

            return ret;
        }
    }
}
