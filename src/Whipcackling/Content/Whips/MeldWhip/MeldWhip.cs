using CalamityMod.Items;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Whipcackling.Content.Whips.MeldWhip
{
    public class MeldWhip : ModItem
    {
        public override string LocalizationCategory => "Whips.MeldWhip";

        public override void SetDefaults()
        {
            Item.DefaultToWhip(projectileId: ModContent.ProjectileType<MeldWhipProjectile>(), dmg: ConstantsMeld.ItemDamage, kb: ConstantsMeld.ItemKnockback, shootspeed: ConstantsMeld.ItemRange, animationTotalTime: ConstantsMeld.ItemUseTime);
            Item.rare = ItemRarityID.Cyan;
            Item.value = CalamityGlobalItem.RarityCyanBuyPrice;
            Item.UseSound = SoundID.Item152;

            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MeldConstruct>(10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }
}
