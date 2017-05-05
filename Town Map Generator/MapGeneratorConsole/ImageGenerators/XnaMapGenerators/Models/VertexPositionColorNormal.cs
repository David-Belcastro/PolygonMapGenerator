using System.Drawing;
using SlimDX;

namespace TerrainGenerator.Models
{
    public struct VertexPositionNormalColor
    {
        public Color Color;
        public Vector3 Normal;
        public Vector3 Position;

        public VertexPositionNormalColor(Vector3 position, Vector3 normal, Color color)
        {
            this.Position = position;
            this.Normal = normal;
            this.Color = color;
        }
    }
}
