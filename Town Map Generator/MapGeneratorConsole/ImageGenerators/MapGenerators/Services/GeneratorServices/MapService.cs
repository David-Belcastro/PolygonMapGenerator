using System;
using System.Linq;
using BrnVoronoi;
using BrnVoronoi.Models;
using TerrainGenerator.Models;
using SlimDX;

namespace TerrainGenerator.Services.GeneratorServices
{
    public interface IMapService
    {
        void LoadMap(LoadMapParams loadMapParams);
    }

    public class MapService : IMapService
    {
        private readonly IIslandService _islandHandler;
        private MapGenService _mapGenService;
        private int _mapX;
        private int _mapY;
        private int _mapZ;

        public MapService(MapGenService mapgenservice, int mapX, int mapY, int mapZ)
        {
            _mapGenService = mapgenservice;
            _mapZ = mapZ;
            _mapY = mapY;
            _mapX = mapX;
            _islandHandler = new IslandService(mapgenservice,_mapX, _mapY, _mapZ);
        }

        public void LoadMap(LoadMapParams loadMapParams)
        {
            //Program.ResetMap();

            VoronoiGraph voronoiMap = null;
            //Smooth(loadMapParams);

            //voronoiMap = Fortune.ComputeVoronoiGraph(loadMapParams.Points);
            var mapper = new VoronoiMapper(loadMapParams.Points);
            voronoiMap = mapper.ComputeVoronoiGraph();
            ImproveMapData(voronoiMap, loadMapParams.Fix);
        }

        private void Smooth(LoadMapParams loadMapParams)
        {
            VoronoiGraph voronoiMap;
            var mapper = new VoronoiMapper(loadMapParams.Points);
            for (int i = 0; i < 3; i++)
            {
                voronoiMap = mapper.ComputeVoronoiGraph();
                foreach (Vector vector in loadMapParams.Points)
                {
                    double v0 = 0.0d;
                    double v1 = 0.0d;
                    int say = 0;
                    foreach (VoronoiEdge edge in voronoiMap.Edges)
                    {
                        if (edge.LeftData == vector || edge.RightData == vector)
                        {
                            double p0 = (edge.VVertexA[0] + edge.VVertexB[0]) / 2;
                            double p1 = (edge.VVertexA[1] + edge.VVertexB[1]) / 2;
                            v0 += double.IsNaN(p0) ? 0 : p0;
                            v1 += double.IsNaN(p1) ? 0 : p1;
                            say++;
                        }
                    }

                    
                    if (((v0 / say) < _mapX) && ((v0 / say) > 0))
                    {
                        vector[0] = v0 / say;
                    }

                    if (((v1 / say) < _mapZ) && ((v1 / say) > 0))
                    {
                        vector[1] = v1 / say;
                    }
                }
            }
        }

        private void ImproveMapData(VoronoiGraph voronoiMap, bool fix = false)
        {
            IFactory fact = new DataFactory(_mapGenService);

            foreach (VoronoiEdge edge in voronoiMap.Edges)
            {
                if (fix)
                {
                    if (!newFix(edge))
                        continue;
                }
                
                Corner c1 = fact.CornerFactory((float) edge.VVertexA[0], 0, (float) edge.VVertexA[1]);
                Corner c2 = fact.CornerFactory((float) edge.VVertexB[0], 0, (float) edge.VVertexB[1]);
                Center cntrLeft = fact.CenterFactory((float) edge.LeftData[0], 0, (float) edge.LeftData[1]);
                Center cntrRight = fact.CenterFactory((float) edge.RightData[0], 0, (float) edge.RightData[1]);

                if(c1.Point.X != (float) edge.VVertexA[0] || c1.Point.Z != (float) edge.VVertexA[1])
                {
                    
                }


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
                    var point = new Vector3(0, 0, 0);
                    foreach (Center c in q.Touches)
                    {
                        point.X += c.Point.X;
                        point.Z += c.Point.Z;
                    }
                    point.X = point.X / q.Touches.Count;
                    point.Z = point.Z / q.Touches.Count;
                    q.Point = point;
                }
            }

            _islandHandler.CreateIsland();
        }

