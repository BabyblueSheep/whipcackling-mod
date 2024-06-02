using CalamityMod.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Whipcackling.Assets;
using Whipcackling.Common.Systems.Drawing;

namespace Whipcackling.Content.Enchantments
{
    public class ConqueringMinionDrawing : MinionDrawingSystem
    {
        public override float Size => 1.2f;
        public override bool AppliesTo(Projectile projectile) => projectile.GetGlobalProjectile<ConqueringMinion>().IsDivided;

        public override void DrawEffectsAfter(int layer, bool startSpriteBatch)
        {
            Effect effect = AssetDirectory.Effects.CalamitousOutline.Value;
            effect.Parameters["uAmount"].SetValue(0.7f);
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);

            if (!startSpriteBatch)
            {
                Main.spriteBatch.End();
            }
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            effect.Parameters["uSpeed"].SetValue(2);
            effect.Parameters["uColor"].SetValue(new Vector4(0.1f, 0, 0, 0.1f));
            effect.Parameters["uAmplitude"].SetValue(0.001f);
            effect.Parameters["uFrequency"].SetValue(5);
            effect.Parameters["uGamerChange"].SetValue(0.000f);
            effect.Parameters["uTriangle"].SetValue(0f);
            effect.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(RenderTargets[layer], Main.screenLastPosition - Main.screenPosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            effect.Parameters["uSpeed"].SetValue(1);
            effect.Parameters["uColor"].SetValue(new Vector4(0.9f, 0.6f, 0, 0.01f));
            effect.Parameters["uAmplitude"].SetValue(-0.002f);
            effect.Parameters["uFrequency"].SetValue(5);
            effect.Parameters["uGamerChange"].SetValue(-0.0005f);
            effect.Parameters["uTriangle"].SetValue(1f);
            effect.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(RenderTargets[layer], Main.screenLastPosition - Main.screenPosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            if (!startSpriteBatch)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

        public override void DrawEffectsBehind(int layer, bool startSpriteBatch)
        {
            
        }
    }
}
