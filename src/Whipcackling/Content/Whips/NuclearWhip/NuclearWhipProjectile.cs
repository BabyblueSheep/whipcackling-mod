using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Whipcackling.Assets;
using Whipcackling.Common;
using Whipcackling.Common.Systems.Drawing;

namespace Whipcackling.Content.Whips.NuclearWhip
{
    public class NuclearWhipProjectile : ModWhip, IDrawPixelated
    {
        public override SoundStyle SwingSound => SoundID.Item153;
        public override Color StringColor => new(140, 234, 87);
        public override int HandleOffset => 4;

        private VertexStrip _strip;

        public override void SafeSetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
        }

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

            if (progress > 0.1f)
            {
                Projectile.WhipPointsForCollision.Clear();
                Projectile.FillWhipControlPoints(Projectile, Projectile.WhipPointsForCollision);

                int id = DustID.CursedTorch;
                Rectangle rectangle = Utils.CenteredRectangle(Projectile.WhipPointsForCollision[^1], new Vector2(30f));
                Dust dust = Dust.NewDustDirect(rectangle.TopLeft(), rectangle.Width, rectangle.Height, id, 0f, 0f, 100, Color.White, progress);
                dust.noGravity = true;
                dust.velocity *= 1.5f;
                dust.scale *= 0.9f + Main.rand.NextFloat() * 0.9f;
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
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 240);
            target.AddBuff(ModContent.BuffType<NuclearWhipNPCDebuff>(), ConstantsNuclear.TagDuration);
        }

        public void DrawPixelated()
        {
            _strip ??= new VertexStrip();

            float timeToFlyOut = Main.player[Projectile.owner].itemAnimationMax * Projectile.MaxUpdates;
            float ratio = Timer / timeToFlyOut;
            float progress = Utils.GetLerpValue(0.2f, 0.5f, ratio, true) * Utils.GetLerpValue(0.9f, 0.7f, ratio, true);

            Color StripColor(float p) => Color.White;

            float StripWidth(float p) => 16 * progress;

            int accuracy = 25; // Accuracy of the Bezier curve. The bigger, the more accurate

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

            effect.Parameters["uTextureNoise0"].SetValue(AssetDirectory.Textures.Extra.Noise.GassyNoise.Value);
            effect.Parameters["uTextureNoise1"].SetValue(AssetDirectory.Textures.Extra.Noise.CellPackedNoise.Value);
            effect.Parameters["uTextureNoise2"].SetValue(AssetDirectory.Textures.Extra.Noise.CellInvertedNoise.Value);
            effect.Parameters["uTexturePalette0"].SetValue(AssetDirectory.Textures.Extra.Palettes.MeldFlamePaletteValue.Value);
            effect.Parameters["uTexturePalette1"].SetValue(AssetDirectory.Textures.Extra.Palettes.AcidFlamePaletteHue.Value);


            effect.CurrentTechnique.Passes[0].Apply();

            _strip.PrepareStripWithProceduralPadding(curvePositions, curveRotations, StripColor, StripWidth, -Main.screenPosition);
            _strip.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }
    }
}
