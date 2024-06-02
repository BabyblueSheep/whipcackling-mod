using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics;
using Terraria.ModLoader;
using Whipcackling.Assets;
using Whipcackling.Common.Systems.Drawing;
using Whipcackling.Common.Utilities;
using Whipcackling.Content.Particles;
using Whipcackling.Core.Particles;

namespace Whipcackling.Content.Whips.MeldWhip
{
    public class MeldExplosion : ModProjectile, IDrawPixelated
    {
        public override string LocalizationCategory => "Whips.MeldWhip";

        public ref float Strength => ref Projectile.ai[0];
        public bool BlackHole
        {
            get => Projectile.ai[1] > 0;
            set => Projectile.ai[1] = value ? 1 : 0;
        }
        public int Size => 50 + 100 * (int)Strength;

        public float Timer => TotalTime - Projectile.timeLeft;
        public float TimeCompletion => Timer / TotalTime;
        public ref float TotalTime => ref Projectile.localAI[0];

        public bool Initialized
        {
            get => Projectile.localAI[1] > 0;
            set => Projectile.localAI[1] = value ? 1 : 0;
        }

        public static SoundStyle SmallExplosion = new($"{AssetDirectory.AssetPath}Sounds/Whips/MeldWhip/SmallExplosion", 3)
        {
            PitchVariance = 0.5f,
            Pitch = 0.2f,
            Volume = 0.4f,
            MaxInstances = 0,
        };

        public static SoundStyle BigExplosion = new($"{AssetDirectory.AssetPath}Sounds/Whips/MeldWhip/BigExplosion")
        {
            PitchVariance = 0.5f,
            Pitch = 0.3f,
            Volume = 1f,
            MaxInstances = 0,
        };

        public static SoundStyle Droning = new($"{AssetDirectory.AssetPath}Sounds/Whips/MeldWhip/Droning")
        {
            IsLooped = true,
            Volume = 1.3f,
            MaxInstances = 0,
        };

        private SlotId _droningSlot;

        private Vector2 _offset;
        VertexStrip _strip;
        private FastNoiseLite _noise;

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20;
            Projectile.hide = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
        }

