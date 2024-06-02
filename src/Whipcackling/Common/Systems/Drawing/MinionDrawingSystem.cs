using CalamityMod.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Whipcackling.Assets;
using Whipcackling.Content.Enchantments;

namespace Whipcackling.Common.Systems.Drawing
{
    public abstract class MinionDrawingSystem : ModSystem
    {
        public ManagedRenderTarget[] RenderTargets { get; private set; }
        private bool[] _shouldDrawTargets;

        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            Main.QueueMainThreadAction(() =>
            {
                RenderTargets = new ManagedRenderTarget[6];
                for (int i = 0; i < 6; i++)
                {
                    RenderTargets[i] = new(
                        true,
                        (int screenWidth, int screenHeight) => new(Main.instance.GraphicsDevice, screenWidth, screenHeight, true, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents)
                        );
                }
            });
            RenderTargetManager.RenderTargetUpdateLoopEvent += PrepareRenderTarget;
            On_Main.DrawProjectiles += DrawEffects;
            On_Main.DrawCachedProjs += DrawCachedEffects;
        }

        public override void Unload()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            RenderTargetManager.RenderTargetUpdateLoopEvent -= PrepareRenderTarget;
            On_Main.DrawProjectiles -= DrawEffects;
            On_Main.DrawCachedProjs -= DrawCachedEffects;
        }

        public static int DetermineDrawLayer(int i)
        {
            if (Main.instance.DrawCacheProjsBehindNPCsAndTiles.Contains(i))
                return 0;
            if (Main.instance.DrawCacheProjsBehindNPCs.Contains(i))
                return 1;
            if (Main.instance.DrawCacheProjsBehindProjectiles.Contains(i))
                return 2;
            if (Main.instance.DrawCacheProjsOverPlayers.Contains(i))
                return 4;
            if (Main.instance.DrawCacheProjsOverWiresUI.Contains(i))
                return 5;
            return 3;
        }

        public static int DetermineDrawLayer(List<int> list)
        {
            if (list == Main.instance.DrawCacheProjsBehindNPCsAndTiles)
                return 0;
            if (list == Main.instance.DrawCacheProjsBehindNPCs)
                return 1;
            if (list == Main.instance.DrawCacheProjsBehindProjectiles)
                return 2;
            if (list == Main.instance.DrawCacheProjsOverPlayers)
                return 4;
            if (list == Main.instance.DrawCacheProjsOverWiresUI)
                return 5;
            return 3;
        }

        private void PrepareRenderTarget()
        {
            bool shouldDraw = false;
            _shouldDrawTargets ??= new bool[6];
            for (int i = 0; i < 6; i++)
                _shouldDrawTargets[i] = false;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (!projectile.active)
                    continue;
                if (!AppliesTo(projectile))
                    continue;
                int layer = DetermineDrawLayer(i);
                if (projectile.hide && layer == 3)
                    continue;

                _shouldDrawTargets[layer] = true;
                shouldDraw = true;
            }

            if (!shouldDraw)
                return;

            GraphicsDevice device = Main.instance.GraphicsDevice;
            for (int i = 0; i < RenderTargets.Length; i++)
            {
                device.SetRenderTarget(RenderTargets[i]);
                device.Clear(Color.Transparent);
            }

            int pastLayer = -1;
            Matrix matrixTransformation = Main.GameViewMatrix.TransformationMatrix;
            Vector2 zoom = Main.GameViewMatrix.Zoom;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (!projectile.active)
                    continue;
                if (!AppliesTo(projectile))
                    continue;

                int currentLayer = DetermineDrawLayer(i);
                if (projectile.hide && currentLayer == 3)
                    continue;

                if (currentLayer != pastLayer)
                {
                    pastLayer = currentLayer;
                    device.SetRenderTarget(RenderTargets[pastLayer]);
                }

                float size = Size;
                Vector2 offset = projectile.position - Main.screenPosition;
                offset *= size - 1;

                Matrix matrix = Matrix.CreateScale(size, size, 1);
                matrix *= Matrix.CreateTranslation(-offset.X, -offset.Y, 0);
                matrix *= Matrix.CreateTranslation(-projectile.width * (size - 1) / 2, -projectile.height * (size - 1) / 2, 0);
                Main.GameViewMatrix._transformationMatrix = matrix;
                Main.GameViewMatrix._zoom = new Vector2(1, 1);

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                Main.instance.DrawProjDirect(projectile);
                Main.spriteBatch.End();
            }
            Main.GameViewMatrix._transformationMatrix = matrixTransformation;
            Main.GameViewMatrix._zoom = zoom;
            device.SetRenderTarget(null);
        }


        private void DrawCachedEffects(On_Main.orig_DrawCachedProjs orig, Main self, List<int> projCache, bool startSpriteBatch)
        {
            int layer = DetermineDrawLayer(projCache);
            if (!_shouldDrawTargets[layer])
            {
                orig(self, projCache, startSpriteBatch);
                return;
            }

            DrawEffectsBehind(layer, startSpriteBatch);
            orig(self, projCache, startSpriteBatch);
            DrawEffectsAfter(layer, startSpriteBatch);
        }

        private void DrawEffects(On_Main.orig_DrawProjectiles orig, Main self)
        {
            int layer = 3;
            if (!_shouldDrawTargets[layer])
            {
                orig(self);
                return;
            }

            DrawEffectsBehind(layer, true);
            orig(self);
            DrawEffectsAfter(layer, true);
        }

        public abstract float Size { get; }
        public abstract bool AppliesTo(Projectile projectile);

        public abstract void DrawEffectsBehind(int layer, bool startSpriteBatch);
        public abstract void DrawEffectsAfter(int layer, bool startSpriteBatch);
    }
}
