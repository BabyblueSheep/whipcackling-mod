using CalamityMod;
using CalamityMod.Cooldowns;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader;
using Whipcackling.Assets;

namespace Whipcackling.Content.Whips.MeldWhip
{
    public class MeldWhipCooldown : CooldownHandler
    {
        public static new string ID => "MeldWhipCooldown";

        public override bool ShouldDisplay => true;

        public override LocalizedText DisplayName => Language.GetOrRegister($"Mods.Whipcackling.Whips.MeldWhip.{ID}");

        public override string Texture => $"{AssetDirectory.AssetPath}Textures/Whips/MeldWhip/MeldWhipCooldown";

        public override Color OutlineColor => new Color(156, 255, 238);
        public override Color CooldownStartColor => Color.Lerp(new Color(24, 64, 79), new Color(117, 255, 159), 1 - instance.Completion);
        public override Color CooldownEndColor => Color.Lerp(new Color(24, 64, 79), new Color(117, 255, 159), 1 - instance.Completion);
    }
}
