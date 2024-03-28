using CalamityMod;
using CalamityMod.Items;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.Projectiles.Summon;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Whipcackling.Assets;
using Whipcackling.Common.Systems.Drawing;
using Whipcackling.Common.Utilities;
using Whipcackling.Content.Whips;
using Whipcackling.Content.Whips.NuclearWhip;
using Whipcackling.Core;

namespace Whipcackling.Content.Accessories.Summoner
{
    public class MartianDataglove : ModItem
    {
        public override string LocalizationCategory => "Accessories";

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 40;

            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.whipRangeMultiplier += 0.1f;
            player.summonerWeaponSpeedBonus += 0.1f;
            player.GetModPlayer<MartianDataglovePlayer>().MartianDataglove = true;

            player.GetDamage<SummonDamageClass>() += 0.05f * (player.maxMinions + player.maxTurrets);
            player.GetCritChance<SummonMeleeSpeedDamageClass>() += 0.01f * (player.maxMinions + player.maxTurrets);
            player.maxMinions = 0;
            player.maxTurrets = 0;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.IntegrateHotkey(MartianDataglovePlayer.ExpandTooltip);
            if (!MartianDataglovePlayer.ExpandTooltip.Current)
                return;
            tooltips.Add(new TooltipLine(
                Mod,
                "Whipcackling:VanillaTagDebuffs",
                Language.GetOrRegister($"Mods.Whipcackling.Accessories.MartianDataglove.TooltipVanillaTags").ToString()
                ));
        }
    }

    public class MartianDataglovePlayer : ModPlayer
    {
        public static ModKeybind ExpandTooltip { get; private set; }

        public bool MartianDataglove { get; set; }

        public override void Load()
        {
            ExpandTooltip = KeybindLoader.RegisterKeybind(Mod, "ExpandTooltip", "LeftShift");

            On_Player.GetTotalDamage += MakeWhipDamageAffectedByMelee;
        }

        public override void Unload()
        {
            ExpandTooltip = null;

            On_Player.GetTotalDamage -= MakeWhipDamageAffectedByMelee;
        }

        public override void ResetEffects()
        {
            MartianDataglove = false;
        }

        private StatModifier MakeWhipDamageAffectedByMelee(On_Player.orig_GetTotalDamage orig, Player self, DamageClass damageClass)
        {
            StatModifier stats = orig(self, damageClass);
            if (damageClass.Type != DamageClass.SummonMeleeSpeed.Type || !self.GetModPlayer<MartianDataglovePlayer>().MartianDataglove)
                return stats;
            stats = stats.CombineWith(self.damageData[DamageClass.Melee.Type].damage.Scale(0.15f));
            return stats;
        }
    }

    public class MartianDatagloveLoot : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.BrainScrambler || npc.type == NPCID.GigaZapper || npc.type == NPCID.MartianEngineer || npc.type == NPCID.MartianOfficer)
            {
                npcLoot.Add(ModContent.ItemType<MartianDataglove>(), 800);
            }
        }
    }
}