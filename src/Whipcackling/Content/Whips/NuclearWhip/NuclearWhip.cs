using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.Rarities;
using CalamityMod.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Whipcackling.Common;
using Whipcackling.Content.Rebalances;
using Whipcackling.Content.Whips.MeldWhip;
using static CalamityMod.Items.CalamityGlobalItem;

namespace Whipcackling.Content.Whips.NuclearWhip
{
    public class NuclearWhip : ModItem
    {
        public override string LocalizationCategory => "Whips.NuclearWhip";

        public override void SetDefaults()
        {
            Item.DefaultToWhip(projectileId: ModContent.ProjectileType<NuclearWhipProjectile>(), dmg: ConstantsNuclear.ItemDamage, kb: ConstantsNuclear.ItemKnockback, shootspeed: ConstantsNuclear.ItemRange, animationTotalTime: ConstantsNuclear.ItemUseTime);
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.UseSound = SoundID.Item152;

            Item.autoReuse = true;

        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }

    public class NuclearTerrorLoot : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == ModContent.NPCType<NuclearTerror>())
            {
                npcLoot.Add(ModContent.ItemType<NuclearWhip>(), 3);
            }
        }
    }
}
