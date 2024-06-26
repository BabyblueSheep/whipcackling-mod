using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Whipcackling.Assets;
using Whipcackling.Common.Systems.Drawing;
using Whipcackling.Common.Utilities;

namespace Whipcackling.Content.Whips.MeldWhip
{
    public class BlackHoleStrip : ModProjectile, IDrawPixelated
    {
        public override string LocalizationCategory => "Whips.MeldWhip";

        VertexStrip _strip;
        Vector2 _targetPosition;

        public ref float TargetX => ref Projectile.ai[0];
        public ref float TargetY => ref Projectile.ai[1];

        public bool ShouldStop
        {
            get => Projectile.localAI[0] == 1;
            set
            {
                Projectile.localAI[0] = value ? 1 : 0;
            }
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 360;

            Projectile.friendly = false;
            Projectile.hostile = false;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (ShouldStop)
                return;
            Vector2 difference = Projectile.position - new Vector2(TargetX, TargetY);
            if (MathF.Abs(difference.X) < 20 && MathF.Abs(difference.Y) < 20)
            {
                ShouldStop = true;
                Projectile.velocity = Vector2.Zero;
                return;
            }
            Projectile.velocity = HelperMethods.RotateTowards(Projectile.velocity, Projectile.position, new Vector2(TargetX, TargetY), 0.05f);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public void DrawPixelated()
        {
            _strip ??= new();

            Effect effect = AssetDirectory.Effects.BlackHoleStripTrail.Value;
            effect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly + Projectile.whoAmI * 0.5f);
            effect.Parameters["uTexturePalette0"].SetValue(AssetDirectory.Textures.Extra.Palettes.BlackHoleStrip.Value);
            effect.Parameters["uTexture0"].SetValue(AssetDirectory.Textures.Extra.Noise.BlurNoise.Value);

            Color StripColor(float p) => new Color(1, 1, 1, 1);
            float StripWidth(float p) => 16;

            _strip.PrepareStrip(Projectile.oldPos, Projectile.oldRot, StripColor, StripWidth, Projectile.Size * 0.5f - Main.screenPosition, Projectile.oldPos.Length);

            effect.CurrentTechnique.Passes[0].Apply();
            _strip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }
    }
}
