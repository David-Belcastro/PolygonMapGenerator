using System;
using System.Drawing;
using System.Collections.Generic;
using WpfApplication1.Models;
using SlimDX;

namespace TerrainGenerator.Models
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
        

        public HashSet<Center> Neighbours { get; set; }
        public HashSet<Edge> Borders { get; set; }
        public HashSet<Corner> Corners { get; set; }

        public Center(float x , float y, float z)
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
        
        public void Draw(Graphics finalimage, int basesize, int mapsize, Color cornercolor)
        {
            //Polygon.Draw(finalimage, basesize, mapsize);
            float imageratio = mapsize / basesize;
            var pen = new SolidBrush(Color.Red);
            var randomizer = new Random();
            var cornerpen = new SolidBrush(Color.Blue);
            var points = new PointF[1];
            finalimage.FillRectangle(pen, Point.X * imageratio, Point.Z * imageratio, 2, 2);
            foreach (Corner crn in Corners)
            {
                finalimage.FillRectangle(cornerpen, crn.Point.X * imageratio, crn.Point.Z * imageratio, 2, 2);
            }

        }

        public float Minmax(float ptvalue, int max)
        {
            if (ptvalue < 0)
            {
                return 0;
            }
            else if (ptvalue > max)
            {
                return max;
            }
            else
            {
                return ptvalue;
            }

        }

    }
}
