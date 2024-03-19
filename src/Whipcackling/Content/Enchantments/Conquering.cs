using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Whipcackling.Assets;

namespace Whipcackling.Content.Enchantments
{
    public class Conquering : ModPlayer
    {
        public override void Load()
        {
            ModLoader.GetMod("CalamityMod").Call(
                "CreateEnchantment",
                Language.GetOrRegister($"Mods.Whipcackling.Enchantments.Conquering.Name"),
                Language.GetOrRegister($"Mods.Whipcackling.Enchantments.Conquering.Description"),
                190,
                new Predicate<Item>((Item item) => item.IsEnchantable() && item.damage > 0 && item.CountsAsClass<SummonDamageClass>() && !item.IsWhip() && !item.sentry),
                $"{AssetDirectory.AssetPath}Textures/Enchantments/Conquering"
            );
        }

        public override void PostUpdateMiscEffects()
        {
            if (Player != Main.LocalPlayer)
                return;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (!projectile.active)
                    continue;
                if (!projectile.minion)
                    continue;
                if (!projectile.GetGlobalProjectile<ConqueringMinion>().IsDivided)
                    continue;
                Player owner = Main.player[projectile.owner];
                owner.statLifeMax2 -= 5;
                owner.statManaMax2 -= 5;
            }
        }
    }

    public class ConqueringMinion : GlobalProjectile
    {
        public bool IsDivided;

        public override bool InstancePerEntity => true;

        public override void Load()
        {
            IL_Player.FreeUpPetsAndMinions += ConqueringSmallerMinions;
            IL_Projectile.Damage += ConqueringLessTag;
        }

        public override void Unload()
        {
            IL_Player.FreeUpPetsAndMinions -= ConqueringSmallerMinions;
            IL_Projectile.Damage -= ConqueringLessTag;
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (!projectile.minion && !projectile.sentry)
                return;
            Player player = Main.player[projectile.owner];
            if (Main.gameMenu || !player.active)
                return;
            if (source is not EntitySource_ItemUse || player.ActiveItem().accessory)
                return;
            if (player.ActiveItem().Calamity().AppliedEnchantment is null)
                return;

            if (player.ActiveItem().Calamity().AppliedEnchantment.Value.ID == 190)
            {
                projectile.minionSlots /= 3f;
                player.statLife -= 5;
                player.statMana -= 5;
                if (player.statLife <= 0)
                {
                    player.KillMe(PlayerDeathReason.ByCustomReason(Language.GetOrRegister($"Mods.Whipcackling.Enchantments.Conquering.DeathReason").Format(player.name)), 5, 0, false);
                }
                IsDivided = true;
            }
        }

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(IsDivided);
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            IsDivided = binaryReader.ReadBoolean();
        }

        public override bool PreAI(Projectile projectile)
        {
            if (IsDivided)
            {
                projectile.damage = (int)Math.Ceiling(projectile.originalDamage * 0.25f);
            }
            return true;
        }

        private void ConqueringSmallerMinions(ILContext il)
        {
            ILCursor? cursor = new(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdsfld(typeof(ItemID.Sets), "StaffMinionSlotsRequired")))
                return;
            cursor.Index += 3;
            Mod.Logger.Info(cursor.Next);
            cursor.EmitLdarg1();
            cursor.EmitDelegate((float slots, Item item) => {
                if (item.Calamity().AppliedEnchantment is null)
                    return slots;
                float modifier = item.Calamity().AppliedEnchantment.Value.ID == 190 ? 3f : 1;
                return slots / modifier;
            });
        }

        private void ConqueringLessTag(ILContext il)
        {
            ILCursor? cursor = new(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdflda(typeof(NPC.HitModifiers), "FlatBonusDamage")))
                return;
            cursor.Index -= 4;
            cursor.EmitLdarg0();
            cursor.EmitDelegate((float flatBonus, Projectile projectile) => 
            {
                if (projectile.GetGlobalProjectile<ConqueringMinion>().IsDivided)
                    return flatBonus * 0.5f;
                return flatBonus;
            });
        }
    }
}
