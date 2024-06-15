using Microsoft.Xna.Framework;
using Terraria;

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

    public struct UVCoordinates(float x, float y, float width, float height)
    {
        public readonly Vector2 Position => new(X, Y);
        public readonly Vector2 Size => new(Width, Height);

        public static explicit operator Vector4(UVCoordinates uv)
        {
            return new Vector4(uv.X, uv.Y, uv.Width, uv.Height);
        }

        public static explicit operator UVCoordinates(Vector4 vector)
        {
            return new UVCoordinates(vector.X,vector.Y, vector.Z, vector.W);
        }

        public float X = x;
        public float Y = y;
        public float Width = width;
        public float Height = height;
    }
}
