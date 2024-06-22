using Arch.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Whipcackling.Core.Particles.Components;

namespace Whipcackling.Core.Particles.Queries
{
    public struct UpdateLinearRotationChange : IForEach<Rotation, LinearRotationChange>
    {
        public static QueryDescription Query => new QueryDescription().WithAll<Rotation, LinearRotationChange>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Update(ref Rotation rotation, ref LinearRotationChange change)
        {
            rotation.Angle += change.Angle;
        }
    }

    public struct UpdateRotateWithLinearVelocity : IForEach<Rotation, LinearVelocityAcceleration, RotateWithLinearVelocity>
    {
        public static QueryDescription Query => new QueryDescription().WithAll<Rotation, LinearVelocityAcceleration, RotateWithLinearVelocity>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Update(ref Rotation rotation, ref LinearVelocityAcceleration velocity, ref RotateWithLinearVelocity value)
        {
            rotation.Angle += velocity.VelocityX * value.AmountX;
            rotation.Angle += velocity.VelocityX * value.AmountY;
        }
    }

    public struct UpdateRotationIsVelocity : IForEach<Rotation, LinearVelocityAcceleration, RotationIsVelocity>
    {
        public static QueryDescription Query => new QueryDescription().WithAll<Rotation, LinearVelocityAcceleration, RotationIsVelocity>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Update(ref Rotation rotation, ref LinearVelocityAcceleration velocity, ref RotationIsVelocity value)
        {
            rotation.Angle = MathF.Atan2(velocity.VelocityY, velocity.VelocityX) + MathHelper.PiOver2;
        }
    }
}
