using Arch.Buffer;
using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.ModLoader;
using Whipcackling.Core.Particles.Components;
using Whipcackling.Core.Particles.Queries;

namespace Whipcackling.Core.Particles
{
    public class ParticleSystem : ModSystem
    {
        public static World World { get; set; }
        public static CommandBuffer CommandBuffer { get; set; }

        public static readonly QueryDescription DrawableParticle = new QueryDescription().WithAll<Position, Scale, Rotation, Color, UVCoordinates>();

        public override void Load()
        {
            World = World.Create();
            CommandBuffer = new CommandBuffer();

            On_Main.DoDraw_Tiles_Solid += DrawParticles;
        }

        public override void Unload()
        {
            World.Destroy(World);
            CommandBuffer.Dispose();

            On_Main.DoDraw_Tiles_Solid -= DrawParticles;
        }

        public override void ClearWorld()
        {
            World.Clear();
        }

        public override void PostUpdateDusts()
        {
            World.InlineEntityQuery<UpdateTime, TimeLeft>(UpdateTime.Query);
            CommandBuffer.Playback(World);

            World.InlineQuery<UpdateLinearVelocity, Position, LinearVelocityAcceleration>(UpdateLinearVelocity.Query);
            World.InlineQuery<UpdateAngularVelocityMoveToTarget, Position, AngularVelocityMoveToTarget>(UpdateAngularVelocityMoveToTarget.Query);
            World.InlineQuery<UpdateLinearVelocityTimed, Position, LinearVelocityExponentialAccelerationTimed, TimeLeft, TimeUntilAction>(UpdateLinearVelocityTimed.Query);
            World.InlineQuery<UpdateInnacurateHomeOnTarget, Position, LinearVelocityAcceleration, InnacurateHomeOnTarget>(UpdateInnacurateHomeOnTarget.Query);

            World.InlineQuery<UpdateLinearScale, Scale, LinearScaleIncrease>(UpdateLinearScale.Query);
            World.InlineQuery<UpdateExponentialScale, Scale, ExponentialScaleIncrease>(UpdateExponentialScale.Query);
            World.InlineQuery<UpdateScaleWithVelocityAndLinearIncrease, Scale, LinearVelocityAcceleration, ScaleWithVelocityAndLinearIncrease>(UpdateScaleWithVelocityAndLinearIncrease.Query);

            World.InlineQuery<UpdateLinearScaleTimed, Scale, LinearScaleIncrease, TimeLeft, TimeUntilAction>(UpdateLinearScaleTimed.Query);
            World.InlineQuery<UpdateExponentialScaleTimed, Scale, ExponentialScaleIncrease, TimeLeft, TimeUntilAction>(UpdateExponentialScaleTimed.Query);

            World.InlineQuery<UpdateLinearAlphaFade, Color, LinearAlphaFade>(UpdateLinearAlphaFade.Query);
            World.InlineQuery<UpdateExponentialAlphaFade, Color, ExponentialAlphaFade>(UpdateExponentialAlphaFade.Query);
            World.InlineQuery<UpdateLinearColorFade, Color, LinearColorFade>(UpdateLinearColorFade.Query);
            World.InlineQuery<UpdateExponentialColorFade, Color, ExponentialColorFade>(UpdateExponentialColorFade.Query);
            World.InlineQuery<UpdateShiftColorThree, Color, ShiftColorThree, TimeLeft>(UpdateShiftColorThree.Query);
            World.InlineQuery<UpdateAlphaFadeInOut, Color, AlphaFadeInOut, TimeLeft>(UpdateAlphaFadeInOut.Query);

            World.InlineQuery<UpdateLinearAlphaFadeTimed, Color, LinearAlphaFade, TimeLeft, TimeUntilAction>(UpdateLinearAlphaFadeTimed.Query);
            World.InlineQuery<UpdateExponentialAlphaFadeTimed, Color, ExponentialAlphaFade, TimeLeft, TimeUntilAction>(UpdateExponentialAlphaFadeTimed.Query);
            World.InlineQuery<UpdateLinearColorFadeTimed, Color, LinearColorFade, TimeLeft, TimeUntilAction>(UpdateLinearColorFadeTimed.Query);
            World.InlineQuery<UpdateExponentialColorFadeTimed, Color, ExponentialColorFade, TimeLeft, TimeUntilAction>(UpdateExponentialColorFadeTimed.Query);

            World.InlineQuery<UpdateLinearRotationChange, Rotation, LinearRotationChange>(UpdateLinearRotationChange.Query);
            World.InlineQuery<UpdateRotateWithLinearVelocity, Rotation, LinearVelocityAcceleration, RotateWithLinearVelocity>(UpdateRotateWithLinearVelocity.Query);
            World.InlineQuery<UpdateRotationIsVelocity, Rotation, LinearVelocityAcceleration, RotationIsVelocity>(UpdateRotationIsVelocity.Query);
        }

        private void DrawParticles(On_Main.orig_DoDraw_Tiles_Solid orig, Main self)
        {
            orig(self);
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Query query = World.Query(in DrawableParticle);

            foreach (Chunk chunk in query)
            {
                chunk.GetSpan<Position, Scale, Rotation, Color, UVCoordinates>(out var positions, out var scales, out var rotations, out var colors, out var uvCoords);

                foreach (int i in chunk)
                {
                    Main.spriteBatch.Draw(
                        texture: ParticleAtlasSystem.Atlas,
                        position: (Vector2)positions[i] - Main.screenPosition,
                        sourceRectangle: (Rectangle)uvCoords[i],
                        color: colors[i],
                        rotation: rotations[i].Angle,
                        origin: uvCoords[i].Size * 0.5f,
                        scale: (Vector2)scales[i],
                        effects: SpriteEffects.None,
                        layerDepth: 0
                        );
                }
            }
            Main.spriteBatch.End();
        }
    }
}
