using CalamityMod;
using CalamityMod.Items;
using CalamityMod.NPCs.Crags;
using CalamityMod.Schematics;
using CalamityMod.World;
using MonoMod.RuntimeDetour;
using System;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Whipcackling.Common.Systems.Drawing;

namespace Whipcackling.Content.Accessories.Summoner
{
    public class IdolOfVitriol : ModItem
    {
        public override string LocalizationCategory => "Accessories";

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<IdolOfVitriolPlayer>().IdolOfVitriol = true;
            player.lavaMax += 180;
        }
    }

    public class IdolOfVitriolPlayer : ModPlayer
    {
        public bool IdolOfVitriol { get; set; }

        public override void ResetEffects()
        {
            IdolOfVitriol = false;
        }
    }

    public class IdolOfVitriolProjectile : GlobalProjectile
    {
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!(projectile.minion || ProjectileID.Sets.MinionShot[projectile.type] || projectile.sentry || ProjectileID.Sets.SentryShot[projectile.type]))
                return;
            if (Main.player[projectile.owner].GetModPlayer<IdolOfVitriolPlayer>().IdolOfVitriol)
            {
                projectile.localNPCImmunity[target.whoAmI] = (int)Math.Ceiling(projectile.localNPCHitCooldown * 0.9f);
                Projectile.perIDStaticNPCImmunity[projectile.type][target.whoAmI] = Main.GameUpdateCount + (uint)Math.Ceiling(projectile.idStaticNPCHitCooldown * 0.9f);
                if (Main.rand.Next(10) == 0)
                {
                    target.AddBuff(BuffID.OnFire, 60);
                }
                projectile.netUpdate = true;
            }
        }
    }

    public class IdolOfVitriolChest : ModSystem
    {
        private static Hook? _fillBrimstoneChests;

        public override void Load()
        {
            _fillBrimstoneChests = new(typeof(BrimstoneCrag).GetMethod("FillBrimstoneChests", BindingFlags.Public | BindingFlags.Static)!, AddIdolOfVitriol);
        }

        public override void Unload()
        {
            _fillBrimstoneChests?.Undo();
        }

        private void AddIdolOfVitriol(orig_FillBrimstoneChests orig, Chest chest, int Type, bool firstItem)
        {
            orig(chest, Type, firstItem);
            if (!firstItem)
            {
                ChestItem item = new(ModContent.ItemType<IdolOfVitriol>(), 1);

                chest.item[1].SetDefaults(item.Type);
                chest.item[1].stack = item.Stack;
            }
        }

        private delegate void orig_FillBrimstoneChests(Chest chest, int Type, bool firstItem);
    }

    public class IdolOfVitriolLoot : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == ModContent.NPCType<DespairStone>() || npc.type == ModContent.NPCType<Scryllar>() || npc.type == ModContent.NPCType<HeatSpirit>())
            {
                npcLoot.Add(ModContent.ItemType<IdolOfVitriol>(), 20);
            }
        }
    }
}
