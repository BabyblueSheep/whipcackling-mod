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
    public struct UpdateLinearScale : IForEach<Scale, LinearScaleIncrease>
    {
        public static QueryDescription Query => new QueryDescription().WithAll<Scale, LinearScaleIncrease>().WithNone<TimeUntilAction>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Update(ref Scale scale, ref LinearScaleIncrease increase)
        {
            scale.X += increase.IncreaseX;
            if (scale.X < 0) scale.X = 0;
            scale.Y += increase.IncreaseY;
            if (scale.Y < 0) scale.Y = 0;
        }
    }

    public struct UpdateExponentialScale : IForEach<Scale, ExponentialScaleIncrease>
    {
        public static QueryDescription Query => new QueryDescription().WithAll<Scale, ExponentialScaleIncrease>().WithNone<TimeUntilAction>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Update(ref Scale scale, ref ExponentialScaleIncrease increase)
        {
            scale.X *= increase.IncreaseX;
            scale.Y *= increase.IncreaseY;
        }
    }
}
