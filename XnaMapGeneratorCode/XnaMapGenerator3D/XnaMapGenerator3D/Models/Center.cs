using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WpfApplication1.Models;
using XnaMapGenerator3D.Services;
using Color = Microsoft.Xna.Framework.Color;

namespace XnaMapGenerator3D.Models
{
    public class Center : IEquatable<Center>, IMapItem
    {
        public int Index { get; set; }

        public int Key { get; set; }
        public Vector3 Point { get; set; }  // location
        public Boolean Water { get; set; }  // lake or ocean
        public Boolean Land { get { return !Water; } set { Water = !value; } }  // lake or ocean
        public Boolean Ocean { get; set; }  // ocean
        public Boolean ShallowWater { get; set; }
        public Boolean Coast { get; set; }  // land polygon touching an ocean
        public Boolean Border { get; set; }  // at the edge of the map
        public Vector3 Normal { get; set; }
        
        public Biome Biome { get; set; }
        Polygon Polygon { get; set; }
        
        public float Elevation
        {
            get { return this.Point.Y; }
            set { this.Point = new Vector3(Point.X, (float)value, Point.Z); }
        }
        public double Moisture { get; set; }  // 0.0-1.0

        private readonly BGame _game;

        public HashSet<Center> Neighbours { get; set; }
        public HashSet<Edge> Borders { get; set; }
        public HashSet<Corner> Corners { get; set; }

        public Center(BGame game, float x , float y, float z)
        {
            //polyNorm = new Vector3D(0, 0, 0);
            Point = new Vector3(x, y , z);
            Key = Point.GetHashCode();
            Index = Point.GetHashCode();

            Water = Coast = Ocean = Border = false;
            Moisture = 0.0f;

            Neighbours = new HashSet<Center>(new CenterComparer());
            Borders = new HashSet<Edge>(new EdgeComparer());
            Corners = new HashSet<Corner>(new CornerComparer());
            
            _game = game;
        }

        #region Methods
        public bool Equals(Center other)
        {
            return this.Point.X == other.Point.X && this.Point.Y == other.Point.Y && this.Point.Z == other.Point.Z;
        }

        public void SetBiome()
        {
            Biome = BiomeTypes.BiomeSelector(Ocean, Water, Coast, Elevation, Moisture);
        }

        public void GetPolygons()
        {
            Polygon = new Polygon(this.Corners, this);
        }
        #endregion
        
        public void Draw(BGame game, Matrix viewMatrix, Matrix projectionMatrix)
        {
            game.ShaderEffect.TextureEnabled = false;
            //game.ShaderEffect.Texture = _texture;
            game.ShaderEffect.VertexColorEnabled = true;

            foreach (var pass in game.ShaderEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
            }

            Polygon.Draw(game.GraphicsDevice);
        }
    }
}
