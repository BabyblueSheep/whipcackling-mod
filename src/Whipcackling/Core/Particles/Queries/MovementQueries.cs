using Arch.Core;
using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;
using Whipcackling.Common.Utilities;
using Whipcackling.Core.Particles.Components;

namespace Whipcackling.Core.Particles.Queries
{
    public struct UpdateLinearVelocity : IForEach<Position, LinearVelocityAcceleration>
    {
        public static QueryDescription Query => new QueryDescription().WithAll<Position, LinearVelocityAcceleration>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Update(ref Position position, ref LinearVelocityAcceleration velocity)
        {
            position.X += velocity.VelocityX;
            position.Y += velocity.VelocityY;

            velocity.VelocityX += velocity.LinearAccelerationX;
            velocity.VelocityY += velocity.LinearAccelerationY;
            velocity.VelocityX *= velocity.ExponentialAccelerationX;
            velocity.VelocityY *= velocity.ExponentialAccelerationY;
        }
    }

    public struct UpdateAngularVelocityMoveToTarget : IForEach<Position, AngularVelocityMoveToTarget>
    {
        public static QueryDescription Query => new QueryDescription().WithAll<Position, AngularVelocityMoveToTarget>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(ref Position position, ref AngularVelocityMoveToTarget velocity)
        {
            position.X += (velocity.TargetX - position.X) * velocity.PositionDifference;
            position.Y += (velocity.TargetY - position.Y) * velocity.PositionDifference;

            float sin = MathF.Sin(velocity.AngleChange);
            float cos = MathF.Cos(velocity.AngleChange);

            position.X -= velocity.TargetX;
            position.Y -= velocity.TargetY;

            float newX = position.X * cos - position.Y * sin;
            float newY = position.X * sin + position.Y * cos;

            position.X = newX + velocity.TargetX;
            position.Y = newY + velocity.TargetY;
        }
    }

    public struct UpdateInnacurateHomeOnTarget : IForEach<Position, LinearVelocityAcceleration, InnacurateHomeOnTarget>
    {
        public static QueryDescription Query => new QueryDescription().WithAll<Position, LinearVelocityAcceleration, InnacurateHomeOnTarget>();

        public void Update(ref Position position, ref LinearVelocityAcceleration velocity, ref InnacurateHomeOnTarget home)
        {
            Vector2 speed = HelperMethods.RotateTowards((Vector2)(velocity), (Vector2)(position), home.Target, home.AngleChange);
            velocity.VelocityX = speed.X;
            velocity.VelocityY = speed.Y;
        }
    }
}
