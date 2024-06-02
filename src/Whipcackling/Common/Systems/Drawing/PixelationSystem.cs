using CalamityMod.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Whipcackling.Assets;

namespace Whipcackling.Common.Systems.Drawing
{
    //https://github.com/DominicKarma/WrathOfTheGodsPublic/blob/main/Core/Graphics/Primitives/PrimitivePixelationSystem.cs
    [Autoload(Side = ModSide.Client)]
    public class PixelationSystem : ModSystem
    {
        public static ManagedRenderTarget PixelationTarget { get; private set; }

        private static bool _shouldDraw;

        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            Main.QueueMainThreadAction(() => PixelationTarget = new(true, ManagedRenderTarget.CreateScreenSizedTarget));
            On_Main.DoDraw_DrawNPCsOverTiles += PixelateRenderTarget;
            RenderTargetManager.RenderTargetUpdateLoopEvent += PrepareRenderTarget;
        }

        public override void Unload()
        {
            On_Main.DoDraw_DrawNPCsOverTiles -= PixelateRenderTarget;
            RenderTargetManager.RenderTargetUpdateLoopEvent -= PrepareRenderTarget;
        }

        private void PrepareRenderTarget()
        {
            _shouldDraw = false;

            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile projectile = Main.projectile[i];

                if (!projectile.active)
                    continue;
                if (projectile.ModProjectile is not IDrawPixelated)
                    continue;

                _shouldDraw = true;
                break;
            }

            if (!_shouldDraw)
                return;

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null);

            GraphicsDevice device = Main.instance.GraphicsDevice;
            device.SetRenderTarget(PixelationTarget);
            device.Clear(Color.Transparent);

            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile projectile = Main.projectile[i];

                if (!projectile.active)
                    continue;
                if (projectile.ModProjectile is not IDrawPixelated pixelatedProjectile)
                    continue;

                pixelatedProjectile.DrawPixelated();
            }

            device.SetRenderTarget(null);

            Main.spriteBatch.End();
        }

        private void PixelateRenderTarget(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            if (!_shouldDraw)
            {
                orig(self);
                return;
            }

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
            
            Effect effect = AssetDirectory.Effects.Pixelise.Value;
            effect.Parameters["uResolution"].SetValue(PixelationTarget.Size * 0.5f / Main.GameZoomTarget);
            effect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(PixelationTarget, Main.screenLastPosition - Main.screenPosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.End();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            orig(self);
        }
    }
}
