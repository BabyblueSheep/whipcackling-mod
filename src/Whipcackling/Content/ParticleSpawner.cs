using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Whipcackling.Core.Particles;
using Whipcackling.Core.Particles.Components;

namespace Whipcackling.Content
{
    public class ParticleSpawner : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 1;
            Item.height = 1;

            Item.useTime = 1;
            Item.useAnimation = 1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
        }

        public override bool AltFunctionUse(Player player) => true;


        public override bool? UseItem(Player player)
        {
            for (int i = 0; i < 100; i++)
            {
                ParticleSystem.World.Create(
                    (Position)(Main.MouseWorld + new Vector2(Main.rand.NextFloat(-100f, 100), Main.rand.NextFloat(-100f, 100))),
                    (Scale)Vector2.One,
                    (Rotation)Main.rand.NextFloat(MathHelper.TwoPi),
                    (Color)Color.White,
                    (UVCoordinates)ParticleAtlasSystem.AtlasDefinitions["HolyGlowLine"]
                    );
            }
            Main.NewText(ParticleSystem.World.CountEntities(ParticleSystem.DrawableParticle));

            return true;
        }
    }
}
