﻿using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Whipcackling.Assets;

namespace Whipcackling.Content.Whips.NuclearWhip
{
    public class NuclearWhipNPCDebuff : ModBuff
    {
        public override string LocalizationCategory => "Whips.NuclearWhip";

        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            NuclearWhipNPC.TaggedAmount += 1;
        }
    }

    public class NuclearWhipNPC : GlobalNPC
    {
        public static int TaggedAmount { get; set; }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (Main.myPlayer != Main.player[projectile.owner].whoAmI)
                return;
            if (!(projectile.minion || ProjectileID.Sets.MinionShot[projectile.type] || projectile.sentry || ProjectileID.Sets.SentryShot[projectile.type]))
                return;
            if (!npc.HasBuff<NuclearWhipNPCDebuff>())
                return;
            float projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
            modifiers.FlatBonusDamage += ConstantsNuclear.TagDamage(TaggedAmount) * projTagMultiplier;

            if (Main.rand.Next(100) < ConstantsNuclear.TagCritChance)
            {
                modifiers.SetCrit();
            }
        }
    }

    public class NuclearWhipSystem : ModSystem
    {
        public static SoundStyle GeigerCounter = new($"{AssetDirectory.AssetPath}Sounds/Whips/NuclearWhip/GeigerCounter")
        {
            IsLooped = true,
        };

        private SlotId _geigerSlot;

        private int _previousAmount;
        private float _volumeLevel;

        public override void PreUpdateNPCs()
        {
            if (NuclearWhipNPC.TaggedAmount > 0)
            {
                float requiredVolume = MathHelper.Min(NuclearWhipNPC.TaggedAmount * 0.1f, 1);
                if (_previousAmount > NuclearWhipNPC.TaggedAmount)
                {
                    _volumeLevel = MathHelper.Max(_volumeLevel - 0.025f, requiredVolume);
                    if (_volumeLevel == requiredVolume)
                        _previousAmount = NuclearWhipNPC.TaggedAmount;
                }
                else if (_previousAmount < NuclearWhipNPC.TaggedAmount)
                {
                    _volumeLevel = MathHelper.Min(_volumeLevel + 0.025f, requiredVolume);
                    if (_volumeLevel == requiredVolume)
                        _previousAmount = NuclearWhipNPC.TaggedAmount;
                }
            }
            else
            {
                _volumeLevel = MathHelper.Max(_volumeLevel - 0.05f, 0);
            }

            SoundEngine.TryGetActiveSound(_geigerSlot, out ActiveSound soundOut);
            if (_volumeLevel > 0)
            {
                if (soundOut == null)
                {
                    _geigerSlot = SoundEngine.PlaySound(GeigerCounter);
                }
                else
                {
                    soundOut.Volume = _volumeLevel;
                }
            }
            else
            {
                if (soundOut != null)
                {
                    soundOut.Stop();
                }
            }

            NuclearWhipNPC.TaggedAmount = 0;
        }
    }
}
