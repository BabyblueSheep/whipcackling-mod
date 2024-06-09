using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Whipcackling.Common.Utilities;

namespace Whipcackling.Content.Whips.StratusWhip.Stars
{
    public abstract class StratusStar<T> : ModProjectile where T : ModBuff
    {
        public override string LocalizationCategory => "Whips.StratusWhip";

        public ref float Target => ref Projectile.ai[0];
        public float ActualTarget => Target - 1;

        public ref float Offset => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];
        public bool HasTarget
        {
            get => Projectile.localAI[0] <= 0;
            set
            {
                Projectile.localAI[0] = value ? 0 : 1;
            }
        }

        public abstract float RotateSpeed { get; }
        public abstract float Radius { get; }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Summon;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Timer += 0.05f;

            if (HasTarget)
            {
                if (ActualTarget >= 0)
                {
                    NPC target = Main.npc[(int)ActualTarget];
                    if (!(target.active && target.HasBuff<T>()))
                    {
                        HasTarget = false;
                        Target = 0;
                        Projectile.friendly = true;
                        OnLosingTarget();
                    }
                }
                else
                {
                    HasTarget = false;
                    Target = 0;
                    Projectile.friendly = true;
                    OnLosingTarget();
                }
            }

            if (HasTarget)
            {
                NPC target = Main.npc[(int)ActualTarget];

                Projectile.velocity = Vector2.Zero;

                Vector2 intendedPosition = target.Center;
                intendedPosition.X += (float)Math.Sin(Timer * RotateSpeed + Offset) * Radius;
                intendedPosition.Y += (float)Math.Cos(Timer * RotateSpeed + Offset) * Radius;

                if (Timer < 4f)
                {
                    Projectile.position = Vector2.Lerp(Projectile.position, intendedPosition, Timer * 0.25f);
                }
                else
                {
                    Projectile.position = intendedPosition;
                }
                Projectile.rotation = (Projectile.position - Projectile.oldPosition).ToRotation();
            }
            else
            {
                int target = Projectile.FindTargetIgnoreCollision();
                if (target == -1)
                    return;
                NPC npc = Main.npc[target];

                if (npc.active)
                    HostileBehavior(npc);
            }
        }

        public abstract void OnLosingTarget();
        public abstract void HostileBehavior(NPC target);
    }
}
