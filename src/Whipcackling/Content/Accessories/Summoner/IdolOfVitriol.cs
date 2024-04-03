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

        public override void PostUpdateBuffs()
        {
            IdolOfVitriol = false; //I HATE UPDATE ORDERS
        }
    }

    public class IdolOfVitriolProjectile : GlobalProjectile
    {
        public bool IsVitriolic { get; set; }

        public override bool InstancePerEntity => true;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.minion && !projectile.sentry)
            {
                Player player = Main.player[projectile.owner];
                if (Main.gameMenu || !player.active)
                    return;
                if (player.GetModPlayer<IdolOfVitriolPlayer>().IdolOfVitriol)
                {
                    IsVitriolic = true;
                    projectile.localNPCHitCooldown = (int)Math.Floor(projectile.localNPCHitCooldown * 0.9f);
                    projectile.idStaticNPCHitCooldown = (int)Math.Floor(projectile.idStaticNPCHitCooldown * 0.9f);
                }
            }
            else if (ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type])
            {
                if (source is EntitySource_Parent parentSource && parentSource.Entity is Projectile proj && (proj.minion || proj.sentry) && proj.GetGlobalProjectile<IdolOfVitriolProjectile>().IsVitriolic)
                {
                    IsVitriolic = true;
                    projectile.localNPCHitCooldown = (int)Math.Floor(projectile.localNPCHitCooldown * 0.9f);
                    projectile.idStaticNPCHitCooldown = (int)Math.Floor(projectile.idStaticNPCHitCooldown * 0.9f);
                }
            }
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!(projectile.minion || ProjectileID.Sets.MinionShot[projectile.type] || projectile.sentry || ProjectileID.Sets.SentryShot[projectile.type]))
                return;
            if (IsVitriolic)
            {
                if (Main.rand.Next(10) == 0)
                {
                    target.AddBuff(BuffID.OnFire, 60);
                }
            }
        }

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(IsVitriolic);
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            IsVitriolic = binaryReader.ReadBoolean();
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
