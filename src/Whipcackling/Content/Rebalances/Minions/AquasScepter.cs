﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader.Config;
using Terraria.ModLoader;
using CalamityMod.Items.Weapons.Summon;
using Whipcackling.Common.Configs;
using CalamityMod.Projectiles.Summon;
using Terraria.DataStructures;

namespace Whipcackling.Content.Rebalances.Minions
{
    public class AquasScepterMinionRaindrop : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ModContent.ProjectileType<AquasScepterRaindrop>() && lateInstantiation;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.SentryShot[ModContent.ProjectileType<AquasScepterRaindrop>()] = true;
            ProjectileID.Sets.SentryShot[ModContent.ProjectileType<AquasScepterTeslaAura>()] = true;
        }

        public override void SetDefaults(Projectile entity)
        {
            if (WhipcacklingConfig.Instance.BalanceMode == BalanceMode.Whipcackling)
            {
                entity.usesIDStaticNPCImmunity = false;
                entity.idStaticNPCHitCooldown = -1;
                entity.usesLocalNPCImmunity = true;
                entity.localNPCHitCooldown = 12;
            }
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (WhipcacklingConfig.Instance.BalanceMode == BalanceMode.Whipcackling)
            {
                projectile.damage = (int)(projectile.damage * 0.5f);
            }
        }
    }
}
