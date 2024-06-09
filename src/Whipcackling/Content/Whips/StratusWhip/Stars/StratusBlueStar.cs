using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Whipcackling.Assets;
using Whipcackling.Common.Systems.Drawing;
using Whipcackling.Common.Utilities;

namespace Whipcackling.Content.Whips.StratusWhip.Stars
{
    public class StratusBlueStar : StratusStar<StratusWhipNPCDebuffBlue>, IDrawPixelated
    {
        VertexStrip _strip;

        public override float RotateSpeed => 0.5f;
        public override float Radius => 500;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.scale = 1.2f;
        }

        public override void PostAI()
        {
            if (Main.rand.NextBool(4))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool() ? DustID.MagicMirror : DustID.BlueTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, 0.25f), 150, Color.White, 1.3f);
            }
            if (Main.rand.NextBool(4))
            {
                Vector2 screenSize = new Vector2(Main.screenWidth, Main.screenHeight);
                if (Projectile.Hitbox.Intersects(Utils.CenteredRectangle(Main.screenPosition + screenSize * 0.5f, screenSize + new Vector2(400f))))
                {
                    Gore.NewGore(Projectile.position, Projectile.velocity * 0.2f, 17);
                }
            }
        }

        public override void OnLosingTarget()
        {
            Projectile.velocity = Projectile.rotation.ToRotationVector2() * 20f;
        }

        public override void HostileBehavior(NPC target)
        {
            Projectile.velocity = HelperMethods.RotateTowards(Projectile.velocity, Projectile.position, target.position, 0.2f);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            for (int i = 0; i < 18; i++)
            {
                Vector2 speed = Projectile.rotation.ToRotationVector2().RotatedByRandom(0.2f);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool() ? DustID.MagicMirror : DustID.BlueTorch, speed.X, speed.Y, 150, Color.White, 1.3f);
            }

            for (int i = 0; i < 24; i++)
            {
                Vector2 speed = Utils.RandomVector2(Main.rand, 4, 5).RotatedByRandom(Math.PI * 2);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool() ? DustID.MagicMirror : DustID.BlueTorch, speed.X, speed.Y, 150, Color.White, 1.1f);
            }

            for (int i = 0; i < 12; i++)
            {
                Vector2 speed = Projectile.rotation.ToRotationVector2().RotatedByRandom(0.3f) * 0.5f;
                Gore.NewGore(Projectile.position, speed, 17);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < 4; i++)
            {
                Main.spriteBatch.Draw(
                    texture: texture,
                    position: Projectile.Center - Main.screenPosition
                    + 7f * new Vector2((float)Math.Sin(Main.GlobalTimeWrappedHourly * 1.5f + MathHelper.TwoPi * i / 4f), (float)Math.Cos(Main.GlobalTimeWrappedHourly * 1.5f + MathHelper.TwoPi * i / 4f)),
                    sourceRectangle: texture.Frame(),
                    color: new Color(1, 1f, 1f, 0.2f),
                    rotation: Projectile.rotation + MathHelper.PiOver2,
                    origin: texture.Size() * 0.5f,
                    scale: Projectile.scale + 0.2f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.2f,
                    effects: SpriteEffects.None,
                    layerDepth: 0);
            }
            Main.spriteBatch.Draw(
                texture: texture,
                position: Projectile.Center - Main.screenPosition,
                sourceRectangle: texture.Frame(),
                color: new Color(0.5f, 1f, 1f, 0.5f),
                rotation: Projectile.rotation + Main.rand.NextFloat(-0.12f, 0.12f) + MathHelper.PiOver2,
                origin: texture.Size() * 0.5f,
                scale: Projectile.scale + 0.25f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.15f,
                effects: SpriteEffects.None,
                layerDepth: 0);

            Main.spriteBatch.Draw(
                texture: texture,
                position: Projectile.Center - Main.screenPosition,
                sourceRectangle: texture.Frame(),
                color: new Color(0.2f, 0.6f, 1f, 0.8f),
                rotation: Projectile.rotation + Main.rand.NextFloat(-0.1f, 0.1f) + MathHelper.PiOver2,
                origin: texture.Size() * 0.5f,
                scale: Projectile.scale + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.1f,
                effects: SpriteEffects.None,
                layerDepth: 0);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public void DrawPixelated()
        {
            _strip ??= new();
            Effect effect = AssetDirectory.Effects.TriangleTrail.Value;
            effect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            effect.Parameters["uTexturePalette0"].SetValue(AssetDirectory.Textures.Extra.Palettes.BlueStarHueTrail.Value);

            Color StripColor(float p) => new Color(1, 1, 1, 1);
            float StripWidth(float p) => 12;

            _strip.PrepareStrip(Projectile.oldPos, Projectile.oldRot, StripColor, StripWidth, Projectile.Size * 0.5f - Main.screenPosition, Projectile.oldPos.Length);

            effect.CurrentTechnique.Passes[0].Apply();
            _strip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }
    }
}
