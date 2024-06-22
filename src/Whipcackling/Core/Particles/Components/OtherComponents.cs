using Arch.Core;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using Terraria;

namespace Whipcackling.Core.Particles.Components
{
    public struct UVCoordinates(int x, int y, int width, int height)
    {
        public readonly Vector2 Position => new(X, Y);
        public readonly Vector2 Size => new(Width, Height);

        public static explicit operator Vector4(UVCoordinates uv)
        {
            return new Vector4(uv.X, uv.Y, uv.Width, uv.Height);
        }

        public static explicit operator UVCoordinates(Vector4 vector)
        {
            return new UVCoordinates((int)vector.X, (int)vector.Y, (int)vector.Z, (int)vector.W);
        }

        public static explicit operator Rectangle(UVCoordinates uv)
        {
            return new Rectangle(uv.X, uv.Y, uv.Width, uv.Height);
        }

        public static explicit operator UVCoordinates(Rectangle vector)
        {
            return new UVCoordinates(vector.X, vector.Y, vector.Width, vector.Height);
        }

        public int X = x;
        public int Y = y;
        public int Width = width;
        public int Height = height;
    }

    public struct TimeLeft(int totalTime)
    {
        public int TotalTime = totalTime;
        public int Time = totalTime;
    }

    public struct TimeUntilAction(int time)
    {
        public int Time = time;
    }
}
