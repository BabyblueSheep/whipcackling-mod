using Arch.Core;
using Microsoft.Xna.Framework;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Whipcackling.Core.Particles.Components
{
    public struct Scale(float x, float y)
    {
        public float X = x; public float Y = y;

        public static explicit operator Vector2(Scale position)
        {
            return new Vector2(position.X, position.Y);
        }

        public static explicit operator Scale(Vector2 vector)
        {
            return new Scale(vector.X, vector.Y);
        }
    }

    public struct LinearScaleIncrease(float increaseX, float increaseY)
    {
        public float IncreaseX = increaseX;
        public float IncreaseY = increaseY;
    }

    public struct ExponentialScaleIncrease(float increaseX, float increaseY)
    {
        public float IncreaseX = increaseX;
        public float IncreaseY = increaseY;
    }

    public struct ScaleWithVelocityAndLinearIncrease(float increaseX, float increaseY, float speedX, float speedY)
    {
        public float LinearModifierX = increaseX;
        public float LinearModifierY = increaseY;

        public float VelocityModifierX = speedY;
        public float VelocityModifierY = speedX;
    }
}
