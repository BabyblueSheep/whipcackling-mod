using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace Whipcackling.Core.Particles
{
    [Autoload(Side = ModSide.Client)]
    public static class ParticleLoader
    {
        /// <summary>
        /// The amount of loaded particle types.
        /// </summary>
        public static int Count => particles.Count;

        internal static readonly List<ModParticle> particles = new();

        internal static int Register(ModParticle particle)
        {
            particles.Add(particle);
            return Count - 1;
        }

        internal static void Unload()
        {
            particles.Clear();
        }

        /// <summary>
        /// Get the ModParticle instance with the given type. Returns null if no ModParticle with the given type exists.
        /// </summary>
        /// <param name="type">The particle type.</param>
        /// <returns>The particle type instance.</returns>
        public static ModParticle GetParticle(int type) => type < Count ? particles[type] : null;

        /// <summary>
        /// Get the type of a ModParticle by class. Assumes one instance per class.
        /// </summary>
        /// <typeparam name="T">The particle type instance.</typeparam>
        /// <returns>The particle type.</returns>
        public static int ParticleType<T>() where T : ModParticle => ModContent.GetInstance<T>()?.Type ?? -1;

    }
}
