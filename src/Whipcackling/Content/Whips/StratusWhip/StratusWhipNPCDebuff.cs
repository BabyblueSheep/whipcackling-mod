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
        }
    }

    public class StratusWhipNPCDebuffRed : StratusWhipNPCDebuff { }
    public class StratusWhipNPCDebuffYellow : StratusWhipNPCDebuff { }
    public class StratusWhipNPCDebuffBlue : StratusWhipNPCDebuff { }
    public class StratusWhipNPCDebuffPurple : StratusWhipNPCDebuff { }
    public class StratusWhipNPCDebuffWhite : StratusWhipNPCDebuff { }
}
