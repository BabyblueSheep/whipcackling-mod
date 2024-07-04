using System;

namespace Whipcackling.Common.Utilities
{
    // Taken from https://easings.net/
    public static class Easings
    {
        public static float InSine(float x)
        {
            return 1 - MathF.Cos((x * MathF.PI) / 2f);
        }

        public static float OutSine(float x)
        {
            return MathF.Sin((x * MathF.PI) / 2f);
        }

        public static float InOutSine(float x)
        {
            return -(MathF.Cos(MathF.PI * x) - 1) / 2f;
        }

        public static float InCubic(float x)
        {
            return x * x * x;
        }

        public static float OutCubic(float x)
        {
            return 1 - MathF.Pow(1 - x, 3);
        }

        public static float InOutCubic(float x)
        {
            return x < 0.5f ? 4 * x * x * x : 1 - MathF.Pow(-2 * x + 2, 3) / 2f;
        }

        public static float InQuint(float x)
        {
            return x * x * x * x * x;
        }

        public static float OutQuint(float x)
        {
            return 1 - MathF.Pow(1 - x, 5);
        }

        public static float InOutQuint(float x)
        {
            return x < 0.5f ? 16 * x * x * x * x * x : 1 - MathF.Pow(-2 * x + 2, 5) / 2f;
        }

        public static float InCirc(float x)
        {
            return 1 - MathF.Sqrt(1 - MathF.Pow(x, 2));
        }

        public static float OutCirc(float x)
        {
            return MathF.Sqrt(1 - MathF.Pow(x - 1, 2));
        }

        public static float InOutCirc(float x)
        {
            return x < 0.5f
            ? (1 - MathF.Sqrt(1 - MathF.Pow(2 * x, 2))) / 2f
            : (MathF.Sqrt(1 - MathF.Pow(-2 * x + 2, 2)) + 1) / 2f;
        }

        public static float InElastic(float x)
        {
            float c4 = (2 * MathF.PI) / 3;

            return x == 0
              ? 0
              : x == 1
              ? 1
              : -MathF.Pow(2, 10 * x - 10) * MathF.Sin((x * 10 - 10.75f) * c4);
        }

        public static float OutElastic(float x)
        {
            float c4 = (2 * MathF.PI) / 3;

            return x == 0
              ? 0
              : x == 1
              ? 1
              : MathF.Pow(2, -10 * x) * MathF.Sin((x * 10 - 0.75f) * c4) + 1;
        }

        public static float InOutElastic(float x)
        {
            float c5 = (2 * MathF.PI) / 4.5f;

            return (float)(x == 0
              ? 0
              : x == 1
              ? 1
              : x < 0.5f
              ? -(MathF.Pow(2, 20 * x - 10) * MathF.Sin((20 * x - 11.125f) * c5)) / 2f
              : MathF.Pow(2, -20 * x + 10) * MathF.Sin((20 * x - 11.125f) * c5)) / 2f + 1;
        }

        public static float InQuad(float x)
        {
            return x * x;
        }

        public static float OutQuad(float x)
        {
            return 1 - (1 - x) * (1 - x);
        }

        public static float InOutQuad(float x)
        {
            return x < 0.5 ? 2 * x * x : 1 - MathF.Pow(-2 * x + 2, 2) / 2f;
        }

        public static float InQuart(float x)
        {
            return x * x * x * x;
        }

        public static float OutQuart(float x)
        {
            return 1 - MathF.Pow(1 - x, 4);
        }

        public static float InOutQuart(float x)
        {
            return x < 0.5 ? 8 * x * x * x * x : 1 - MathF.Pow(-2 * x + 2, 4) / 2f;
        }

        public static float InExpo(float x)
        {
            return x == 0 ? 0 : MathF.Pow(2, 10 * x - 10);
        }

        public static float OutExpo(float x)
        {
            return x == 1 ? 1 : 1 - MathF.Pow(2, -10 * x);
        }

        public static float InOutExpo(float x)
        {
            return (float)(x == 0
            ? 0
            : x == 1
            ? 1
            : x < 0.5 ? MathF.Pow(2, 20 * x - 10) / 2f
            : 2 - MathF.Pow(2, -20 * x + 10)) / 2f;
        }

        public static float InBack(float x)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1;

            return c3 * x * x * x - c1 * x * x;
        }

        public static float OutBack(float x)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1;

            return 1 + c3 * MathF.Pow(x - 1, 3) + c1 * MathF.Pow(x - 1, 2);
        }

        public static float InOutBack(float x)
        {
            float c1 = 1.70158f;
            float c2 = c1 * 1.525f;

            return x < 0.5f
              ? MathF.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2) / 2f
              : (MathF.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2f;
        }

        public static float InBounce(float x)
        {
            return 1 - OutBounce(1 - x);
        }

        public static float OutBounce(float x)
        {
            float n1 = 7.5625f;
            float d1 = 2.75f;

            if (x < 1 / d1)
            {
                return n1 * x * x;
            }
            else if (x < 2 / d1)
            {
                return n1 * (x - 1.5f / d1) * x + 0.75f;
            }
            else if (x < 2.5 / d1)
            {
                return n1 * (x - 2.25f / d1) * x + 0.9375f;
            }
            else
            {
                return n1 * (x - 2.625f / d1) * x + 0.984375f;
            }
        }

        public static float InOutBounce(float x)
        {
            return x < 0.5f
            ? (1 - OutBounce(1 - 2 * x)) / 2
            : (1 + OutBounce(2 * x - 1)) / 2;
        }
    }
}
