using CalamityMod.Items.Potions.Alcohol;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Whipcackling.Core.Particles;
using static Terraria.ModLoader.BackupIO;
using Whipcackling.Core.Particles.Components;

namespace Whipcackling.Content.Whips.BloodstoneWhip
{
    public class BloodFlames : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.Flames}";

        public ref float Timer => ref Projectile.localAI[0];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 60;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Flames);
            Projectile.ranged = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.aiStyle = 0;
        }

        public override bool PreAI()
        {
            Timer++;

            if (Timer >= 72)
            {
                Projectile.Kill();
                return false;
            }
            if (Timer >= 60)
            {
                Projectile.velocity *= 0.95f;
            }

            float frequency = Utils.Remap(Timer, 15, 45, 0, 1);
            byte alpha = (byte)(200 * frequency);
            byte color = (byte)(40 * frequency);

            Vector2 velocity = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.PiOver4) * 5f * frequency;

            if (Main.rand.NextFloat() < 0.7f) // Smaller smoke
            {
                ParticleSystem.World.Create(
                    (UVCoordinates)ParticleAtlasSystem.AtlasDefinitions[$"Smoke{Main.rand.Next(1, 4)}"],
                    (Position)(Projectile.Center + Main.rand.NextVector2Circular(40f, 40f)),
                    (Scale)(new Vector2(Main.rand.NextFloat(1.2f, 1.5f))),
                    new Rotation(Main.rand.NextFloat(MathHelper.TwoPi)),
                    new Color(80, 80, 80, 0),
                    new TimeLeft(15),
                    new TimeUntilAction(Main.rand.Next(0, 10)),
                    new LinearVelocityAcceleration(velocity, 0, -0.7f, 0.97f, 0.97f),
                    new RotateWithLinearVelocity(0.02f, 0),
                    new LinearScaleIncrease(0.01f, 0.01f),
                    new AlphaFadeInOut(10, 5, color, color, color, alpha)
                    );
            }
            else if (Main.rand.NextFloat() < 0.5f) // Bigger smoke
            {
                ParticleSystem.World.Create(
                    (UVCoordinates)ParticleAtlasSystem.AtlasDefinitions[$"Smoke{Main.rand.Next(1, 4)}"],
                    (Position)(Projectile.Center + Main.rand.NextVector2Circular(10f, 10f)),
                    (Scale)(new Vector2(Main.rand.NextFloat(1.2f, 2f))),
                    new Rotation(Main.rand.NextFloat(MathHelper.TwoPi)),
                    new Color(80, 80, 80, 0),
                    new TimeLeft(25),
                    new TimeUntilAction(Main.rand.Next(0, 20)),
                    new LinearVelocityAcceleration(velocity * 0.6f, 0, -0.4f, 0.97f, 0.97f),
                    new RotateWithLinearVelocity(0.02f, 0),
                    new LinearScaleIncrease(0.01f, 0.01f),
                    new AlphaFadeInOut(10, 5, color, color, color, alpha)
                    );
            }

            if (Projectile.wet && !Projectile.lavaWet)
            {
                Projectile.Kill();
                return false;
            }

            return false;
        }

        public override bool? CanDamage() => Timer < 54;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.85f);
        }

        public override void OnHitPlayer(Terraria.Player target, Terraria.Player.HurtInfo info)
        {
            Projectile.damage = (int)(Projectile.damage * 0.85f);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int num = (int)Utils.Remap(Timer, 0f, 72f, 10f, 40f);
            hitbox.Inflate(num, num);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!projHitbox.Intersects(targetHitbox))
            {
                return false;
            }
            return Collision.CanHit(Projectile.Center, 0, 0, targetHitbox.Center.ToVector2(), 0, 0);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity;
            Projectile.position += Projectile.velocity;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float num = 60f;
            float num10 = 12f;
            float fromMax = num + num10;
            Texture2D value = TextureAssets.Projectile[Projectile.type].Value;
            Color color = new(200, 20, 20, 200);
            Color color2 = new(255, 140, 100, 70);
            Color color3 = new(120, 20, 20, 100);
            Color color4 = new(90, 90, 90, 120);
            float num11 = 0.35f;
            float num12 = 0.7f;
            float num13 = 0.85f;
            float num14 = ((Timer > num - 10f) ? 0.175f : 0.2f);
            float num15 = Utils.Remap(Timer, num, fromMax, 1f, 0f);
            float num2 = Math.Min(Timer, 20f);
            float num3 = Utils.Remap(Timer, 0f, fromMax, 0f, 1f);
            float num4 = Utils.Remap(num3, 0.2f, 0.5f, 0.25f, 1f);
            Rectangle rectangle = value.Frame(1, 7, 0, (int)Utils.Remap(num3, 0.5f, 1f, 3f, 5f));
            if (!(num3 < 1f))
            {
                return false;
            }
            for (int i = 0; i < 2; i++)
            {
                for (float num5 = 1f; num5 >= 0f; num5 -= num14)
                {
                    Color val = ((num3 < 0.1f) ? Color.Lerp(Color.Transparent, color, Utils.GetLerpValue(0f, 0.1f, num3, clamped: true)) : ((num3 < 0.2f) ? Color.Lerp(color, color2, Utils.GetLerpValue(0.1f, 0.2f, num3, clamped: true)) : ((num3 < num11) ? color2 : ((num3 < num12) ? Color.Lerp(color2, color3, Utils.GetLerpValue(num11, num12, num3, clamped: true)) : ((num3 < num13) ? Color.Lerp(color3, color4, Utils.GetLerpValue(num12, num13, num3, clamped: true)) : ((!(num3 < 1f)) ? Color.Transparent : Color.Lerp(color4, Color.Transparent, Utils.GetLerpValue(num13, 1f, num3, clamped: true))))))));
                    float num6 = (1f - num5) * Utils.Remap(num3, 0f, 0.2f, 0f, 1f);
                    Vector2 vector = Projectile.Center - Main.screenPosition + Projectile.velocity * (0f - num2) * num5;
                    Color color5 = val * num6;
                    Color color6 = color5;
                    color6.G = (byte)(color6.G / 2);
                    color6.B = (byte)(color6.B / 2);
                    color6.A = (byte)Math.Min(color5.A + 80f * num6, 255f);
                    Utils.Remap(Timer, 20f, fromMax, 0f, 1f);
                    float num7 = 1f / num14 * (num5 + 1f);
                    float num8 = Projectile.rotation + num5 * (MathF.PI / 2f) + Main.GlobalTimeWrappedHourly * num7 * 2f;
                    float num9 = Projectile.rotation - num5 * (MathF.PI / 2f) - Main.GlobalTimeWrappedHourly * num7 * 2f;
                    switch (i)
                    {
                        case 0:
                            Main.spriteBatch.Draw(value, vector + Projectile.velocity * (0f - num2) * num14 * 0.5f, rectangle, color6 * num15 * 0.25f, num8 + MathF.PI / 4f, rectangle.Size() / 2f, num4, SpriteEffects.None, 0);
                            Main.spriteBatch.Draw(value, vector, rectangle, color6 * num15, num9, rectangle.Size() / 2f, num4, SpriteEffects.None, 0);
                            break;
                        case 1:
                            Main.spriteBatch.Draw(value, vector + Projectile.velocity * (0f - num2) * num14 * 0.2f, rectangle, color5 * num15 * 0.25f, num8 + MathF.PI / 2f, rectangle.Size() / 2f, num4 * 0.75f, SpriteEffects.None, 0);
                            Main.spriteBatch.Draw(value, vector, rectangle, color5 * num15, num9 + MathF.PI / 2f, rectangle.Size() / 2f, num4 * 0.75f, SpriteEffects.None, 0);
                            break;
                    }
                }
            }

            return false;
        }
    }
}
