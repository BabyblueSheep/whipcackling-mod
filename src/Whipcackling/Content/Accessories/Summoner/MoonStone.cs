using CalamityMod.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Whipcackling.Common.Systems.Drawing;

namespace Whipcackling.Content.Accessories.Summoner
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
        public bool IsLunar { get; set; }
        public float LunarAttackCharge { get; set; }

        public override bool InstancePerEntity => true;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.minion && !projectile.sentry)
            {
                Player player = Main.player[projectile.owner];
                if (Main.gameMenu || !player.active)
                    return;
                if (player.GetModPlayer<MoonStonePlayer>().MoonStone)
                {
                    IsLunar = true;
                    if (projectile.extraUpdates == 0)
                        projectile.extraUpdates += 1;
                    else
                         projectile.extraUpdates = (int)Math.Ceiling(projectile.extraUpdates * 1.2f);
                }
            }
            else if (ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type])
            {
                if (source is EntitySource_Parent parentSource && parentSource.Entity is Projectile proj && (proj.minion || proj.sentry) && proj.GetGlobalProjectile<MoonStoneProjectile>().IsLunar)
                {
                    IsLunar = true;
                    if (projectile.extraUpdates == 0)
                        projectile.extraUpdates += 1;
                    else
                        projectile.extraUpdates = (int)Math.Ceiling(projectile.extraUpdates * 1.2f);
                }
            }
        }

        public override bool PreAI(Projectile projectile)
        {
            if (!projectile.minion && !projectile.sentry)
                return true;
            Player player = Main.player[projectile.owner];
            if (Main.gameMenu || !player.active)
                return true;
            if (IsLunar)
            {
                projectile.damage = (int)player.GetTotalDamage(projectile.DamageType).ApplyTo(projectile.originalDamage * 0.3f);
                projectile.position.X += projectile.velocity.X * 0.25f;
                if (!projectile.tileCollide || projectile.velocity.Y < 0 || projectile.shouldFallThrough)
                    projectile.position.Y += projectile.velocity.Y * 0.25f;

                LunarAttackCharge += Vector2.Clamp(projectile.velocity, Vector2.Zero, new(10, 10)).Length() * 0.0005f;
                LunarAttackCharge = MathHelper.Min(LunarAttackCharge, 1);
            }
            return true;
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
                            if (projectile.hide && MinionDrawingSystem.DetermineDrawLayer(i) == 2)
                                continue;
                            if (!projectile.GetGlobalProjectile<MoonStoneProjectile>().IsLunar)
                                continue;
                            Vector2 position = projectile.Center;
                            position.Y -= projectile.height * 0.5f + 10;
                            Main.spriteBatch.Draw(TextureAssets.Hb2.Value, position - Main.screenPosition, TextureAssets.Hb2.Frame(), Color.DarkBlue, 0f, TextureAssets.Hb2.Size() * 0.5f, 1f, SpriteEffects.None, 0);
                            Rectangle fill = new Rectangle(0, 0, (int)(TextureAssets.Hb1.Width() * projectile.GetGlobalProjectile<MoonStoneProjectile>().LunarAttackCharge), TextureAssets.Hb1.Height());
                            Color chargeColor = Color.Lerp(Color.Navy, Color.PaleGreen, projectile.GetGlobalProjectile<MoonStoneProjectile>().LunarAttackCharge);
                            Main.spriteBatch.Draw(TextureAssets.Hb1.Value, position - Main.screenPosition, fill, chargeColor, 0f, TextureAssets.Hb1.Size() * 0.5f, 1f, SpriteEffects.None, 0);
                        }

                        return true;
                    },
                    InterfaceScaleType.Game));
            }
        }
    }
}
