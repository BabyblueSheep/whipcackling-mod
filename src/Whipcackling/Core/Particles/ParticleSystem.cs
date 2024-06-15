using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Peripherals.RGB;
using ReLogic.Threading;
using Schedulers;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Whipcackling.Assets;
using Whipcackling.Common.Utilities;
using Whipcackling.Core.Particles.Components;
using static Whipcackling.Assets.AssetDirectory;

namespace Whipcackling.Core.Particles
{
    public partial class ParticleSystem : ModSystem
    {
        private static DynamicVertexBuffer _vertexBuffer;
        private static DynamicIndexBuffer _indexBuffer;

        private static JobScheduler _jobScheduler;

        public static World World { get; set; }

        public static readonly QueryDescription DrawableParticle = new QueryDescription().WithAll<Position, Scale, Rotation, Color, UVCoordinates>();

        public const int MAX_PARTICLES = 1000000;

        public override void Load()
        {
            World = World.Create();
            _jobScheduler = new(new JobScheduler.Config()
            {
                ThreadPrefixName = "Whipcackling",
                ThreadCount = 0,
                MaxExpectedConcurrentJobs = 64,
                StrictAllocationMode = false,
            });
            World.SharedJobScheduler = _jobScheduler;

            Main.QueueMainThreadAction(() =>
            {
                _vertexBuffer = new(Main.graphics.GraphicsDevice, typeof(ParticleVertex), 4 * MAX_PARTICLES, BufferUsage.WriteOnly);
                _indexBuffer = new(Main.graphics.GraphicsDevice, typeof(int), 6 * MAX_PARTICLES, BufferUsage.WriteOnly);
            });

            On_Main.DrawDust += DrawParticles;
        }

        public override void Unload()
        {
            World.Destroy(World);
            _jobScheduler.Dispose();

            On_Main.DrawDust -= DrawParticles;
        }

        public override void PostUpdateDusts()
        {
            World.InlineQuery<UpdateLinearVelocity, Position, LinearVelocity>(UpdateLinearVelocity.Query);
        }

        private void DrawParticles(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);

            Main.graphics.GraphicsDevice.SetVertexBuffer(_vertexBuffer);
            Main.graphics.GraphicsDevice.Indices = _indexBuffer;

            RasterizerState rasterizerState = new()
            {
                CullMode = CullMode.None
            };
            Main.graphics.GraphicsDevice.RasterizerState = rasterizerState;
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            Effect particle = AssetDirectory.Effects.Particle.Value;
            particle.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            particle.Parameters["uTextureAtlas"].SetValue(ParticleAtlasSystem.Atlas);
            particle.CurrentTechnique.Passes[0].Apply();

            Query query = World.Query(in DrawableParticle);

            int amount = World.CountEntities(in DrawableParticle);
            ParticleVertex[] vertices = new ParticleVertex[4 * amount];
            int[] indices = new int[6 * amount];

            int index = 0;
            foreach (Chunk chunk in query)
            {
                chunk.GetSpan<Position, Scale, Rotation, Color, UVCoordinates>(out var positions, out var scales, out var rotations, out var colors, out var uvCoords);

                foreach (int i in chunk)
                {
                    Vector2 actualPosition = (Vector2)positions[i] - Main.screenPosition;
                    Vector2 centerPos = actualPosition + uvCoords[i].Size * 0.5f;
                    Color color = colors[i];

                    vertices[index * 4] = new ParticleVertex(
                        HelperMethods.LerpRectangular(centerPos, actualPosition, (Vector2)scales[i]).RotatedBy((float)rotations[i], centerPos),
                        color,
                        uvCoords[i].Position / 512f);
                    vertices[1 + index * 4] = new ParticleVertex(
                        HelperMethods.LerpRectangular(centerPos, actualPosition + new Vector2(0, uvCoords[i].Height), (Vector2)scales[i]).RotatedBy((float)rotations[i], centerPos),
                        color,
                        (uvCoords[i].Position + new Vector2(0, uvCoords[i].Height)) / 512f);
                    vertices[2 + index * 4] = new ParticleVertex(
                        HelperMethods.LerpRectangular(centerPos, actualPosition + new Vector2(uvCoords[i].Width, uvCoords[i].Height), (Vector2)scales[i]).RotatedBy((float)rotations[i], centerPos),
                        color,
                        (uvCoords[i].Position + new Vector2(uvCoords[i].Width, uvCoords[i].Height)) / 512f);
                    vertices[3 + index * 4] = new ParticleVertex(
                        HelperMethods.LerpRectangular(centerPos, actualPosition + new Vector2(uvCoords[i].Width, 0), (Vector2)scales[i]).RotatedBy((float)rotations[i], centerPos),
                        color,
                        (uvCoords[i].Position + new Vector2(uvCoords[i].Width, 0)) / 512f);

                    indices[index * 6] = (short)(index * 4); indices[1 + index * 6] = (short)(index * 4 + 1); indices[2 + index * 6] = (short)(index * 4 + 2);
                    indices[3 + index * 6] = (short)(index * 4); indices[4 + index * 6] = (short)(index * 4 + 2); indices[5 + index * 6] = (short)(index * 4 + 3);
                    index++;
                }
            }

            _vertexBuffer.SetData(vertices, SetDataOptions.None);
            _indexBuffer.SetData(indices);

            Main.graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4 * amount, 0, 2 * amount);
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

        private struct ParticleVertex : IVertexType
        {
            public Vector2 Position;
            public Color Color;
            public Vector2 TextureCoordinate;

            private static readonly VertexDeclaration _vertexDeclaration = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),                  // Position
                new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),                       // Color
                new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)         // UVInfo
            );

            readonly VertexDeclaration IVertexType.VertexDeclaration => _vertexDeclaration;

            public ParticleVertex(Vector2 position, Color color, Vector2 texturePos)
            {
                Position = position;
                Color = color;
                TextureCoordinate = texturePos;
            }
        }
    }
}
