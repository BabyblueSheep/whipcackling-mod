using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Summon;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.ModLoader;
using Whipcackling.Common.Configs;

namespace Whipcackling.Content.Rebalances.Minions
{
    public class VileFeederItem : BaseRebalance
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ModContent.ItemType<VileFeeder>() && lateInstantiation;

        public override int Damage => 10;
    }

    public class VileFeederBabyEater : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ModContent.ProjectileType<VileFeederSummon>() && lateInstantiation;

        private static ILHook? _vileFeederAI;

        public override void Load()
        {
            if (WhipcacklingConfig.Instance.BalanceMode == BalanceMode.Whipcackling)
                _vileFeederAI = new(typeof(VileFeederSummon).GetMethod("AI")!, NerfVileFeeder);
        }

        public override void Unload()
        {
            _vileFeederAI?.Undo();
        }

        private void NerfVileFeeder(ILContext il)
        {
            ILCursor? cursor = new(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(80)))
                return;
            cursor.EmitPop();
            cursor.EmitLdcI4(120);
        }
    }
}
