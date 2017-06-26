using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using CubesFortune.Utilities;
using TerrainGenerator.Helpers;
using TerrainGenerator.Services;
using TerrainGenerator.Services.GeneratorServices;

namespace CubesFortune.ImageGenerators.TownMapGen
{
    class TownMapService : LandFormGenerator, ILandFormGenerator
    {
        private int counter;
        public double MapMaxHeight = 0.0d;
        public double MapDeepest = 1.0d;
        private int _mapX;
        private int _mapY;
        private int _mapZ;


        public TownMapService(MapGenService mapgen, int mapX, int mapY, int mapZ)
        {
            _mapX = mapX;
            _mapZ = mapZ;
            _mapY = mapY;
            _mapGen = mapgen;
        }

        public void CreateIsland()
        {
            foreach (var ct in _mapGen.Centers.Values)
            {
                FixBorders(ct);
                ct.GetPolygons();
            }
            AddTouchesAndNormals();
        }
    }
}
