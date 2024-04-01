using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Whipcackling.Assets;
using Whipcackling.Content.Particles;
using Whipcackling.Core.Particles;

namespace Whipcackling.Content.Accessories.Summoner.MartianDataglove
{
    public class MartianDataglove : ModItem
    {
        public override string LocalizationCategory => "Accessories";

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 40;

            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.whipRangeMultiplier += 0.1f;
            player.summonerWeaponSpeedBonus += 0.1f;
            player.GetModPlayer<MartianDataglovePlayer>().MartianDataglove = true;

            player.GetDamage<SummonDamageClass>() += 0.05f * (player.maxMinions + player.maxTurrets);
            player.GetCritChance<SummonMeleeSpeedDamageClass>() += 0.01f * (player.maxMinions + player.maxTurrets);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.IntegrateHotkey(MartianDataglovePlayer.ExpandTooltip);
            if (!MartianDataglovePlayer.ExpandTooltip.Current)
                return;

            foreach (KeyValuePair<int, MartianDatagloveEffect> entry in MartianDatagloveProjectile.TagConversions)
            {
                tooltips.Add(new TooltipLine(
                    Mod,
                    "Whipcackling:VanillaTagDebuffs",
                    entry.Value.Tooltip.ToString()
                ));
            }
        }
    }

    public class MartianDataglovePlayer : ModPlayer
    {
        public static ModKeybind ExpandTooltip { get; private set; }

        public bool MartianDataglove { get; set; }

        public override void Load()
        {
            ExpandTooltip = KeybindLoader.RegisterKeybind(Mod, "ExpandTooltip", "LeftShift");

            On_Player.GetTotalDamage += MakeWhipDamageAffectedByMelee;
        }

        public override void Unload()
        {
            ExpandTooltip = null;

            On_Player.GetTotalDamage -= MakeWhipDamageAffectedByMelee;
        }

        public override void ResetEffects()
        {
            MartianDataglove = false;
        }

        public override void PostUpdateEquips()
        {
            if (MartianDataglove)
            {
                Player.maxMinions = 0;
                Player.maxTurrets = 0;
            }
        }

        private StatModifier MakeWhipDamageAffectedByMelee(On_Player.orig_GetTotalDamage orig, Player self, DamageClass damageClass)
        {
            StatModifier stats = orig(self, damageClass);
            if (damageClass.Type != DamageClass.SummonMeleeSpeed.Type || !self.GetModPlayer<MartianDataglovePlayer>().MartianDataglove)
                return stats;
            stats = stats.CombineWith(self.damageData[DamageClass.Melee.Type].damage.Scale(0.15f));
            return stats;
        }
    }   

    public class MartianDatagloveProjectile : GlobalProjectile
    {
        public static SoundStyle HolyAttackSound = new($"{AssetDirectory.AssetPath}Sounds/MartianDataglove/HolyAttack", 3)
        {
            PitchVariance = 0.5f,
            Volume = 0.7f
        };

        public static Dictionary<int, MartianDatagloveEffect> TagConversions { get; set; }

        public override void Load()
        {
            TagConversions ??= new();
            TagConversions.Clear();

            TagConversions.Add(BuffID.BlandWhipEnemyDebuff, new((owner, whip, target, buffTime) => target.AddBuff(BuffID.Confused, buffTime), Language.GetOrRegister($"Mods.Whipcackling.Accessories.MartianDataglove.LeatherWhipTooltip")));
            TagConversions.Add(BuffID.ThornWhipNPCDebuff, new((owner, whip, target, buffTime) => target.AddBuff(BuffID.Poisoned, buffTime), Language.GetOrRegister($"Mods.Whipcackling.Accessories.MartianDataglove.SnapthornTooltip")));
            TagConversions.Add(BuffID.BoneWhipNPCDebuff, new((owner, whip, target, buffTime) =>
            {
                int counter = 0;
                for (int i = 0; i < whip.localNPCImmunity.Length; i++)
                {
                    if (whip.localNPCImmunity[i] == -1)
                        counter++;
                }
                if ((counter - 1) % 3 != 0)
                    return;
                int amount = (int)Math.Ceiling(buffTime / 60.0);
                for (int i = 0; i < amount; i++)
                {
                    Vector2 speed = new(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-0.9f, -1.1f));
                    Projectile proj = Projectile.NewProjectileDirect(whip.GetSource_FromThis(), target.Center - new Vector2(0, target.height * 0.75f), speed * 5, ProjectileID.Bone, (int)Math.Ceiling((double)whip.damage / amount), 0, whip.owner);
                    proj.ranged = false;
                }
            }, Language.GetOrRegister($"Mods.Whipcackling.Accessories.MartianDataglove.SpinalTapTooltip")));
            TagConversions.Add(BuffID.FlameWhipEnemyDebuff, new((owner, whip, target, buffTime) =>
            {
                Projectile.NewProjectile(whip.GetSource_FromThis(), target.Center, Vector2.Zero, ProjectileID.FireWhipProj, (int)Math.Ceiling((double)whip.damage * 0.5f * Math.Ceiling(buffTime / 240.0)), 0, whip.owner);
            }, Language.GetOrRegister($"Mods.Whipcackling.Accessories.MartianDataglove.FirecrackerTooltip")));
            TagConversions.Add(BuffID.CoolWhipNPCDebuff, new((owner, whip, target, buffTime) =>
            {
                int counter = 0;
                for (int i = 0; i < whip.localNPCImmunity.Length; i++)
                {
                    if (whip.localNPCImmunity[i] == -1)
                        counter++;
                    if (counter == 2)
                        return;
                }
                Projectile proj = Projectile.NewProjectileDirect(whip.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<LessCoolFlake>(), 5, 0, whip.owner, ai2: (float)Math.Ceiling(buffTime / 4f));
            }, Language.GetOrRegister($"Mods.Whipcackling.Accessories.MartianDataglove.CoolWhipTooltip")));
            TagConversions.Add(BuffID.SwordWhipNPCDebuff, new((owner, whip, target, buffTime) =>
            {
                int counter = 0;
                for (int i = 0; i < whip.localNPCImmunity.Length; i++)
                {
                    if (whip.localNPCImmunity[i] == -1)
                        counter++;
                    if (counter == 2)
                        return;
                }
                List<NPC> npcs = new();
                int maxNPCs = (int)Math.Ceiling(buffTime / 60f);
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.active || npc.friendly || npc.dontTakeDamage || npc == target)
                        continue;
                    if (Vector2.Distance(npc.Center, target.Center) < 650)
                    {
                        npcs.Add(npc);
                    }
                }
                npcs.Sort((NPC x, NPC y) => Vector2.Distance(x.Center, target.Center).CompareTo(Vector2.Distance(y.Center, target.Center)));
                for (int npcCount = 0; npcCount < npcs.Count; npcCount++)
                {
                    if (npcCount == 0)
                        SoundEngine.PlaySound(HolyAttackSound, target.Center);
                    if (npcCount == maxNPCs)
                        break;
                    NPC npc = npcs[npcCount];
                    Projectile k = Projectile.NewProjectileDirect(whip.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<HolyAttack>(), 20, 0, whip.owner, ai1: npc.whoAmI);
                    k.Damage();

                    Vector2 position = Vector2.Lerp(target.Center, npc.Center, 0.5f);
                    float rotation = (npc.Center - target.Center).ToRotation();
                    float distance = Vector2.Distance(target.Center, npc.Center);

                    Color[] colors = new Color[]
                    {
                        Color.Wheat, Color.Khaki, Color.PeachPuff
                    };

                    for (int particleCount = 0; particleCount < 3; particleCount++)
                    {
                        Vector2 positionMod = new(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f));
                        Vector2 scaleMod = new(Main.rand.NextFloat(-0.2f, 0.1f), Main.rand.NextFloat(-0.1f, 0.2f));

                        ParticleSystem.SpawnParticle(
                            type: ParticleLoader.ParticleType<HolyConnectedBeam>(),
                            position: position + positionMod * distance * 0.0025f,
                            velocity: Vector2.Zero,
                            scale: new Vector2(0.5f, distance * 0.0125f) + scaleMod * distance * 0.0025f,
                            rotation: rotation + MathHelper.PiOver2,
                            color: colors[particleCount] * 0.5f,
                            variant: 0,
                            lifetime: 12 + Main.rand.Next(10),
                            custom1: Main.rand.NextFloat(0.25f, 0.55f), // Decay
                            custom2: target.whoAmI, // First NPC anchor
                            custom3: npc.whoAmI // Second NPC anchor
                        );
                    }
                }
            }, Language.GetOrRegister($"Mods.Whipcackling.Accessories.MartianDataglove.DurendalTooltip")));
            TagConversions.Add(BuffID.ScytheWhipEnemyDebuff, new((owner, whip, target, buffTime) =>
            {
                if (target.life > 0)
                    return;
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.BlackLightningHit, new ParticleOrchestraSettings
                {
                    PositionInWorld = target.Center,
                });

                List<NPC> npcs = new();
                int maxNPCs = (int)Math.Ceiling(buffTime / 30f);
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.active || npc.friendly || npc.dontTakeDamage || npc == target)
                        continue;
                    if (Vector2.Distance(npc.Center, target.Center) < 1200)
                    {
                        npcs.Add(npc);
                    }
                }
                npcs.Sort((NPC x, NPC y) => Vector2.Distance(x.Center, target.Center).CompareTo(Vector2.Distance(y.Center, target.Center)));
                for (int npcCount = 0; npcCount < npcs.Count; npcCount++)
                {
                    if (npcCount == maxNPCs)
                        break;
                    NPC npc = npcs[npcCount];
                    Main.NewText(npc.whoAmI);
                    Projectile k = Projectile.NewProjectileDirect(whip.GetSource_FromThis(), target.Center, Vector2.Zero, ProjectileID.ScytheWhipProj, 100, 0, whip.owner, ai0: npc.whoAmI + 1);
                }
            }, Language.GetOrRegister($"Mods.Whipcackling.Accessories.MartianDataglove.DarkHarvestTooltip")));
            TagConversions.Add(BuffID.MaceWhipNPCDebuff, new((owner, whip, target, buffTime) => target.AddBuff(ModContent.BuffType<CrushDepth>(), buffTime), Language.GetOrRegister($"Mods.Whipcackling.Accessories.MartianDataglove.MorningStarTooltip")));
            TagConversions.Add(BuffID.RainbowWhipNPCDebuff, new((owner, whip, target, buffTime) =>
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f) + Main.rand.NextVector2CircularEdge(5f, 5f);

                    Projectile.NewProjectile(whip.GetSource_FromThis(), target.position, velocity, ProjectileID.FairyQueenMagicItemShot, 10, 2, whip.owner, -1, i / 32f + owner.miscCounterNormalized);
                }
            }, Language.GetOrRegister($"Mods.Whipcackling.Accessories.MartianDataglove.KaleidoscopeTooltip")));
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!ProjectileID.Sets.IsAWhip[projectile.type])
                return;
            if (!Main.player[projectile.owner].GetModPlayer<MartianDataglovePlayer>().MartianDataglove)
                return;
            foreach (KeyValuePair<int, MartianDatagloveEffect> entry in TagConversions)
            {
                if (target.HasBuff(entry.Key))
                {
                    entry.Value.Effect(Main.player[projectile.owner], projectile, target, target.buffTime[target.FindBuffIndex(entry.Key)]);
                }
            }
        }
    }

    public class MartianDatagloveLoot : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.BrainScrambler || npc.type == NPCID.GigaZapper || npc.type == NPCID.MartianEngineer || npc.type == NPCID.MartianOfficer)
            {
                npcLoot.Add(ModContent.ItemType<MartianDataglove>(), 800);
            }
        }
    }

    public struct MartianDatagloveEffect
    {
        public Action<Player, Projectile, NPC, int> Effect;
        public LocalizedText Tooltip;

        public MartianDatagloveEffect(Action<Player, Projectile, NPC, int> effect, LocalizedText tooltip)
        {
            Effect = effect;
            Tooltip = tooltip;
        }
    }
}