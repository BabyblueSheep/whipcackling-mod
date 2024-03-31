using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Whipcackling.Common.Configs;

namespace Whipcackling.Content.Rebalances.Minions
{
    public class SpiderStaffMinion : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ProjectileID.VenomSpider || entity.type == ProjectileID.JumperSpider || entity.type == ProjectileID.DangerousSpider;

        public override void SetDefaults(Projectile entity)
        {
            if (WhipcacklingConfig.Instance.BalanceMode == BalanceMode.Whipcackling)
            {
                entity.usesIDStaticNPCImmunity = false;
                entity.idStaticNPCHitCooldown = -2;
                entity.usesLocalNPCImmunity = true;
                entity.localNPCHitCooldown = 15;
            }
        }
    }
}