        private bool newFix(VoronoiEdge edge)
        {
            var x1 = (float) edge.VVertexA[0];
            var y1 = (float)edge.VVertexA[1];

            var x2 = (float)edge.VVertexB[0];
            var y2 = (float)edge.VVertexB[1];



            //if both ends are in map, not much to do
            if ((DotInMap(x1, y1) && DotInMap(x2, y2)))
                return true;

            //if one end is out of map
            if ((DotInMap(x1, y1) && !DotInMap(x2, y2)) || (!DotInMap(x1, y1) && DotInMap(x2, y2)))
            {
                double b = 0.0d, slope = 0.0d;

                //and that point is actually a number ( not going to infinite ) 
                if (!(double.IsNaN(x2) || double.IsNaN(y2)))
                {
                    slope = ((y2 - y1) / (x2 - x1));

                    b = edge.VVertexA[1] - (slope * edge.VVertexA[0]);

                    // y = ( slope * x ) + b


                    if (edge.VVertexA[0] < 0)
                        edge.VVertexA = new Vector(0, b);

                    if (edge.VVertexA[0] > _mapX)
                        edge.VVertexA = new Vector(_mapX, (_mapX * slope) + b);

                    if (edge.VVertexA[1] < 0)
                        edge.VVertexA = new Vector((-b / slope), 0);

                    if (edge.VVertexA[1] > _mapZ)
                        edge.VVertexA = new Vector((_mapZ - b) / slope, _mapZ);



                    if (edge.VVertexB[0] < 0)
                        edge.VVertexB = new Vector(0, b);

                    if (edge.VVertexB[0] > _mapX)
                        edge.VVertexB = new Vector(_mapX, (_mapX * slope) + b);

                    if (edge.VVertexB[1] < 0)
                        edge.VVertexB = new Vector((-b / slope), 0);

                    if (edge.VVertexB[1] > _mapZ)
                        edge.VVertexB = new Vector((_mapZ - b) / slope, _mapZ);

                }
                else
                {
                    //and if that end is actually not a number ( going to infinite )
                    if (double.IsNaN(x2) || double.IsNaN(y2))
                    {
                        var x3 = (edge.LeftData[0] + edge.RightData[0]) / 2;
                        var y3 = (edge.LeftData[1] + edge.RightData[1]) / 2;

                        slope = ((y3 - y1) / (x3 - x1));

                        slope = Math.Abs(slope);

                        b = edge.VVertexA[1] - (slope * edge.VVertexA[0]);

                        // y = ( slope * x ) + b
                        var i = 0.0d;

                        if (x3 < y3)
                        {
                            if (_mapX - x3 > y3)
                            {
                                i = b;
                                if (i > 0 && i < 400)
                                    edge.VVertexB = new Vector(0, i);

                            }
                            else
                            {
                                i = (_mapZ - b) / slope;
                                if (i > 0 && i < 400)
                                    edge.VVertexB = new Vector(i, _mapZ);

                            }
                        }
                        else
                        {
                            if (_mapX - x3 > y3)
                            {
                                i = (-b / slope);
                                if (i > 0 && i < 400)
                                    edge.VVertexB = new Vector(i, 0);
                            }
                            else
                            {
                                i = (_mapX * slope) + b;
                                if (i > 0 && i < 400)
                                    edge.VVertexB = new Vector(_mapX, i);

                            }
                        }

                        //if (x3 < App.MapSize / 4)
                        //{
                        //    i = b;
                        //    if (i > 0 && i < 400)
                        //        edge.VVertexB = new BenTools.Mathematics.Vector(0, i);

                        //    //left
                        //}

                        //if (x3 > App.MapSize * 3 / 4)
                        //{
                        //    i = (App.MapSize * slope) + b;
                        //    if (i > 0 && i < 400)
                        //        edge.VVertexB = new BenTools.Mathematics.Vector(App.MapSize, i);

                        //    //right
                        //}

                        //if (y3 > App.MapSize * 3 / 4)
                        //{
                        //    i = (App.MapSize - b) / slope;
                        //    if (i > 0 && i < 400)
                        //        edge.VVertexB = new BenTools.Mathematics.Vector(i, App.MapSize);

                        //    //bottom
                        //}

                        //if (y3 < App.MapSize / 4)
                        //{
                        //    i = (-b / slope);
                        //    if (i > 0 && i < 400)
                        //        edge.VVertexB = new BenTools.Mathematics.Vector(i, 0);

                        //    //top
                        //}


                    }
                }
                return true;
            }
            return false;
        }

