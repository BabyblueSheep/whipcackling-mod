using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using static CalamityMod.Items.CalamityGlobalItem;

namespace Whipcackling.Content.Rebalances
{
    public class RebalanceSystem : ModSystem
    {
        public static SortedDictionary<int, WeaponStats> VanillaStats = new();
        public static SortedDictionary<int, WeaponStats> CalamityStats = new();

        public override void PostSetupContent()
        {
            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                Item item = new();
                if (i <= 1000)
                    item.SetDefaults1(i);
                else if (i <= 2001)
                    item.SetDefaults2(i);
                else if (i <= 3000)
                    item.SetDefaults3(i);
                else if (i <= 3989)
                    item.SetDefaults4(i);
                else if (i <= 5456)
                    item.SetDefaults5(i);
                else
                    ItemLoader.GetItem(i).NewInstance(item);
                if (!item.CountsAsClass(DamageClass.Summon))
                    continue;

                WeaponStats stats = new()
                {
                    Damage = item.damage,
                    Knockback = item.knockBack,
                    Mana = item.mana,
                    UseTime = item.useTime,
                    UseAnimation = item.useAnimation,
                    Velocity = item.shootSpeed
                };
                VanillaStats.Add(i, stats);

                bool needsTweaking = currentTweaks.TryGetValue(i, out IItemTweak[] tweaks);
                if (!needsTweaking)
                {
                    CalamityStats.Add(i, stats);
                    continue;
                }
                foreach (IItemTweak tweak in tweaks)
                    if (tweak.AppliesTo(item))
                        tweak.ApplyTweak(item);

                WeaponStats calStats = new()
                {
                    Damage = item.damage,
                    Knockback = item.knockBack,
                    Mana = item.mana,
                    UseTime = item.useTime,
                    UseAnimation = item.useAnimation,
                    Velocity = item.shootSpeed
                };
                CalamityStats.Add(i, calStats);
            }
            
        }
    }

    public struct WeaponStats
    {
        public int Damage;
        public float Knockback;
        public int Mana;
        public int UseTime;
        public int UseAnimation;
        public float Velocity;
    }
}
