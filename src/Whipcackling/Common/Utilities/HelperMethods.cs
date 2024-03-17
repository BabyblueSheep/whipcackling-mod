using CalamityMod;
using CalamityMod.Dusts;
using CalamityMod.Systems;
using CalamityMod.Tiles.PlayerTurrets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Whipcackling.Common.Utilities
{
    public static class HelperMethods
    {
        /// <summary>
        /// Shorthand to get a parameter of an <see cref="Effect"/>.
        /// </summary>
        /// <param name="shader">The shader/effect to use.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The parameter.</returns>
        public static EffectParameter GetParameter(this Filter shader, string name)
        {
            return shader.GetShader().Shader.Parameters[name];
        }

        /// <summary>
        /// Takes a color at a point from a manually inputted gradient.
        /// </summary>
        /// <param name="progress">The point from where to grab the color.</param>
        /// <param name="colors">The gradient of colors.</param>
        /// <returns>The color.</returns>
        public static Color GetGradient(float progress, params Color[] colors)
        {
            int length = colors.Length;

            float time = (progress * length) % 1;
            int index = (int)Math.Floor(Utils.GetLerpValue(0f, 1f / length, progress));

            return Color.Lerp(colors[Math.Min(index, length - 1)], colors[Math.Min(index + 1, length - 1)], time);
        }

        /// <summary>
        /// A linear interpolation function for <see cref="Vector2"/>s.
        /// </summary>
        /// <param name="value1">The first point.</param>
        /// <param name="value2">The second point.</param>
        /// <param name="amount">The interpolation amount.</param>
        /// <returns>The interpolated result.</returns>
        public static Vector2 LerpRectangular(Vector2 value1, Vector2 value2, Vector2 amount)
        {
            return new Vector2(MathHelper.Lerp(value1.X, value2.X, amount.X), MathHelper.Lerp(value1.Y, value2.Y, amount.Y));
        }
    }
}
