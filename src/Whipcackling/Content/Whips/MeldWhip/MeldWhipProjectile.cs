using CalamityMod;
using CalamityMod.Cooldowns;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Whipcackling.Assets;
using Whipcackling.Common;
using Whipcackling.Common.Systems.Drawing;

namespace Whipcackling.Content.Whips.MeldWhip
{
    public class MeldWhipProjectile : ModWhip, IDrawPixelated
    {
        public override SoundStyle SwingSound => SoundID.Item153;
        public override Color StringColor => new(30, 45, 72);
        public override int HandleOffset => 8;

        private VertexStrip _strip;

        public ref float Hits => ref Projectile.ai[1];

        public bool Initialized
        {
            get => Projectile.localAI[0] > 0;
            set => Projectile.localAI[0] = value ? 1 : 0;
        }


        public override void SafeSetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25;
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

            Projectile.oldPos[0] = Projectile.WhipPointsForCollision[^2];
            Projectile.oldRot[0] = (Projectile.WhipPointsForCollision[^1] - Projectile.WhipPointsForCollision[^2]).ToRotation() - MathHelper.PiOver2;

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
            _strip ??= new VertexStrip();

            float timeToFlyOut = Main.player[Projectile.owner].itemAnimationMax * Projectile.MaxUpdates;
            float ratio = Timer / timeToFlyOut;
            float progress = Utils.GetLerpValue(0.2f, 0.5f, ratio, true) * Utils.GetLerpValue(0.9f, 0.7f, ratio, true);

            Color StripColor(float p) => Color.White;

            float StripWidth(float p) => 32 * progress;

            int accuracy = 30; // Accuracy of the Bezier curve. The bigger, the more accurate

            Vector2 startPoint = Projectile.oldPos[0];
            Vector2 endPoint = Projectile.oldPos[^1];
            int middleIndex = Projectile.oldPos.Length / 2;
            Vector2 middlePoint = Projectile.oldPos[middleIndex];

            // Make middle point further for a more pronounced curve https://stackoverflow.com/questions/47177493/python-point-on-a-line-closest-to-third-point
            Vector2 line = endPoint - startPoint;
            float lineLength = line.LengthSquared();
            float coefficient = (line.Y * (middlePoint.Y - startPoint.Y) + line.X * (middlePoint.X - startPoint.X)) / lineLength;
            Vector2 middlePointOnLine = startPoint + line * coefficient;
            Vector2 controlPoint = Vector2.Lerp(middlePoint, middlePointOnLine, -(1 + 0f * progress));

            Vector2[] curvePositions = new Vector2[accuracy];
            curvePositions[0] = startPoint;
            float[] curveRotations = new float[accuracy];

            //Quadratic Bezier curve https://en.wikipedia.org/wiki/De_Casteljau%27s_algorithm
            for (int i = 1; i < accuracy; i++)
            {
                float p = i / (float)accuracy;

                Vector2 lerpedStart = Vector2.Lerp(startPoint, controlPoint, p);
                Vector2 lerpedEnd = Vector2.Lerp(controlPoint, endPoint, p);

                curvePositions[i] = Vector2.Lerp(lerpedStart, lerpedEnd, p);
                curveRotations[i] = (curvePositions[i] - curvePositions[i - 1]).ToRotation();
            }
            curveRotations[0] = curveRotations[1];

            Effect effect = AssetDirectory.Effects.FlameTrail.Value;
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            effect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);

            effect.Parameters["uTextureNoise0"].SetValue(AssetDirectory.Textures.Extra.Noise.BlurredPerlinNoise.Value);
            effect.Parameters["uTextureNoise1"].SetValue(AssetDirectory.Textures.Extra.Noise.CellNoise.Value);
            effect.Parameters["uTextureNoise2"].SetValue(AssetDirectory.Textures.Extra.Noise.CirclyNoise.Value);
            effect.Parameters["uTexturePalette0"].SetValue(AssetDirectory.Textures.Extra.Palettes.MeldFlamePaletteValue.Value);
            effect.Parameters["uTexturePalette1"].SetValue(AssetDirectory.Textures.Extra.Palettes.MeldFlamePaletteHue.Value);


            effect.CurrentTechnique.Passes[0].Apply();

            _strip.PrepareStripWithProceduralPadding(curvePositions, curveRotations, StripColor, StripWidth, -Main.screenPosition);
            _strip.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }
    }
}
