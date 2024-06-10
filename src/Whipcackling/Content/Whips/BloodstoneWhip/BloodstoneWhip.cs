using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Whipcackling.Content.Whips.NuclearWhip;

namespace Whipcackling.Content.Whips.BloodstoneWhip
{
    public class BloodstoneWhip : ModItem
    {
        public override string LocalizationCategory => "Whips.BloodstoneWhip";

        public override void SetDefaults()
        {
            Item.DefaultToWhip(projectileId: ModContent.ProjectileType<BloodstoneWhipProjectile>(), dmg: ConstantsBloodstone.ItemDamage, kb: ConstantsBloodstone.ItemKnockback, shootspeed: ConstantsBloodstone.ItemRange, animationTotalTime: ConstantsBloodstone.ItemUseTime);
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.UseSound = SoundID.Item152;

            Item.autoReuse = true;
        }

        public override bool MeleePrefix()
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodstoneCore>(6).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
