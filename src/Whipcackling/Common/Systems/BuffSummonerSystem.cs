using CalamityMod;
using CalamityMod.Balancing;
using CalamityMod.Buffs.Summon.Whips;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Accessories;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Providence;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Whipcackling.Common.Configs;

namespace Whipcackling.Common.Systems
{
    public class BuffSummonerSystem : ModSystem
    {
        private static ILHook?_modifyHitNPCWithProj;
        private static Hook? _editWhipTagDamage;

        public override void Load()
        {
            _modifyHitNPCWithProj = new(typeof(CalamityPlayer).GetMethod("ModifyHitNPCWithProj")!, RemoveSummonerPenalty);
            _editWhipTagDamage = new(typeof(CalamityGlobalNPC).GetMethod("EditWhipTagDamage", BindingFlags.NonPublic | BindingFlags.Instance)!, RemoveMultiplicativeTagDamage);
        }

        public override void Unload()
        {
            _modifyHitNPCWithProj?.Undo();
            _editWhipTagDamage?.Undo();
        }

        private static void RemoveSummonerPenalty(ILContext il)
        {
            ILCursor? cursor = new(il);
            FieldInfo? summonerNerf = typeof(BalancingConstants).GetField("SummonerCrossClassNerf", BindingFlags.NonPublic | BindingFlags.Static);
            if (summonerNerf is null)
                return;

            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdsfld(summonerNerf)))
                return;

            cursor.Emit(OpCodes.Pop);
            cursor.EmitDelegate(() => WhipcacklingConfig.Instance.RemoveSummonerPenalty ? 1f : 0.75f);
        }

        private static void RemoveMultiplicativeTagDamage(orig_EditWhipTagDamage orig, CalamityGlobalNPC self, Projectile proj, NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (WhipcacklingConfig.Instance.BalanceMode == BalanceMode.Calamity || WhipcacklingConfig.Instance.BalanceMode == BalanceMode.Default)
                orig(self, proj, npc, ref modifiers);
            else
            {
                float tagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[proj.type];

                if (npc.HasBuff<ProfanedCrystalWhipDebuff>() && Main.player[proj.owner].Calamity().pscState >= (int)ProfanedSoulCrystal.ProfanedSoulCrystalState.Buffs)
                {
                    var empowered = Main.player[proj.owner].Calamity().pscState == (int)ProfanedSoulCrystal.ProfanedSoulCrystalState.Empowered;
                    modifiers.ScalingBonusDamage += (empowered ? 0.4f : 0.2f) * tagMultiplier;

                    if (Main.netMode != NetmodeID.Server)
                    {
                        Color color = ProvUtils.GetProjectileColor((int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night), 0);
                        float power = Math.Min(npc.height / 100f, 3f);
                        Vector2 position = new(Main.rand.NextFloat(npc.Left.X, npc.Right.X), Main.rand.NextFloat(npc.Top.Y, npc.Bottom.Y));
                        Particle particle = new FlameParticle(
                            position: position, 
                            lifetime: 50, 
                            scale: 0.25f, 
                            relativePower: power, 
                            brightColor: color * (Main.dayTime ? 1f : 1.25f), 
                            darkColor: color * (Main.dayTime ? 1.25f : 1f));
                        GeneralParticleHandler.SpawnParticle(particle);
                    }
                }
            }
        }

        private delegate void orig_EditWhipTagDamage(CalamityGlobalNPC self, Projectile proj, NPC npc, ref NPC.HitModifiers modifiers);
    }
}
