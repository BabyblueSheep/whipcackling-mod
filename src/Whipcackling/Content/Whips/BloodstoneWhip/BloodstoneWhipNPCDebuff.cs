using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Whipcackling.Content.Whips.NuclearWhip;

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
        }
    }
}
