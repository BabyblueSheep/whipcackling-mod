using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Whipcackling.Assets;
using Whipcackling.Common;
using Whipcackling.Common.Systems.Drawing;
using Whipcackling.Common.Utilities;
using Whipcackling.Core;

namespace Whipcackling.Content.Whips.NuclearWhip
{
    public class NuclearWhipProjectile : ModWhip, IDrawPixelated
    {
        public override string LocalizationCategory => "Whips.NuclearWhip";

        public override SoundStyle SwingSound => SoundID.Item153;
        public override Color StringColor => new(140, 234, 87);
        public override int HandleOffset => 4;

        private Vector2[] _prevPositionsSmoothed;
        private float[] _prevRotationsSmoothed;
        private VertexStrip _strip;

        private Vector2[,] _prevPositionsPlane;
        private Vector2[,] _prevPositionsPlaneSmoothed;
        private VertexPlane _plane;

        public override void SafeSetDefaults()
        {
            Projectile.WhipSettings.Segments = ConstantsNuclear.WhipSegments;
            Projectile.WhipSettings.RangeMultiplier = ConstantsNuclear.WhipRangeMultiplier;
            Projectile.extraUpdates = 2;
        }

        public override void ArcAI()
        {
            float timeToFlyOut = Main.player[Projectile.owner].itemAnimationMax * Projectile.MaxUpdates;
            float ratio = Timer / timeToFlyOut;
            float progress = Utils.GetLerpValue(0.1f, 0.7f, ratio, true) * Utils.GetLerpValue(0.9f, 0.7f, ratio, true);

            Projectile.WhipPointsForCollision.Clear();
            Projectile.FillWhipControlPoints(Projectile, Projectile.WhipPointsForCollision);

            if (progress > 0.1f)
            {
                int id = DustID.CursedTorch;
                Rectangle rectangle = Utils.CenteredRectangle(Projectile.WhipPointsForCollision[^1], new Vector2(30f));
                Dust dust = Dust.NewDustDirect(rectangle.TopLeft(), rectangle.Width, rectangle.Height, id, 0f, 0f, 100, Color.White, progress);
                dust.noGravity = true;
                dust.velocity *= 1.5f;
                dust.scale *= 0.9f + Main.rand.NextFloat() * 0.9f;
            }

            #region Previous positions
            if (_prevPositionsPlane == null)
            {
                _prevPositionsPlane ??= new Vector2[10, Projectile.WhipSettings.Segments];
                for (int i = 0; i < 10; i++)
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
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 240);
            target.AddBuff(ModContent.BuffType<NuclearWhipNPCDebuff>(), ConstantsNuclear.TagDuration);
        }

        public void DrawPixelated()
        {
            _plane ??= new VertexPlane();
            _strip ??= new VertexStrip();

            float timeToFlyOut = Main.player[Projectile.owner].itemAnimationMax * Projectile.MaxUpdates;
            float ratio = Timer / timeToFlyOut;
            float progress = Utils.GetLerpValue(0.2f, 0.5f, ratio, true) * Utils.GetLerpValue(0.9f, 0.7f, ratio, true);

            Color StripColorPlane(float progressX, float progressY) => new(1, 1, 1, (1 - Easings.InSine(progressY)) * Easings.OutQuint(progressX));

            Color StripColor(float p) => Color.White;
            float StripWidth(float p) => 16 * progress;

            #region Plane positions
            // Prepare Bezier curving
            int accuracy = 10; // Accuracy of the Bezier curve. The bigger, the more accurate
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
            planeEffect.Parameters["uTexturePalette0"].SetValue(AssetDirectory.Textures.Extra.Palettes.AcidFlameTrailPaletteHue.Value);

            planeEffect.CurrentTechnique.Passes[0].Apply();
            _plane.DrawMesh();

            _strip.PrepareStrip(_prevPositionsSmoothed, _prevRotationsSmoothed, StripColor, StripWidth, -Main.screenPosition);

            Effect effect = AssetDirectory.Effects.FlameTrail.Value;
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            effect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);

            effect.Parameters["uTextureNoise0"].SetValue(AssetDirectory.Textures.Extra.Noise.GassyNoise.Value);
            effect.Parameters["uTextureNoise1"].SetValue(AssetDirectory.Textures.Extra.Noise.CellPackedNoise.Value);
            effect.Parameters["uTextureNoise2"].SetValue(AssetDirectory.Textures.Extra.Noise.CellInvertedNoise.Value);
            effect.Parameters["uTexturePalette0"].SetValue(AssetDirectory.Textures.Extra.Palettes.MeldFlamePaletteValue.Value);
            effect.Parameters["uTexturePalette1"].SetValue(AssetDirectory.Textures.Extra.Palettes.AcidFlamePaletteHue.Value);

            effect.CurrentTechnique.Passes[0].Apply();
            _strip.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }
    }
}
