using Arch.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Whipcackling.Core.Particles.Components;

namespace Whipcackling.Core.Particles.Queries
{
    public struct UpdateLinearVelocityTimed : IForEach<Position, LinearVelocityExponentialAccelerationTimed, TimeLeft, TimeUntilAction>
    {
        public static QueryDescription Query => new QueryDescription().WithAll<Position, LinearVelocityExponentialAccelerationTimed, TimeLeft, TimeUntilAction>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Update(ref Position position, ref LinearVelocityExponentialAccelerationTimed velocity, ref TimeLeft time, ref TimeUntilAction decay)
        {
            if (time.Time > decay.Time)
                return;
            position.X += velocity.VelocityX;
            position.Y += velocity.VelocityY;

            velocity.VelocityX += velocity.LinearAccelerationX;
            velocity.VelocityY += velocity.LinearAccelerationY;
            velocity.VelocityX *= velocity.ExponentialAccelerationX;
            velocity.VelocityY *= velocity.ExponentialAccelerationY;
        }
    }
}
