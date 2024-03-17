using CalamityMod.Items.Potions.Alcohol;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Animations;
using Terraria.ID;
using Terraria.ModLoader;

namespace Whipcackling.Content.Whips
{
    /// <summary>
    /// Base class to use for implementing modded whips.
    /// </summary>
    public abstract class ModWhip : ModProjectile
    {
        /// <summary>
        /// Shorthand for <c>Projectile.ai[0]</c>, use it as a timer.
        /// </summary>
        /// <remarks>Don't manually increment this. It is already incremented in <see cref="ModProjectile.PreAI"/>.</remarks>
        public ref float Timer => ref Projectile.ai[0];

        /// <summary>
        /// The <see cref="SoundStyle"/> that plays when the whip reaches maximum distance.
        /// </summary>
        public virtual SoundStyle SwingSound => SoundID.Item153;

        /// <summary>
        /// The color of the string of the drawn whip.
        /// </summary>
        public virtual Color StringColor => Color.White;

        /// <summary>
        /// The offset of the held handle.
        /// </summary>
        public virtual int HandleOffset => 0;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.IsAWhip[Type] = true;
            SafeSetStaticDefaults();
        }

        /// <summary>
        /// Allows you to safely needed constant properties, seperated from <see cref="ModProjectile.SetStaticDefaults"/>.
        /// </summary>
        public virtual void SafeSetStaticDefaults() { }

        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();
            SafeSetDefaults();
        }

        /// <summary>
        /// Allows you to safely needed properties, seperated from <see cref="ModProjectile.SetDefaults"/>.
        /// </summary>
        public virtual void SafeSetDefaults() { }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];

            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.Center = Main.GetPlayerArmPosition(Projectile) + Projectile.velocity * (Timer - 1f);
            Projectile.spriteDirection = !(Vector2.Dot(Projectile.velocity, Vector2.UnitX) < 0f) ? 1 : -1;

            float timeToFlyOut = player.itemAnimationMax * Projectile.MaxUpdates;

            if (Timer >= timeToFlyOut)
            {
                Projectile.Kill();
                return false;
            }

            player.heldProj = Projectile.whoAmI;
            player.MatchItemTimeToItemAnimation();
            if (Timer == (int)(timeToFlyOut * 0.5f))
            {
                Projectile.WhipPointsForCollision.Clear();
                Projectile.FillWhipControlPoints(Projectile, Projectile.WhipPointsForCollision);
                Vector2 position = Projectile.WhipPointsForCollision[^1];
                SoundStyle sound = SwingSound;
                SoundEngine.PlaySound(in sound, position);
            }

            ArcAI();

            return false;
        }

        /// <summary>
        /// The behavior of the projetile, seperated from <see cref="ModProjectile.AI"/>
        /// </summary>
        public virtual void ArcAI() { }

        public override void CutTiles()
        {
            Vector2 midPoint = new(Projectile.width * Projectile.scale * 0.5f, 0f);

            for (int i = 0; i < Projectile.WhipPointsForCollision.Count; i++)
            {
                DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
                Utils.PlotTileLine(Projectile.WhipPointsForCollision[i] - midPoint, Projectile.WhipPointsForCollision[i] + midPoint, Projectile.height * Projectile.scale, DelegateMethods.CutTiles);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }

        /// <summary>
        /// Draws a line that follows a list of points, used for whip drawing.
        /// </summary>
        /// <param name="list">The list of points.</param>
        /// <param name="originalColor">The color of the whip.</param>
        public static void DrawWhipLine(List<Vector2> list, Color originalColor)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new(frame.Width / 2, 2);

            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates(), originalColor);
                Vector2 scale = new(1, (diff.Length() + 2) / frame.Height);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

                pos += diff;
            }
        }

        /// <summary>
        /// Draws stuff behind the whip.
        /// </summary>
        /// <param name="lightColor">The color of the light at the projectile's center.</param>
        public virtual void DrawBehindWhip(ref Color lightColor) { }

        /// <summary>
        /// Draws stuff ahead of the whip.
        /// </summary>
        /// <param name="lightColor">The color of the light at the projectile's center.</param>
        public virtual void DrawAheadWhip(ref Color lightColor) { }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawBehindWhip(ref lightColor);

            List<Vector2> points = new();
            Projectile.FillWhipControlPoints(Projectile, points);

            DrawWhipLine(points.SkipLast(1).ToList(), StringColor);

            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Rectangle frame = texture.Frame(1, 5, 0, 0);
            int height = frame.Height;
            Vector2 position = points[0];

            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector2 origin = frame.Size() * 0.5f;

                if (i == 0)
                    origin.Y += HandleOffset;
                else if (i == points.Count - 2)
                    frame.Y = height * 4;
                else
                    frame.Y = height * SegmentVariant(i);

                Vector2 difference = points[i + 1] - points[i];

                float rotation = difference.ToRotation() - MathHelper.PiOver2;
                float scale = SegmentScale(i);
                Color color = Lighting.GetColor(points[i].ToTileCoordinates());

                Main.EntitySpriteDraw(texture.Value, position - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

                position += difference;
            }

            DrawAheadWhip(ref lightColor);

            return false;
        }

        /// <summary>
        /// Sets the scale of the point depending on the index.
        /// </summary>
        /// <param name="i">The index of the point.</param>
        /// <returns>The scale of the point.</returns>
        public virtual float SegmentScale(int i) => 1f;

        /// <summary>
        /// Sets the variant of the point depending on the index.
        /// </summary>
        /// <param name="i">The index of the point.</param>
        /// <returns>The variant of the point.</returns>
        public virtual int SegmentVariant(int i) => 1 + i % 3;
    }
}
