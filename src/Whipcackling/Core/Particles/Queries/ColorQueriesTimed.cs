using Arch.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Whipcackling.Core.Particles.Components;

namespace Whipcackling.Core.Particles.Queries
{
    public struct UpdateLinearAlphaFadeTimed : IForEach<Color, LinearAlphaFade, TimeLeft, TimeUntilAction>
    {
        public static QueryDescription Query => new QueryDescription().WithAll<Color, LinearAlphaFade, TimeLeft, TimeUntilAction>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Update(ref Color color, ref LinearAlphaFade fade, ref TimeLeft time, ref TimeUntilAction decay)
        {
            if (time.Time > decay.Time)
                return;
            color.A = (byte)Math.Max(color.A - fade.DecreaseAmount, 0);
        }
    }

    public struct UpdateLinearColorFadeTimed : IForEach<Color, LinearColorFade, TimeLeft, TimeUntilAction>
    {
        public static QueryDescription Query => new QueryDescription().WithAll<Color, LinearColorFade, TimeLeft, TimeUntilAction>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Update(ref Color color, ref LinearColorFade fade, ref TimeLeft time, ref TimeUntilAction decay)
        {
            if (time.Time > decay.Time)
                return;
            color.R = (byte)Math.Max(color.R - fade.DecreaseAmount, 0);
            color.G = (byte)Math.Max(color.G - fade.DecreaseAmount, 0);
            color.B = (byte)Math.Max(color.B - fade.DecreaseAmount, 0);
        }
    }

    public struct UpdateExponentialAlphaFadeTimed : IForEach<Color, ExponentialAlphaFade, TimeLeft, TimeUntilAction>
    {
        public static QueryDescription Query => new QueryDescription().WithAll<Color, ExponentialAlphaFade, TimeLeft, TimeUntilAction>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Update(ref Color color, ref ExponentialAlphaFade fade, ref TimeLeft time, ref TimeUntilAction decay)
        {
            if (time.Time > decay.Time)
                return;
            color.A = (byte)Math.Max(color.A * fade.DecreaseAmount, 0);
        }
    }

    public struct UpdateExponentialColorFadeTimed : IForEach<Color, ExponentialColorFade, TimeLeft, TimeUntilAction>
    {
        public static QueryDescription Query => new QueryDescription().WithAll<Color, ExponentialColorFade, TimeLeft, TimeUntilAction>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Update(ref Color color, ref ExponentialColorFade fade, ref TimeLeft time, ref TimeUntilAction decay)
        {
            if (time.Time > decay.Time)
                return;
            color.R = (byte)Math.Max(color.R * fade.DecreaseAmount, 0);
            color.G = (byte)Math.Max(color.G * fade.DecreaseAmount, 0);
            color.B = (byte)Math.Max(color.B * fade.DecreaseAmount, 0);
        }
    }
}
