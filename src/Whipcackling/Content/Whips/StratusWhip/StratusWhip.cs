using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Rarities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Whipcackling.Content.Whips.NuclearWhip;

namespace Whipcackling.Content.Whips.StratusWhip
{
    public class StratusWhip : ModItem
    {
        public override string LocalizationCategory => "Whips.StratusWhip";

        public override void SetDefaults()
        {
            Item.DefaultToWhip(projectileId: ModContent.ProjectileType<StratusWhipProjectile>(), dmg: ConstantsStratus.ItemDamage, kb: ConstantsStratus.ItemKnockback, shootspeed: ConstantsStratus.ItemRange, animationTotalTime: ConstantsStratus.ItemUseTime);
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.UseSound = SoundID.Item152;

            Item.autoReuse = true;

        }

        public override bool MeleePrefix()
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DeathSickle)
                .AddIngredient<Lumenyl>(5)
                .AddIngredient<RuinousSoul>(6)
                .AddIngredient<ExodiumCluster>(10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
