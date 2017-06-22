using System;
using System.Collections.Generic;
using BrnVoronoi;
using ceometric.DelaunayTriangulator;
using MapGeneratorConsole.CubesFortune;
using TerrainGenerator.Models;
using SlimDX;
using System.Linq;

namespace TerrainGenerator.Services.GeneratorServices
{
    public interface IMapService
    {
        VoronoiMap mapgraphy { get; set; }

        void LoadMap(List<Point> points);
        void GenerateIsland();
        List<ceometric.DelaunayTriangulator.Triangle> ReturnTris();

    }

    public class MapService : IMapService
    {
        private readonly ILandFormGenerator _islandHandler;
        private MapGenService _mapGenService;
        private int _mapX;
        private int _mapY;
        private int _mapZ;
        public List<ceometric.DelaunayTriangulator.Triangle> tris;
        public VoronoiMap mapgraphy { get; set; }

        public MapService() { }

        public MapService(MapGenService mapgenservice, int mapX, int mapY, int mapZ, ILandFormGenerator islandhandler)
        {
            _mapGenService = mapgenservice;
            _mapZ = mapZ;
            _mapY = mapY;
            _mapX = mapX;
            _islandHandler = islandhandler;
        }

        public void LoadMap(List<Point> points)
        {
            var voronoiMap = new CubesVoronoiMapper();
            mapgraphy = voronoiMap.GimmesomeVeoroiois(points);
            //ImproveMapData(mapgraphy);
            
        }

        public void GenerateIsland()
        {
            _islandHandler.CreateIsland();
        }

        public bool DotInMap(float x, float y)
        {
            return (x > 0 && x < _mapX) && (y > 0 && y < _mapZ);
        }

        private void ImproveMapData(VoronoiMap voronoiMap)
        {
            IFactory fact = new DataFactory(_mapGenService);
            foreach (VoronoiSegment segment in voronoiMap.graph)
            {
                Corner c1 = fact.CornerFactory((float)segment.start.X, 0, (float)segment.start.Y);
                Corner c2 = fact.CornerFactory((float)segment.end.X, 0, (float)segment.end.Y);
                Center cntrLeft = fact.CenterFactory((float)segment.LeftNode.X, 0, (float)segment.LeftNode.Y);
                Center cntrRight = fact.CenterFactory((float)segment.RightNode.X, 0, (float)segment.RightNode.Y);

                c1.AddAdjacent(c2);
                c2.AddAdjacent(c1);

                cntrRight.Corners.Add(c1);
                cntrRight.Corners.Add(c2);

                cntrLeft.Corners.Add(c1);
                cntrLeft.Corners.Add(c2);

                Edge e = fact.EdgeFactory(c1, c2, cntrLeft, cntrRight);                    
                cntrLeft.Borders.Add(e);
                cntrRight.Borders.Add(e);

                cntrLeft.Neighbours.Add(cntrRight);
                cntrRight.Neighbours.Add(cntrLeft);

                c1.AddProtrudes(e);
                c2.AddProtrudes(e);
                c1.AddTouches(cntrLeft);
                c1.AddTouches(cntrRight);
                c2.AddTouches(cntrLeft);
                c2.AddTouches(cntrRight);
            }

            foreach (Corner q in _mapGenService.Corners.Values.SelectMany(x => x))
            {
                if (!q.Border)
                {
                    AddPointToCorner(q);
                }
                }
            }

        void AddPointToCorner(Corner inspectedCorner)
        {
                var point = new Vector3(0, 0, 0);
                foreach (Center c in inspectedCorner.Touches)
                {
                    point.X += c.Point.X;
                    point.Z += c.Point.Z;
                }
                point.X = point.X / inspectedCorner.Touches.Count;
                point.Z = point.Z / inspectedCorner.Touches.Count;
                inspectedCorner.Point = point;
            

          //  _islandHandler.CreateIsland();
        }

        public List<ceometric.DelaunayTriangulator.Triangle> ReturnTris()
        {
            return tris;
        }
    }
}
