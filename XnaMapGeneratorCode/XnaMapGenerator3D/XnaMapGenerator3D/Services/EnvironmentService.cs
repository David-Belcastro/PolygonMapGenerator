using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaMapGenerator3D.Models;

namespace XnaMapGenerator3D.Services
{
    public class RiverObject
    {
        public Vector3 First { get; set; }
        public Vector3 Second { get; set; }
        public Vector3 FirstR1 { get; set; }
        public Vector3 FirstR2 { get; set; }
        public Vector3 SecondR1 { get; set; }
        public Vector3 SecondR2 { get; set; }
        
    }

    public class EnvironmentService
    {
        private BGame _game;
        public MapGenService MapGenService { get; set; }
        public static int MapX = 3200;
        public static int MapZ = 3200;
        public static int MapY = 3200;
        public static float MultiplyY = (1);
        private List<VertexPositionNormalTexture> riverVerticesList = new List<VertexPositionNormalTexture>();
        private List<RiverObject> riverList = new List<RiverObject>();
        private readonly Texture2D _riverTexture;
        public EnvironmentService(BGame game)
        {
            _game = game;
            MapGenService = new MapGenService(_game, MapX, MapY, MapZ);

            //if (BGame.DrawRivers)
            //{
            //    CreateRivers();

            //    //CreateRivers2();
            //    _riverTexture = _game.Content.Load<Texture2D>("water");

            //    foreach (RiverObject r in riverList)
            //    {
            //        AddRectangle(riverVerticesList, r.FirstR1, r.FirstR2, r.SecondR1, r.SecondR2);
            //    }
            //}
        }
    

        private void CreateRivers2()
        {
            RiverObject[] Buffer = new RiverObject[riverList.Count];
            riverList.CopyTo(Buffer);

            foreach (RiverObject r in Buffer)
            {
                var sc = riverList.FirstOrDefault(x => x.First == r.Second);

                if (sc == null)
                    continue;

                var a1 = Vector3.Lerp(r.FirstR1, r.SecondR1, 0.5f);
                var a2 = Vector3.Lerp(r.FirstR2, r.SecondR2, 0.5f);

                var b1 = Vector3.Lerp(sc.FirstR1, sc.SecondR1, 0.5f);
                var b2 = Vector3.Lerp(sc.FirstR2, sc.SecondR2, 0.5f);

                var r1 = Vector3.CatmullRom(r.FirstR1, a1, b1, sc.SecondR1, 0.5f);
                var r2 = Vector3.CatmullRom(r.FirstR2, a2, b2, sc.SecondR2, 0.5f);


                riverList.Add(new RiverObject()
                                {
                                    First = (a1 + a2) / 2,
                                    Second = (r1 + r2) / 2,
                                    FirstR1 = a1,
                                    FirstR2 = a2,
                                    SecondR1 = r1,
                                    SecondR2 = r2
                                });

                riverList.Add(new RiverObject()
                {
                    First = (r1 + r2) / 2,
                    Second = (b1 + b2) / 2,
                    FirstR1 = r1,
                    FirstR2 = r2,
                    SecondR1 = b1,
                    SecondR2 = b2
                });

                r.SecondR1 = a1;
                r.SecondR2 = a2;

                sc.FirstR1 = b1;
                sc.FirstR2 = b2;
            }

            //for (int i = 0;  i < riverList.Count - 1;  i++)
            //{
            //    if(riverList[i].Second == riverList[i + 1].First)
            //    {
            //        riverList[i].SecondR1 = new Vector3(riverList[i].SecondR1.X, 100, riverList[i].SecondR1.Z);
            //        riverList[i].SecondR2 = new Vector3(riverList[i].SecondR2.X, 100, riverList[i].SecondR2.Z);

            //        riverList[i + 1].FirstR1 = new Vector3(riverList[i + 1].FirstR1.X, 100, riverList[i + 1].FirstR1.Z);
            //        riverList[i + 1].FirstR2 = new Vector3(riverList[i + 1].FirstR2.X, 100, riverList[i + 1].FirstR2.Z);
            //    }
            //}
        }

        public void CreateRivers()
        {
            foreach (Edge edge in MapGenService.Edges.Values.SelectMany(x => x).Where(x=>x.River > 0))
            {
                var first = edge.VoronoiStart;
                var second = edge.VoronoiEnd;
                var kat = 3;

                var a1 = (first.Point - edge.DelaunayStart.Point);
                a1.Normalize();
                a1 *= edge.River * kat;
                a1.Y += 5;

                var a2 = (first.Point - edge.DelaunayEnd.Point);
                a2.Normalize();
                a2 *= edge.River * kat;
                a2.Y += 5;

                var b1 = (second.Point - edge.DelaunayStart.Point);
                b1.Normalize();
                b1 *= edge.River * kat;
                b1.Y += 5;

                var b2 = (second.Point - edge.DelaunayEnd.Point);
                b2.Normalize();
                b2 *= edge.River * kat;
                b2.Y += 5;


                riverList.Add(new RiverObject()
                                  {
                                      First = first.Point,
                                      Second = second.Point,
                                      FirstR1 = first.Point + a1,
                                      FirstR2 = first.Point + a2,
                                      SecondR1 = second.Point + b1,
                                      SecondR2 = second.Point + b2
                                  });

                //AddRectangle(riverVerticesList, first.Point + a1, first.Point + a2, second.Point + b1, second.Point + b2);
            }
        }

