using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;
using Whipcackling.Common.Configs;

namespace Whipcackling.Content.Rebalances.Minions
{
    public class RustyBeaconPrototypePulse : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ModContent.ProjectileType<RustyBeaconPulse>() && lateInstantiation;

        public override void SetDefaults(Projectile entity)
        {
            if (WhipcacklingConfig.Instance.BalanceMode == BalanceMode.Whipcackling)
            {
                entity.usesIDStaticNPCImmunity = false;
                entity.idStaticNPCHitCooldown = -2;
                entity.usesLocalNPCImmunity = true;
                entity.localNPCHitCooldown = 30;
            }
        }
    }
}
