using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace Whipcackling.Core
{
    public class VertexPlane
    {
        public delegate Color StripColorFunction(float progressX, float progressY);

        private struct CustomVertexInfo : IVertexType
        {
            public Vector2 Position;

            public Color Color;

            public Vector2 TexCoord;

            private static readonly VertexDeclaration _vertexDeclaration = new((VertexElement[])(object)new VertexElement[3]
            {
                new(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                new(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
            });

            public readonly VertexDeclaration VertexDeclaration => _vertexDeclaration;

            public CustomVertexInfo(Vector2 position, Color color, Vector2 texCoord)
            {
                this.Position = position;
                this.Color = color;
                this.TexCoord = texCoord;
            }
        }

        private DynamicVertexBuffer _vertexBuffer;
        private DynamicIndexBuffer _indexBuffer;
        private RasterizerState _rasterizerState;

        private Vector2 _vertexAmount;
        private List<CustomVertexInfo> _verticesList;
        private List<short> _indicesList;

        public VertexPlane()
        {
            _verticesList = new List<CustomVertexInfo>();
            _indicesList = new List<short>();

            // 5000 and 30000 are arbitrary numbers, roughly picked out based on an overesimation of what I'll realistically need
            _vertexBuffer = new(Main.instance.GraphicsDevice, typeof(CustomVertexInfo), 50000, BufferUsage.WriteOnly);
            _indexBuffer = new(Main.instance.GraphicsDevice, typeof(short), 300000, BufferUsage.WriteOnly);
            _rasterizerState = new();
            _rasterizerState.CullMode = CullMode.None;
        }

        public void PreparePlane(Vector2[,] positions, StripColorFunction colorFunction, Vector2 offsetForAllPositions = default)
        {
            float width = positions.GetLength(1);
            float height = positions.GetLength(0);
            _vertexAmount = new(width, height);
            if (width == 1 || height == 1)
                return;

            _verticesList.Clear();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    _verticesList.Add(new CustomVertexInfo()
                    {
                        Position = positions[y, x] + offsetForAllPositions,
                        Color = colorFunction(x / (width - 1), y / (height - 1)),
                        TexCoord = new Vector2(x / (width - 1), y / (height - 1))
                    });
                }
            }
            _vertexBuffer.SetData(_verticesList.ToArray());

            short[] _indices = new short[(int)((width - 1) * (height - 1) * 6)];
            int counter = 0;
            _indicesList.Clear();
            for (int x = 0; x < width - 1; x++)
            {
                for (int y = 0; y < height - 1; y++)
                {
                    _indicesList.Add((short)(x + y * width));
                    _indicesList.Add((short)(x + 1 + y * width));
                    _indicesList.Add((short)(x + (y + 1) * width));
                    _indicesList.Add((short)(x + 1 + y * width));
                    _indicesList.Add((short)(x + (y + 1) * width));
                    _indicesList.Add((short)(x + 1 + (y + 1) * width));
                    counter += 6;
                }
            }
            _indexBuffer.SetData(_indicesList.ToArray());
        }

        public void DrawMesh()
        {
            if (_vertexAmount.X == 1 || _vertexAmount.Y == 1)
                return;
            Main.instance.GraphicsDevice.SetVertexBuffer(_vertexBuffer);
            Main.instance.GraphicsDevice.Indices = _indexBuffer;
            Main.instance.GraphicsDevice.RasterizerState = _rasterizerState;
            Main.instance.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _verticesList.Count, 0, _indicesList.Count / 3);
        }
    }
}
