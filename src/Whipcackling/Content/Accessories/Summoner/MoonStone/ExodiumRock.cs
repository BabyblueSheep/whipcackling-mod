using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Whipcackling.Assets;
using Whipcackling.Common.Systems.Drawing;
using Whipcackling.Content.Particles;
using Whipcackling.Core.Particles;

namespace Whipcackling.Content.Accessories.Summoner.MoonStone
{
    public class ExodiumRock : ModProjectile, IDrawPixelated
    {
        public static SoundStyle RockBreak = new($"{AssetDirectory.AssetPath}Sounds/MoonStone/RockBreak", 3)
        {
            PitchVariance = 0.5f,
            MaxInstances = 0,
        };

        public override string LocalizationCategory => "Accessories.MoonStone";

        public ref float TimeOutsideTiles => ref Projectile.ai[0];
        public ref float RotationDrawing => ref Projectile.ai[1];

        VertexStrip _strip;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;

            Projectile.penetrate = 3;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = -1;

            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            TimeOutsideTiles++;
            float ticksNeeded = Projectile.velocity.Y < 0 ? 15 : 3;
            if (TimeOutsideTiles >= ticksNeeded)
                Projectile.tileCollide = true;
            if (Collision.SolidCollision(Projectile.Center, (int)(Projectile.width * 0.5f), (int)(Projectile.height * 0.5f)))
            {
                TimeOutsideTiles = 0;
            }

            Projectile.velocity.Y = MathHelper.Min(Projectile.velocity.Y + 0.5f, 20);
            if (Projectile.velocity.Y > 0)
                Projectile.velocity.X *= 0.98f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            RotationDrawing += Projectile.velocity.X * 0.05f;
        }

        public override void OnKill(int timeLeft)
        {;
            SoundEngine.PlaySound(RockBreak, Projectile.Center);

            Gore.NewGore(Projectile.Center, Vector2.Zero, ModContent.GoreType<ExodiumRockGore0>());
            Gore.NewGore(Projectile.Center, Vector2.Zero, ModContent.GoreType<ExodiumRockGore1>());
            Gore.NewGore(Projectile.Center, Vector2.Zero, ModContent.GoreType<ExodiumRockGore2>());
            Gore.NewGore(Projectile.Center, Vector2.Zero, ModContent.GoreType<ExodiumRockGore3>());

            for (int i = 0; i < 3; i++)
                ParticleSystem.SpawnParticle(
                    type: ParticleLoader.ParticleType<ExodiumGlowDot>(),
                    position: Projectile.Center,
                    velocity: Projectile.velocity * 0.3f,
                    scale: new Vector2(Main.rand.NextFloat(0.9f, 1.1f) - i * 0.25f),
                    rotation: Main.rand.NextFloat(MathHelper.TwoPi),
                    color: new Color(i / 2f, 1f, 0.3f + i / 2f),
                    lifetime: Main.rand.Next(20, 30),
                    custom1: Main.rand.NextFloat(0.5f) // Decay
                    );
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D textureGlow = AssetDirectory.Textures.Accessories.Summoner.MoonStone.ExodiumRockGlow.Value;
            Main.spriteBatch.Draw(textureGlow, Projectile.Center - Main.screenPosition, textureGlow.Frame(), Color.White, RotationDrawing, textureGlow.Size() * 0.5f, Projectile.scale * 1.1f, SpriteEffects.None, 0);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), lightColor, RotationDrawing, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public void DrawPixelated()
        {
            _strip ??= new();
            Effect effect = AssetDirectory.Effects.BasicTrail.Value;
            effect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            effect.Parameters["uTexturePalette0"].SetValue(AssetDirectory.Textures.Extra.Palettes.RockTrailPalette.Value);

            Color StripColor(float p) => new(1, 1, 1, (1 - p) * 0.85f);
            float StripWidth(float p) => 16 * (1 - p);

            _strip.PrepareStrip(Projectile.oldPos, Projectile.oldRot, StripColor, StripWidth, Projectile.Size * 0.5f - Main.screenPosition, Projectile.oldPos.Length);

            effect.CurrentTechnique.Passes[0].Apply();
            _strip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }
    }

    public abstract class ExodiumRockGore : ModGore
    {
        public override void SetStaticDefaults()
        {
            ChildSafety.SafeGore[ModContent.GoreType<ExodiumRockGore0>()] = true;
            ChildSafety.SafeGore[ModContent.GoreType<ExodiumRockGore1>()] = true;
            ChildSafety.SafeGore[ModContent.GoreType<ExodiumRockGore2>()] = true;
            ChildSafety.SafeGore[ModContent.GoreType<ExodiumRockGore3>()] = true;
        }
    }
    public class ExodiumRockGore0 : ExodiumRockGore { }
    public class ExodiumRockGore1 : ExodiumRockGore { }
    public class ExodiumRockGore2 : ExodiumRockGore { }
    public class ExodiumRockGore3 : ExodiumRockGore { }
}
