using SuspiciousGames.Saligia.Core.Entities;
using SuspiciousGames.Saligia.Core.Entities.Buffs;
using SuspiciousGames.Saligia.Core.Entities.Components;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace SuspiciousGames.Saligia.Core.Potions
{
    [CreateAssetMenu(menuName = "Saligia/Potions/PotionEffects/Regenerate Health Potion Effect", fileName = "newRegenerateHealthPotionEffect")]
    public class RegenerateHealthPotionEffect : BuffPotionEffect<HealthRegenBuffData>
    {
        public override void Init(PotionTier potionTier)
        {
            currentTieredEffect = buffTieredEffects[(int)potionTier];

            //TODO write a class that simplifies setting these variables

            if (potionEffectDescriptionLocalizedString.ContainsKey("healHP"))
                ((FloatVariable)potionEffectDescriptionLocalizedString["healHP"]).Value = currentTieredEffect.buffData.PercentualHealthRegenerationValue * 100f;
            else
                potionEffectDescriptionLocalizedString.Add("healHP", new FloatVariable() { Value = currentTieredEffect.buffData.PercentualHealthRegenerationValue * 100f });

            if (potionEffectDescriptionLocalizedString.ContainsKey("duration"))
                ((FloatVariable)potionEffectDescriptionLocalizedString["duration"]).Value = currentTieredEffect.buffData.Duration;
            else
                potionEffectDescriptionLocalizedString.Add("duration", new FloatVariable() { Value = currentTieredEffect.buffData.Duration });

            potionEffectDescriptionLocalizedString.StringChanged += OnPotionEffectLocalizationChange;
        }

        public override void Activate(Entity targetEntity)
        {
            if (targetEntity.HealthComponent)
            {
                base.Activate(targetEntity);
                if (cancelOnDamage)
                {
                    ownerEntity = targetEntity;
                    ownerEntity.HealthComponent.OnReceiveDamage.AddListener(OnEntityReceiveDamage);
                }
            }
        }

        public override bool CanBeActivated(Entity targetEntity)
        {
            if (!targetEntity.HealthComponent || !base.CanBeActivated(targetEntity))
                return false;

            return targetEntity.HealthComponent.CurrentHitPoints < targetEntity.HealthComponent.MaxHitPoints;
        }
    }
}