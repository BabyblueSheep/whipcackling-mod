using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whipcackling.Core.Particles;

namespace Whipcackling.Content.Particles
{
    public class StratusWhipAfterImage : ModParticle
    {
        public override BlendState BlendMode => BlendState.NonPremultiplied;

        public override void Update(ref Particle particle)
        {
            Color color = particle.Color;
            color.A = (byte)Math.Max(color.A - 26, 0);
            particle.Color = color;
        }
    }
}