        public override void AI()
        {
            if (!Initialized)
            {
                _noise = new(Main.rand.Next(99999));
                _noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
                _offset = new Vector2(Main.rand.NextFloat(0, 1), Main.rand.NextFloat(0, 1));

                if (Strength < 3)
                    SoundEngine.PlaySound(SmallExplosion, Projectile.Center);
                else
                    SoundEngine.PlaySound(BigExplosion, Projectile.Center);

                #region Glow lines
                for (int i = 0; i < 5 + 2 * Strength; i++) // Glow line
                {
                    float scale = Main.rand.NextFloat(0.35f, 1f);
                    ParticleSystem.SpawnParticle(
                    type: ParticleLoader.ParticleType<MeldGlowLine>(),
                    position: Projectile.Center,
                    velocity: Utils.RandomVector2(Main.rand, 3f + 5f * Strength, 4f + 5f * Strength).RotatedByRandom(MathHelper.TwoPi),
                    scale: new(scale, scale * 1.5f + 2f),
                    rotation: 0,
                    color: Color.White,
                    variant: 0,
                    lifetime: 10 + Main.rand.Next(5) + 2 * (int)Strength,
                    custom1: Main.rand.NextFloat(0.3f, 0.5f) // Decay
                    );
                }
                #endregion

                #region Glow Particles
                for (int i = 0; i < 10 * Strength; i++) // Normal glow dot
                {
                    ParticleSystem.SpawnParticle(
                    type: ParticleLoader.ParticleType<MeldGlowDot>(),
                    position: Projectile.Center,
                    velocity: Utils.RandomVector2(Main.rand, 2f + 1.5f * Strength, 4f + 2.5f * Strength).RotatedByRandom(MathHelper.TwoPi),
                    scale: new(Main.rand.NextFloat(0.8f, 1f) + 0.15f * Strength),
                    rotation: 0,
                    color: Main.rand.NextFromList(Color.PaleGreen, Color.SkyBlue, Color.Aquamarine),
                    variant: 0,
                    lifetime: 20 + Main.rand.Next(10) + 10 * (int)Strength,
                    custom1: Main.rand.NextFloat(0.3f, 0.8f) // Decay
                    );
                }

                for (int i = 0; i < 5 * Strength; i++) // Big glow dot
                {
                    ParticleSystem.SpawnParticle(
                    type: ParticleLoader.ParticleType<MeldGlowDot>(),
                    position: Projectile.Center,
                    velocity: Utils.RandomVector2(Main.rand, 1f + Strength, 3f + 1.5f * Strength).RotatedByRandom(MathHelper.TwoPi),
                    scale: new(Main.rand.NextFloat(0.85f, 1.1f) * 1.35f + 0.2f * Strength),
                    rotation: 0,
                    color: Main.rand.NextFromList(Color.PaleGreen, Color.SkyBlue, Color.Aquamarine),
                    variant: 0,
                    lifetime: 25 + Main.rand.Next(5, 10) + 10 * (int)Strength,
                    custom1: Main.rand.NextFloat(0.3f, 0.9f) // Decay
                    );
                }

                for (int i = 0; i < 5 * Strength; i++) // Small glow dot
                {
                    ParticleSystem.SpawnParticle(
                    type: ParticleLoader.ParticleType<MeldGlowDot>(),
                    position: Projectile.Center,
                    velocity: Utils.RandomVector2(Main.rand, 3f + 2f * Strength, 4.5f + 3f * Strength).RotatedByRandom(MathHelper.TwoPi),
                    scale: new(Main.rand.NextFloat(0.65f, 0.9f) + 0.1f * Strength),
                    rotation: 0,
                    color: Main.rand.NextFromList(Color.PaleGreen, Color.SkyBlue, Color.Aquamarine),
                    variant: 0,
                    lifetime: 15 + Main.rand.Next(5) + 10 * (int)Strength,
                    custom1: Main.rand.NextFloat(0.2f, 0.85f) // Decay
                    );
                }
                #endregion

                #region Smoke Particles
                for (int i = 0; i < 5 + 10 * Strength; i++) // Standard smoke
                {
                    ParticleSystem.SpawnParticle(
                    type: ParticleLoader.ParticleType<MeldSmoke>(),
                    position: Projectile.Center,
                    velocity: Utils.RandomVector2(Main.rand, Strength, 4f * Strength).RotatedByRandom(MathHelper.TwoPi),
                    scale: new(Main.rand.NextFloat(0.75f, 1f) + 0.3f * Strength),
                    rotation: Main.rand.NextFloat(MathHelper.TwoPi),
                    color: new(Color.SpringGreen.R, Color.SpringGreen.G, Color.SpringGreen.B, 240),
                    variant: Main.rand.Next(3),
                    lifetime: Main.rand.Next(30, 60) + 10 * (int)Strength,
                    custom1: Main.rand.NextFloat(-0.1f, 0.2f) // Decay
                    );
                }

                for (int i = 0; i < 10 + 5 * Strength; i++) // Slower smoke
                {
                    ParticleSystem.SpawnParticle(
                    type: ParticleLoader.ParticleType<MeldSmoke>(),
                    position: Projectile.Center,
                    velocity: new Vector2(Main.rand.NextFloat(0.5f * Strength, 3f * Strength), Main.rand.NextFloat(0, Strength)).RotatedByRandom(MathHelper.TwoPi),
                    scale: new(Main.rand.NextFloat(0.5f, 1.2f) + 0.3f * Strength),
                    rotation: Main.rand.NextFloat(MathHelper.TwoPi),
                    color: new(Color.SpringGreen.R, Color.SpringGreen.G, Color.SpringGreen.B, 200),
                    variant: Main.rand.Next(3),
                    lifetime: Main.rand.Next(40, 60) + 5 * (int)Strength,
                    custom1: Main.rand.NextFloat(0f, 0.3f) // Decay
                    );
                }


                #endregion

                if (BlackHole)
                {
                    Projectile.timeLeft = 360;
                    if (!SoundEngine.TryGetActiveSound(_droningSlot, out var idleSoundOut) || !idleSoundOut.IsPlaying)
                    {
                        _droningSlot = SoundEngine.PlaySound(Droning, Projectile.Center);
                    }
                }
                TotalTime = Projectile.timeLeft;

                Initialized = true;
            }
            _noise.SetFrequency(MathHelper.Lerp(0.6f, 0.4f, Easings.OutQuart(TimeCompletion)));

            if (BlackHole && Projectile.timeLeft > 20)
            {
                if (Main.rand.Next(5) % 1 == 0)
                {
                    int tileCenterX = (int)(Projectile.Center.X / 16f);
                    int tileCenterY = (int)(Projectile.Center.Y / 16f);
                    int tileRandomX = Main.rand.Next(tileCenterX - Size / 8, tileCenterX + Size / 8);
                    int tileRandomY = Main.rand.Next(tileCenterY - Size / 8, tileCenterY + Size / 8);

                    Tile tile = Framing.GetTileSafely(tileRandomX, tileRandomY);
                    Vector2 randomPos = new(Main.rand.NextFloat(Projectile.Center.X - Size, Projectile.Center.X + Size), Main.rand.NextFloat(Projectile.Center.Y - Size, Projectile.Center.Y + Size));
                    if (!tile.HasTile)
                    {
                        ParticleSystem.SpawnParticle(
                            type: ParticleLoader.ParticleType<BlackHoleStar>(),
                            position: randomPos,
                            scale: new(Main.rand.NextFloat(1.5f, 3f)),
                            rotation: Main.rand.NextFloat(MathHelper.TwoPi),
                            lifetime: Main.rand.Next(20, 35),
                            custom1: Projectile.Center.X, // Origin X
                            custom2: Projectile.Center.Y, // Origin Y
                            custom3: Main.rand.NextFloat(-0.1f, 0.1f) // Angle rotation
                            );
                    }
                    else
                    {
                        Dust dust = Main.dust[WorldGen.KillTile_MakeTileDust(tileRandomX, tileRandomY, tile)];
                        dust.scale = Main.rand.NextFloat(2, 3);
                        dust.position = randomPos;
                        dust.velocity = (Projectile.Center - randomPos);
                        dust.velocity *= Main.rand.NextFloat(0.001f, 0.05f);
                        dust.velocity *= 2.5f;
                        dust.noGravity = true;
                        dust.fadeIn = 0.1f;
                    }
                }

            }

            if (Projectile.timeLeft < 20)
            {
                if (SoundEngine.TryGetActiveSound(_droningSlot, out var soundOut))
                {
                    soundOut.Volume *= 0.9f;
                }
                Projectile.damage = 0;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Point projCenter = projHitbox.Center;
            Point targetCenter = targetHitbox.Center;
            return (projCenter - targetCenter).ToVector2().Length() < Size;
        }

        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(_droningSlot, out var soundOut))
            {
                soundOut.Stop();
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            #region Black Hole
            if (BlackHole)
            {
                Effect blackHoleEffect = AssetDirectory.Effects.BlackHole.Value;
                blackHoleEffect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * (1 + Projectile.whoAmI * 0.01f) + Projectile.whoAmI);
                blackHoleEffect.Parameters["uResolution"].SetValue(new Vector2(Size * 0.5f));
                blackHoleEffect.Parameters["uRadius"].SetValue(
                    MathHelper.Lerp(-2, 0.1f, 
                    Timer < 25 ?
                    Easings.OutCirc(Utils.GetLerpValue(5, 25, Timer, true)) :
                    Easings.InCirc(Utils.GetLerpValue(TotalTime, TotalTime - 20, Timer, true))
                    ));
                blackHoleEffect.Parameters["uHoleRadius"].SetValue(
                    MathHelper.Lerp(0, 0.1f,
                    Timer < 20 ?
                    Easings.OutSine(Utils.GetLerpValue(0, 20, Timer, true)) :
                    Easings.InSine(Utils.GetLerpValue(TotalTime - 5, TotalTime - 25, Timer, true))
                    ));

                blackHoleEffect.Parameters["uColorOuter"].SetValue(new Vector4(0.5f, 0.9f, 0.7f, 1));
                blackHoleEffect.Parameters["uColorInner"].SetValue(new Vector4(0.3f, 0.5f, 0.5f, 1));

                blackHoleEffect.Parameters["uTextureNoise0"].SetValue(AssetDirectory.Textures.Extra.Noise.GassyNoise.Value);
                blackHoleEffect.Parameters["uTextureNoise1"].SetValue(AssetDirectory.Textures.Extra.Noise.BlurredDarkPerlinNoise.Value);
                blackHoleEffect.Parameters["uTextureNoise2"].SetValue(AssetDirectory.Textures.Extra.Noise.WobblyNoise.Value);
                blackHoleEffect.Parameters["uTexturePalette0"].SetValue(AssetDirectory.Textures.Extra.Palettes.OuterBlackHolePaletteValue.Value);
                blackHoleEffect.Parameters["uTexturePalette1"].SetValue(AssetDirectory.Textures.Extra.Palettes.InnerBlackHolePaletteValue.Value);
                blackHoleEffect.Parameters["uTexturePalette2"].SetValue(AssetDirectory.Textures.Extra.Palettes.AuraBlackHolePaletteHue.Value);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, blackHoleEffect, Main.GameViewMatrix.TransformationMatrix);

