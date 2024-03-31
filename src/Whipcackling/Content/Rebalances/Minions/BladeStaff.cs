using Terraria;
using Terraria.ID;

namespace Whipcackling.Content.Rebalances.Minions
{
    public class BladeStaffItem : BaseRebalance
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Smolstar && lateInstantiation;

        public override int Damage => 6;
    }
}
