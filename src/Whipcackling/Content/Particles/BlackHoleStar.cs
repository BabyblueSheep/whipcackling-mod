using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Whipcackling.Core.Particles;

namespace Whipcackling.Content.Particles
{
    public class BlackHoleStar : ModParticle
    {
        public override BlendState BlendMode => BlendState.Additive;

        public override void Update(ref Particle particle)
        {
            float originX = particle.Custom[0];
            float originY = particle.Custom[1];
            float angle = particle.Custom[2];

            // Move closer to origin point.
            particle.Position.X += (originX - particle.Position.X) * 0.05f;
            particle.Position.Y += (originY - particle.Position.Y) * 0.05f;

            
            // Rotate around origin point
            float sin = (float)Math.Sin(angle); 
            float cos = (float)Math.Cos(angle);

            particle.Position.X -= originX;
            particle.Position.Y -= originY;

            float newX = particle.Position.X * cos - particle.Position.Y * sin;
            float newY = particle.Position.X * sin + particle.Position.Y * cos;

            particle.Position.X = newX + originX;
            particle.Position.Y = newY + originY;

            particle.Rotation += angle * 2;
            particle.Scale *= 0.95f;
        }

        public override Color GetColor(Particle particle, Color lightColor)
        {
            particle.Color.A = (byte)(255 * Utils.GetLerpValue(0, 5, particle.Time, true) * Utils.GetLerpValue(particle.Lifetime, particle.Lifetime - 5, particle.Time, true));
            return particle.Color;
        }
    }
}
