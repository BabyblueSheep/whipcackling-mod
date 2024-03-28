using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Whipcackling.Assets
{
    public static class AssetDirectory
    {
        public static readonly string AssetPath = $"{nameof(Whipcackling)}/Assets/";

        public static class Textures
        {
            public static readonly Asset<Texture2D> Empty = ModContent.Request<Texture2D>($"{AssetPath}Textures/Empty", AssetRequestMode.ImmediateLoad);
            public static readonly Asset<Texture2D> Pixel = ModContent.Request<Texture2D>($"{AssetPath}Textures/Pixel", AssetRequestMode.ImmediateLoad);

            public static class Extra
            {


                public static class Noise
                {
                    public static readonly Asset<Texture2D> BlurNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/BlurNoise", AssetRequestMode.ImmediateLoad);
                    public static readonly Asset<Texture2D> BlurredDarkPerlinNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/BlurredDarkPerlinNoise", AssetRequestMode.ImmediateLoad);
                    public static readonly Asset<Texture2D> BlurredPerlinNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/BlurredPerlinNoise", AssetRequestMode.ImmediateLoad);
                    public static readonly Asset<Texture2D> CellInvertedNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/CellInvertedNoise", AssetRequestMode.ImmediateLoad);
                    public static readonly Asset<Texture2D> CellNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/CellNoise", AssetRequestMode.ImmediateLoad);
                    public static readonly Asset<Texture2D> CellPackedNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/CellPackedNoise", AssetRequestMode.ImmediateLoad);
                    public static readonly Asset<Texture2D> CirclyNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/CirclyNoise", AssetRequestMode.ImmediateLoad);
                    public static readonly Asset<Texture2D> GassyNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/GassyNoise", AssetRequestMode.ImmediateLoad);
                    public static readonly Asset<Texture2D> GoopyNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/GoopyNoise", AssetRequestMode.ImmediateLoad);
                    public static readonly Asset<Texture2D> PerlinNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/PerlinNoise", AssetRequestMode.ImmediateLoad);
                    public static readonly Asset<Texture2D> SaturatedGassyNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/SaturatedGassyNoise", AssetRequestMode.ImmediateLoad);
                    public static readonly Asset<Texture2D> TechyNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/TechyNoise", AssetRequestMode.ImmediateLoad);
                    public static readonly Asset<Texture2D> TechySaturatedNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/TechySaturatedNoise", AssetRequestMode.ImmediateLoad);
                    public static readonly Asset<Texture2D> WobblyEnergyNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/WobblyEnergyNoise", AssetRequestMode.ImmediateLoad);
                    public static readonly Asset<Texture2D> WobblyNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/WobblyNoise", AssetRequestMode.ImmediateLoad);
                    public static readonly Asset<Texture2D> WobblyWeakEnergyNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/WobblyWeakEnergyNoise", AssetRequestMode.ImmediateLoad);
                }

                public static class Palettes
                {
                    public static readonly Asset<Texture2D> AcidFlamePaletteHue = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/AcidFlamePaletteHue", AssetRequestMode.ImmediateLoad);
                    public static readonly Asset<Texture2D> AuraBlackHolePaletteHue = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/AuraBlackHolePaletteHue", AssetRequestMode.ImmediateLoad);
                    public static readonly Asset<Texture2D> InnerBlackHolePaletteValue = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/InnerBlackHolePaletteValue", AssetRequestMode.ImmediateLoad);
                    public static readonly Asset<Texture2D> OuterBlackHolePaletteValue = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/OuterBlackHolePaletteValue", AssetRequestMode.ImmediateLoad);
                    public static readonly Asset<Texture2D> AcidFlameTrailPaletteHue = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/AcidFlameTrailPaletteHue", AssetRequestMode.ImmediateLoad);
                    public static readonly Asset<Texture2D> MeldFlamePaletteHue = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/MeldFlamePaletteHue", AssetRequestMode.ImmediateLoad);
                    public static readonly Asset<Texture2D> MeldFlamePaletteValue = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/MeldFlamePaletteValue", AssetRequestMode.ImmediateLoad);
                    public static readonly Asset<Texture2D> ShockwavePalette = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/ShockwavePalette", AssetRequestMode.ImmediateLoad);
                    public static readonly Asset<Texture2D> ToenailFlameTrailPaletteHue = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/ToenailFlameTrailPaletteHue", AssetRequestMode.ImmediateLoad);
                }
            }
        }

        public static class Effects
        {
            public static readonly Asset<Effect> BlackHole = ModContent.Request<Effect>($"{AssetPath}Effects/BlackHole", AssetRequestMode.ImmediateLoad);
            public static readonly Asset<Effect> FlameTrail = ModContent.Request<Effect>($"{AssetPath}Effects/FlameTrail", AssetRequestMode.ImmediateLoad);
            public static readonly Asset<Effect> MeldOutline = ModContent.Request<Effect>($"{AssetPath}Effects/MeldOutline", AssetRequestMode.ImmediateLoad);
            public static readonly Asset<Effect> NegazoneEffect = ModContent.Request<Effect>($"{AssetPath}Effects/NegazoneEffect", AssetRequestMode.ImmediateLoad);
            public static readonly Asset<Effect> Particle = ModContent.Request<Effect>($"{AssetPath}Effects/Particle", AssetRequestMode.ImmediateLoad);
            public static readonly Asset<Effect> Pixelise = ModContent.Request<Effect>($"{AssetPath}Effects/Pixelise", AssetRequestMode.ImmediateLoad);
            public static readonly Asset<Effect> ShockwaveTrail = ModContent.Request<Effect>($"{AssetPath}Effects/ShockwaveTrail", AssetRequestMode.ImmediateLoad);
            public static readonly Asset<Effect> WhipSwingTrail = ModContent.Request<Effect>($"{AssetPath}Effects/WhipSwingTrail", AssetRequestMode.ImmediateLoad);
        }
    }
}
