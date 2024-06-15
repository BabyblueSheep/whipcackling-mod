using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using static Terraria.Localization.NetworkText;

namespace Whipcackling.Core.Particles
{
    [Autoload(Side = ModSide.Client)]
    public class ParticleAtlasSystem : ModSystem
    {
        public static RenderTarget2D? Atlas;

        public static Dictionary<string, Vector4> AtlasDefinitions;

        public override void PostSetupContent()
        {
            AtlasDefinitions = [];

            Main.QueueMainThreadAction(() =>
            {
                const int DIMENSION = 512;
                Atlas = new RenderTarget2D(Main.graphics.GraphicsDevice, DIMENSION, DIMENSION);
                Main.graphics.GraphicsDevice.SetRenderTarget(Atlas);
                Main.graphics.GraphicsDevice.Clear(Color.Transparent);
                Vector2 offset = Vector2.Zero;

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
                foreach (string fullTexturePath in Mod.RootContentSource.EnumerateAssets().Where(t => t.Contains("Particles/")))
                {
                    string texturePath = Path.ChangeExtension(fullTexturePath, null);
                    string assetName = texturePath.Substring(texturePath.LastIndexOf("/") + 1);
                    Asset<Texture2D> textureAsset = ModContent.Request<Texture2D>($"Whipcackling/{texturePath}", AssetRequestMode.ImmediateLoad);
                    Texture2D texture = textureAsset.Value;
                    
                    if (offset.X + texture.Width > DIMENSION)
                    {
                        offset.X = 0;
                        offset.Y += texture.Height;
                    }
                    Main.spriteBatch.Draw(texture, offset, Color.White);
                    AtlasDefinitions.Add(assetName, new Vector4(offset.X, offset.Y, texture.Width, texture.Height));
                    offset.X += texture.Width;
                }
                Main.spriteBatch.End();
                Main.graphics.GraphicsDevice.SetRenderTarget(null);
            });
        }

        public override void Unload()
        {
            Atlas?.Dispose();
            Atlas = null;
            AtlasDefinitions.Clear();
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            Main.spriteBatch.Draw(Atlas, new Vector2(200), Color.White);
        }
    }
}
