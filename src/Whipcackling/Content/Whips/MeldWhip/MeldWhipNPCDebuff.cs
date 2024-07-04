using CalamityMod.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Whipcackling.Assets;

namespace Whipcackling.Content.Whips.MeldWhip
{
    public class MeldWhipNPCDebuff : ModBuff
    {
        public override string LocalizationCategory => "Whips.MeldWhip";

        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
            BuffID.Sets.CanBeRemovedByNetMessage[Type] = true;
        }
    }

    public class MeldWhipNPCDebuffSuper : ModBuff
    {
        public override string LocalizationCategory => "Whips.MeldWhip";

        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
            BuffID.Sets.CanBeRemovedByNetMessage[Type] = true;
        }
    }

    public class MeldWhipNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (Main.myPlayer != Main.player[projectile.owner].whoAmI)
                return;
            if (!(projectile.minion || ProjectileID.Sets.MinionShot[projectile.type] || projectile.sentry || ProjectileID.Sets.SentryShot[projectile.type]))
                return;
            float projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
            if (npc.HasBuff<MeldWhipNPCDebuff>())
            {
                npc.RequestBuffRemoval(ModContent.BuffType<MeldWhipNPCDebuff>());

                int proj = Projectile.NewProjectile(projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<MeldExplosion>(), (int)(projectile.damage * ConstantsMeld.TagDamage * projTagMultiplier), 0f, ai0: 1);
            }
            else if (npc.HasBuff<MeldWhipNPCDebuffSuper>())
            {
                npc.RequestBuffRemoval(ModContent.BuffType<MeldWhipNPCDebuffSuper>());

                int proj = Projectile.NewProjectile(projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<MeldExplosion>(), (int)(projectile.damage * ConstantsMeld.TagDamage * projTagMultiplier), 0f, ai0: 3, ai1: 1);
            }
        }


        public static ManagedRenderTarget[] RenderTargets { get; private set; }
        private static bool[] _shouldDrawTargets; 


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
            On_Main.DrawCachedNPCs += DrawCachedOutlines;
            On_Main.DrawNPCs += DrawOutlines;
        }

        public override void Unload()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            RenderTargetManager.RenderTargetUpdateLoopEvent -= PrepareRenderTarget;
            On_Main.DrawCachedNPCs -= DrawCachedOutlines;
            On_Main.DrawNPCs -= DrawOutlines;
        }

        private int DetermineDrawLayer(int i)
        {
            if (Main.instance.DrawCacheNPCsMoonMoon.Contains(i))
                return 0;
            if (Main.instance.DrawCacheNPCsBehindNonSolidTiles.Contains(i))
                return 1;
            if (Main.instance.DrawCacheNPCProjectiles.Contains(i))
                return 4;
            if (Main.instance.DrawCacheNPCsOverPlayers.Contains(i))
                return 5;
            return -1;
        }

        private int DetermineDrawLayer(List<int> list)
        {
            if (list == Main.instance.DrawCacheNPCsMoonMoon)
                return 0;
            if (list == Main.instance.DrawCacheNPCsBehindNonSolidTiles)
                return 1;
            if (list == Main.instance.DrawCacheNPCProjectiles)
                return 4;
            if (list == Main.instance.DrawCacheNPCsOverPlayers)
                return 5;
            return 3;
        }

        private void PrepareRenderTarget()
        {
            bool shouldDraw = false;
            _shouldDrawTargets ??= new bool[6];
            for (int i = 0; i < 6; i++)
                _shouldDrawTargets[i] = false;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active)
                    continue;
                if (!npc.HasBuff<MeldWhipNPCDebuffSuper>())
                    continue;


                int layer = DetermineDrawLayer(i);
                if (layer == -1)
                    layer = npc.behindTiles ? 2 : 3;
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
            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active)
                    continue;
                if (!npc.HasBuff<MeldWhipNPCDebuffSuper>())
                    continue;

                int currentLayer = DetermineDrawLayer(i);
                if (currentLayer == -1)
                    currentLayer = npc.behindTiles ? 2 : 3;

                if (currentLayer != pastLayer)
                {
                    pastLayer = currentLayer;
                    device.SetRenderTarget(RenderTargets[pastLayer]);
                }

                float size = 1.05f;
                Vector2 offset = npc.position - Main.screenPosition;
                offset *= size - 1;

                Matrix matrix = Matrix.CreateScale(size, size, 1);
                matrix *= Matrix.CreateTranslation(-offset.X, -offset.Y, 0);
                matrix *= Matrix.CreateTranslation(-npc.width * (size - 1) / 2, -npc.height * (size - 1) / 2, 0);
                Main.GameViewMatrix._transformationMatrix = matrix;
                Main.GameViewMatrix._zoom = new Vector2(1, 1);

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                Main.instance.DrawNPCDirect(Main.spriteBatch, npc, false, Main.screenPosition);
                Main.spriteBatch.End();
            }
            Main.GameViewMatrix._transformationMatrix = matrixTransformation;
            Main.GameViewMatrix._zoom = zoom;
            device.SetRenderTarget(null);
        }

        private void DrawOutlines(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            int layer = behindTiles ? 2 : 3;
            if (!_shouldDrawTargets[layer])
            {
                orig(self, behindTiles);
                return;
            }

            DrawOutlinesDirect(layer);

            orig(self, behindTiles);
        }

        private void DrawCachedOutlines(On_Main.orig_DrawCachedNPCs orig, Main self, List<int> npcCache, bool behindTiles)
        {
            int layer = DetermineDrawLayer(npcCache);
            if (!_shouldDrawTargets[layer])
            {
                orig(self, npcCache, behindTiles);
                return;
            }

            DrawOutlinesDirect(layer);

            orig(self, npcCache, behindTiles);
        }

        public void DrawOutlinesDirect(int layer)
        {
            Effect effect = AssetDirectory.Effects.MeldOutline.Value;
            effect.Parameters["uAmount"].SetValue(1);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (float i = 0; i < 3; i++)
            {
                Color color = i switch
                {
                    0 => Color.White,
                    1 => Color.SpringGreen,
                    _ => Color.Cyan,
                };
                effect.Parameters["uColor"].SetValue(color.ToVector4());
                effect.CurrentTechnique.Passes[0].Apply();

                float angle = Main.GlobalTimeWrappedHourly * 5 + i / 3 * MathHelper.TwoPi;
                Vector2 offset = new(MathF.Cos(angle), MathF.Sin(angle));
                Main.spriteBatch.Draw(RenderTargets[layer], Main.screenLastPosition - Main.screenPosition + offset * 2, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }
    }
}