                Texture2D empty = AssetDirectory.Textures.Pixel.Value;
                Main.spriteBatch.Draw(empty, Projectile.Center - Main.screenPosition, empty.Frame(), Color.White, 0, empty.Size() * 0.5f, Size, SpriteEffects.None, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            #endregion

            return false;
        }

        public void DrawPixelated()
        {
            if (!Initialized)
                return;
            #region Shockwave
            _strip ??= new();

            Vector2[] positions = new Vector2[129];
            float[] rotations = new float[129];

            for (int i = 0; i < 129; i++)
            {
                double rad = i / 128f * Math.PI * 2;
                Vector2 offset = new((float)Math.Sin(rad), (float)Math.Cos(rad));
                float radOffset = 1 + _noise.GetNoise(offset.X, offset.Y) * 0.1f;

                positions[i] = Projectile.Center + offset * Size * 0.5f * radOffset * Easings.OutQuart(Utils.GetLerpValue(0, 20, Timer, true));
                if (i > 0)
                    rotations[i] = (positions[i] - positions[i - 1]).ToRotation();
            }
            rotations[0] = rotations[128];

            Color ShockwaveColor(float p) => Color.White;
            float ShockwaveWidth(float p) => (8 + (float)Math.Pow(1.5f, Strength)) * Utils.GetLerpValue(20, 5, Timer, true) * Utils.GetLerpValue(0, 5, Timer, true);

            _strip.PrepareStrip(positions, rotations, ShockwaveColor, ShockwaveWidth, -Main.screenPosition, 129, true);

            Effect effect = AssetDirectory.Effects.ShockwaveTrail.Value;
            effect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            effect.Parameters["uOffset"].SetValue(_offset);

            effect.Parameters["uTextureNoise0"].SetValue(AssetDirectory.Textures.Extra.Noise.CirclyNoise.Value);
            effect.Parameters["uTexturePalette0"].SetValue(AssetDirectory.Textures.Extra.Palettes.ShockwavePalette.Value);
            effect.Parameters["uTexturePalette1"].SetValue(AssetDirectory.Textures.Extra.Palettes.ShockwavePaletteAlpha.Value);


            effect.CurrentTechnique.Passes[0].Apply();
            _strip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            #endregion
        }
    }
}
