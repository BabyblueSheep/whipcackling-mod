using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Whipcackling.Core.Particles;

namespace Whipcackling.Content.Particles
{
    public class MeldGlowDot : ModParticle
    {
        public override BlendState BlendMode => BlendState.Additive;

        public override void Update(ref Particle particle)
        {
            float decay = particle.Custom[0];

            particle.Velocity *= 0.95f;
            if (particle.Progress > decay)
            {
                particle.Scale = Vector2.Max(particle.Scale - new Vector2(0.05f), Vector2.Zero);
            }

            particle.Rotation = particle.Velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override Color GetColor(Particle particle, Color lightColor)
        {
            return particle.Color;
        }
    }
}
