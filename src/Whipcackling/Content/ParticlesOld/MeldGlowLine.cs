/*
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Whipcackling.Core.Particles;

namespace Whipcackling.Content.Particles
{
    public class MeldGlowLine : ModParticle
    {
        public override BlendState BlendMode => BlendState.Additive;

        public override void Update(ref Particle particle)
        {
            float decay = particle.Custom[0];

            particle.Velocity *= 0.95f;
            if (particle.Progress > decay)
            {
                particle.Scale = Vector2.Max(particle.Scale - new Vector2(0.1f, 0.5f), Vector2.Zero);
                Color color = particle.Color;
                color.A = (byte)Math.Max(color.A - 5, 0);
                particle.Color = color;
            }

            particle.Rotation = particle.Velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override Color GetColor(Particle particle, Color lightColor)
        {
            return particle.Color;
        }
    }
}
*/