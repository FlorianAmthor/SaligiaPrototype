using SuspiciousGames.Saligia.Core.Entities.Buffs;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace SuspiciousGames.Saligia.Core.Potions
{
    [CreateAssetMenu(menuName = "Saligia/Potions/PotionEffects/Weapon Mp Regen Buff Potion Effect", fileName = "newWeaponMpRegenBuffPotionEffect")]
    public class WeaponMpRegenBuffPotionEffect : BuffPotionEffect<WeaponMpRegenMultiplierBuffData>
    {
        public override void Init(PotionTier potionTier)
        {
            currentTieredEffect = buffTieredEffects[(int)potionTier];

            if (potionEffectDescriptionLocalizedString.ContainsKey("manaOnHitBuff"))
                ((FloatVariable)potionEffectDescriptionLocalizedString["manaOnHitBuff"]).Value = currentTieredEffect.buffData.MpRegenMultiplier * 100f;
            else
                potionEffectDescriptionLocalizedString.Add("manaOnHitBuff", new FloatVariable() { Value = currentTieredEffect.buffData.MpRegenMultiplier * 100f });

            if (potionEffectDescriptionLocalizedString.ContainsKey("duration"))
                ((FloatVariable)potionEffectDescriptionLocalizedString["duration"]).Value = currentTieredEffect.buffData.Duration;
            else
                potionEffectDescriptionLocalizedString.Add("duration", new FloatVariable() { Value = currentTieredEffect.buffData.Duration });

            potionEffectDescriptionLocalizedString.StringChanged += OnPotionEffectLocalizationChange;
        }
    }
}