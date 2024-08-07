﻿using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Social.WeGame;
using Whipcackling.Assets;
using Whipcackling.Content.Whips.NuclearWhip;
using Whipcackling.Core.Particles;
using Whipcackling.Core.Particles.Components;

namespace Whipcackling.Content.Whips.BloodstoneWhip
{
    public class BloodstoneWhip : ModItem
    {
        public override string LocalizationCategory => "Whips.BloodstoneWhip";

        public override void SetDefaults()
        {
            Item.DefaultToWhip(projectileId: ModContent.ProjectileType<BloodstoneWhipProjectile>(), dmg: ConstantsBloodstone.ItemDamage, kb: ConstantsBloodstone.ItemKnockback, shootspeed: ConstantsBloodstone.ItemRange, animationTotalTime: ConstantsBloodstone.ItemUseTime);
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.UseSound = SoundID.Item152 with { Volume = 0 };

            Item.autoReuse = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return player.altFunctionUse != 2;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse > 0)
            {
                player.itemAnimation = player.itemAnimationMax = 0;
                if (player.GetModPlayer<BloodstoneWhipPlayer>().BloodCharge == 1 && !player.GetModPlayer<BloodstoneWhipPlayer>().IsAwakened)
                {
                    AwakenWhip(player);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        ModPacket packet = Mod.GetPacket();
                        packet.Write((byte)WhipcacklingMod.MessageType.BloodstoneWhipAwakenServer);
                        packet.Write(player.whoAmI);
                        packet.Send();
                    }

                    if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(player.GetProjectileSource_Item(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<Sparkle>(), 0, 0, player.whoAmI);
                    }
                }
                return false;
            }
            SoundEngine.PlaySound(SoundID.Item152, player.Center);

            return true;
        }
        
        public static void AwakenWhip(Player player)
        {
            SoundEngine.PlaySound(BloodstoneWhipPlayer.AwakenActivate, player.Center);
            player.GetModPlayer<BloodstoneWhipPlayer>().IsAwakened = true;
            player.GetModPlayer<BloodstoneWhipPlayer>().BloodCharge = 1;

            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 20; i++)
                {
                    ParticleSystem.World.Create(
                        (UVCoordinates)ParticleAtlasSystem.AtlasDefinitions[$"Smoke{Main.rand.Next(1, 4)}"],
                        (Position)player.Center,
                        (Scale)(new Vector2(Main.rand.NextFloat(0.8f, 1.2f))),
                        new Rotation(Main.rand.NextFloat(MathHelper.TwoPi)),
                        Color.Maroon,
                        new TimeLeft(Main.rand.Next(50, 80)),
                        new TimeUntilAction(Main.rand.Next(20, 40)),
                        new LinearVelocityAcceleration(Utils.RandomVector2(Main.rand, 0, 3f).RotatedByRandom(MathHelper.TwoPi), 0, 0, 0.95f, 0.95f),
                        new RotateWithLinearVelocity(0.01f, 0),
                        new LinearScaleIncrease(0.01f, 0.01f),
                        new AlphaFadeInOut(5, 100, 120, 20, 20, 210)
                        );
                }

                for (int i = 0; i < 10; i++)
                {
                    ParticleSystem.World.Create(
                        (UVCoordinates)ParticleAtlasSystem.AtlasDefinitions[$"Smoke{Main.rand.Next(1, 4)}"],
                        (Position)player.Center,
                        (Scale)(new Vector2(Main.rand.NextFloat(0.9f, 1.35f))),
                        new Rotation(Main.rand.NextFloat(MathHelper.TwoPi)),
                        Color.Maroon,
                        new TimeLeft(Main.rand.Next(60, 90)),
                        new TimeUntilAction(Main.rand.Next(20, 40)),
                        new LinearVelocityAcceleration(Utils.RandomVector2(Main.rand, 0, 2f).RotatedByRandom(MathHelper.TwoPi), 0, 0, 0.97f, 0.97f),
                        new RotateWithLinearVelocity(0.01f, 0),
                        new LinearScaleIncrease(0.01f, 0.01f),
                        new AlphaFadeInOut(10, 100, 120, 20, 20, 210)
                        );
                }
            }
        }

        public override bool MeleePrefix()
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodstoneCore>(6).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
