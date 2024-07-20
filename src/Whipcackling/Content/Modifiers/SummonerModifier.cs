using CalamityMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Whipcackling.Common.Configs;

namespace Whipcackling.Content.Modifiers
{
    public abstract class SummonerPrefix : ModPrefix
    {
        public virtual float DamageMultiplier => 1f;
        public virtual float TagDamageMultiplier => 1f;
        public virtual float SpeedMultiplier => 0f;
        public virtual float SizeMultiplier => 1f; // broken, wait until nitrate or smth fixes projectile.scale
        public virtual float KnockbackMultiplier => 1f;
        public virtual float CritChance => 0f;
        public virtual float SlotsMultiplier => 1f;

        public override PrefixCategory Category => PrefixCategory.AnyWeapon;
        public override bool CanRoll(Item item) => item.CountsAsClass<SummonDamageClass>() && !item.CountsAsClass<SummonMeleeSpeedDamageClass>() && !item.IsWhip() && WhipcacklingConfig.Instance.NewSummonPrefixes;

        public override void ModifyValue(ref float valueMult)
        {
            float extraDamage = DamageMultiplier - 1f;
            float extraTagDamage = TagDamageMultiplier - 1f;
            float extraKnockback = KnockbackMultiplier - 1f;

            valueMult *= SlotsMultiplier * SizeMultiplier + (extraDamage * 0.35f + extraTagDamage * 0.35f + SpeedMultiplier * 0.2f + extraKnockback * 0.1f) * (1 + CritChance);
        }

        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult = DamageMultiplier;
            knockbackMult = KnockbackMultiplier;
        }

        public override void Apply(Item item)
        {
            SummonModifierItem modItem = item.GetGlobalItem<SummonModifierItem>();

            modItem.TagDamageMultiplier = TagDamageMultiplier;
            modItem.SizeMultiplier = SizeMultiplier;
            modItem.MinionSpeedMultiplier = SpeedMultiplier;
            modItem.MinionCriticalChance = CritChance;
            modItem.SlotsAmountMultiplier = SlotsMultiplier;
        }

        public LocalizedText SummonerTagDamageTooltip => Language.GetOrRegister($"Mods.Whipcackling.Prefixes.Effects.TagDamage");
        public LocalizedText SummonerSpeedTooltip => Language.GetOrRegister($"Mods.Whipcackling.Prefixes.Effects.Speed");
        public LocalizedText SummonerKnockbackTooltip => Language.GetOrRegister($"Mods.Whipcackling.Prefixes.Effects.Knockback");
        public LocalizedText SummonerCritChanceTooltip => Language.GetOrRegister($"Mods.Whipcackling.Prefixes.Effects.CriticalChance");
        public LocalizedText SummonerSlotsTooltip => Language.GetOrRegister($"Mods.Whipcackling.Prefixes.Effects.Slots");
        public LocalizedText SummonerSizeooltip => Language.GetOrRegister($"Mods.Whipcackling.Prefixes.Effects.Size");

        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            yield return new TooltipLine(Mod, "SummonerTagDamagePrefixLine", SummonerTagDamageTooltip.Format((TagDamageMultiplier > 1f ? "+" : "") + (TagDamageMultiplier * 100 - 100).ToString("N0")))
            {
                Visible = TagDamageMultiplier != 1f,
                IsModifier = true,
                IsModifierBad = TagDamageMultiplier < 1f
            };

            yield return new TooltipLine(Mod, "SummonerSpeedPrefixLine", SummonerSpeedTooltip.Format((SpeedMultiplier > 0f ? "+" : "") + (SpeedMultiplier * 100).ToString("N0")))
            {
                Visible = SpeedMultiplier != 0f,
                IsModifier = true,
                IsModifierBad = SpeedMultiplier < 0f
            };

            yield return new TooltipLine(Mod, "SummonerCritChancePrefixLine", SummonerCritChanceTooltip.Format((CritChance * 100).ToString("N0")))
            {
                Visible = CritChance != 0f,
                IsModifier = true
            };

            yield return new TooltipLine(Mod, "SummonerSlotsPrefixLine", SummonerSlotsTooltip.Format((SlotsMultiplier > 1f ? "+" : "") + (SlotsMultiplier * 100 - 100).ToString("N0")))
            {
                Visible = SlotsMultiplier != 1f,
                IsModifier = true,
                IsModifierBad = SlotsMultiplier > 1f
            };

            yield return new TooltipLine(Mod, "SummonerSizePrefixLine", SummonerSizeooltip.Format((SizeMultiplier > 1f ? "+" : "") + (SizeMultiplier * 100 - 100).ToString("N0")))
            {
                Visible = SizeMultiplier != 1f,
                IsModifier = true,
                IsModifierBad = SizeMultiplier < 1f
            };
        }
    }

    public class SummonItemsNoMagePrefixes : GlobalItem
    {
        public override bool AllowPrefix(Item item, int pre)
        {
            if (!WhipcacklingConfig.Instance.NewSummonPrefixes)
                return true;
            if (!item.CountsAsClass<SummonDamageClass>() || item.CountsAsClass<SummonMeleeSpeedDamageClass>() || item.IsWhip())
                return true;
            if (PrefixLoader.GetPrefix(pre) == null)
                return false;
            if (PrefixLoader.GetPrefix(pre).Category == PrefixCategory.Magic)
                return false;

            return true;
        }
    }
}
