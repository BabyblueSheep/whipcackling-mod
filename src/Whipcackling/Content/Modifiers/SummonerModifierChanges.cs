using CalamityMod;
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
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI;

namespace Whipcackling.Content.Modifiers
{
    public class SummonModifierItem : GlobalItem
    {
        public float TagDamageMultiplier { get; set; } = 1f;
        public float SizeMultiplier { get; set; } = 1f;
        public float MinionSpeedMultiplier { get; set; } = 0f;
        public float MinionCriticalChance { get; set; } = 0f;
        public float SlotsAmountMultiplier { get; set; } = 1f;

        public override bool InstancePerEntity => true;
    }

    public class SummonModifierProjectile : GlobalProjectile
    {
        public float TagDamageMultiplier { get; set; } = 1f;
        public float SizeMultiplier { get; set; } = 1f;
        public float MinionSpeedMultiplier { get; set; } = 0f;
        public float MinionCriticalChance { get; set; } = 0f;
        public float SlotsAmountMultiplier { get; set; } = 1f;

        public int OriginalWidth { get; set; }
        public int OriginalHeight { get; set; }

        public override bool InstancePerEntity => true;

        public override void Load()
        {
            On_Player.FreeUpPetsAndMinions += PrefixSmallerMinions;
            IL_Projectile.Damage += PrefixLessTag;
        }

        public override void Unload()
        {
            On_Player.FreeUpPetsAndMinions -= PrefixSmallerMinions;
            IL_Projectile.Damage -= PrefixLessTag;
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            OriginalWidth = projectile.width;
            OriginalHeight = projectile.height;
            if (projectile.minion || projectile.sentry)
            {
                Player player = Main.player[projectile.owner];
                if (Main.gameMenu || !player.active)
                    return;
                Item item = player.ActiveItem();
                if (source is not EntitySource_ItemUse || item.accessory)
                    return;
                if (item.prefix == 0)
                    return;

                SummonModifierItem modItem = item.GetGlobalItem<SummonModifierItem>();

                TagDamageMultiplier = modItem.TagDamageMultiplier;
                SizeMultiplier = modItem.SizeMultiplier;
                MinionSpeedMultiplier = modItem.MinionSpeedMultiplier;
                MinionCriticalChance = modItem.MinionCriticalChance;
                SlotsAmountMultiplier = modItem.SlotsAmountMultiplier;

                projectile.width = (int)Math.Ceiling(projectile.width * SizeMultiplier);
                projectile.height = (int)Math.Ceiling(projectile.height * SizeMultiplier);
                projectile.scale = SizeMultiplier;

                projectile.minionSlots *= SlotsAmountMultiplier;

                projectile.netUpdate = true;
            }
            else if (ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type])
            {
                if (source is EntitySource_Parent parentSource && parentSource.Entity is Projectile proj && (proj.minion || proj.sentry))
                {
                    SummonModifierProjectile parentModProjectile = proj.GetGlobalProjectile<SummonModifierProjectile>();

                    TagDamageMultiplier = parentModProjectile.TagDamageMultiplier;
                    SizeMultiplier = parentModProjectile.SizeMultiplier;
                    MinionSpeedMultiplier = parentModProjectile.MinionSpeedMultiplier;
                    MinionCriticalChance = parentModProjectile.MinionCriticalChance;
                    SlotsAmountMultiplier = parentModProjectile.SlotsAmountMultiplier;

                    projectile.width = (int)Math.Ceiling(projectile.width * SizeMultiplier);
                    projectile.height = (int)Math.Ceiling(projectile.height * SizeMultiplier);
                    projectile.scale = SizeMultiplier;

                    projectile.netUpdate = true;
                }
            }
        }

        public override void PostAI(Projectile projectile)
        {
            if (!(projectile.minion || projectile.sentry || ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type]))
                return;

            projectile.position.X += projectile.velocity.X * MinionSpeedMultiplier;
            if (!projectile.tileCollide || projectile.velocity.Y < 0 || projectile.shouldFallThrough)
                projectile.position.Y += projectile.velocity.Y * MinionSpeedMultiplier;

            if (projectile.width != Math.Ceiling(OriginalWidth * SizeMultiplier))
            {
                OriginalWidth = projectile.width;
                projectile.width = (int)Math.Ceiling(projectile.width * SizeMultiplier);
            }

            if (projectile.height != Math.Ceiling(OriginalHeight * SizeMultiplier))
            {
                OriginalHeight = projectile.height;
                projectile.height = (int)Math.Ceiling(projectile.height * SizeMultiplier);
            }
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!(projectile.minion || projectile.sentry || ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type]))
                return;
            if (MinionCriticalChance <= 0f)
                return;
            if (Main.rand.NextFloat() < MinionCriticalChance)
            {
                modifiers.SetCrit();
            }
        }

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(TagDamageMultiplier);
            binaryWriter.Write(SizeMultiplier);
            binaryWriter.Write(MinionSpeedMultiplier);
            binaryWriter.Write(MinionCriticalChance);
            binaryWriter.Write(SlotsAmountMultiplier);
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            TagDamageMultiplier = binaryReader.ReadSingle();
            SizeMultiplier = binaryReader.ReadSingle();
            MinionSpeedMultiplier = binaryReader.ReadSingle();
            MinionCriticalChance = binaryReader.ReadSingle();
            SlotsAmountMultiplier = binaryReader.ReadSingle();
        }


        private void PrefixLessTag(ILContext il)
        {
            ILCursor? cursor = new(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdflda(typeof(NPC.HitModifiers), "FlatBonusDamage")))
                return;
            cursor.Index -= 4;
            cursor.EmitLdarg0();
            cursor.EmitDelegate((float flatBonus, Projectile projectile) =>
            {
                return flatBonus * projectile.GetGlobalProjectile<SummonModifierProjectile>().TagDamageMultiplier;
            });
        }

        private void PrefixSmallerMinions(On_Player.orig_FreeUpPetsAndMinions orig, Player self, Item sItem)
        {
            float originalSlotsAmount = ItemID.Sets.StaffMinionSlotsRequired[sItem.type];
            ItemID.Sets.StaffMinionSlotsRequired[sItem.type] *= sItem.GetGlobalItem<SummonModifierItem>().SlotsAmountMultiplier;
            orig(self, sItem);

            ItemID.Sets.StaffMinionSlotsRequired[sItem.type] = originalSlotsAmount;
        }
    }
}