        public void Draw()
        {
            foreach (Center center in MapGenService.Centers.Values)
            {
                center.Draw(_game, _game.CameraService.ViewMatrix, _game.CameraService.ProjectionMatrix);
            }

            //if (riverVerticesList.Count > 0)
            //{
            //    _game.ShaderEffect.TextureEnabled = true;
            //    _game.ShaderEffect.Texture = _riverTexture;
            //    _game.ShaderEffect.VertexColorEnabled = false;
            //    //_game.ShaderEffect.LightingEnabled = true;

            //    foreach (var pass in _game.ShaderEffect.CurrentTechnique.Passes)
            //    {
            //        pass.Apply();
            //    }

            //    _game.ShaderEffect.CurrentTechnique.Passes[0].Apply();

            //    _game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, riverVerticesList.ToArray(), 0,
            //                                            riverVerticesList.Count/3);
            //}
        }


        public void AddRectangle(List<VertexPositionNormalTexture> list, Vector3 first, Vector3 second, Vector3 third, Vector3 fourth)
        {
            var p1 = first;
            var p2 = second;
            var p3 = third;
            var p4 = fourth;

            bool check = Area(p1, p2, p3) > 0;
            
            if (check)
            {
                Vector3 Norm = new Vector3(0, 1, 0);

                list.Add(new VertexPositionNormalTexture(
                                    new Vector3(p3.X, p3.Y, p3.Z),
                                    Norm,
                                    new Vector2(0, 1)
                                    ));

                list.Add(new VertexPositionNormalTexture(
                                    new Vector3(p1.X, p1.Y, p1.Z),
                                    Norm,
                                    new Vector2(0, 0)
                                    ));

                list.Add(new VertexPositionNormalTexture(
                                    new Vector3(p2.X, p2.Y, p2.Z),
                                    Norm,
                                    new Vector2(1, 0)
                                    ));
            }
            else
            {
                Vector3 Norm = new Vector3(0, 1, 0);

                list.Add(new VertexPositionNormalTexture(
                                        new Vector3(p1.X, p1.Y, p1.Z),
                                        Norm,
                                        new Vector2(0, 0)
                                        ));


                list.Add(new VertexPositionNormalTexture(
                                        new Vector3(p3.X, p3.Y, p3.Z),
                                        Norm,
                                        new Vector2(0, 1)
                                        ));

                list.Add(new VertexPositionNormalTexture(
                                        new Vector3(p2.X, p2.Y, p2.Z),
                                        Norm,
                                        new Vector2(1, 0)
                                        ));

            }

            check = Area(p2, p3, p4) > 0;

            if (check)
            {
                Vector3 Norm = new Vector3(0, 1, 0);

                list.Add(new VertexPositionNormalTexture(
                                    new Vector3(p2.X, p2.Y, p2.Z),
                                    Norm,
                                    new Vector2(1, 0)
                                    ));

                list.Add(new VertexPositionNormalTexture(
                                    new Vector3(p3.X, p3.Y, p3.Z),
                                    Norm,
                                    new Vector2(0, 1)
                                    ));

                list.Add(new VertexPositionNormalTexture(
                                    new Vector3(p4.X, p4.Y, p4.Z),
                                    Norm,
                                    new Vector2(1, 1)
                                    ));
            }
            else
            {
                Vector3 Norm = new Vector3(0, 1, 0);

                list.Add(new VertexPositionNormalTexture(
                                    new Vector3(p2.X, p2.Y, p2.Z),
                                    Norm,
                                    new Vector2(1, 0)
                                    ));

                list.Add(new VertexPositionNormalTexture(
                                    new Vector3(p4.X, p4.Y, p4.Z),
                                    Norm,
                                    new Vector2(1, 1)
                                    ));

                list.Add(new VertexPositionNormalTexture(
                                    new Vector3(p3.X, p3.Y, p3.Z),
                                    Norm,
                                    new Vector2(0, 1)
                                    ));


            }
        }

        private Vector3 CalculateNormal(Vector3 newMain, Vector3 p1, Vector3 p2)
        {
            var v1 = new Vector3(newMain.X - p1.X, newMain.Y - p1.Y, newMain.Z - p1.Z);
            var v2 = new Vector3(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
            return Vector3.Cross(v1, v2);
        }

        public float Area(Vector3 A, Vector3 B, Vector3 C)
        {
            return ((A.X - C.X) * (B.Z - C.Z) - (A.Z - C.Z) * (B.X - C.X)) / 2;
        }

        private Vector3 CalculateNormal2(Vector3 newMain, Vector3 p1, Vector3 p2)
        {
            var v1 = new Vector3(newMain.X - p2.X, newMain.Y - p2.Y, newMain.Z - p2.Z);
            var v2 = new Vector3(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return Vector3.Cross(v1, v2);
        }

    }
}
