using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Whipcackling.Common.Configs
{
    [BackgroundColor(62, 46, 71, 216)]
    public class WhipcacklingConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        public static WhipcacklingConfig Instance;

        [Header("CalamityChanges")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(BalanceMode.Whipcackling)]
        [DrawTicks]
        [ReloadRequired]
        public BalanceMode BalanceMode { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool RemoveSummonerPenalty { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool NewSummonPrefixes { get; set; }
    }
}
