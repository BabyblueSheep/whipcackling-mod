using CalamityMod.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Whipcackling.Content.Rebalances
{
    public class RebalanceSystem : ModSystem
    {
        public static SortedDictionary<int, WeaponStats> VanillaStats;
        public static SortedDictionary<int, WeaponStats> CalamityStats;

        public override void PostSetupContent()
        {
            foreach (MethodInfo method in  typeof(CalamityGlobalItem).GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)) 
            {
                Mod.Logger.Info($"{method.Name}");
            }

            MethodInfo applyTweaksInfo = typeof(CalamityGlobalItem).GetMethod("SetDefaults_ApplyTweaks", BindingFlags.NonPublic | BindingFlags.Static);
            //Mod.Logger.Info(applyTweaksInfo);
            var SetDefaults_ApplyTweaks = (Action<Item>)Delegate.CreateDelegate(typeof(Action<Item>), applyTweaksInfo, true);

            VanillaStats = new();
            CalamityStats = new();

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
                else if (i <= ItemID.Count)
                    item.SetDefaults5(i);
                else
                    item.SetDefaults(i);
                if (!item.CountsAsClass(DamageClass.Summon))
                    continue;

                WeaponStats stats = new();
                stats.Damage = item.damage;
                stats.Knockback = item.knockBack;
                stats.Mana = item.mana;
                stats.UseTime = item.useTime;
                stats.UseAnimation = item.useAnimation;
                stats.Velocity = item.shootSpeed;
                VanillaStats.Add(i, stats);

                SetDefaults_ApplyTweaks(item);
                WeaponStats calStats = new();
                calStats.Damage = item.damage;
                calStats.Knockback = item.knockBack;
                calStats.Mana = item.mana;
                calStats.UseTime = item.useTime;
                calStats.UseAnimation = item.useAnimation;
                calStats.Velocity = item.shootSpeed;
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

        public override string ToString()
        {
            return $"Damage: {Damage}";
        }
    }
}
