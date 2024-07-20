using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whipcackling.Content.Modifiers
{
    public class BloodthirstySummonerPrefix : SummonerPrefix
    {
        public override float SpeedMultiplier => 0.1f;
        public override float DamageMultiplier => 0.85f;
        public override float TagDamageMultiplier => 1.1f;
    }

    public class CommandingSummonerPrefix : SummonerPrefix
    {
        public override float DamageMultiplier => 1.2f;
        public override float SlotsMultiplier => 1.15f;
    }

    public class CooperativeSummonerPrefix : SummonerPrefix
    {
        public override float TagDamageMultiplier => 1.2f;
        public override float DamageMultiplier => 0.6f;
        public override float SlotsMultiplier => 0.9f;
    }

    public class CowardlySummonerPrefix : SummonerPrefix
    {
        public override float SpeedMultiplier => 0.2f;
        public override float SlotsMultiplier => 0.8f;
    }

    public class CrazedSummonerPrefix : SummonerPrefix
    {
        public override float SpeedMultiplier => 0.1f;
        public override float DamageMultiplier => 1.05f;
    }

    public class EfficientSummonerPrefix : SummonerPrefix
    {
        public override float SlotsMultiplier => 0.9f;
        public override float DamageMultiplier => 0.9f;
    }

    public class StrikingSummonerPrefix : SummonerPrefix
    {
        public override float SpeedMultiplier => 0.05f;
        public override float KnockbackMultiplier => 1.05f;
    }

    public class SwarmingSummonerPrefix : SummonerPrefix
    {
        public override float SlotsMultiplier => 0.7f;
        public override float DamageMultiplier => 0.7f;
        public override float TagDamageMultiplier => 0.5f;
        public override float SpeedMultiplier => 0.2f;
    }

    public class UnstableSummonerPrefix : SummonerPrefix
    {
        public override float CritChance => 0.02f;
        public override float DamageMultiplier => 1.1f;
        public override float TagDamageMultiplier => 0.5f;
    }

    public class WatchingSummonerPrefix : SummonerPrefix
    {
        public override float SpeedMultiplier => -0.95f;
        public override float DamageMultiplier => 1.15f;
    }
}
