using System;
using System.Drawing;
using System.Drawing.Imaging;
using TerrainGenerator.Services;

namespace Town_Map_Generator
{
    public interface IImageGenerator {}

    public class ImageGenerator : IImageGenerator
    {
        public EnvironmentService EnvironmentService { get; set; }
        private static bool _AddVegetation = false;
        private static bool _DrawRivers = true;
        private static bool _Subdivide = true;
        private static int _RandomSeed = 757575424;

        public ImageGenerator(bool veg, bool rivers, bool subdivide, int seed, int mapsize)
        {
            _AddVegetation = veg;
            _DrawRivers = rivers;
            _Subdivide = subdivide;
            _RandomSeed = seed;
            EnvironmentService = new EnvironmentService(_DrawRivers, seed, mapsize);
        }

        internal void createimage(int basesize)
        {
            var b = new Bitmap(basesize, basesize);
            var g = Graphics.FromImage(b);
            EnvironmentService.Draw(g);

            string savestring = string.Format("E:\\Projects\\Town Map Generator\\Images\\Image{0}.PNG", DateTime.Now.Ticks);
            b.Save(@savestring, ImageFormat.Png);

        }
    }
}