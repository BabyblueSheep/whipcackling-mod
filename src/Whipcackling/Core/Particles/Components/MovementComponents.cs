using Arch.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Whipcackling.Core.Particles.Components
{
    public struct LinearVelocity
    {
        public float VelocityX;
        public float VelocityY;

        public float AccelerationX;
        public float AccelerationY;
    }

    public struct UpdateLinearVelocity : IForEach<Position, LinearVelocity>
    {
        public static QueryDescription Query => new QueryDescription().WithAll<Position, LinearVelocity>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Update(ref Position position, ref LinearVelocity velocity)
        {
            position.X += velocity.VelocityX;
            position.Y += velocity.VelocityY;

            velocity.VelocityX += velocity.AccelerationX;
            velocity.VelocityY += velocity.AccelerationY;
        }
    }
}
