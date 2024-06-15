/*
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Whipcackling.Core.Particles;

namespace Whipcackling.Content.Particles
{
    public class HolyConnectedBeam : ModParticle
    {
        public override BlendState BlendMode => BlendState.Additive;

        public override void Update(ref Particle particle)
        {
            float decay = particle.Custom[0];

            NPC npc1 = Main.npc[(int)particle.Custom[1]];
            NPC npc2 = Main.npc[(int)particle.Custom[2]];
            particle.Position += Vector2.Lerp(npc1.position, npc2.position, 0.5f) - Vector2.Lerp(npc1.oldPosition, npc2.oldPosition, 0.5f);


            if (particle.Progress > decay)
            {
                particle.Scale = Vector2.Max(particle.Scale - new Vector2(0.1f, 0), Vector2.Zero);
                Color color = particle.Color;
                color.A = (byte)Math.Max(color.A * 0.85f, 0);
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