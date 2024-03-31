using System;

namespace Whipcackling.Common.Utilities
{
    // Taken from https://easings.net/
    public static class Easings
    {
        public static float InSine(float x)
        {
            return 1 - (float)Math.Cos((x * Math.PI) / 2f);
        }

        public static float OutSine(float x)
        {
            return (float)Math.Sin((x * Math.PI) / 2f);
        }

        public static float InOutSine(float x)
        {
            return -(float)(Math.Cos(Math.PI * x) - 1) / 2f;
        }

        public static float InCubic(float x)
        {
            return x * x * x;
        }

        public static float OutCubic(float x)
        {
            return 1 - (float)Math.Pow(1 - x, 3);
        }

        public static float InOutCubic(float x)
        {
            return x < 0.5f ? 4 * x * x * x : 1 - (float)Math.Pow(-2 * x + 2, 3) / 2f;
        }

        public static float InQuint(float x)
        {
            return x * x * x * x * x;
        }

        public static float OutQuint(float x)
        {
            return 1 - (float)Math.Pow(1 - x, 5);
        }

        public static float InOutQuint(float x)
        {
            return x < 0.5f ? 16 * x * x * x * x * x : 1 - (float)Math.Pow(-2 * x + 2, 5) / 2f;
        }

        public static float InCirc(float x)
        {
            return 1 - (float)Math.Sqrt(1 - Math.Pow(x, 2));
        }

        public static float OutCirc(float x)
        {
            return (float)Math.Sqrt(1 - Math.Pow(x - 1, 2));
        }

        public static float InOutCirc(float x)
        {
            return x < 0.5f
            ? (1 - (float)Math.Sqrt(1 - Math.Pow(2 * x, 2))) / 2f
            : (float)(Math.Sqrt(1 - Math.Pow(-2 * x + 2, 2)) + 1) / 2f;
        }

        public static float InElastic(float x)
        {
            double c4 = (2 * Math.PI) / 3;

            return x == 0
              ? 0
              : x == 1
              ? 1
              : (float)(-Math.Pow(2, 10 * x - 10) * Math.Sin((x * 10 - 10.75) * c4));
        }

        public static float OutElastic(float x)
        {
            double c4 = (2 * Math.PI) / 3;

            return x == 0
              ? 0
              : x == 1
              ? 1
              : (float)(Math.Pow(2, -10 * x) * Math.Sin((x * 10 - 0.75) * c4) + 1);
        }

        public static float InOutElastic(float x)
        {
            double c5 = (2 * Math.PI) / 4.5;

            return (float)(x == 0
              ? 0
              : x == 1
              ? 1
              : x < 0.5f
              ? -(Math.Pow(2, 20 * x - 10) * Math.Sin((20 * x - 11.125) * c5)) / 2f
              : (Math.Pow(2, -20 * x + 10) * Math.Sin((20 * x - 11.125) * c5)) / 2f + 1);
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
            return x < 0.5 ? 2 * x * x : 1 - (float)Math.Pow(-2 * x + 2, 2) / 2f;
        }

        public static float InQuart(float x)
        {
            return x * x * x * x;
        }

        public static float OutQuart(float x)
        {
            return 1 - (float)Math.Pow(1 - x, 4);
        }

        public static float InOutQuart(float x)
        {
            return x < 0.5 ? 8 * x * x * x * x : 1 - (float)Math.Pow(-2 * x + 2, 4) / 2f;
        }

        public static float InExpo(float x)
        {
            return x == 0 ? 0 : (float)Math.Pow(2, 10 * x - 10);
        }

        public static float OutExpo(float x)
        {
            return x == 1 ? 1 : 1 - (float)Math.Pow(2, -10 * x);
        }

        public static float InOutExpo(float x)
        {
            return (float)(x == 0
            ? 0
            : x == 1
            ? 1
            : x < 0.5 ? Math.Pow(2, 20 * x - 10) / 2f
            : (2 - Math.Pow(2, -20 * x + 10)) / 2f);
        }

        public static float InBack(float x)
        {
            double c1 = 1.70158;
            double c3 = c1 + 1;

            return (float)(c3 * x * x * x - c1 * x * x);
        }

        public static float OutBack(float x)
        {
            double c1 = 1.70158;
            double c3 = c1 + 1;

            return (float)(1 + c3 * Math.Pow(x - 1, 3) + c1 * Math.Pow(x - 1, 2));
        }

        public static float InOutBack(float x)
        {
            double c1 = 1.70158;
            double c2 = c1 * 1.525;

            return (float)(x < 0.5
              ? (Math.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
              : (Math.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2);
        }

        public static float InBounce(float x)
        {
            return 1 - OutBounce(1 - x);
        }

        public static float OutBounce(float x)
        {
            double n1 = 7.5625;
            double d1 = 2.75;

            if (x < 1 / d1)
            {
                return (float)(n1 * x * x);
            }
            else if (x < 2 / d1)
            {
                return (float)(n1 * (x - 1.5 / d1) * x + 0.75);
            }
            else if (x < 2.5 / d1)
            {
                return (float)(n1 * (x - 2.25 / d1) * x + 0.9375);
            }
            else
            {
                return (float)(n1 * (x - 2.625 / d1) * x + 0.984375);
            }
        }

        public static float InOutBounce(float x)
        {
            return x < 0.5
            ? (1 - OutBounce(1 - 2 * x)) / 2
            : (1 + OutBounce(2 * x - 1)) / 2;
        }
    }
}
