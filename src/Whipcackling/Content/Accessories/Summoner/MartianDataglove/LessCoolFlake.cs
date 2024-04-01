using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Whipcackling.Content.Accessories.Summoner.MartianDataglove
{
    public class LessCoolFlake : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CoolWhipProj;
        public override string LocalizationCategory => "Accessories.MartianDataglove";

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.CoolWhipProj);
            Projectile.ArmorPenetration = 30;
            Projectile.timeLeft = 60;
        }

        public override void PostAI()
        {
            if (Projectile.timeLeft < 60 - Projectile.ai[2])
                Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.DrawProj_CoolWhipMinion(Projectile);
            return false;
        }
    }
}
