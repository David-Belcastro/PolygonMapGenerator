using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using Microsoft.Xna.Framework;
using WpfApplication1.Models;
using XnaMapGenerator3D.Services;
using Color = Microsoft.Xna.Framework.Color;


namespace XnaMapGenerator3D.Models
{
    public class Corner : IEquatable<Corner>, IMapItem, IEqualityComparer<Corner>
    {
        public Corner(float ax, float ay, float az)
        {
            Point = new Vector3(ax,ay,az);
            Key = Point.GetHashCode();
            Index = Point.GetHashCode();
            Ocean = Water = Coast = Border = false;
            Moisture = 0.0d;
            //Elevation = 100.0d;

            Touches = new HashSet<Center>(new CenterComparer());
            Protrudes = new HashSet<Edge>(new EdgeComparer());
            Adjacents = new HashSet<Corner>(new CornerComparer());

            River = WatershedSize = 0;
        }

        public int Index { get; set; }

        public int Key { get; set; }
        public Vector3 Point { get; set; }  // location
        public Boolean Ocean { get; set; }  // ocean
        public Boolean Water { get; set; }  // lake or ocean
        public Boolean Land { get { return !Water; } set { Water = !value; } }  // lake or ocean
        public Boolean Coast { get; set; }  // touches ocean and land polygons
        public Boolean Border { get; set; }  // at the edge of the map
        public float Elevation 
        {
            get { return this.Point.Y; }
            set { this.Point = new Vector3(Point.X, (float)value, Point.Z); }
        }
        public double Moisture { get; set; }  // 0.0-1.0
        public Vector3 Normal { get; set; }

        public HashSet<Center> Touches { get; set; }
        public HashSet<Edge> Protrudes { get; set; }
        public HashSet<Corner> Adjacents { get; set; }

        public int River { get; set; }  // 0 if no river, or volume of water in river
        public Corner Downslope { get; set; }  // pointer to adjacent corner most downhill
        public Corner Watershed { get; set; }  // pointer to coastal corner, or null
        public int WatershedSize { get; set; }
        
        public bool Equals(Corner other)
        {
            return this.Point.Equals(other.Point);
        }

        public void AddProtrudes(Edge edge)
        {
            Protrudes.Add(edge);
        }

        public void AddAdjacent(Corner corner)
        {
            Adjacents.Add(corner);
        }

        public void AddTouches(Center center)
        {
            Touches.Add(center);
        }

        public Biome Biome { get; set; }
        
        public Color GetColor(string hex)
        {
            System.Drawing.Color cv = ColorTranslator.FromHtml("#"+hex);

            Color ret = new Color(cv.R, cv.G, cv.B, 255);

            return ret;
        }

        public void SetBiome()
        {
            Biome = BiomeTypes.BiomeSelector(Ocean, Water, Coast, Elevation, Moisture);
        }

        #region Implementation of IEqualityComparer<in Corner>

        public bool Equals(Corner x, Corner y)
        {
            return x.Point.Equals(y.Point);
        }

        public int GetHashCode(Corner obj)
        {
            return obj.Point.GetHashCode(); 
        }

        #endregion
    }
}
