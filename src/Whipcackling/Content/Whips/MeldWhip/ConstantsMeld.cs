namespace Whipcackling.Content.Whips.MeldWhip
{
    public static class ConstantsMeld
    {

        //How many hits needed to trigger an explosion.
        public static int BlackHoleHitsByWhip => 6;

        //The damage falloff of the whip.
        public static float DamageFalloff => 0.025f;
        //The base damage of the whip.
        public static int ItemDamage => 160;

        //The base knockback of the whip.
        public static float ItemKnockback => 1f;

        //The base range (shoot speed/velocity) of the whip.
        public static float ItemRange => 3f;

        //The base use time of the whip.
        public static int ItemUseTime => 65;

        //The duration of the super tag cooldown.
        public static int SuperTagCooldown => 60 * 10;

        //The duration of the super tag debuff.
        public static int SuperTagDuration => 60 * 12;

        //The multiplicative tag damage of the whip.
        public static float TagDamage => 2.25f;

        //The duration of the normal tag debuff.
        public static int TagDuration => 60 * 4;

        //After how many hits should an enemy be tagged.
        public static int TagHitsByWhip => 2;

        //The range multiplier of the whip.
        public static float WhipRangeMultiplier => 2f;

        //The amount of segments on the whip. Practically visual.
        public static int WhipSegments => 45;
    }
}