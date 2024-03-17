using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content.Sources;
using ReLogic.Content;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Cooldowns;
using Whipcackling.Core.Particles;
using Whipcackling.Assets;
using Whipcackling.Common.Utilities;

namespace Whipcackling
{
    public class WhipcacklingMod : Mod
    {
        public override void Load()
        {
            CooldownRegistry.RegisterModCooldowns(this);
        }

        public override void Unload()
        {
            ParticleLoader.Unload();
        }

        public override void PostSetupContent()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            LoadShaders();
        }

        private void LoadShaders()
        {
            RegisterScreenFilter(AssetDirectory.Effects.Negazone);
        }

        private void RegisterScreenFilter(Asset<Effect> shader, EffectPriority priority = EffectPriority.High)
        {
            string name = shader.Name.Split('\\')[^1];
            Ref<Effect> effect = new(shader.Value);
            Filters.Scene[$"Whipcackling:{name}"] = new Filter(new(effect, $"{name}Pass"), priority);
            Filters.Scene[$"Whipcackling:{name}"].Load();
        }

        public override IContentSource CreateDefaultContentSource()
        {
            SmartContentSource source = new(base.CreateDefaultContentSource());
            source.AddDirectoryRedirect("Content", "Assets/Textures");
            source.AddDirectoryRedirect("Common", "Assets/Textures");
            return source;
        }
    }
}
