using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrnVoronoi.Models;
using WpfApplication1.Models;
using XnaMapGenerator3D.Models;
using XnaMapGenerator3D.Services.GeneratorServices;

namespace XnaMapGenerator3D.Services
{
    public class MapGenService
    {
        private BGame _game;
        public Dictionary<int,Center> Centers { get; set; }
        public Dictionary<int, List<Corner>> Corners { get; set; }
        public Dictionary<int, List<Edge>> Edges { get; set; }

        public List<River> Rivers { get; set; }

        private IMapService _mapService;
        private int MapX;
        private int MapY;
        private int MapZ;

        public MapGenService(BGame game, int mapX, int mapY, int mapZ)
        {
            _game = game;
            Centers = new Dictionary<int, Center>();
            Corners = new Dictionary<int, List<Corner>>();
            Edges = new Dictionary<int, List<Edge>>();
            Rivers = new List<River>();

            MapX = mapX;
            MapY = mapY;
            MapZ = mapZ;
            _mapService = new MapService(game, this,mapX,mapY,mapZ);

            var points = new HashSet<Vector>();
            var rnd = new Random(BGame.RandomSeed);

            points.Clear();
            for (int i = 0; i < 5000; i++)
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
