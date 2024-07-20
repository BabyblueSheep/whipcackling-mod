using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whipcackling.Content.Whips.BloodstoneWhip
{
    public static class ConstantsBloodstone
    {
        //The damage falloff of the whip.
        public static float DamageFalloff => 0.1f;

        //The base damage of the whip.
        public static int ItemDamage => 220;

        //The base knockback of the whip.
        public static float ItemKnockback => 4f;

        //The base range (shoot speed/velocity) of the whip.
        public static float ItemRange => 8f;

        //The base use time of the whip.
        public static int ItemUseTime => 30;

        //The range multiplier of the whip.
        public static float WhipRangeMultiplier => 1.2f;

        //The amount of segments on the whip. Practically visual.
        public static int WhipSegments => 45;

        //The additive tag damage of the whip.
        public static int TagDamage => 30;

        //The critical strike chance of the tag debuff.
        public static int TagCritChance => 10;

        //The duration of the normal tag debuff.
        public static int TagDuration => 60 * 4;

        //Amount of charge gained when a minion hits a tagged enemy.
        public static float ChargeGained => 0.1f;//0.005f;

        //Amount of charge lost when in an awakened state.
        public static float ChargeLost => 0.005f;//0.001f;
    }
}
