using System;
using System.Drawing;
using System.Drawing.Imaging;
using CubesFortune;

namespace Town_Map_Generator
{
    public interface IImageGenerator {}

    public class ImageGenerator : IImageGenerator
    {
        private static int _RandomSeed = 757575424;
        private PointGenerator pointgen;
        private CubesVoronoiMapper VoronoiGenerator;
        private ImageDrawer ImagePainter;

        public ImageGenerator(int seed)
        {
            _RandomSeed = seed;
            ImagePainter = new ImageDrawer();
        }

        internal void createimage(int points)
        {
            pointgen = new PointGenerator(_RandomSeed);
            VoronoiGenerator = new CubesVoronoiMapper();
            var pointlist = pointgen.Givemepoints(points);
            var voronoimap = VoronoiGenerator.GimmesomeVeoroiois(pointlist);
            var polymap = new PolyMap(voronoimap, pointlist);
            var b = new Bitmap(1000,1000);
            var g = Graphics.FromImage(b);            
            var finallimage = ImagePainter.DrawMapGraph(pointlist, polymap.polys);
            finallimage = ImagePainter.DrawVoronoi(pointlist, voronoimap);
            string savestring = string.Format("E:\\Projects\\MapGenerator\\Images\\Image{0}.PNG", DateTime.Now.Ticks);
            finallimage.Save(@savestring, ImageFormat.Png);

        }
    }
}