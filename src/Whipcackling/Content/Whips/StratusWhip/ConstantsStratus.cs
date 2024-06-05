using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whipcackling.Content.Whips.StratusWhip
{
    public static class ConstantsStratus
    {
        //The damage falloff of the whip.
        public static float DamageFalloff => 0.1f;

        //The base damage of the whip.
        public static int ItemDamage => 600;

        //The base knockback of the whip.
        public static float ItemKnockback => 4f;

        //The base range (shoot speed/velocity) of the whip.
        public static float ItemRange => 6f;

        //The base use time of the whip.
        public static int ItemUseTime => 30;

        //The range multiplier of the whip.
        public static float WhipRangeMultiplier => 1.8f;

        //The amount of segments on the whip. Practically visual.
        public static int WhipSegments => 45;

        //The duration of the normal tag debuff.
        public static int TagDuration => 60 * 3;
    }
}
