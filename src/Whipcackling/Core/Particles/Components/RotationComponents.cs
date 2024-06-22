using Arch.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Whipcackling.Core.Particles.Components
{
    public struct Rotation(float angle)
    {
        public float Angle = angle;

        public static explicit operator float(Rotation rotation)
        {
            return rotation.Angle;
        }

        public static explicit operator Rotation(float angle)
        {
            return new Rotation(angle);
        }
    }

    public struct LinearRotationChange(float angle)
    {
        public float Angle = angle;
    }

    public struct RotateWithLinearVelocity(float amountX, float amountY)
    {
        public float AmountX = amountX;
        public float AmountY = amountY;
    }

    public struct RotationIsVelocity { }
}
