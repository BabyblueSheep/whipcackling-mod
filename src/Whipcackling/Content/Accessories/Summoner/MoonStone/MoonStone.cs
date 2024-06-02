using CalamityMod;
using CalamityMod.Balancing;
using CalamityMod.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Whipcackling.Assets;
using Whipcackling.Common.Systems.Drawing;
using Whipcackling.Common.Utilities;

namespace Whipcackling.Content.Accessories.Summoner.MoonStone
{
    public class MoonStone : ModItem
    {
        public override string LocalizationCategory => "Accessories";

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 38;

            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<MoonStonePlayer>().MoonStone = true;
        }
    }

    public class MoonStonePlayer : ModPlayer
    {
        public bool MoonStone { get; set; }

        public override void PostUpdateBuffs()
        {
            MoonStone = false; //I HATE UPDATE ORDERS
        }
    }

    public class MoonStoneProjectile : GlobalProjectile
    {
        public static SoundStyle SpellActivation = new($"{AssetDirectory.AssetPath}Sounds/MoonStone/SpellActivation")
        {
            PitchVariance = 0.5f,
            MaxInstances = 0,
        };

        public float LunarAttackCharge { get; set; }
        public bool LunarIsAttacking { get; set; }
        public bool ValidLunar { get; set; }

        public override bool InstancePerEntity => true;

        public override void Load()
        {
            IL_Projectile.Update += MoreUpdatesIfMoonStone;
        }

        public override void Unload()
        {
            IL_Projectile.Update -= MoreUpdatesIfMoonStone;
        }

        private static void MoreUpdatesIfMoonStone(ILContext il)
        {
            ILCursor? cursor = new(il);

            FieldInfo? numUpdatesField = typeof(Projectile).GetField("numUpdates", BindingFlags.Public | BindingFlags.Instance);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchStfld(numUpdatesField)))
                return;

            cursor.EmitLdarg0();
            cursor.EmitLdarg0();
            cursor.EmitLdfld(numUpdatesField);
            cursor.EmitDelegate((Projectile projectile, int numUpdates) =>
            {
                if ((projectile.minion || projectile.sentry || ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type]) && Main.player[projectile.owner].GetModPlayer<MoonStonePlayer>().MoonStone && projectile.GetGlobalProjectile<MoonStoneProjectile>().ValidLunar)
                {
                    if (projectile.extraUpdates == 0)
                        projectile.numUpdates += 1;
                    else
                        projectile.numUpdates = (int)Math.Ceiling(projectile.numUpdates * 1.2f);
                }
            });
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.minion && !projectile.sentry)
            {
                Player player = Main.player[projectile.owner];
                if (Main.gameMenu || !player.active)
                    return;
                Main.instance.CacheProjDraws();
                if (projectile.hide && MinionDrawingSystem.DetermineDrawLayer(projectile.whoAmI) == 3)
                    return;
                ValidLunar = true;
                projectile.netUpdate = true;
            }
        }

        public override void PostAI(Projectile projectile)
        {
            if (!projectile.minion)
                return;
            Player owner = Main.player[projectile.owner];
            if (Main.gameMenu || !owner.active)
                return;
            if (owner.GetModPlayer<MoonStonePlayer>().MoonStone)
            {
                if (projectile.minion || projectile.sentry || ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type])
                {
                    projectile.damage = (int)owner.GetTotalDamage(projectile.DamageType).ApplyTo(projectile.originalDamage * 0.3f);
                }
            }
            if (ValidLunar && projectile.minion && owner.GetModPlayer<MoonStonePlayer>().MoonStone)
            {
                if (projectile.numUpdates != (projectile.extraUpdates) - 1)
                    return;
                projectile.position.X += projectile.velocity.X * 0.5f;
                if (!projectile.tileCollide || projectile.velocity.Y < 0 || projectile.shouldFallThrough)
                    projectile.position.Y += projectile.velocity.Y * 0.5f;

                if (LunarIsAttacking)
                {
                    LunarAttackCharge -= 0.01f;
                    LunarAttackCharge = MathHelper.Max(LunarAttackCharge, 0);
                    if (LunarAttackCharge == 0)
                    {
                        LunarIsAttacking = false;
                        projectile.netUpdate = true;
                    }
                }
                else
                {
                    Vector2 velocity = projectile.position - projectile.oldPosition; // oldPosition instead of velocity since some minions can stick to players but still move via player movement
                    velocity *= 0.5f;
                    Vector2 correctedVelocity = Vector2.Clamp(new Vector2(Math.Abs(velocity.X), Math.Abs(velocity.Y)), Vector2.Zero, new(10, 10));
                    float distance = correctedVelocity.X > correctedVelocity.Y ? correctedVelocity.X : correctedVelocity.Y;
                    LunarAttackCharge += distance * 0.0005f;
                    LunarAttackCharge = MathHelper.Min(LunarAttackCharge, 1);
                    if (LunarAttackCharge == 1)
                    {
                        int npcID = owner.MinionAttackTargetNPC;
                        if (npcID == -1)
                        {
                            for (int i = 0; i < Main.npc.Length; i++)
                            {
                                NPC npc = Main.npc[i];
                                if (!npc.active || npc.friendly || !npc.CanBeChasedBy(projectile))
                                    continue;
                                if (Vector2.Distance(projectile.Center, npc.Center) <= 800 && Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1))
                                {
                                    npcID = i;
                                    break;
                                }
                            }
                        }

                        bool hasTarget = npcID != -1;
                        if (hasTarget)
                        {
                            SoundEngine.PlaySound(SpellActivation, projectile.Center);
                            HelperMethods.GenerateDustStarShape(projectile.Center, DustID.Vortex, velocity: 0.1f);
                            LunarIsAttacking = true;

                            NPC target = Main.npc[npcID];
                            bool isClose = Vector2.Distance(projectile.Center, target.Center) < 200;
                            bool nearbyPlayers = false;
                            for (int i = 0; i < Main.player.Length; i++)
                            {
                                Player player = Main.player[i];
                                if (!player.active)
                                    continue;
                                if (owner.team != 0 && player.team != 0 && player.team != owner.team)
                                    continue;
                                if (Vector2.Distance(player.Center, projectile.Center) > 100)
                                    continue;
                                nearbyPlayers = true;
                                break;
                            }
                            if (nearbyPlayers)
                                ActionAttackFarPassive(projectile);
                            else if (isClose)
                                ActionAttackClose(projectile, target);
                            else
                                ActionAttackFarActive(projectile, target);
                            projectile.netUpdate = true;
                        }
                    }
                }
            }
        }

        public void ActionAttackClose(Projectile projectile, NPC target)
        {
            Vector2 velocity = target.Center - projectile.Center;
            velocity.X *= 0.2f;
            velocity.Y *= 0.2f;

            for (int i = 0; i < 2; i++)
            {
                Vector2 finalVelocity = velocity;
                finalVelocity = finalVelocity.RotatedByRandom(0.3f);
                finalVelocity *= Main.rand.NextFloat(0.9f, 1.1f);
                Projectile.NewProjectile(projectile.GetProjectileSource_FromThis(), projectile.Center, finalVelocity, ModContent.ProjectileType<ExodiumRock>(), (int)(projectile.damage * 2f), 3, projectile.owner, Main.rand.NextFloat(MathHelper.TwoPi));
            }
        }

        public void ActionAttackFarActive(Projectile projectile, NPC target)
        {
            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(projectile.GetProjectileSource_FromThis(), projectile.Center, new Vector2(3f, 0).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<ExodiumBeam>(), 10, 0f, projectile.owner);
            }
        }

        public void ActionAttackFarPassive(Projectile projectile)
        {
            Projectile.NewProjectile(projectile.GetProjectileSource_FromThis(), projectile.Center, Vector2.Zero, ModContent.ProjectileType<ExodiumHeal>(), 0, 0, projectile.owner, 30, 200);
            Projectile.NewProjectile(projectile.GetProjectileSource_FromThis(), projectile.Center, Vector2.Zero, ModContent.ProjectileType<ExodiumHeal>(), 0, 0, projectile.owner, 40, 190);

            Player owner = Main.player[projectile.owner];
            for (int i = 0; i < Main.player.Length; i++)
            {
                Player player = Main.player[i];
                if (!player.active)
                    continue;
                if (owner.team != 0 && player.team != 0 && player.team != owner.team)
                    continue;
                if (Vector2.Distance(player.Center, projectile.Center) > 100)
                    continue;
                player.Heal(10);
            }
        }

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(ValidLunar);
            binaryWriter.Write(LunarAttackCharge);
            binaryWriter.Write(LunarIsAttacking);
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            ValidLunar = binaryReader.ReadBoolean();
            LunarAttackCharge = binaryReader.ReadSingle();
            LunarIsAttacking = binaryReader.ReadBoolean();
        }
    }

    public class MoonStoneChargeBar : ModSystem
    {
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int barIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Entity Health Bars"));
            if (barIndex != -1)
            {
                layers.Insert(barIndex, new LegacyGameInterfaceLayer(
                    "Whipcackling: Minion Bars",
                    delegate
                    {
                        for (int i = 0; i < Main.projectile.Length; i++)
                        {
                            Projectile projectile = Main.projectile[i];
                            if (!projectile.active)
                                continue;
                            if (!projectile.minion)
                                continue;
                            if (!Main.player[projectile.owner].GetModPlayer<MoonStonePlayer>().MoonStone)
                                continue;

                            Vector2 position = projectile.Center;
                            position.Y -= projectile.height * 0.5f + 10;
                            Color chargeColor = Color.Lerp(Color.Navy, Color.PaleGreen, projectile.GetGlobalProjectile<MoonStoneProjectile>().LunarAttackCharge);
                            Main.spriteBatch.Draw(TextureAssets.Hb2.Value, position - Main.screenPosition, TextureAssets.Hb2.Frame(), chargeColor, 0f, TextureAssets.Hb2.Size() * 0.5f, 1f, SpriteEffects.None, 0);
                            Rectangle fill = new Rectangle(0, 0, (int)(TextureAssets.Hb1.Width() * projectile.GetGlobalProjectile<MoonStoneProjectile>().LunarAttackCharge), TextureAssets.Hb1.Height());
                            Main.spriteBatch.Draw(TextureAssets.Hb1.Value, position - Main.screenPosition, fill, chargeColor, 0f, TextureAssets.Hb1.Size() * 0.5f, 1f, SpriteEffects.None, 0);
                        }

                        return true;
                    },
                    InterfaceScaleType.Game));
            }
        }
    }
}
