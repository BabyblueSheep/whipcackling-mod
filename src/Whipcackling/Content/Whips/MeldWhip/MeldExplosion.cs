﻿using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Whipcackling.Assets;
using Whipcackling.Common.Systems.Drawing;
using Whipcackling.Common.Utilities;
using Whipcackling.Core.Particles;
using Whipcackling.Core.Particles.Components;

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
        public int Size => 32 + 128 * (int)Strength;

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
            Volume = 1f,
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
                    ParticleSystem.World.Create(
                        (UVCoordinates)ParticleAtlasSystem.AtlasDefinitions["GlowLine"],
                        (Position)Projectile.Center,
                        new Scale(scale, scale * 1.5f + 2f),
                        new Rotation(0),
                        new Color(255, 255, 255, 0),
                        new TimeLeft(10 + Main.rand.Next(5) + 2 * (int)Strength),
                        new TimeUntilAction(8 + Main.rand.Next(2) + (int)Strength),
                        new LinearVelocityAcceleration(Utils.RandomVector2(Main.rand, 3f + 5f * Strength, 4f + 5f * Strength).RotatedByRandom(MathHelper.TwoPi), 0, 0, 0.95f, 0.95f),
                        new RotationIsVelocity(),
                        new LinearScaleIncrease(-0.1f, -0.5f),
                        new LinearColorFade(5)
                        );
                }
                #endregion

                #region Glow Particles
                for (int i = 0; i < 10 * Strength; i++) // Normal glow dot
                {
                    ParticleSystem.World.Create(
                        (UVCoordinates)ParticleAtlasSystem.AtlasDefinitions["GlowDot"],
                        (Position)Projectile.Center,
                        (Scale)(new Vector2(Main.rand.NextFloat(0.8f, 1f) + 0.15f * Strength)),
                        new Rotation(0),
                        Main.rand.NextFromList(Color.PaleGreen, Color.SkyBlue, Color.Aquamarine).MultiplyRGBA(new Color(255, 255, 255, 0)),
                        new TimeLeft(20 + Main.rand.Next(10) + 10 * (int)Strength),
                        new TimeUntilAction(8 + Main.rand.Next(5) + 7 * (int)Strength),
                        new LinearVelocityAcceleration(Utils.RandomVector2(Main.rand, 2f + 1.5f * Strength, 4f + 2.5f * Strength).RotatedByRandom(MathHelper.TwoPi), 0, 0, 0.95f, 0.95f),
                        new RotationIsVelocity(),
                        new LinearScaleIncrease(-0.05f, -0.05f),
                        new LinearColorFade(5)
                        );
                }

                for (int i = 0; i < 5 * Strength; i++) // Big glow dot
                {
                    ParticleSystem.World.Create(
                        (UVCoordinates)ParticleAtlasSystem.AtlasDefinitions["GlowDot"],
                        (Position)Projectile.Center,
                        (Scale)(new Vector2(Main.rand.NextFloat(0.85f, 1.1f) * 1.35f + 0.2f * Strength)),
                        new Rotation(0),
                        Main.rand.NextFromList(Color.PaleGreen, Color.SkyBlue, Color.Aquamarine).MultiplyRGBA(new Color(255, 255, 255, 0)),
                        new TimeLeft(25 + Main.rand.Next(5, 10) + 10 * (int)Strength),
                        new TimeUntilAction(12 + Main.rand.Next(5) + 7 * (int)Strength),
                        new LinearVelocityAcceleration(Utils.RandomVector2(Main.rand, 1f + Strength, 3f + 1.5f * Strength).RotatedByRandom(MathHelper.TwoPi), 0, 0, 0.95f, 0.95f),
                        new RotationIsVelocity(),
                        new LinearScaleIncrease(-0.05f, -0.05f),
                        new LinearColorFade(5)
                        );
                }

                for (int i = 0; i < 5 * Strength; i++) // Small glow dot
                {
                    ParticleSystem.World.Create(
                        (UVCoordinates)ParticleAtlasSystem.AtlasDefinitions["GlowDot"],
                        (Position)Projectile.Center,
                        (Scale)(new Vector2(Main.rand.NextFloat(0.65f, 0.9f) + 0.1f * Strength)),
                        new Rotation(0),
                        Main.rand.NextFromList(Color.PaleGreen, Color.SkyBlue, Color.Aquamarine).MultiplyRGBA(new Color(255, 255, 255, 0)),
                        new TimeLeft(15 + Main.rand.Next(5) + 10 * (int)Strength),
                        new TimeUntilAction(7 + Main.rand.Next(5) + 7 * (int)Strength),
                        new LinearVelocityAcceleration(Utils.RandomVector2(Main.rand, 3f + 2f * Strength, 4.5f + 3f * Strength).RotatedByRandom(MathHelper.TwoPi), 0, 0, 0.95f, 0.95f),
                        new RotationIsVelocity(),
                        new LinearScaleIncrease(-0.05f, -0.05f),
                        new LinearColorFade(5)
                        );
                }
                #endregion

                #region Smoke Particles
                for (int i = 0; i < 5 + 10 * Strength; i++) // Standard smoke
                {
                    ParticleSystem.World.Create(
                        (UVCoordinates)ParticleAtlasSystem.AtlasDefinitions[$"Smoke{Main.rand.Next(1, 4)}"],
                        (Position)Projectile.Center,
                        (Scale)(new Vector2(Main.rand.NextFloat(0.75f, 1f) + 0.3f * Strength)),
                        new Rotation(Main.rand.NextFloat(MathHelper.TwoPi)),
                        Color.SpringGreen,
                        new TimeLeft(Main.rand.Next(30, 60) + 10 * (int)Strength),
                        new TimeUntilAction(Main.rand.Next(30, 40) + 10 * (int)Strength),
                        new LinearVelocityAcceleration(Utils.RandomVector2(Main.rand, Strength, 4f * Strength).RotatedByRandom(MathHelper.TwoPi), 0, -0.05f, 0.95f, 0.95f),
                        new RotateWithLinearVelocity(0.01f, 0),
                        new LinearScaleIncrease(0.02f, 0.02f),
                        new ShiftColorThree(Color.SpringGreen, new Color(30, 45, 72, 200), new Color(0, 0, 0, 0))
                        );
                }

                for (int i = 0; i < 10 + 5 * Strength; i++) // Slower smoke
                {
                    ParticleSystem.World.Create(
                        (UVCoordinates)ParticleAtlasSystem.AtlasDefinitions[$"Smoke{Main.rand.Next(1, 4)}"],
                        (Position)Projectile.Center,
                        (Scale)(new Vector2(Main.rand.NextFloat(0.5f, 1.2f) + 0.3f * Strength)),
                        new Rotation(Main.rand.NextFloat(MathHelper.TwoPi)),
                        Color.SpringGreen,
                        new TimeLeft(Main.rand.Next(40, 60) + 5 * (int)Strength),
                        new TimeUntilAction(Main.rand.Next(30, 45) + 5 * (int)Strength),
                        new LinearVelocityAcceleration(new Vector2(Main.rand.NextFloat(0.5f * Strength, 3f * Strength), Main.rand.NextFloat(0, Strength)).RotatedByRandom(MathHelper.TwoPi), 0, -0.05f, 0.95f, 0.95f),
                        new RotateWithLinearVelocity(0.01f, 0),
                        new LinearScaleIncrease(0.02f, 0.02f),
                        new ShiftColorThree(Color.SpringGreen, new Color(30, 45, 72, 200), new Color(0, 0,0, 0))
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

            float lightStrength = Utils.GetLerpValue(0, 10, Timer, true) * Utils.GetLerpValue(TotalTime, TotalTime - 10, Timer, true) * Strength * 5;
            Lighting.AddLight(Projectile.Center, 0.6f * lightStrength, 0.9f * lightStrength, 1f * lightStrength);

            if (BlackHole && Projectile.timeLeft > 20)
            {
                if (Projectile.timeLeft % 20 == 0 || Main.rand.Next(20) == 0)
                {
                    Vector2 position = Projectile.Center + new Vector2(Size, 0).RotatedByRandom(Math.PI * 2);
                    Projectile.NewProjectile(
                        spawnSource: Projectile.GetSource_FromThis(),
                        position: position,
                        velocity: (Projectile.Center - position).SafeNormalize(Vector2.Zero).RotatedBy(0.5f * (Main.rand.NextBool() ? 1 : -1)) * 15,
                        Type: ModContent.ProjectileType<BlackHoleStrip>(),
                        Damage: 0,
                        KnockBack: 0,
                        ai0: Projectile.Center.X,
                        ai1: Projectile.Center.Y);
                }
                if (Main.rand.Next(5) == 0)
                {
                    int tileCenterX = (int)(Projectile.Center.X / 16f);
                    int tileCenterY = (int)(Projectile.Center.Y / 16f);
                    int tileRandomX = Main.rand.Next(tileCenterX - Size / 8, tileCenterX + Size / 8);
                    int tileRandomY = Main.rand.Next(tileCenterY - Size / 8, tileCenterY + Size / 8);

                    Tile tile = Framing.GetTileSafely(tileRandomX, tileRandomY);
                    Vector2 randomPos = new(Main.rand.NextFloat(Projectile.Center.X - Size, Projectile.Center.X + Size), Main.rand.NextFloat(Projectile.Center.Y - Size, Projectile.Center.Y + Size));
                    if (!tile.HasTile)
                    {
                        float angle = Main.rand.NextFloat(-0.1f, 0.1f);

                        ParticleSystem.World.Create(
                            (UVCoordinates)ParticleAtlasSystem.AtlasDefinitions["GlowDot"],
                            (Position)randomPos,
                            (Scale)(new Vector2(Main.rand.NextFloat(1f, 2f))),
                            new Rotation(Utils.AngleFrom(Projectile.Center, randomPos) + MathHelper.PiOver2),
                            new Color(0, 0, 0, 0),
                            new TimeLeft(Main.rand.Next(30, 55)),
                            new AngularVelocityMoveToTarget(angle, Projectile.Center.X, Projectile.Center.Y, 0.05f),
                            new RotationIsVelocity(),
                            new LinearScaleIncrease(-0.05f, -0.05f),
                            new AlphaFadeInOut(5, 5, 120, 255, 255, 0)
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
            behindNPCsAndTiles.Add(index);
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
                    MathHelper.Lerp(-1.2f, 0, 
                    Timer < 25 ?
                    Easings.OutCirc(Utils.GetLerpValue(5, 25, Timer, true)) :
                    Easings.InCirc(Utils.GetLerpValue(TotalTime, TotalTime - 20, Timer, true))
                    ));
                blackHoleEffect.Parameters["uHoleRadius"].SetValue(
                    MathHelper.Lerp(-2.4f, -0.7f,
                    Timer < 20 ?
                    Easings.OutSine(Utils.GetLerpValue(0, 20, Timer, true)) :
                    Easings.InSine(Utils.GetLerpValue(TotalTime - 5, TotalTime - 25, Timer, true))
                    ));

                blackHoleEffect.Parameters["uTextureNoise0"].SetValue(AssetDirectory.Textures.Extra.Noise.GassyNoise.Value);
                blackHoleEffect.Parameters["uTextureNoise1"].SetValue(AssetDirectory.Textures.Extra.Noise.WobblyNoise.Value);
                blackHoleEffect.Parameters["uTexturePalette0"].SetValue(AssetDirectory.Textures.Extra.Palettes.AuraBlackHolePaletteHue.Value);
                blackHoleEffect.Parameters["uTexturePalette1"].SetValue(AssetDirectory.Textures.Extra.Palettes.OuterBlackHolePaletteValue.Value);
                blackHoleEffect.Parameters["uTextureDither"].SetValue(AssetDirectory.Textures.Extra.BlackHoleDither.Value);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, blackHoleEffect, Main.GameViewMatrix.TransformationMatrix);

                Texture2D empty = AssetDirectory.Textures.Pixel.Value;
                Main.spriteBatch.Draw(empty, Projectile.Center - Main.screenPosition, empty.Frame(), Color.White, 0, empty.Size() * 0.5f, Size * 0.5f, SpriteEffects.None, 0);

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
                float rad = i / 128f * MathF.PI * 2;
                Vector2 offset = new(MathF.Sin(rad), MathF.Cos(rad));
                float radOffset = 1 + _noise.GetNoise(offset.X, offset.Y) * 0.1f;

                positions[i] = Projectile.Center + offset * Size * 0.5f * radOffset * Easings.OutQuart(Utils.GetLerpValue(0, 20, Timer, true));
                if (i > 0)
                    rotations[i] = (positions[i] - positions[i - 1]).ToRotation();
            }
            rotations[0] = rotations[128];

            Color ShockwaveColor(float p) => Color.White;
            float ShockwaveWidth(float p) => (8 + MathF.Pow(1.5f, Strength)) * Utils.GetLerpValue(20, 5, Timer, true) * Utils.GetLerpValue(0, 5, Timer, true);

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