        private bool NewestFix(VoronoiEdge edge)
        {
            float x1 = (float) edge.VVertexA[0];
            float y1 = (float) edge.VVertexA[1];

            float x2 = (float) edge.VVertexB[0];
            float y2 = (float) edge.VVertexB[1];



            //if both ends are in map, not much to do
            if ((DotInMap(x1, y1) && DotInMap(x2, y2)))
                return true;

            //if one end is out of map
            if ((DotInMap(x1, y1) && !DotInMap(x2, y2)) || (!DotInMap(x1, y1) && DotInMap(x2, y2)))
            {
                float b = 0.0f, slope = 0.0f;

                //and that Vector3 is actually a number ( not going to infinite ) 
                if (!(float.IsNaN(x2) || float.IsNaN(y2)))
                {
                    slope = ((y2 - y1) / (x2 - x1));

                    b = (float) (edge.VVertexA[1] - (slope * edge.VVertexA[0]));

                    // y = ( slope * x ) + b


                    if (edge.VVertexA[0] < 0)
                        edge.VVertexA = new Vector(0, b);

                    if (edge.VVertexA[0] > _mapX)
                        edge.VVertexA = new Vector(_mapX, (_mapX * slope) + b);

                    if (edge.VVertexA[1] < 0)
                        edge.VVertexA = new Vector((-b / slope), 0);

                    if (edge.VVertexA[1] > _mapY)
                        edge.VVertexA = new Vector((_mapY - b) / slope, _mapY);



                    if (edge.VVertexB[0] < 0)
                        edge.VVertexB = new Vector(0, b);

                    if (edge.VVertexB[0] > _mapX)
                        edge.VVertexB = new Vector(_mapX, (_mapX * slope) + b);

                    if (edge.VVertexB[1] < 0)
                        edge.VVertexB = new Vector((-b / slope), 0);

                    if (edge.VVertexB[1] > _mapY)
                        edge.VVertexB = new Vector((_mapY - b) / slope, _mapY);

                }
                else
                {
                    //and if that end is actually not a number ( going go infinite )
                    if (float.IsNaN(x2) || float.IsNaN(y2))
                    {
                        var x3 = (edge.LeftData[0] + edge.RightData[0]) / 2;
                        var y3 = (edge.LeftData[1] + edge.RightData[1]) / 2;

                        slope = (float) ((y3 - y1) / (x3 - x1));

                        slope = Math.Abs(slope);

                        b = (float) (edge.VVertexA[1] - (slope * edge.VVertexA[0]));

                        // y = ( slope * x ) + b
                        var i = 0.0d;

                        if (x3 < y3)
                        {
                            if (_mapX - x3 > y3)
                            {
                                i = b;
                                if (i > 0 && i < 480)
                                    edge.VVertexB = new Vector(0, i);

                            }
                            else
                            {
                                i = (_mapY - b) / slope;
                                if (i > 0 && i < 800)
                                    edge.VVertexB = new Vector(i, _mapY);

                            }
                        }
                        else
                        {
                            if (_mapX - x3 > y3)
                            {
                                i = (-b / slope);
                                if (i > 0 && i < 800)
                                    edge.VVertexB = new Vector(i, 0);
                            }
                            else
                            {
                                i = (_mapX * slope) + b;
                                if (i > 0 && i < 480)
                                    edge.VVertexB = new Vector(_mapX, i);

                            }
                        }

                        //if (x3 < Program.MapSize / 4)
                        //{
                        //    i = b;
                        //    if (i > 0 && i < 400)
                        //        edge.VVertexB = new BenTools.Mathematics.Vector(0, i);

                        //    //left
                        //}

                        //if (x3 > Program.MapSize * 3 / 4)
                        //{
                        //    i = (Program.MapSize * slope) + b;
                        //    if (i > 0 && i < 400)
                        //        edge.VVertexB = new BenTools.Mathematics.Vector(Program.MapSize, i);

                        //    //right
                        //}

                        //if (y3 > Program.MapSize * 3 / 4)
                        //{
                        //    i = (Program.MapSize - b) / slope;
                        //    if (i > 0 && i < 400)
                        //        edge.VVertexB = new BenTools.Mathematics.Vector(i, Program.MapSize);

                        //    //bottom
                        //}

                        //if (y3 < Program.MapSize / 4)
                        //{
                        //    i = (-b / slope);
                        //    if (i > 0 && i < 400)
                        //        edge.VVertexB = new BenTools.Mathematics.Vector(i, 0);

                        //    //top
                        //}


                    }
                }
                return true;
            }
            return false;
        }

        private bool DotInMap(float x, float y)
        {
            return (x > 0 && x < _mapX) && (y > 0 && y < _mapZ);
        }

    }
}
