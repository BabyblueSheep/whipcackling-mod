using CalamityMod;
using CalamityMod.Items;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
