using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaMapGenerator3D.Helpers
{
    public class BMathHelper
    {
        public static double GetPointLength(Vector3 Vector3)
        {
            return Math.Sqrt((Math.Pow(Vector3.X, 2)) + (Math.Pow(Vector3.Z, 2)));
        }

        public static float Area(Vector3 A, Vector3 B, Vector3 C)
        {
            return ((A.X - C.X) * (B.Z - C.Z) - (A.Z - C.Z) * (B.X - C.X)) / 2;
        }

        public static Vector3 CalculateNormal(Vector3 newMain, Vector3 p1, Vector3 p2)
        {
            var v1 = new Vector3(newMain.X - p1.X, newMain.Y - p1.Y, newMain.Z - p1.Z);
            var v2 = new Vector3(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
            return Vector3.Cross(v1, v2);
        }
    }
}
