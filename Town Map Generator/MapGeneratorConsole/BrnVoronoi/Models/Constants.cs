using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrnVoronoi.Models
{
    public static class Constants
    {
        public static readonly Vector VVInfinite = new Vector(double.PositiveInfinity, double.PositiveInfinity);
        public static readonly Vector VVUnkown = new Vector(double.NaN, double.NaN);
    }
}
