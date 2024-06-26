using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace Whipcackling.Assets
{
    public static class AssetDirectory
    {
        public static readonly string AssetPath = $"{nameof(Whipcackling)}/Assets/";

        public static class Textures
        {
            public static readonly Asset<Texture2D> Empty = ModContent.Request<Texture2D>($"{AssetPath}Textures/Empty");
            public static readonly Asset<Texture2D> Pixel = ModContent.Request<Texture2D>($"{AssetPath}Textures/Pixel");

            public static class Accessories
            {
                public static class Summoner
                {
                    public static class MoonStone
                    {
                        public static readonly Asset<Texture2D> ExodiumRockGlow = ModContent.Request<Texture2D>($"{AssetPath}Textures/Accessories/Summoner/MoonStone/ExodiumRockGlow");
                    }
                }
            }

            public static class Whips
            {
                public static class BloodstoneWhip
                {
                    public static readonly Asset<Texture2D> BloodstoneSkull = ModContent.Request<Texture2D>($"{AssetPath}Textures/Whips/BloodstoneWhip/BloodstoneSkull");
                }
            }

            public static class Extra
            {
                public static readonly Asset<Texture2D> SineGradient = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/SineGradient");
                public static readonly Asset<Texture2D> BlackHoleDither = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/BlackHoleDither");

                public static class Noise
                {
                    public static readonly Asset<Texture2D> BlurNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/BlurNoise");
                    public static readonly Asset<Texture2D> BlurredDarkPerlinNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/BlurredDarkPerlinNoise");
                    public static readonly Asset<Texture2D> BlurredPerlinNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/BlurredPerlinNoise");
                    public static readonly Asset<Texture2D> CellInvertedNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/CellInvertedNoise");
                    public static readonly Asset<Texture2D> CellNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/CellNoise");
                    public static readonly Asset<Texture2D> CellPackedNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/CellPackedNoise");
                    public static readonly Asset<Texture2D> CirclyNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/CirclyNoise");
                    public static readonly Asset<Texture2D> GassyNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/GassyNoise");
                    public static readonly Asset<Texture2D> GoopyNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/GoopyNoise");
                    public static readonly Asset<Texture2D> PerlinNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/PerlinNoise");
                    public static readonly Asset<Texture2D> SaturatedGassyNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/SaturatedGassyNoise");
                    public static readonly Asset<Texture2D> TechyNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/TechyNoise");
                    public static readonly Asset<Texture2D> TechySaturatedNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/TechySaturatedNoise");
                    public static readonly Asset<Texture2D> WobblyEnergyNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/WobblyEnergyNoise");
                    public static readonly Asset<Texture2D> WobblyNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/WobblyNoise");
                    public static readonly Asset<Texture2D> WobblyWeakEnergyNoise = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Noise/WobblyWeakEnergyNoise");
                }

                public static class Palettes
                {
                    public static readonly Asset<Texture2D> AcidFlamePaletteHue = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/AcidFlamePaletteHue");
                    public static readonly Asset<Texture2D> AuraBlackHolePaletteHue = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/AuraBlackHolePaletteHue");
                    public static readonly Asset<Texture2D> ExodiumHealPaletteAlpha = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/ExodiumHealPaletteAlpha");
                    public static readonly Asset<Texture2D> OuterBlackHolePaletteValue = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/OuterBlackHolePaletteValue");
                    public static readonly Asset<Texture2D> AcidFlameTrailPaletteHue = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/AcidFlameTrailPaletteHue");
                    public static readonly Asset<Texture2D> MeldFlamePaletteHue = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/MeldFlamePaletteHue");
                    public static readonly Asset<Texture2D> MeldFlamePaletteValue = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/MeldFlamePaletteValue");
                    public static readonly Asset<Texture2D> ShockwavePaletteAlpha = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/ShockwavePaletteAlpha");
                    public static readonly Asset<Texture2D> ShockwavePalette = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/ShockwavePalette");
                    public static readonly Asset<Texture2D> ToenailFlameTrailPaletteHue = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/ToenailFlameTrailPaletteHue");
                    public static readonly Asset<Texture2D> RockTrailPalette = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/RockTrailPalette");
                    public static readonly Asset<Texture2D> ExodiumBeamPalette = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/ExodiumBeamPalette");
                    public static readonly Asset<Texture2D> LunarTrailPaletteHue = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/LunarTrailPaletteHue");
                    public static readonly Asset<Texture2D> RedStarHueTrail = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/RedStarHueTrail");
                    public static readonly Asset<Texture2D> BlueStarHueTrail = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/BlueStarHueTrail");
                    public static readonly Asset<Texture2D> WhiteStarHueTrail = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/WhiteStarHueTrail");
                    public static readonly Asset<Texture2D> PurpleStarHueTrail = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/PurpleStarHueTrail");
                    public static readonly Asset<Texture2D> YellowStarHueTrail = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/YellowStarHueTrail");
                    public static readonly Asset<Texture2D> BlackHoleStrip = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/BlackHoleStrip");
                    public static readonly Asset<Texture2D> BloodyTrailPaletteHue = ModContent.Request<Texture2D>($"{AssetPath}Textures/Extra/Palettes/BloodyTrailPaletteHue");
                }
            }
        }

        public static class Effects
        {
            public static readonly Asset<Effect> BasicTrail = ModContent.Request<Effect>($"{AssetPath}Effects/BasicTrail");
            public static readonly Asset<Effect> BlackHole = ModContent.Request<Effect>($"{AssetPath}Effects/BlackHole");
            public static readonly Asset<Effect> BlackHoleStripTrail = ModContent.Request<Effect>($"{AssetPath}Effects/BlackHoleStripTrail");
            public static readonly Asset<Effect> CalamitousOutline = ModContent.Request<Effect>($"{AssetPath}Effects/CalamitousOutline");
            public static readonly Asset<Effect> FieryOutline = ModContent.Request<Effect>($"{AssetPath}Effects/FieryOutline");
            public static readonly Asset<Effect> FlameTrail = ModContent.Request<Effect>($"{AssetPath}Effects/FlameTrail");
            public static readonly Asset<Effect> MeldOutline = ModContent.Request<Effect>($"{AssetPath}Effects/MeldOutline");
            public static readonly Asset<Effect> MoonOutline = ModContent.Request<Effect>($"{AssetPath}Effects/MoonOutline");
            public static readonly Asset<Effect> NegazoneEffect = ModContent.Request<Effect>($"{AssetPath}Effects/NegazoneEffect");
            public static readonly Asset<Effect> Particle = ModContent.Request<Effect>($"{AssetPath}Effects/Particle");
            public static readonly Asset<Effect> ParticleOld = ModContent.Request<Effect>($"{AssetPath}Effects/ParticleOld");
            public static readonly Asset<Effect> Pixelise = ModContent.Request<Effect>($"{AssetPath}Effects/Pixelise");
            public static readonly Asset<Effect> ShockwaveTrail = ModContent.Request<Effect>($"{AssetPath}Effects/ShockwaveTrail");
            public static readonly Asset<Effect> TriangleTrail = ModContent.Request<Effect>($"{AssetPath}Effects/TriangleTrail");
            public static readonly Asset<Effect> VortexTrail = ModContent.Request<Effect>($"{AssetPath}Effects/VortexTrail");
            public static readonly Asset<Effect> WhipSwingTrail = ModContent.Request<Effect>($"{AssetPath}Effects/WhipSwingTrail");
        }
    }
}
