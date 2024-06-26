using Arch.Core;
using CalamityMod.Particles;
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
    public struct UpdateLinearAlphaFade : IForEach<Color, LinearAlphaFade>
    {
        public static QueryDescription Query => new QueryDescription().WithAll<Color, LinearAlphaFade>().WithNone<TimeUntilAction>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Update(ref Color color, ref LinearAlphaFade fade)
        {
            color.A = (byte)Math.Max(color.A - fade.DecreaseAmount, 0);
        }
    }

    public struct UpdateLinearColorFade : IForEach<Color, LinearColorFade>
    {
        public static QueryDescription Query => new QueryDescription().WithAll<Color, LinearColorFade>().WithNone<TimeUntilAction>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Update(ref Color color, ref LinearColorFade fade)
        {
            color.R = (byte)Math.Max(color.R - fade.DecreaseAmount, 0);
            color.G = (byte)Math.Max(color.G - fade.DecreaseAmount, 0);
            color.B = (byte)Math.Max(color.B - fade.DecreaseAmount, 0);
        }
    }

    public struct UpdateExponentialAlphaFade : IForEach<Color, ExponentialAlphaFade>
    {
        public static QueryDescription Query => new QueryDescription().WithAll<Color, ExponentialAlphaFade>().WithNone<TimeUntilAction>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Update(ref Color color, ref ExponentialAlphaFade fade)
        {
            color.A = (byte)Math.Max(color.A * fade.DecreaseAmount, 0);
        }
    }

    public struct UpdateExponentialColorFade : IForEach<Color, ExponentialColorFade>
    {
        public static QueryDescription Query => new QueryDescription().WithAll<Color, ExponentialColorFade>().WithNone<TimeUntilAction>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Update(ref Color color, ref ExponentialColorFade fade)
        {
            color.R = (byte)Math.Max(color.R * fade.DecreaseAmount, 0);
            color.G = (byte)Math.Max(color.G * fade.DecreaseAmount, 0);
            color.B = (byte)Math.Max(color.B * fade.DecreaseAmount, 0);
        }
    }

    public struct UpdateShiftColorThree : IForEach<Color, ShiftColorThree, TimeLeft>
    {
        public static QueryDescription Query => new QueryDescription().WithAll<Color, ShiftColorThree, TimeLeft>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Update(ref Color color, ref ShiftColorThree shift, ref TimeLeft time)
        {
            float progress = 1 - (float)time.Time / time.TotalTime;

            if (progress <= 0.5f)
            {
                color = Color.Lerp(shift.FirstColor, shift.SecondColor, progress * 2f);
            }
            else
            {
                color = Color.Lerp(shift.SecondColor, shift.ThirdColor, (progress - 0.5f) * 2f);
            }
        }
    }

    public struct UpdateAlphaFadeInOut : IForEach<Color, AlphaFadeInOut, TimeLeft>
    {
        public static QueryDescription Query => new QueryDescription().WithAll<Color, AlphaFadeInOut, TimeLeft>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Update(ref Color color, ref AlphaFadeInOut fade, ref TimeLeft time)
        {
            float value = Utils.GetLerpValue(0, fade.FadeInTime, time.Time, true) * Utils.GetLerpValue(time.TotalTime, time.TotalTime - fade.FadeOutTime, time.Time, true);
            byte alpha = (byte)(255 * value);
            color.R = (byte)(fade.Red * value); color.B = (byte)(fade.Green * value); color.G = (byte)(fade.Blue * value);
        }
    }
}
