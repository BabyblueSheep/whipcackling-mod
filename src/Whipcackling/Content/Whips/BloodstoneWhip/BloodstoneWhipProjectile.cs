using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Whipcackling.Assets;
using Whipcackling.Common.Systems.Drawing;
using Whipcackling.Common.Utilities;
using Whipcackling.Content.Whips.NuclearWhip;
using Whipcackling.Content.Whips.StratusWhip;
using Whipcackling.Core;
using Whipcackling.Core.Particles.Components;
using static Whipcackling.Assets.AssetDirectory;

namespace Whipcackling.Content.Whips.BloodstoneWhip
{
    public class BloodstoneWhipProjectile : ModWhip, IDrawPixelated
    {
        public override string LocalizationCategory => "Whips.BloodstoneWhip";

        public override SoundStyle SwingSound => SoundID.Item153;
        public override Color StringColor => new(122, 10, 60);
        public override int HandleOffset => 8;

        private Vector2[,] _prevPositionsPlane;
        private Vector2[,] _prevPositionsPlaneSmoothed;
        private VertexPlane _plane;

        public override void SafeSetDefaults()
        {
            Projectile.WhipSettings.Segments = ConstantsBloodstone.WhipSegments;
            Projectile.WhipSettings.RangeMultiplier = ConstantsBloodstone.WhipRangeMultiplier;
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
                int id = DustID.Blood;
                Rectangle spawnArea = Utils.CenteredRectangle(Projectile.WhipPointsForCollision[Projectile.WhipPointsForCollision.Count - 1], new Vector2(30f, 30f));
                Vector2 velocity = Projectile.WhipPointsForCollision[Projectile.WhipPointsForCollision.Count - 2].DirectionTo(Projectile.WhipPointsForCollision[Projectile.WhipPointsForCollision.Count - 1]).SafeNormalize(Vector2.Zero);
                Dust dust = Dust.NewDustDirect(spawnArea.TopLeft(), spawnArea.Width, spawnArea.Height, id, 0f, 0f, 0, default(Color), 1.2f);
                dust.noGravity = Main.rand.Next(3) == 0;
                dust.velocity += velocity * 2f;
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
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
            Projectile.damage = (int)(Projectile.damage * (1f - ConstantsBloodstone.DamageFalloff));

            target.AddBuff(ModContent.BuffType<BurningBlood>(), 240);
        }

        public override void DrawAheadWhip(ref Color lightColor)
        {
            List<Vector2> points = new();
            Projectile.FillWhipControlPoints(Projectile, points);

            SpriteEffects direction = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Asset<Texture2D> texture = AssetDirectory.Textures.Whips.BloodstoneWhip.BloodstoneSkull;
            Rectangle frame = texture.Frame(1, 5, 0, 0);
            int i = points.Count - 2;

            Vector2 position = points[i];
            Vector2 difference = points[i] - points[i - 1];
            float rotation = difference.ToRotation() - MathHelper.PiOver2;
            Color color = Lighting.GetColor(points[i].ToTileCoordinates());

            Main.EntitySpriteDraw(texture.Value, position - Main.screenPosition, frame, color, rotation, frame.Size() * 0.5f, 1, direction, 0);
        }

        public override int SegmentVariant(int i)
        {
            if (i == ConstantsBloodstone.WhipSegments - 1)
                return 4;
            if (i == ConstantsBloodstone.WhipSegments - 2)
                return 3;
            return 1 + i % 2;
        }

        public void DrawPixelated()
        {
            _plane ??= new VertexPlane();

            float timeToFlyOut = Main.player[Projectile.owner].itemAnimationMax * Projectile.MaxUpdates;
            float ratio = Timer / timeToFlyOut;
            float progress = Utils.GetLerpValue(0.2f, 0.5f, ratio, true) * Utils.GetLerpValue(0.9f, 0.7f, ratio, true);

            Color StripColorPlane(float progressX, float progressY) => new(1, 1, 1, (1 - Easings.InSine(progressY)) * Easings.OutQuint(progressX));

            #region Plane positions
            // Prepare Bezier curving
            int accuracy = 15; // Accuracy of the Bezier curve. The bigger, the more accurate
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

            _plane.PreparePlane(_prevPositionsPlaneSmoothed, StripColorPlane, -Main.screenPosition);

            Effect planeEffect = AssetDirectory.Effects.WhipSwingTrail.Value;
            planeEffect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            planeEffect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            planeEffect.Parameters["uTexturePalette0"].SetValue(AssetDirectory.Textures.Extra.Palettes.BloodyTrailPaletteHue.Value);

            planeEffect.CurrentTechnique.Passes[0].Apply();
            _plane.DrawMesh();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }
    }
}
