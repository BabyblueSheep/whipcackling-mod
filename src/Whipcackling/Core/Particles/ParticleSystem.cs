using CalamityMod.Items.Weapons.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Whipcackling.Assets;
using Whipcackling.Common.Utilities;
using Whipcackling.Core.Particles.Enums;

namespace Whipcackling.Core.Particles
{
    public class ParticleSystem : ModSystem
    {
        const int PARTICLE_LIMIT = 100000;

        public static SpriteViewMatrix UIViewMatrix;

        private static DynamicVertexBuffer _vertexBuffer;
        private static DynamicIndexBuffer _indexBuffer;

        private static Dictionary<int, List<Particle>>[] _particles;

        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            _particles = new Dictionary<int, List<Particle>>[Enum.GetValues(typeof(ParticleDrawLayer)).Cast<ParticleDrawLayer>().Distinct().Count()];
            for (int i = 0; i < _particles.Length; i++)
                _particles[i] = new Dictionary<int, List<Particle>>();

            Main.QueueMainThreadAction(() => UIViewMatrix = new SpriteViewMatrix(Main.graphics.GraphicsDevice));

            On_Dust.UpdateDust += UpdatePaticles;

            On_Main.DrawSurfaceBG += DrawParticlesAfterBG;
            On_Main.DrawBackgroundBlackFill += DrawParticlesAfterWalls;
            On_Main.DrawDust += DrawParticlesAfterNPCsProjectiles;
        }

        public override void Unload()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            On_Dust.UpdateDust -= UpdatePaticles;

