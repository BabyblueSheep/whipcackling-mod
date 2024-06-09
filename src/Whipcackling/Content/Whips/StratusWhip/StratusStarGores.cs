using CalamityMod.Items.Potions.Alcohol;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Whipcackling.Content.Whips.StratusWhip
{
    public abstract class StratusStarGore : ModGore
    {
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.sticky = false;
            gore.alpha = 100;
            gore.scale = 0.7f;
            gore.light = 1f;
        }

        public override bool Update(Gore gore)
        {
            gore.velocity.Y *= 0.98f;
            gore.velocity.X *= 0.98f;
            gore.scale -= 0.01f;
            if (gore.scale < 0.1f)
            {
                gore.scale = 0.1f;
                gore.alpha = 255;
            }
            gore.position += gore.velocity;

            if (gore.alpha >= 255)
            {
                gore.active = false;
            }
            if (gore.light > 0)
            {
                float r = gore.light * gore.scale * Red;
                float g = gore.light * gore.scale * Green;
                float b = gore.light * gore.scale * Blue;

                Lighting.AddLight((int)((gore.position.X + (float)TextureAssets.Gore[gore.type].Width() * gore.scale / 2f) / 16f), (int)((gore.position.Y + (float)TextureAssets.Gore[gore.type].Height() * gore.scale / 2f) / 16f), r, g, b);
            }

            return false;
        }

        public abstract float Red { get; }
        public abstract float Green { get; }
        public abstract float Blue { get; }
    }

    public class StratusRedStarGore : StratusStarGore
    {
        public override float Red => 1f;
        public override float Green => 0.15f;
        public override float Blue => 0.2f;
    }

    public class StratusPurpleStarGore : StratusStarGore
    {
        public override float Red => 0.8f;
        public override float Green => 0.6f;
        public override float Blue => 1f;
    }

    public class StratusWhiteStarGore : StratusStarGore
    {
        public override float Red => 0.8f;
        public override float Green => 0.8f;
        public override float Blue => 0.8f;
    }
}
