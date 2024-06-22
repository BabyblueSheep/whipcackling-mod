using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Whipcackling.Assets;
using Whipcackling.Common.Systems.Drawing;
using Whipcackling.Common.Utilities;
using CalamityMod.Buffs.DamageOverTime;
using Terraria.DataStructures;

namespace Whipcackling.Content.Whips.StratusWhip.Stars
{
    public class StratusYellowStar : StratusStar<StratusWhipNPCDebuffYellow>
    {
        public override float RotateSpeed => 1f;
        public override float Radius => 200;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hide = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0, 12).RotatedByRandom(Math.PI * 2), ModContent.ProjectileType<StratusYellowStarMini>(), (int)Math.Ceiling(Projectile.damage * 0.333f), 1f, Projectile.owner,
    Main.netMode == NetmodeID.SinglePlayer ? Projectile.whoAmI + 1 : Projectile.identity, 0, 0);
            }
        }

        public override void OnLosingTarget()
        {
            Projectile.Kill();
        }

        public override bool PreKill(int timeLeft)
        {
            return true;
        }

        public override void HostileBehavior(NPC target)
        {
            
        }
    }
}
