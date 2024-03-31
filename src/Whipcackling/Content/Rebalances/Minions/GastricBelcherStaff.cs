using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;
using Whipcackling.Common.Configs;

namespace Whipcackling.Content.Rebalances.Minions
{
    public class GastricBelcherStaffVomit : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ModContent.ProjectileType<GastricBelcherVomit>() && lateInstantiation;

        public override void SetDefaults(Projectile entity)
        {
            if (WhipcacklingConfig.Instance.BalanceMode == BalanceMode.Whipcackling)
            {
                entity.usesIDStaticNPCImmunity = false;
                entity.idStaticNPCHitCooldown = -2;
                entity.penetrate = 1;
            }
        }
    }
}
