using System;

namespace Whipcackling.Content.Whips.NuclearWhip
{
    public static class ConstantsNuclear
    {
        //The damage falloff of the whip.
        public static float DamageFalloff => 0.15f;
        //The base damage of the whip.
        public static int ItemDamage => 550;

        //The base knockback of the whip.
        public static float ItemKnockback => 4f;

        //The base range (shoot speed/velocity) of the whip.
        public static float ItemRange => 5f;

        //The base use time of the whip.
        public static int ItemUseTime => 25;

        //The duration of the normal tag debuff.
        public static int TagDuration => 60 * 5;

        //The range multiplier of the whip.
        public static float WhipRangeMultiplier => 2f;

        //The amount of segments on the whip. Practically visual.
        public static int WhipSegments => 30;

        //The additive tag damage of the whip.
        public static int TagDamage(int enemiesTagged) => 20 + 5 * (int)Math.Ceiling(Math.Pow(enemiesTagged, 0.3));

        // The critical strike chance of the tag debuff.
        public static int TagCritChance => 8;
    }
}