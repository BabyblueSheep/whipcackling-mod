using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Whipcackling.Assets;
using Whipcackling.Common.Systems.Drawing;
using Whipcackling.Common.Utilities;

namespace Whipcackling.Content.Accessories.Summoner.MoonStone
{
    public class ExodiumBeam : ModProjectile, IDrawPixelated
    {
        public override string LocalizationCategory => "Accessories.MoonStone";

        VertexStrip _strip;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.timeLeft = 720;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 5;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;

            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            int target = Projectile.FindTargetIgnoreCollision();
            if (target == -1)
                return;
            NPC npc = Main.npc[target];

            if (npc.active)
            {
                Projectile.velocity += Projectile.DirectionTo(npc.Center) * 0.5f;
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 3f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public void DrawPixelated()
        {
            _strip ??= new();
            Effect effect = AssetDirectory.Effects.VortexTrail.Value;
            effect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            effect.Parameters["uTextureNoise0"].SetValue(AssetDirectory.Textures.Extra.SineGradient.Value);
            effect.Parameters["uTextureNoise1"].SetValue(AssetDirectory.Textures.Extra.Noise.BlurredDarkPerlinNoise.Value);
            effect.Parameters["uTexturePalette0"].SetValue(AssetDirectory.Textures.Extra.Palettes.ExodiumBeamPalette.Value);

            Color StripColor(float p) => new Color(1, 1, 1, Projectile.timeLeft / 720f);
            float StripWidth(float p) => 32 * (1 - p) * Projectile.timeLeft / 720f;

            _strip.PrepareStrip(Projectile.oldPos, Projectile.oldRot, StripColor, StripWidth, Projectile.Size * 0.5f - Main.screenPosition, Projectile.oldPos.Length);

            effect.CurrentTechnique.Passes[0].Apply();
            _strip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }
    }
}