            On_Main.DrawSurfaceBG -= DrawParticlesAfterBG;
            On_Main.DrawBackgroundBlackFill -= DrawParticlesAfterWalls;
            On_Main.DrawDust -= DrawParticlesAfterNPCsProjectiles;
        }

        public override void PostSetupContent()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            Main.QueueMainThreadAction(() => _vertexBuffer = new DynamicVertexBuffer(Main.graphics.GraphicsDevice, typeof(ParticleVertex), 4 * PARTICLE_LIMIT, BufferUsage.WriteOnly));
            Main.QueueMainThreadAction(() => _indexBuffer = new DynamicIndexBuffer(Main.graphics.GraphicsDevice, typeof(short), 6 * PARTICLE_LIMIT, BufferUsage.WriteOnly));

            List<ModParticle> particleTypes = ParticleLoader.particles;
            for (int i = 0; i < ParticleLoader.Count; i++)
            {
                Mod.Logger.Info(_particles[(int)particleTypes[i].DrawLayer]);
                _particles[(int)particleTypes[i].DrawLayer].Add(i, new List<Particle>());
            }
        }

        public override void OnWorldUnload()
        {
            ClearParticles();
        }

        /// <summary>
        /// Attempts to spawn a single particle into the game world.
        /// </summary>
        /// <param name="type">The particle type.</param>
        /// <param name="position">The position of the particle.</param>
        /// <param name="velocity">The velocity of the particle. Defaults to <see cref="Vector2.Zero"/>.</param>
        /// <param name="scale">The position of the particle. Defaults to <see cref="Vector2.One"/>.</param>
        /// <param name="rotation">The rotation of the particle. Defaults to 0.</param>
        /// <param name="color">The color of the particle. Defaults to <see cref="Color.White"/>.</param>
        /// <param name="variant">The frame of the particle. Defaults to 0.</param>
        /// <param name="lifetime">The total amount of frames the particle will exist for. Defaults to 60.</param>
        /// <param name="custom1">A custom float for any purpose. Defaults to 0.</param>
        /// <param name="custom2">A custom float for any purpose. Defaults to 0.</param>
        /// <param name="custom3">A custom float for any purpose. Defaults to 0.</param>
        public static void SpawnParticle(int type, Vector2 position, Vector2? velocity = null, Vector2? scale = null, float rotation = 0f, Color? color = null, int variant = 0, int lifetime = 60, float custom1 = 0, float custom2 = 0, float custom3 = 0)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            if (Main.gamePaused)
                return;
            if (WorldGen.gen)
                return;

            ModParticle particleType = ParticleLoader.particles[type];

            List<Particle> particles = _particles[(int)particleType.DrawLayer][type];

            Particle particle = new()
            {
                Type = type,
                Velocity = velocity ?? Vector2.Zero,
                Time = 0,
                Lifetime = lifetime,
                Custom = new float[] { custom1, custom2, custom3 },
                Scale = scale ?? Vector2.One,
                Rotation = rotation,
                Position = position,
                Color = color ?? Color.White,
                Variant = variant,
            };

            particleType.OnSpawn(ref particle);

            particles.Add(particle);
        }

        /// <summary>
        /// Clears all particles at a specific layer.
        /// </summary>
        /// <param name="layer">The draw layer.</param>
        public static void ClearParticles(int layer)
        {
            for (int i = 0; i < _particles[layer].Count; i++)
                _particles[layer][i].Clear();
        }

        /// <summary>
        /// Clears all particles.
        /// </summary>
        public static void ClearParticles()
        {
            for (int i = 0; i < _particles.Length; i++)
                for (int j = 0; j < _particles[i].Count; j++)
                    _particles[i][j].Clear();
        }

        // Go over every every particle instance to run its Update method, update its position with velocity, increase time and remove it if it expires. 
        private void UpdatePaticles(On_Dust.orig_UpdateDust orig)
        {
            orig();

            for (int layer = 0; layer < _particles.Length; layer++)
            {
                for (int type = 0; type < _particles[layer].Count; type++)
                {
                    List<Particle> particles = _particles[layer][type];
                    ModParticle particleType = ParticleLoader.GetParticle(type);

                    for (int i = 0; i < _particles[layer][type].Count;  i++)
                    {
                        Particle particle = particles[i];

                        particleType.Update(ref particle);
                        particle.Time++;
                        if (particle.Progress >= 1)
                        {
                            _particles[layer][type].Remove(particles[i]);
                            i--;
                            continue;
                        }
                        particle.Position += particle.Velocity;

                        particles[i] = particle;
                    }
                    _particles[layer][type] = particles;
                }
            }
        }

        private void DrawParticlesAfterBG(On_Main.orig_DrawSurfaceBG orig, Main self)
        {
            UIViewMatrix.Effects = SpriteEffects.None;
            UIViewMatrix.Zoom = new Vector2(Main.UIScale);

            orig(self);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            DrawParticles(ParticleDrawLayer.AfterBackgrounds, background: true);
        }

        private void DrawParticlesAfterWalls(On_Main.orig_DrawBackgroundBlackFill orig, Main self)
        {
            orig(self);

            DrawParticles(ParticleDrawLayer.AfterWalls);
        }

        public override void PostDrawTiles()
        {
            DrawParticles(ParticleDrawLayer.AfterTiles);
        }

        private void DrawParticlesAfterNPCsProjectiles(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);

            DrawParticles(ParticleDrawLayer.AfterNPCsProjectiles);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int hotbarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Hotbar"));
            if (hotbarIndex != -1)
            {
                layers.Insert(hotbarIndex, new LegacyGameInterfaceLayer(
                    "Whipcackling: UI Particles",
                    delegate
                    {
                        DrawParticles(ParticleDrawLayer.AfterUI, ui: true);
                        return true;
                    },
                    InterfaceScaleType.None));
            }
        }

        // Draw every particle in an individual draw layer.
        //
        // background = true -> Not affected by zoom.
        // ui = true -> In-game UI Scale.
        // otherwise -> In-game Zoom.
        //
        // Instead of using Main.spritebatch.Draw, each particle is a quad and every instance of a specific type is drawn with 1 call.
        // This is for performance reasons, but also lets me not end and begin the spritebatch for every single particle (type) just for shaders.
        private void DrawParticles(ParticleDrawLayer layer, bool background = false, bool ui = false)
        {
            Matrix matrix = background ? Main.BackgroundViewMatrix.NormalizedTransformationmatrix : (ui ? UIViewMatrix.NormalizedTransformationmatrix : Main.GameViewMatrix.NormalizedTransformationmatrix);

            Main.graphics.GraphicsDevice.SetVertexBuffer(_vertexBuffer);
            Main.graphics.GraphicsDevice.Indices = _indexBuffer;

            RasterizerState rasterizerState = new();
            rasterizerState.CullMode = CullMode.None;
            Main.graphics.GraphicsDevice.RasterizerState = rasterizerState;

            var particles = _particles[(int)layer];
            for (int type = 0; type < particles.Count; type++)
            {
                int amount = particles[type].Count;

                ModParticle particleType = ParticleLoader.GetParticle(type);
                Texture2D texture = ModContent.Request<Texture2D>(particleType.Texture).Value;
                Effect particle = particleType.Effect;
                particle.Parameters["uTransformMatrix"].SetValue(matrix);

                ParticleVertex[] vertices = new ParticleVertex[4 * amount];
                short[] indices = new short[6 * amount];

                Main.graphics.GraphicsDevice.BlendState = particleType.BlendMode;

                for (int i = 0; i < amount; i++)
                {
                    Particle instance = particles[type][i];
                    Vector2 actualPos = ui ? instance.Position : instance.Position - Main.screenPosition;

                    Rectangle frame = particleType.GetFrame(instance);
                    float frameStartX = frame.X / (float)texture.Width;
                    float frameStartY = frame.Y / (float)texture.Height;
                    float frameEndX = (frame.X + frame.Width) / (float)texture.Width;
                    float frameEndY = (frame.Y + frame.Height) / (float)texture.Height;
                    Vector2 centerPos = actualPos + frame.Size() * 0.5f;

                    float angle = instance.Rotation;
                    int time = instance.Time;
                    float custom0 = instance.Custom[0]; float custom1 = instance.Custom[1]; float custom2 = instance.Custom[2];
                    Color lightColor = ui ? Color.White : Lighting.GetColor((int)(instance.Position.X / 16), (int)(instance.Position.Y / 16));
                    Color color = particleType.GetColor(instance, lightColor);

                    vertices[i * 4] = new ParticleVertex(
                        HelperMethods.LerpRectangular(centerPos, actualPos, instance.Scale).RotatedBy(angle, centerPos),
                        color,
                        new Vector2(frameStartX, frameStartY),
                        instance.Time,
                        new Vector3(custom0, custom1, custom2));
                    vertices[1 + i * 4] = new ParticleVertex(
                        HelperMethods.LerpRectangular(centerPos, actualPos + new Vector2(0, frame.Height), instance.Scale).RotatedBy(angle, centerPos),
                        color,
                        new Vector2(frameStartX, frameEndY),
                        instance.Time,
                        new Vector3(custom0, custom1, custom2));
                    vertices[2 + i * 4] = new ParticleVertex(
                        HelperMethods.LerpRectangular(centerPos, actualPos + new Vector2(frame.Width, frame.Height), instance.Scale).RotatedBy(angle, centerPos),
                        color,
                        new Vector2(frameEndX, frameEndY),
                        instance.Time,
                        new Vector3(custom0, custom1, custom2));
                    vertices[3 + i * 4] = new ParticleVertex(
                        HelperMethods.LerpRectangular(centerPos, actualPos + new Vector2(frame.Width, 0), instance.Scale).RotatedBy(angle, centerPos),
                        color,
                        new Vector2(frameEndX, frameStartY),
                        instance.Time,
                        new Vector3(custom0, custom1, custom2));

                    indices[i * 6] = (short)(i * 4); indices[1 + i * 6] = (short)(i * 4 + 1); indices[2 + i * 6] = (short)(i * 4 + 2);
                    indices[3 + i * 6] = (short)(i * 4); indices[4 + i * 6] = (short)(i * 4 + 2); indices[5 + i * 6] = (short)(i * 4 + 3);
                }

                _vertexBuffer.SetData(vertices, SetDataOptions.None);
                _indexBuffer.SetData(indices);

                particle.Parameters["uTexture"].SetValue(texture);

                particle.CurrentTechnique.Passes[0].Apply();
                Main.graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4 * amount, 0, 2 * amount);
            }

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }

        private struct ParticleVertex : IVertexType
        {
            public Vector2 Position;

            public Color Color;

            public Vector2 TextureCoordinate;

            public Single Time;

            public Vector3 Custom;

            private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),                  // Particle.Position
                new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),                       // Particle.Color
                new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),        // Texture coordinates
                new VertexElement(20, VertexElementFormat.Single, VertexElementUsage.Position, 1),                  // Particle.Time
                new VertexElement(24, VertexElementFormat.Vector3, VertexElementUsage.Position, 2)                  // Particle.Custom
            );

            VertexDeclaration IVertexType.VertexDeclaration => _vertexDeclaration;

            public ParticleVertex(Vector2 position, Color color, Vector2 textureCoordinate, Single time, Vector3 custom)
            {
                Position = position;
                Color = color;
                TextureCoordinate = textureCoordinate;
                Time = time;
                Custom = custom;
            }
        }
    }
}
