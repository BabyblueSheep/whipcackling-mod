using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Whipcackling.Common.Configs;

namespace Whipcackling.Content.Rebalances.Minions
{
    public class FlinxStaffItem : BaseRebalance
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.FlinxStaff && lateInstantiation;

        public override int Damage => 6;
    }

    public class FlinxStaffMinion : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ProjectileID.FlinxMinion;

        public override void SetDefaults(Projectile entity)
        {
            if (WhipcacklingConfig.Instance.BalanceMode == BalanceMode.Whipcackling)
            {
                entity.usesIDStaticNPCImmunity = false;
                entity.idStaticNPCHitCooldown = -2;
                entity.usesLocalNPCImmunity = true;
                entity.localNPCHitCooldown = 12;
            }
        }
    }
}
