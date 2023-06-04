using SuspiciousGames.Saligia.Core.Entities.Buffs;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace SuspiciousGames.Saligia.Core.Potions
{
    [CreateAssetMenu(menuName = "Saligia/Potions/PotionEffects/Speed Buff Potion Effect", fileName = "newSpeedBuffPotionEffect")]
    public class SpeedBuffPotionEffect : BuffPotionEffect<SpeedBuffData>
    {
        public override void Init(PotionTier potionTier)
        {
            currentTieredEffect = buffTieredEffects[(int)potionTier];

            if (potionEffectDescriptionLocalizedString.ContainsKey("movementBuff"))
                ((FloatVariable)potionEffectDescriptionLocalizedString["movementBuff"]).Value = currentTieredEffect.buffData.SpeedMultiplier * 100f;
            else
                potionEffectDescriptionLocalizedString.Add("movementBuff", new FloatVariable() { Value = currentTieredEffect.buffData.SpeedMultiplier * 100f });

            if (potionEffectDescriptionLocalizedString.ContainsKey("duration"))
                ((FloatVariable)potionEffectDescriptionLocalizedString["duration"]).Value = currentTieredEffect.buffData.Duration;
            else
                potionEffectDescriptionLocalizedString.Add("duration", new FloatVariable() { Value = currentTieredEffect.buffData.Duration });

            potionEffectDescriptionLocalizedString.StringChanged += OnPotionEffectLocalizationChange;
        }
    }
}