using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Whipcackling.Assets;

namespace Whipcackling.Content.Whips.BloodstoneWhip
{
    public class BloodstoneWhipPlayer : ModPlayer
    {
        public static SoundStyle AwakenActivate = new($"{AssetDirectory.AssetPath}Sounds/Whips/BloodstoneWhip/AwakenActivate")
        {
            MaxInstances = 0,
        };

        public static SoundStyle AwakenEnd = new($"{AssetDirectory.AssetPath}Sounds/Whips/BloodstoneWhip/AwakenEnd")
        {
            MaxInstances = 0,
        };

        public bool IsAwakened { get; set; }
        public float BloodCharge { get; set; }

        public override void PostUpdateBuffs()
        {
            if (IsAwakened)
            {
                BloodCharge -= ConstantsBloodstone.ChargeLost;

                if (BloodCharge <= 0)
                {
                    IsAwakened = false;
                    SoundEngine.PlaySound(AwakenEnd, Player.Center);
                }
            }

            BloodCharge = MathHelper.Clamp(BloodCharge, 0, 1);
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            BloodstoneWhipPlayer clone = (BloodstoneWhipPlayer)clientPlayer;

            if (BloodCharge != clone.BloodCharge)
                SyncPlayer(-1, Main.myPlayer, false);
        }

        public override void CopyClientState(ModPlayer targetCopy)
        {
            BloodstoneWhipPlayer clone = (BloodstoneWhipPlayer)targetCopy;
            clone.BloodCharge = BloodCharge;
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)WhipcacklingMod.MessageType.BloodstoneWhipSyncCharge);
            packet.Write((byte)Player.whoAmI);
            packet.Write(BloodCharge);
            packet.Send(toWho, fromWho);
        }

        public void RecievePlayer(BinaryReader reader)
        {
            BloodCharge = reader.ReadSingle();
        }
    }
    public class BloodstoneWhipBarSystem : ModSystem
    {
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {

            int barIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Entity Health Bars"));
            if (barIndex != -1)
            {
                layers.Insert(barIndex, new LegacyGameInterfaceLayer(
                    "Whipcackling: Bloodstone Bars",
                    delegate
                    {
                        Player player = Main.LocalPlayer;

                        if (player.HeldItem.type != ModContent.ItemType<BloodstoneWhip>())
                        {
                            return true;
                        }

                        Vector2 position = player.Center;
                        position.Y -= 48;

                        Color chargeColor = Color.Lerp(Color.Maroon, Color.OrangeRed, player.GetModPlayer<BloodstoneWhipPlayer>().BloodCharge);
                        Main.spriteBatch.Draw(TextureAssets.Hb2.Value, position - Main.screenPosition, TextureAssets.Hb2.Frame(), chargeColor, 0f, TextureAssets.Hb2.Size() * 0.5f, 1f, SpriteEffects.None, 0);
                        Rectangle fill = new Rectangle(0, 0, (int)(TextureAssets.Hb1.Width() * player.GetModPlayer<BloodstoneWhipPlayer>().BloodCharge), TextureAssets.Hb1.Height());
                        Main.spriteBatch.Draw(TextureAssets.Hb1.Value, position - Main.screenPosition, fill, chargeColor, 0f, TextureAssets.Hb1.Size() * 0.5f, 1f, SpriteEffects.None, 0);

                        return true;
                    },
                    InterfaceScaleType.Game));
            }
        }
    }
}
