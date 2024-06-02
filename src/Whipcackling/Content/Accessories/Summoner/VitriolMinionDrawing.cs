using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Whipcackling.Assets;
using Whipcackling.Common.Systems.Drawing;
using Whipcackling.Content.Enchantments;

namespace Whipcackling.Content.Accessories.Summoner
{
    public class VitriolMinionDrawing : MinionDrawingSystem
    {
        public override float Size => 1.1f;

        public override bool AppliesTo(Projectile projectile)
        {
            return projectile.GetGlobalProjectile<IdolOfVitriolProjectile>().IsVitriolic;
        }

        public override void DrawEffectsBehind(int layer, bool startSpriteBatch)
        {
            Effect effect = AssetDirectory.Effects.FieryOutline.Value;
            effect.Parameters["uAmount"].SetValue(0.7f);
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);

            if (!startSpriteBatch)
            {
                Main.spriteBatch.End();
            }
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            effect.Parameters["uSpeed"].SetValue(2);
            effect.Parameters["uColor"].SetValue(new Vector4(1, 0.4f, 0.4f, 0.5f));
            effect.Parameters["uAmplitude"].SetValue(0.001f);
            effect.Parameters["uFrequency"].SetValue(150);
            effect.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(RenderTargets[layer], Main.screenLastPosition - Main.screenPosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            effect.Parameters["uSpeed"].SetValue(1);
            effect.Parameters["uColor"].SetValue(new Vector4(0.7f, 0.1f, 0, 0.5f));
            effect.Parameters["uAmplitude"].SetValue(0.005f);
            effect.Parameters["uFrequency"].SetValue(200);
            effect.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(RenderTargets[layer], Main.screenLastPosition - Main.screenPosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            if (!startSpriteBatch)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

        public override void DrawEffectsAfter(int layer, bool startSpriteBatch)
        {

        }
    }
}
