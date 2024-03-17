using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Whipcackling.Common.Configs;

namespace Whipcackling.Content.Rebalances
{
    public abstract class BaseRebalance : GlobalItem
    {
        public abstract int Damage { get; }

        public override void SetDefaults(Item entity)
        {
            //int damage = switch (WhipcacklingConfig.Instance.BalanceMode)
        }
    }
}
