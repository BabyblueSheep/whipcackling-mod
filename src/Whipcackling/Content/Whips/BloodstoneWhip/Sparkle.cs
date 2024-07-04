using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Whipcackling.Content.Whips.BloodstoneWhip
{
    public class Sparkle : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;

            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = 40;
            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);

            float timer = Utils.GetLerpValue(0, 40, 40 - Projectile.timeLeft, true);

            Vector2 scale;
            scale.X = MathF.Sin(timer) + MathF.Sin(timer * 2);
            scale.Y = MathF.Sin(3 * MathF.Sin(timer));
            float alpha = MathF.Sin(timer * MathF.PI);

            Main.spriteBatch.Draw(texture.Value, Projectile.position - Main.screenPosition, texture.Frame(), new Color(255, 120, 120, 0) * alpha, 0, texture.Size() * 0.5f, scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
