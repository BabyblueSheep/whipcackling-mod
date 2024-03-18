using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Whipcackling.Common.Configs;

namespace Whipcackling.Content.Rebalances
{
    public abstract class BaseRebalance : GlobalItem
    {
        public virtual int Damage => -1;
        public virtual float Knockback => -1;
        public virtual int Mana => -1;
        public virtual int UseTime => -1;
        public virtual int UseAnimationTime => -1;
        public virtual int Velocity => -1;

        public override void SetDefaults(Item entity)
        {
            switch (WhipcacklingConfig.Instance.BalanceMode)
            {
                case BalanceMode.Whipcackling:
                    if (Damage >= 0)
                        entity.damage = Damage;
                    if (Knockback >= 0)
                        entity.knockBack = Knockback;
                    if (Mana >= 0)
                        entity.mana = Mana;
                    if (UseTime >= 0)
                        entity.useTime = UseTime;
                    if (UseAnimationTime >= 0)
                        entity.useAnimation = UseAnimationTime;
                    if (Velocity >= 0)
                        entity.shootSpeed = Velocity;
                    break;
                case BalanceMode.Calamity:
                    if (!RebalanceSystem.CalamityStats.TryGetValue(entity.type, out WeaponStats calStats))
                        break;
                    entity.damage = calStats.Damage;
                    entity.knockBack = calStats.Knockback;
                    entity.mana = calStats.Mana;
                    entity.useTime = calStats.UseTime;
                    entity.useAnimation = calStats.UseAnimation;
                    entity.shootSpeed = calStats.Velocity;
                    break;
                case BalanceMode.Vanilla:
                    if (!RebalanceSystem.VanillaStats.TryGetValue(entity.type, out WeaponStats stats))
                        break;
                    entity.damage = stats.Damage;
                    entity.knockBack = stats.Knockback;
                    entity.mana = stats.Mana;
                    entity.useTime = stats.UseTime;
                    entity.useAnimation = stats.UseAnimation;
                    entity.shootSpeed = stats.Velocity;
                    break;
                case BalanceMode.Default:
                    break;
            }
        }
    }
}
