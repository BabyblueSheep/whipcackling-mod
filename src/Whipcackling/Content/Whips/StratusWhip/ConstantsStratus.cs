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

        //The amount of damage the red star inherits from the whip.
        public static float RedStarDamageMult => 0.25f;

        //The amount of knockbacck dealt by the red star.
        public static float RedStarKnockback => 6f;

        //The amount of damage the yellow star inherits from the whip.
        public static float YellowStarDamageMult => 0.15f;

        //The amount of knockbacck dealt by the yellow star.
        public static float YellowStarKnockback => 1f;

        //The amount of damage the blue star inherits from the whip.
        public static float BlueStarDamageMult => 0.5f;

        //The amount of knockbacck dealt by blue red star.
        public static float BlueStarKnockback => 10f;

        //The amount of damage the purple star inherits from the whip.
        public static float PurpleStarDamageMult => 0.2f;

        //The amount of knockbacck dealt by the purple star.
        public static float PurpleStarKnockback => 3f;

        //The amount of damage the white star inherits from the whip.
        public static float WhiteStarDamageMult => 0.2f;

        //The amount of knockbacck dealt by the white star.
        public static float WhiteStarKnockback => 3f;
    }
}
