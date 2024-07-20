using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Content.Sources;
using System.IO;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Whipcackling.Assets;
using Whipcackling.Common.Utilities;
using Whipcackling.Content.Whips.BloodstoneWhip;

namespace Whipcackling
{
    public class WhipcacklingMod : Mod
    {
        public override void Load()
        {
            
        }

        public override void PostSetupContent()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            LoadShaders();
        }

        private void LoadShaders()
        {
            RegisterScreenFilter(AssetDirectory.Effects.NegazoneEffect);
        }

        private void RegisterScreenFilter(Asset<Effect> shader, EffectPriority priority = EffectPriority.High)
        {
            string name = shader.Name.Split('\\')[^1];
            Filters.Scene[$"Whipcackling:{name}"] = new Filter(new(shader, $"{name}Pass"), priority);
            Filters.Scene[$"Whipcackling:{name}"].Load();
        }

        public override IContentSource CreateDefaultContentSource()
        {
            SmartContentSource source = new(base.CreateDefaultContentSource());
            source.AddDirectoryRedirect("Content", "Assets/Textures");
            source.AddDirectoryRedirect("Common", "Assets/Textures");
            return source;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MessageType messageType = (MessageType)reader.ReadByte();

            switch (messageType)
            {
                case MessageType.BloodstoneWhipAwakenServer:
                    int playerWhoAmIServer = reader.ReadInt32();
                    ModPacket packet = GetPacket();
                    packet.Write((byte)MessageType.BloodstoneWhipAwakenClient);
                    packet.Write(playerWhoAmIServer);
                    packet.Send(-1, whoAmI);
                    break;
                case MessageType.BloodstoneWhipAwakenClient:
                    int playerWhoAmIClient = reader.ReadInt32();
                    BloodstoneWhip.AwakenWhip(Main.player[playerWhoAmIClient]);
                    break;
                case MessageType.BloodstoneWhipSyncCharge:
                    byte playerNumber = reader.ReadByte();
                    BloodstoneWhipPlayer bloodstonePlayer = Main.player[playerNumber].GetModPlayer<BloodstoneWhipPlayer>();
                    bloodstonePlayer.RecievePlayer(reader);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        bloodstonePlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
            }
        }

        public enum MessageType
        {
            BloodstoneWhipAwakenServer,
            BloodstoneWhipAwakenClient,
            BloodstoneWhipSyncCharge,
        }
    }
}
