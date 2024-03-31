using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Whipcackling.Assets;
using Whipcackling.Common.Systems.Drawing;
using Whipcackling.Common.Utilities;
using Whipcackling.Core;

namespace Whipcackling.Content.Whips.MeldWhip
{
    public class MeldWhipProjectile : ModWhip, IDrawPixelated
    {
        public override string LocalizationCategory => "Whips.MeldWhip";

        public override SoundStyle SwingSound => SoundID.Item153;
        public override Color StringColor => new(30, 45, 72);
        public override int HandleOffset => 8;

        private Vector2[] _prevPositionsSmoothed;
        private float[] _prevRotationsSmoothed;
        private VertexStrip _strip;

        private Vector2[,] _prevPositionsPlane;
        private Vector2[,] _prevPositionsPlaneSmoothed;
        private VertexPlane _plane;

        public ref float Hits => ref Projectile.ai[1];

        public bool Initialized
        {
            get => Projectile.localAI[0] > 0;
            set => Projectile.localAI[0] = value ? 1 : 0;
        }

        public override void SafeSetDefaults()
        {
            Projectile.WhipSettings.Segments = ConstantsMeld.WhipSegments;
            Projectile.WhipSettings.RangeMultiplier = ConstantsMeld.WhipRangeMultiplier;
            Projectile.extraUpdates = 2;
        }

