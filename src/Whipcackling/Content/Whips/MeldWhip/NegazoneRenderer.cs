using System;
using System.Collections.Generic;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Whipcackling.Common.Utilities;

namespace Whipcackling.Content.Whips.MeldWhip
{
    public class NegazoneRenderer : ModSystem
    {
        public const int NegazoneAmount = 10;

        public static List<Projectile> WhipPoints = new();

        private static float[] _radiuses = new float[NegazoneAmount];
        private static float[] _innerRadiuses = new float[NegazoneAmount];

        public override void PostUpdateProjectiles()
        {
            if (Main.gameMenu || Main.netMode == NetmodeID.Server)
                return;

            Vector2 offset = Main.drawToScreen ? Vector2.Zero : new(Main.offScreenRange);

            Vector2[] positions = new Vector2[NegazoneAmount];
            int points = 0;

            for (int i = 0; i < WhipPoints.Count; i++)
            {
                Projectile projectile = WhipPoints[i];
                if (!projectile.active)
                {
                    WhipPoints.Remove(projectile);
                    i--;
                    _radiuses[points] = 0;
                    _innerRadiuses[points] = 0;
                    continue;
                }

                float timeToFlyOut = Main.player[projectile.owner].itemAnimationMax * projectile.MaxUpdates;

                positions[points] = projectile.WhipPointsForCollision[^2] - offset;

                if (projectile.ai[1] % ConstantsMeld.BlackHoleHitsByWhip != 0 && projectile.ai[0] <= timeToFlyOut * 0.9f)
                {
                    _radiuses[points] = WhipRadiusCalc(projectile.ai[1]);
                    _innerRadiuses[points] = WhipInnerRadiusCalc(projectile.ai[1]);
                }
                else
                {
                    _radiuses[points] = Math.Max(_radiuses[points] - 0.005f, 0f);
                    _innerRadiuses[points] = Math.Max(_innerRadiuses[points] - 0.003f, 0f);
                }

                points++;

                if (points > 9)
                    break;
            }

            if (points == 0)
            {
                Filters.Scene["Whipcackling:NegazoneEffect"].GetShader().UseOpacity(0);
                Filters.Scene["Whipcackling:NegazoneEffect"].Deactivate();
                return;
            }

            while (points < 9)
            {
                positions[points] = Vector2.Zero;
                _radiuses[points] = 0f;
                _innerRadiuses[points] = 0f;
                points++;
            }

            Filters.Scene["Whipcackling:NegazoneEffect"].GetParameter("uPositions").SetValue(positions);
            Filters.Scene["Whipcackling:NegazoneEffect"].GetParameter("uRadiuses").SetValue(_radiuses);
            Filters.Scene["Whipcackling:NegazoneEffect"].GetParameter("uInnerRadiuses").SetValue(_innerRadiuses);

            if (Main.netMode != NetmodeID.Server && !Filters.Scene["Whipcackling:NegazoneEffect"].IsActive())
                Filters.Scene.Activate("Whipcackling:NegazoneEffect").GetShader().UseOpacity(1);
        }

        public static float WhipRadiusCalc(float hits)
        {
            hits %= ConstantsMeld.BlackHoleHitsByWhip;
            return 0.1f / (ConstantsMeld.BlackHoleHitsByWhip - (float)Math.Pow(1.25, hits));
        }

        public static float WhipInnerRadiusCalc(float hits)
        {
            hits %= ConstantsMeld.BlackHoleHitsByWhip;
            return 0.1f / (ConstantsMeld.BlackHoleHitsByWhip + 2 - (float)Math.Pow(1.3, hits));
        }

        public override void PreSaveAndQuit()
        {
            if (Filters.Scene["Whipcackling:NegazoneEffect"].Active)
                Filters.Scene["Whipcackling:NegazoneEffect"].Deactivate();
        }
    }
}
