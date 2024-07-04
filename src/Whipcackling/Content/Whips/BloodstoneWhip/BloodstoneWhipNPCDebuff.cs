using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Whipcackling.Content.Accessories.Summoner.MoonStone;

namespace Whipcackling.Content.Whips.BloodstoneWhip
{
    public class BloodstoneWhipNPCDebuff : ModBuff
    {
        public override string LocalizationCategory => "Whips.BloodstoneWhip";

        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
            BuffID.Sets.CanBeRemovedByNetMessage[Type] = true;
        }
    }

    public class BloodstoneWhipNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (Main.myPlayer != Main.player[projectile.owner].whoAmI)
                return;
            if (!(projectile.minion || ProjectileID.Sets.MinionShot[projectile.type] || projectile.sentry || ProjectileID.Sets.SentryShot[projectile.type]))
                return;
            if (!npc.HasBuff<BloodstoneWhipNPCDebuff>())
                return;

            float projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
            modifiers.FlatBonusDamage += ConstantsBloodstone.TagDamage * projTagMultiplier;

            if (Main.rand.Next(100) < ConstantsBloodstone.TagCritChance)
            {
                modifiers.SetCrit();
            }

            BloodstoneWhipPlayer modPlayer = Main.player[projectile.owner].GetModPlayer<BloodstoneWhipPlayer>();
            float multiplier = modPlayer.IsAwakened ? 0.4f : 1f;
            modPlayer.BloodCharge += 0.005f * projTagMultiplier * multiplier;
        }
    }
}