        public override void ArcAI()
        {
            float timeToFlyOut = Main.player[Projectile.owner].itemAnimationMax * Projectile.MaxUpdates;
            float ratio = Timer / timeToFlyOut;
            float progress = Utils.GetLerpValue(0.1f, 0.7f, ratio, true) * Utils.GetLerpValue(0.9f, 0.7f, ratio, true);

            if (progress > 0.1f)
            {
                Projectile.WhipPointsForCollision.Clear();
                Projectile.FillWhipControlPoints(Projectile, Projectile.WhipPointsForCollision);

                int id = DustID.CoralTorch;
                Rectangle rectangle = Utils.CenteredRectangle(Projectile.WhipPointsForCollision[^1], new Vector2(30f));
                Dust dust = Dust.NewDustDirect(rectangle.TopLeft(), rectangle.Width, rectangle.Height, id, 0f, 0f, 100, Color.White, 2 * progress);
                int index = Main.rand.Next(Projectile.WhipPointsForCollision.Count - 10, Projectile.WhipPointsForCollision.Count);
                Vector2 spinningPoint = Projectile.WhipPointsForCollision[index] - Projectile.WhipPointsForCollision[index - 1];
                dust.fadeIn = 0.1f;
                dust.noGravity = true;
                dust.velocity += spinningPoint.RotatedBy(Main.player[Projectile.owner].direction * ((float)Math.PI / 2f));
                dust.velocity *= 0.5f;
            }

            for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
            {
                Projectile.oldPos[i] = Projectile.oldPos[i - 1];
            }
            for (int i = Projectile.oldRot.Length - 1; i > 0; i--)
            {
                Projectile.oldRot[i] = Projectile.oldRot[i - 1];
            }

            #region Previous positions
            if (_prevPositionsPlane == null)
            {
                _prevPositionsPlane ??= new Vector2[25, Projectile.WhipSettings.Segments];
                for (int i = 0; i < 25; i++)
                {
                    for (int j = 0; j < Projectile.WhipSettings.Segments; j++)
                    {
                        _prevPositionsPlane[i, j] = Projectile.WhipPointsForCollision[j];
                    }
                }
            }

            int width = _prevPositionsPlane.GetLength(1);
            int height = _prevPositionsPlane.GetLength(0);

            for (int x = width - 1; x >= 0; x--)
            {
                for (int y = height - 1; y > 0; y--)
                {
                    _prevPositionsPlane[y, x] = _prevPositionsPlane[y - 1, x];
                }
            }
            for (int x = 0; x < width; x++)
            {
                _prevPositionsPlane[0, x] = Projectile.WhipPointsForCollision[x];
            }
            #endregion

            if (!Initialized)
            {
                NegazoneRenderer.WhipPoints.Add(Projectile);
                Initialized = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
            Projectile.damage = (int)(Projectile.damage * (1f - ConstantsMeld.DamageFalloff));

            Hits++;
            if (Hits == ConstantsMeld.BlackHoleHitsByWhip && !Main.player[Projectile.owner].HasCooldown(MeldWhipCooldown.ID))
            {
                target.AddBuff(ModContent.BuffType<MeldWhipNPCDebuffSuper>(), ConstantsMeld.SuperTagDuration);
                Main.player[Projectile.owner].AddCooldown(MeldWhipCooldown.ID, ConstantsMeld.SuperTagCooldown);
            }
            else if ((Hits + 1) % ConstantsMeld.TagHitsByWhip == 0)
            {
                target.AddBuff(ModContent.BuffType<MeldWhipNPCDebuff>(), ConstantsMeld.TagDuration);
            }
        }

        public override float SegmentScale(int i) => i == Projectile.WhipSettings.Segments - 2 ? 1.2f : 0.8f;

        public void DrawPixelated()
        {
            _plane ??= new VertexPlane();
            _strip ??= new VertexStrip();

            float timeToFlyOut = Main.player[Projectile.owner].itemAnimationMax * Projectile.MaxUpdates;
            float ratio = Timer / timeToFlyOut;
            float progress = Utils.GetLerpValue(0.2f, 0.5f, ratio, true) * Utils.GetLerpValue(0.9f, 0.7f, ratio, true);

            Color StripColorPlane(float progressX, float progressY) => new(1, 1, 1, (1 - Easings.InSine(progressY)) * Easings.OutQuint(progressX));

            Color StripColor(float p) => Color.White;
            float StripWidth(float p) => 32 * progress;

            #region Plane positions
            // Prepare Bezier curving
            int accuracy = 25; // Accuracy of the Bezier curve. The bigger, the more accurate
            _prevPositionsPlaneSmoothed ??= new Vector2[accuracy, Projectile.WhipSettings.Segments];
            Vector2 startPoint, endPoint, middlePoint;
            float middleIndex;

            int width = _prevPositionsPlane.GetLength(1);
            int height = _prevPositionsPlane.GetLength(0);
            int widthSmooth = _prevPositionsPlaneSmoothed.GetLength(1);
            int heightSmooth = _prevPositionsPlaneSmoothed.GetLength(0);

            middleIndex = (height - 1) / 2f;
            for (int x = 0; x < width; x++)
            {
                startPoint = _prevPositionsPlane[0, x];
                endPoint = _prevPositionsPlane[height - 1, x];
                middlePoint = _prevPositionsPlane[(int)middleIndex, x];

                // Make middle point further for a more pronounced curve https://stackoverflow.com/questions/47177493/python-point-on-a-line-closest-to-third-point
                Vector2 line = endPoint - startPoint;
                float lineLength = line.LengthSquared();
                float coefficient = (line.Y * (middlePoint.Y - startPoint.Y) + line.X * (middlePoint.X - startPoint.X)) / lineLength;
                Vector2 middlePointOnLine = startPoint + line * coefficient;
                Vector2 controlPoint = Vector2.Lerp(middlePoint, middlePointOnLine, -(1 + 0f * progress));

                _prevPositionsPlaneSmoothed[0, x] = startPoint;
                for (int i = 1; i < accuracy; i++)
                {
                    float p = i / (float)accuracy;

                    Vector2 lerpedStart = Vector2.Lerp(startPoint, controlPoint, p);
                    Vector2 lerpedEnd = Vector2.Lerp(controlPoint, endPoint, p);

                    _prevPositionsPlaneSmoothed[i, x] = Vector2.Lerp(lerpedStart, lerpedEnd, p);
                }
            }
            #endregion
            #region Strip positions
            _prevPositionsSmoothed ??= new Vector2[accuracy];
            _prevRotationsSmoothed ??= new float[accuracy];
            for (int i = 0; i < accuracy - 1; i++)
            {
                _prevPositionsSmoothed[i] = _prevPositionsPlaneSmoothed[i, width - 1];
                _prevRotationsSmoothed[i] = (_prevPositionsPlaneSmoothed[i + 1, width - 1] - _prevPositionsPlaneSmoothed[i, width - 1]).ToRotation();
            }
            _prevPositionsSmoothed[accuracy - 1] = _prevPositionsPlaneSmoothed[accuracy - 1, width - 1];
            _prevRotationsSmoothed[accuracy - 1] = _prevRotationsSmoothed[accuracy - 2];
            #endregion

            _plane.PreparePlane(_prevPositionsPlaneSmoothed, StripColorPlane, -Main.screenPosition);

            Effect planeEffect = AssetDirectory.Effects.WhipSwingTrail.Value;
            planeEffect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            planeEffect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            planeEffect.Parameters["uTexturePalette0"].SetValue(AssetDirectory.Textures.Extra.Palettes.ToenailFlameTrailPaletteHue.Value);

            planeEffect.CurrentTechnique.Passes[0].Apply();
            _plane.DrawMesh();

            _strip.PrepareStrip(_prevPositionsSmoothed, _prevRotationsSmoothed, StripColor, StripWidth, -Main.screenPosition);

            Effect effect = AssetDirectory.Effects.FlameTrail.Value;
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            effect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);

            effect.Parameters["uTextureNoise0"].SetValue(AssetDirectory.Textures.Extra.Noise.BlurredPerlinNoise.Value);
            effect.Parameters["uTextureNoise1"].SetValue(AssetDirectory.Textures.Extra.Noise.CellNoise.Value);
            effect.Parameters["uTextureNoise2"].SetValue(AssetDirectory.Textures.Extra.Noise.CirclyNoise.Value);
            effect.Parameters["uTexturePalette0"].SetValue(AssetDirectory.Textures.Extra.Palettes.MeldFlamePaletteValue.Value);
            effect.Parameters["uTexturePalette1"].SetValue(AssetDirectory.Textures.Extra.Palettes.MeldFlamePaletteHue.Value);

            effect.CurrentTechnique.Passes[0].Apply();

            _strip.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }
    }
}
