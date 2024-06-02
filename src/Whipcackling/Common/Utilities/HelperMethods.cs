using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteelSeries.GameSense;
using System;
using Terraria;
using Terraria.Graphics.Effects;

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

        /// <summary>
        /// Creates dust in the pattern of a star.
        /// </summary>
        /// <param name="position">The center of the star</param>
        /// <param name="dustType">The dust ID.</param>
        /// <param name="amount">The amount of points in the star</param>
        /// <param name="density">The density of points in one line. Must be lower than <c>0</c>.</param>
        /// <param name="radius">The distance of the outer points from the center.</param>
        /// <param name="pointiness">The distance of the inner points from the center.</param>
        /// <param name="velocity">The velocity modifier of the dust.</param>
        /// <param name="dustAlpha">The alpha of the dust.</param>
        /// <param name="dustColor">The color of the dust.</param>
        public static void GenerateDustStarShape(Vector2 position, int dustType, int amount = 5, float density = 0.05f, float radius = 100, float pointiness = 0.5f, float velocity = 0.05f, int dustAlpha = 0, Color? dustColor = null)
        {
            dustColor ??= Color.White;

            float randomAngle = Main.rand.NextFloat(MathHelper.TwoPi);

            Vector2 spinningPoint = new(0, -radius);
            for (int point = 0; point < amount; point++)
            {
                Vector2 firstPoint = position + spinningPoint.RotatedBy(randomAngle + point * (MathHelper.TwoPi / amount));
                Vector2 secondPoint = position + spinningPoint.RotatedBy(randomAngle + (point + 1f) * (MathHelper.TwoPi / amount));
                Vector2 middlePoint = position + spinningPoint.RotatedBy(randomAngle + (point + 0.5f) * (MathHelper.TwoPi / amount)) * pointiness;

                for (int line = 0; line < 2; line++)
                {
                    Vector2 firstLinePoint = line == 0 ? firstPoint : middlePoint;
                    Vector2 secondLinePoint = line == 0 ? middlePoint : secondPoint;
                    for (float interpolant = 0; interpolant < 1; interpolant += density)
                    {
                        Vector2 intendedPosition = Vector2.Lerp(firstLinePoint, secondLinePoint, interpolant);
                        Vector2 direction = position - intendedPosition;
                        direction.Normalize();
                        Dust dust = Dust.NewDustPerfect(position, dustType, direction * Vector2.Distance(intendedPosition, position) * velocity, dustAlpha, (Color)dustColor);
                        dust.noGravity = true;
                    }
                }
            }
        }

        /// <summary>
        /// A copy of <see cref="Projectile.FindTargetWithLineOfSight(float)"/> with removed collision checks.
        /// </summary>
        /// <param name="projectile">The projectile instance.</param>
        /// <param name="maxRange">The maximum range to look for.</param>
        /// <returns>The index of the npc.</returns>
        public static int FindTargetIgnoreCollision(this Projectile projectile, float maxRange = 800f)
        {
            float range = maxRange;
            int result = -1;
            for (int i = 0; i < 200; i++)
            {
                NPC npc = Main.npc[i];
                bool flag = npc.CanBeChasedBy(projectile);
                if (projectile.localNPCImmunity[i] != 0)
                {
                    flag = false;
                }
                if (flag)
                {
                    float distance = projectile.Distance(Main.npc[i].Center);
                    if (distance < range)
                    {
                        range = distance;
                        result = i;
                    }
                }
            }
            return result;
        }
    }
}
