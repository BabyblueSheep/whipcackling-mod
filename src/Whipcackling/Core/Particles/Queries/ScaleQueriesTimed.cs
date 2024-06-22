using Arch.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Whipcackling.Core.Particles.Components;

namespace Whipcackling.Core.Particles.Queries
{
    public struct UpdateLinearScaleTimed : IForEach<Scale, LinearScaleIncrease, TimeLeft, TimeUntilAction>
    {
        public static QueryDescription Query => new QueryDescription().WithAll<Scale, LinearScaleIncrease, TimeLeft, TimeUntilAction>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Update(ref Scale scale, ref LinearScaleIncrease increase, ref TimeLeft time, ref TimeUntilAction decay)
        {
            if (time.Time > decay.Time)
                return;
            scale.X += increase.IncreaseX;
            if (scale.X < 0) scale.X = 0;
            scale.Y += increase.IncreaseY;
            if (scale.Y < 0) scale.Y = 0;
        }
    }

    public struct UpdateExponentialScaleTimed : IForEach<Scale, ExponentialScaleIncrease, TimeLeft, TimeUntilAction>
    {
        public static QueryDescription Query => new QueryDescription().WithAll<Scale, ExponentialScaleIncrease, TimeLeft, TimeUntilAction>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Update(ref Scale scale, ref ExponentialScaleIncrease increase, ref TimeLeft time, ref TimeUntilAction decay)
        {
            if (time.Time > decay.Time)
                return;
            scale.X *= increase.IncreaseX;
            scale.Y *= increase.IncreaseY;
        }
    }
}
