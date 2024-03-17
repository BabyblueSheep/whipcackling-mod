using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Composition.Hosting.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.RGB;
using Terraria.ModLoader;
using Whipcackling.Assets;
using Whipcackling.Core.Particles.Enums;

namespace Whipcackling.Core.Particles
{
    /// <summary>
    /// This class allows you to define custom, client-side particles.
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    public abstract class ModParticle : ModTexturedType
    {
        /// <summary>
        /// The type/ID of the particle.
        /// </summary>
        public int Type { get; internal set; }


        protected sealed override void Register()
        {
            ModTypeLookup<ModParticle>.Register(this);
            Type = ParticleLoader.Register(this);
        }

        public sealed override void SetupContent()
        {
            SetStaticDefaults();
        }

        public override void SetStaticDefaults() { }

        /// <summary>
        /// In which layer the particle will be drawn.
        /// </summary>
        /// <remarks>
        /// If <see cref="ParticleDrawLayer.AfterBackgrounds"/>, particles will not be affected by any zoom.<br></br>
        /// If <see cref="ParticleDrawLayer.AfterUI"/>, particles will be affected by UI scale. <br></br>
        /// Otherwise, particles will be affected by Zoom.
        /// </remarks>
        public virtual ParticleDrawLayer DrawLayer => ParticleDrawLayer.AfterNPCsProjectiles;

        /// <summary>
        /// The blending mode of the particle.
        /// </summary>
        public virtual BlendState BlendMode => BlendState.AlphaBlend;

        /// <summary>
        /// The shader of the particle.
        /// </summary>
        public virtual Effect Effect => AssetDirectory.Effects.Particle.Value;

        /// <summary>
        /// The amount of frames the particle has.
        /// </summary>
        public virtual int Variants => 1;

        /// <summary>
        /// Allows you to override the color of a particle. If this isn't overidden, the particle will also be affected by in-game lighting.
        /// </summary>
        /// <param name="particle">The particle instance.</param>
        /// /// <param name="lightColor">The color at the particle's position.</param>
        /// <returns>The overidden color.</returns>
        public virtual Color GetColor(Particle particle, Color lightColor) => new Color(particle.Color.R * lightColor.R / 255, particle.Color.G * lightColor.G / 255, particle.Color.B * lightColor.B / 255, particle.Color.A);

        /// <summary>
        /// Allows you to set the texture frame of a particle. Used for multi-frame textures.
        /// </summary>
        /// <param name="particle">The particle instance.</param>
        /// <returns>The texture frame.</returns>
        public virtual Rectangle GetFrame(Particle particle) => new Rectangle(0, ModContent.Request<Texture2D>(Texture).Height() * particle.Variant / Variants, ModContent.Request<Texture2D>(Texture).Width(), ModContent.Request<Texture2D>(Texture).Height() / Variants);


        /// <summary>
        /// Allows you to modify a particle upon spawning.
        /// </summary>
        /// <param name="particle">The particle instance.</param>
        public virtual void OnSpawn(ref Particle particle) { }

        /// <summary>
        /// Allows you to modify a particle's behavior every frame.
        /// </summary>
        /// <param name="particle">The particle instance.</param>
        public virtual void Update(ref Particle particle) { }
    }
}
