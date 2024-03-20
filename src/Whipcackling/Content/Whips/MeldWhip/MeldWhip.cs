using CalamityMod.Items;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Whipcackling.Common;

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
