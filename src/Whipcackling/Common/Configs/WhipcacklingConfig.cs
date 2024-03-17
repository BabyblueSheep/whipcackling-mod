using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
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
        public BalanceMode BalanceMode { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool RemoveSummonerPenalty { get; set; }
    }
}
