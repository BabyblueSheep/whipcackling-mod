using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Whipcackling.Common.Utilities;
using Whipcackling.Core.Particles;

namespace Whipcackling.Content.Particles
{
    public class MeldSmoke : ModParticle
    {
        public override BlendState BlendMode => BlendState.NonPremultiplied;

        public override int Variants => 3;

        public override void Update(ref Particle particle)
        {
            particle.Velocity.Y -= 0.05f * (1 - particle.Progress);
            particle.Velocity *= 0.95f;

            particle.Scale += new Vector2(0.02f);
            particle.Rotation += particle.Velocity.X * 0.01f;

            Color color = particle.Color;
            color.A = (byte)Math.Max(color.A - 2, 0);
            particle.Color = color;
        }

        public override Color GetColor(Particle particle, Color lightColor)
        {
            Color oldColor = new(particle.Color.R, particle.Color.G, particle.Color.B, particle.Color.A);
            Color newColor = new(30 * lightColor.R / 255, particle.Color.G * 45 / 255, 72 * lightColor.B / 255, 0);

            return Color.Lerp(oldColor, newColor, Easings.InSine(particle.Progress) + particle.Custom[0]);
        }
    }
}
