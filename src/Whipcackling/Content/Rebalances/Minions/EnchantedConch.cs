﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Summon;
using Whipcackling.Common.Configs;
using Microsoft.Xna.Framework;
using CalamityMod;

namespace Whipcackling.Content.Rebalances.Minions
{
    public class EnchantedConchItem : BaseRebalance
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ModContent.ItemType<EnchantedConch>() && lateInstantiation;

        public override int Damage => 14;
    }

    public class EnchantedConchMinion : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ModContent.ProjectileType<HermitCrabMinion>() && lateInstantiation;

        public override void SetDefaults(Projectile entity)
        {
            if (WhipcacklingConfig.Instance.BalanceMode == BalanceMode.Whipcackling)
            {
                entity.localNPCHitCooldown = 20;
            }
        }

        public override void PostAI(Projectile projectile)
        {
            if (WhipcacklingConfig.Instance.BalanceMode != BalanceMode.Whipcackling)
                return;

            if (!((HermitCrabMinion)(projectile.ModProjectile)).fly)
            {
                Player player = Main.player[projectile.owner];
                float attackDistance = 600f;
                bool chaseNPC = false;
                float npcPositionX = 0f;
                float npcPositionY = 0f;
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        float targetDist = Vector2.Distance(npc.Center, projectile.Center);
                        if (!chaseNPC && targetDist < attackDistance)
                        {
                            attackDistance = targetDist;
                            npcPositionX = npc.position.X;
                            npcPositionY = npc.position.Y;
                            chaseNPC = true;
                        }
                    }
                }
                if (!chaseNPC)
                {
                    for (int j = 0; j < Main.maxNPCs; j++)
                    {
                        NPC npcTarget = Main.npc[j];
                        if (npcTarget.CanBeChasedBy(projectile, false))
                        {
                            float targetDist = Vector2.Distance(npcTarget.Center, projectile.Center);
                            if (!chaseNPC && targetDist < attackDistance)
                            {
                                attackDistance = targetDist;
                                npcPositionX = npcTarget.position.X;
                                npcPositionY = npcTarget.position.Y;
                                chaseNPC = true;
                            }
                        }
                    }
                }
                if (chaseNPC && !((HermitCrabMinion)(projectile.ModProjectile)).fly && projectile.position.Y == projectile.oldPosition.Y && !((HermitCrabMinion)(projectile.ModProjectile)).HoleBelow())
                {
                    float distX = Math.Abs(npcPositionX - projectile.position.X);
                    projectile.velocity.X *= Utils.GetLerpValue(-1800, 100, distX, true);
                    projectile.velocity.X *= Utils.GetLerpValue(-10, 30, distX, true);
                    float distY = npcPositionY - projectile.position.Y;
                    if (distY < -100 && distX < 20)
                    {
                        projectile.velocity.Y -= 20 * Utils.GetLerpValue(0, -200, distY, true);
                    }
                }
            }
        }
    }
}