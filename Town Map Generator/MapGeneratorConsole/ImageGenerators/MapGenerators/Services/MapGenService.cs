using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrnVoronoi.Models;
using WpfApplication1.Models;
using TerrainGenerator.Models;
using TerrainGenerator.Services.GeneratorServices;

namespace TerrainGenerator.Services
{
    public class MapGenService
    {
        public Dictionary<int,Center> Centers { get; set; }
        public Dictionary<int, List<Corner>> Corners { get; set; }
        public Dictionary<int, List<Edge>> Edges { get; set; }

        public List<River> Rivers { get; set; }
        public bool subdivide;
        private IMapService _mapService;
        private int MapX;
        private int MapY;
        private int MapZ;
        public int mapseed;

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
            _mapService = new MapService(this,mapX,mapY,mapZ);

            var points = new HashSet<Vector>();
            var rnd = new Random(seed);

            points.Clear();
            for (int i = 0; i < 16000; i++)
            {
                points.Add(new Vector(Math.Abs(rnd.NextDouble() * EnvironmentService.MapX),
                                      Math.Abs(rnd.NextDouble() * EnvironmentService.MapZ)));
            }

            _mapService.LoadMap(new LoadMapParams(points, true));
        }

        public void Draw()
        {
            
        }
    }
}
