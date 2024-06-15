/*
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
    public class HolyGlowLine : ModParticle
    {
        public override BlendState BlendMode => BlendState.Additive;

        public override void Update(ref Particle particle)
        {
            float decay = particle.Custom[0];
            int npcID1 = (int)particle.Custom[1];
            int npcID2 = (int)particle.Custom[1];


            if (particle.Progress > decay)
            {
                particle.Position -= (particle.Rotation - MathHelper.PiOver2).ToRotationVector2() * 5f;
                particle.Scale = Vector2.Max(particle.Scale - new Vector2(0.05f, 0.2f), Vector2.Zero);
                Color color = particle.Color;
                color.A = (byte)Math.Max(color.A * 0.7f, 0);
                particle.Color = color;
            }
        }

        public override Color GetColor(Particle particle, Color lightColor)
        {
            return particle.Color;
        }
    }
}
*/