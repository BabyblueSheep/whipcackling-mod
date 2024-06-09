using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Whipcackling.Content.Whips.NuclearWhip;

namespace Whipcackling.Content.Whips.StratusWhip
{
    public abstract class StratusWhipNPCDebuff : ModBuff
    {
        public override string LocalizationCategory => "Whips.StratusWhip";

        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
            BuffID.Sets.CanBeRemovedByNetMessage[Type] = true;
        }
    }

    public class StratusWhipNPCDebuffRed : StratusWhipNPCDebuff { }
    public class StratusWhipNPCDebuffYellow : StratusWhipNPCDebuff { }
    public class StratusWhipNPCDebuffBlue : StratusWhipNPCDebuff { }
    public class StratusWhipNPCDebuffPurple : StratusWhipNPCDebuff { }
    public class StratusWhipNPCDebuffWhite : StratusWhipNPCDebuff { }

    public class StratusWhipNPC : GlobalNPC
    {
        public int RedStarProjectile;
        public int YellowStarProjectile;
        public int BlueStarProjectile;
        public int PurpleStarProjectile;
        public int WhiteStarProjectile;

        public override bool InstancePerEntity => true;

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (Main.myPlayer != Main.player[projectile.owner].whoAmI)
                return;
            if (!(projectile.minion || ProjectileID.Sets.MinionShot[projectile.type] || projectile.sentry || ProjectileID.Sets.SentryShot[projectile.type]))
                return;
            List<int> debuffs = [];
            for (int i = 0; i < StratusWhipProjectile.TagDebuffList.Length; i++)
            {
                int debuff = StratusWhipProjectile.TagDebuffList[i];
                if (npc.HasBuff(debuff))
                {
                    debuffs.Add(debuff);
                }
            }
            if (debuffs.Count <= 0)
                return;

            int randomDebuff = Main.rand.NextFromCollection(debuffs);
            npc.RequestBuffRemoval(randomDebuff);
        }
    }
}
