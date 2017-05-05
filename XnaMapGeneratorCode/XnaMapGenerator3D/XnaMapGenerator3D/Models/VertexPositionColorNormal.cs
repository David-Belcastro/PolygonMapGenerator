using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XnaMapGenerator3D.Models
{
    public struct VertexPositionNormalColor : IVertexType
    {
        public Color Color;
        public Vector3 Normal;
        public Vector3 Position;

        public static readonly VertexElement[] VertexElements = { 
                new VertexElement(0, VertexElementFormat.Color, VertexElementUsage.Color, 0), 
                new VertexElement(4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0), 
                new VertexElement(16, VertexElementFormat.Vector3, VertexElementUsage.Position, 0) 
            };

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return new VertexDeclaration(VertexElements); }
        }

        public VertexPositionNormalColor(Vector3 position, Vector3 normal, Color color)
        {
            this.Position = position;
            this.Normal = normal;
            this.Color = color;
        }
    }
}
