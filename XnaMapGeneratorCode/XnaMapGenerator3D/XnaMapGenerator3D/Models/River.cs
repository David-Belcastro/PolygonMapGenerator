using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaMapGenerator3D.Models
{
    public class River
    {
        public List<Corner> Corners { get; set; }
        public Corner Source { get { return Corners[0]; } }

        public River(Corner c)
        {
            Corners = new List<Corner>();
            Corners.Add(c);
        }

        public void Add(Corner c)
        {
            Corners.Add(c);
        }
    }
}
