using Microsoft.Xna.Framework;

namespace Whipcackling.Core.Particles.Components
{
    public struct Position(float x, float y)
    {
        public float X = x; public float Y = y;

        public static explicit operator Vector2(Position position)
        {
            return new Vector2(position.X, position.Y);
        }

        public static explicit operator Position(Vector2 vector)
        {
            return new Position(vector.X, vector.Y);
        }
    }

    public struct LinearVelocityAcceleration
    {
        public float VelocityX;
        public float VelocityY;

        public float LinearAccelerationX;
        public float LinearAccelerationY;
        public float ExponentialAccelerationX;
        public float ExponentialAccelerationY;

        public static explicit operator Vector2(LinearVelocityAcceleration velocity)
        {
            return new Vector2(velocity.VelocityX, velocity.VelocityY);
        }

        public static explicit operator LinearVelocityAcceleration(Vector2 vector)
        {
            return new LinearVelocityAcceleration(vector.X, vector.Y);
        }

        public LinearVelocityAcceleration(float velocityX, float velocityY, float accelerationX = 0, float accelerationY = 0, float expAccelerationX = 1, float expAccelerationY = 1)
        {
            VelocityX = velocityX;
            VelocityY = velocityY;

            LinearAccelerationX = accelerationX;
            LinearAccelerationY = accelerationY;

            ExponentialAccelerationX = expAccelerationX;
            ExponentialAccelerationY = expAccelerationY;
        }

        public LinearVelocityAcceleration(Vector2 velocity, float accelerationX = 0, float accelerationY = 0, float expAccelerationX = 1, float expAccelerationY = 1)
        {
            VelocityX = velocity.X; 
            VelocityY = velocity.Y;

            LinearAccelerationX = accelerationX;
            LinearAccelerationY = accelerationY;

            ExponentialAccelerationX = expAccelerationX;
            ExponentialAccelerationY = expAccelerationY;
        }
    }

    public struct LinearVelocityExponentialAccelerationTimed
    {
        public float VelocityX;
        public float VelocityY;

        public float LinearAccelerationX;
        public float LinearAccelerationY;
        public float ExponentialAccelerationX;
        public float ExponentialAccelerationY;

        public LinearVelocityExponentialAccelerationTimed(float velocityX, float velocityY, float accelerationX = 0, float accelerationY = 0, float expAccelerationX = 1, float expAccelerationY = 1)
        {
            VelocityX = velocityX;
            VelocityY = velocityY;

            LinearAccelerationX = accelerationX;
            LinearAccelerationY = accelerationY;

            ExponentialAccelerationX = expAccelerationX;
            ExponentialAccelerationY = expAccelerationY;
        }

        public LinearVelocityExponentialAccelerationTimed(Vector2 velocity, float accelerationX = 0, float accelerationY = 0, float expAccelerationX = 1, float expAccelerationY = 1)
        {
            VelocityX = velocity.X;
            VelocityY = velocity.Y;

            LinearAccelerationX = accelerationX;
            LinearAccelerationY = accelerationY;

            ExponentialAccelerationX = expAccelerationX;
            ExponentialAccelerationY = expAccelerationY;
        }
    }

    public struct AngularVelocityMoveToTarget(float angle, float targetX, float targetY, float posDifference)
    {
        public float AngleChange = angle;

        public float TargetX = targetX;
        public float TargetY = targetY;

        public float PositionDifference = posDifference;
    }

    public struct InnacurateHomeOnTarget(float targetX, float targetY, float angle)
    {
        public float AngleChange = angle;

        public Vector2 Target = new Vector2(targetX, targetY);
    }
}
