using Microsoft.Win32;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;
using Whipcackling.Assets;
using Whipcackling.Core.Particles;

namespace Whipcackling.Content.Accessories.Summoner.MartianDataglove
{
    public class HolyAttack : ModProjectile
    {
        public override string LocalizationCategory => "Accessories.MartianDataglove";

        public bool Hit
        {
            get => Projectile.ai[0] > 0;
            set => Projectile.ai[0] = value ? 1 : 0;
        }

        public ref float NPCID => ref Projectile.ai[1];

        public bool Initialized
        {
            get => Projectile.localAI[0] > 0;
            set => Projectile.localAI[0] = value ? 1 : 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (!Initialized)
            {
                /*for (int i = 0; i < 4 + Main.rand.Next(3); i++)
                {
                    float rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    ParticleSystem.SpawnParticle(
                        type: ParticleLoader.ParticleType<HolyGlowLine>(),
                        position: Projectile.position + (rotation - MathHelper.PiOver2).ToRotationVector2() * 50,
                        velocity: Vector2.Zero,
                        scale: new Vector2(1f, 3f),
                        rotation: rotation,
                        color: Color.Khaki,
                        variant: 0,
                        lifetime: 10 + Main.rand.Next(5) + 2,
                        custom1: Main.rand.NextFloat(0.3f, 0.5f), // Decay
                        custom2: NPCID // NPC anchor
                    );
                }*/

                Initialized = true;
            }
            Lighting.AddLight((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16f), 2 * Projectile.timeLeft / 20f, 2 * Projectile.timeLeft / 20f, 1.5f * Projectile.timeLeft / 20f);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Hit = true;
        }

        public override bool? CanDamage()
        {
            if (Hit)
                return false;
            return null;
        }
    }
}
