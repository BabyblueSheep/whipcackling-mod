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
    public struct UpdateTime : IForEachWithEntity<TimeLeft>
    {
        public static QueryDescription Query => new QueryDescription().WithAll<TimeLeft>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Update(Arch.Core.Entity entity, ref TimeLeft time)
        {
            time.Time--;
            if (time.Time <= 0)
            {
                ParticleSystem.CommandBuffer.Destroy(in entity);
            }
        }
    }
}
