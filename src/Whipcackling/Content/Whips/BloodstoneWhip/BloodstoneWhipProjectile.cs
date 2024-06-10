using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Whipcackling.Common.Systems.Drawing;
using Whipcackling.Content.Whips.NuclearWhip;
using Whipcackling.Core;

namespace Whipcackling.Content.Whips.BloodstoneWhip
{
    public class BloodstoneWhipProjectile : ModWhip, IDrawPixelated
    {
        public override string LocalizationCategory => "Whips.BloodstoneWhip";

        public override SoundStyle SwingSound => SoundID.Item153;
        public override Color StringColor => new(140, 234, 87);
        public override int HandleOffset => 4;

        private Vector2[,] _prevPositionsPlane;
        private Vector2[,] _prevPositionsPlaneSmoothed;
        private VertexPlane _plane;

        public override void SafeSetDefaults()
        {
            Projectile.WhipSettings.Segments = ConstantsBloodstone.WhipSegments;
            Projectile.WhipSettings.RangeMultiplier = ConstantsBloodstone.WhipRangeMultiplier;
            Projectile.extraUpdates = 2;
        }

        public override void ArcAI()
        {
            float timeToFlyOut = Main.player[Projectile.owner].itemAnimationMax * Projectile.MaxUpdates;
            float ratio = Timer / timeToFlyOut;
            float progress = Utils.GetLerpValue(0.1f, 0.7f, ratio, true) * Utils.GetLerpValue(0.9f, 0.7f, ratio, true);

            Projectile.WhipPointsForCollision.Clear();
            Projectile.FillWhipControlPoints(Projectile, Projectile.WhipPointsForCollision);

            if (progress > 0.1f)
            {
                int id = DustID.CursedTorch;
                Rectangle rectangle = Utils.CenteredRectangle(Projectile.WhipPointsForCollision[^1], new Vector2(30f));
                Dust dust = Dust.NewDustDirect(rectangle.TopLeft(), rectangle.Width, rectangle.Height, id, 0f, 0f, 100, Color.White, progress);
                dust.noGravity = true;
                dust.velocity *= 1.5f;
                dust.scale *= 0.9f + Main.rand.NextFloat() * 0.9f;
            }

            #region Previous positions
            if (_prevPositionsPlane == null)
            {
                _prevPositionsPlane ??= new Vector2[10, Projectile.WhipSettings.Segments];
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < Projectile.WhipSettings.Segments; j++)
                    {
                        _prevPositionsPlane[i, j] = Projectile.WhipPointsForCollision[j];
                    }
                }
            }

            int width = _prevPositionsPlane.GetLength(1);
            int height = _prevPositionsPlane.GetLength(0);

            for (int x = width - 1; x >= 0; x--)
            {
                for (int y = height - 1; y > 0; y--)
                {
                    _prevPositionsPlane[y, x] = _prevPositionsPlane[y - 1, x];
                }
            }
            for (int x = 0; x < width; x++)
            {
                _prevPositionsPlane[0, x] = Projectile.WhipPointsForCollision[x];
            }
            #endregion
        }

        public void DrawPixelated()
        {
            
        }
    }
}
