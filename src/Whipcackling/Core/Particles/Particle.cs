using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics;
using Whipcackling.Core.Particles.Enums;

namespace Whipcackling.Core.Particles
{
    /// <summary>
    /// Represents an individual particle instance.
    /// </summary>
    public struct Particle
    {
        /// <summary>
        /// The type of the particle, represented as a number.
        /// </summary>
        public int Type;

        /// <summary>
        /// The in-world or on-screen position of the particle, depending on the draw layer.
        /// </summary>
        /// <remarks>This is automatically </remarks>
        public Vector2 Position;

        /// <summary>
        /// The rotation in radians of the particle.
        /// </summary>
        public float Rotation;

        /// <summary>
        /// The scale factor of the particle.
        /// </summary>
        public Vector2 Scale;

        /// <summary>
        /// The velocity of the particle.
        /// </summary>
        public Vector2 Velocity;

        /// <summary>
        /// The color of the particle.
        /// </summary>
        public Color Color;

        /// <summary>
        /// The amount of frames this particle has existed for.
        /// </summary>
        /// <remarks>This is automatically incremented. Don't set this.</remarks>
        public int Time;

        /// <summary>
        /// The total amount of frames the particle will exist for.
        /// </summary>
        public int Lifetime;

        /// <summary>
        /// The variant of the particle.
        /// </summary>
        public int Variant;
        
        /// <summary>
        /// An array with 3 items for custom data usage.
        /// </summary>
        public float[] Custom;

        /// <summary>
        /// The percentage of total time passed.
        /// </summary>
        public readonly float Progress => (float)Lifetime != 0 ? (float)Time / Lifetime : 1;
    }
}
