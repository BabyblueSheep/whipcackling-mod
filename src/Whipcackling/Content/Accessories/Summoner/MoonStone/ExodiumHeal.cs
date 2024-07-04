using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;
using Whipcackling.Assets;
using Whipcackling.Common.Systems.Drawing;
using Whipcackling.Common.Utilities;

namespace Whipcackling.Content.Accessories.Summoner.MoonStone
{
    public class ExodiumHeal : ModProjectile, IDrawPixelated
    {
        public override string LocalizationCategory => "Accessories.MoonStone";

        public float Timer => TotalTime - Projectile.timeLeft;
        public float TimeCompletion => Timer / TotalTime;
        public ref float TotalTime => ref Projectile.localAI[0];

        public bool Initialized
        {
            get => Projectile.localAI[1] > 0;
            set => Projectile.localAI[1] = value ? 1 : 0;
        }

        VertexStrip _strip;
        private FastNoiseLite _noise;

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (!Initialized)
            {
                _noise = new(Main.rand.Next(99999));
                _noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);

                Projectile.timeLeft = (int)Projectile.ai[0];
                TotalTime = Projectile.timeLeft;

                Initialized = true;
            }
            _noise.SetFrequency(MathHelper.Lerp(0.6f, 0.4f, Easings.OutQuart(TimeCompletion)));
        }

        public void DrawPixelated()
        {
            if (!Initialized)
                return;
            float width = 4f;
            float size = Projectile.ai[1];

            _strip ??= new();

            Vector2[] positions = new Vector2[129];
            float[] rotations = new float[129];

            for (int i = 0; i < 129; i++)
            {
                float rad = i / 128f * MathF.PI * 2;
                Vector2 offset = new(MathF.Sin(rad), MathF.Cos(rad));
                float radOffset = 1 + _noise.GetNoise(offset.X, offset.Y) * 0.1f;

                positions[i] = Projectile.Center + offset * size * 0.5f * radOffset * Easings.OutQuart(Utils.GetLerpValue(0, TotalTime - 10, Timer, true));
                if (i > 0)
                    rotations[i] = (positions[i] - positions[i - 1]).ToRotation();
            }
            rotations[0] = rotations[128];

            Color ShockwaveColor(float p) => Color.PaleGreen;
            float ShockwaveWidth(float p) => (8 + MathF.Pow(1.5f, width)) * Utils.GetLerpValue(TotalTime - 10, 5, Timer, true) * Utils.GetLerpValue(0, 5, Timer, true);

            _strip.PrepareStrip(positions, rotations, ShockwaveColor, ShockwaveWidth, -Main.screenPosition, 129, true);

            Effect effect = AssetDirectory.Effects.ShockwaveTrail.Value;
            effect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            effect.Parameters["uOffset"].SetValue(Vector2.One);

            effect.Parameters["uTextureNoise0"].SetValue(AssetDirectory.Textures.Pixel.Value);
            effect.Parameters["uTexturePalette0"].SetValue(AssetDirectory.Textures.Extra.Palettes.ShockwavePalette.Value);
            effect.Parameters["uTexturePalette1"].SetValue(AssetDirectory.Textures.Extra.Palettes.ExodiumHealPaletteAlpha.Value);

            effect.CurrentTechnique.Passes[0].Apply();
            _strip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }
    }
}
