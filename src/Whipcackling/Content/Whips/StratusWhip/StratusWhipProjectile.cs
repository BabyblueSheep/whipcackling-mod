using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Whipcackling.Assets;
using Whipcackling.Common.Systems.Drawing;
using Whipcackling.Common.Utilities;
using Whipcackling.Content.Whips.StratusWhip.Stars;
using Whipcackling.Core;
using Whipcackling.Core.Particles;

namespace Whipcackling.Content.Whips.StratusWhip
{
    public class StratusWhipProjectile : ModWhip, IDrawPixelated
    {
        public override string LocalizationCategory => "Whips.StratusWhip";

        public override SoundStyle SwingSound => SoundID.Item153;
        public override Color StringColor => new(47, 46, 100);
        public override int HandleOffset => 4;

        public readonly static int[] TagDebuffList = [ModContent.BuffType<StratusWhipNPCDebuffRed>(),
                                                      ModContent.BuffType<StratusWhipNPCDebuffYellow>(),
                                                      ModContent.BuffType<StratusWhipNPCDebuffBlue>(),
                                                      ModContent.BuffType<StratusWhipNPCDebuffPurple>(),
                                                      ModContent.BuffType<StratusWhipNPCDebuffWhite>()];

        private Vector2[,] _prevPositionsPlane;
        private Vector2[,] _prevPositionsPlaneSmoothed;
        private VertexPlane _plane;

        public ref float Hits => ref Projectile.ai[1];

        public override void SafeSetDefaults()
        {
            Projectile.WhipSettings.Segments = ConstantsStratus.WhipSegments;
            Projectile.WhipSettings.RangeMultiplier = ConstantsStratus.WhipRangeMultiplier;
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
                int id = Main.rand.NextBool() ? (int)CalamityDusts.BlueCosmilite : (int)CalamityDusts.PurpleCosmilite;
                Rectangle rectangle = Utils.CenteredRectangle(Projectile.WhipPointsForCollision[^2], new Vector2(30f));
                Dust dust = Dust.NewDustDirect(rectangle.TopLeft(), rectangle.Width, rectangle.Height, id, 0f, 0f, 0, Color.White, progress);
                dust.noGravity = true;
                dust.velocity *= 0.8f;
                dust.scale *= 1.2f + Main.rand.NextFloat() * 0.5f;
                dust.fadeIn = progress;
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
            Projectile.damage = (int)(Projectile.damage * (1f - ConstantsStratus.DamageFalloff));

            Hits++;
            if (Hits > 1)
                return;
            List<int> debuffs = [];
            for (int i = 0; i < TagDebuffList.Length; i++)
            {
                int debuff = TagDebuffList[i];
                if (target.HasBuff(debuff))
                {
                    target.AddBuff(debuff, ConstantsStratus.TagDuration);
                }
                else
                {
                    debuffs.Add(debuff);
                }
            }

            if (debuffs.Count <= 0)
                return;

            int randomDebuff = Main.rand.NextFromCollection(debuffs);
            target.AddBuff(randomDebuff, ConstantsStratus.TagDuration);

            if (randomDebuff == ModContent.BuffType<StratusWhipNPCDebuffRed>())
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<StratusRedStar>(), (int)Math.Ceiling(Projectile.damage * ConstantsStratus.RedStarDamageMult), ConstantsStratus.RedStarKnockback, Projectile.owner,
                    target.whoAmI + 1, Main.rand.NextFloat(0, MathHelper.TwoPi), 0);
            }
            else if (randomDebuff == ModContent.BuffType<StratusWhipNPCDebuffYellow>())
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<StratusYellowStar>(), (int)Math.Ceiling(Projectile.damage * ConstantsStratus.YellowStarDamageMult), ConstantsStratus.YellowStarKnockback, Projectile.owner,
    target.whoAmI + 1, Main.rand.NextFloat(0, MathHelper.TwoPi), 0);
            }
            else if (randomDebuff == ModContent.BuffType<StratusWhipNPCDebuffBlue>())
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<StratusBlueStar>(), (int)Math.Ceiling(Projectile.damage * ConstantsStratus.BlueStarDamageMult), ConstantsStratus.BlueStarKnockback, Projectile.owner,
    target.whoAmI + 1, Main.rand.NextFloat(0, MathHelper.TwoPi), 0);
            }
            else if (randomDebuff == ModContent.BuffType<StratusWhipNPCDebuffPurple>())
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<StratusPurpleStar>(), (int)Math.Ceiling(Projectile.damage * ConstantsStratus.PurpleStarDamageMult), ConstantsStratus.PurpleStarKnockback, Projectile.owner,
target.whoAmI + 1, Main.rand.NextFloat(0, MathHelper.TwoPi), 0);
            }
            else if (randomDebuff == ModContent.BuffType<StratusWhipNPCDebuffWhite>())
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<StratusWhiteStar>(), (int)Math.Ceiling(Projectile.damage * ConstantsStratus.WhiteStarDamageMult), ConstantsStratus.WhiteStarKnockback, Projectile.owner,
target.whoAmI + 1, Main.rand.NextFloat(0, MathHelper.TwoPi), 0);
            }
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
            planeEffect.Parameters["uTexturePalette0"].SetValue(AssetDirectory.Textures.Extra.Palettes.LunarTrailPaletteHue.Value);

            planeEffect.CurrentTechnique.Passes[0].Apply();
            _plane.DrawMesh();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }
    }
}
