using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ceometric.DelaunayTriangulator;
using WpfApplication1.Models;
using TerrainGenerator.Models;
using TerrainGenerator.Services.GeneratorServices;
using CubesFortune.ImageGenerators.TownMapGen;

namespace TerrainGenerator.Services
{
    public class MapGenService
    {
        public Dictionary<int, Center> Centers { get; set; }
        public Dictionary<int, List<Corner>> Corners { get; set; }
        public Dictionary<int, List<Edge>> Edges { get; set; }

        public List<River> Rivers { get; set; }
        public bool subdivide;
        public IMapService _mapService;
        private int MapX;
        private int MapY;
        private int MapZ;
        public int mapseed;
        public List<Point> points;

        public MapGenService() { }

        public MapGenService(int mapX, int mapY, int mapZ, int seed, bool subdivide)
        {
            mapseed = seed;
            Centers = new Dictionary<int, Center>();
            Corners = new Dictionary<int, List<Corner>>();
            Edges = new Dictionary<int, List<Edge>>();
            Rivers = new List<River>();
            this.subdivide = subdivide;
            MapX = mapX;
            MapY = mapY;
            MapZ = mapZ;
            _mapService = new MapService(this, mapX, mapY, mapZ, new TownMapService(this, mapX, mapY, mapZ));

            points = new List<Point>();
            var rnd = new Random(seed);

            points.Clear();
            for (int i = 0; i < mapX; i++)
            {
                points.Add(new Point(Math.Abs(rnd.NextDouble() * 10),
                                      Math.Abs(rnd.NextDouble() * 10), 0));
            }

        _mapService.LoadMap(points);
            _mapService.GenerateIsland();
            
        }

        public void Draw()
        {
            
        }
    }
}
