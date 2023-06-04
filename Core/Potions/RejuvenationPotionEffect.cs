using SuspiciousGames.Saligia.Core.Entities;
using SuspiciousGames.Saligia.Core.Entities.Buffs;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace SuspiciousGames.Saligia.Core.Potions
{
    [CreateAssetMenu(menuName = "Saligia/Potions/PotionEffects/Rejuvenation Potion Effect", fileName = "newRejuvenationPotionEffect")]
    public class RejuvenationPotionEffect : BuffPotionEffect<HealthAndManaRegenBuffData>
    {
        public override void Init(PotionTier potionTier)
        {
            currentTieredEffect = buffTieredEffects[(int)potionTier];

            //TODO write a class that simplifies setting these variables

            if (potionEffectDescriptionLocalizedString.ContainsKey("healHP"))
                ((FloatVariable)potionEffectDescriptionLocalizedString["healHP"]).Value = currentTieredEffect.buffData.PercentualHealthRegenerationValue * 100f;
            else
                potionEffectDescriptionLocalizedString.Add("healHP", new FloatVariable() { Value = currentTieredEffect.buffData.PercentualHealthRegenerationValue * 100f });

            if (potionEffectDescriptionLocalizedString.ContainsKey("healMP"))
                ((FloatVariable)potionEffectDescriptionLocalizedString["healMP"]).Value = currentTieredEffect.buffData.PercentualManaRegenerationValue * 100f;
            else
                potionEffectDescriptionLocalizedString.Add("healMP", new FloatVariable() { Value = currentTieredEffect.buffData.PercentualManaRegenerationValue * 100f });


            if (potionEffectDescriptionLocalizedString.ContainsKey("duration"))
                ((FloatVariable)potionEffectDescriptionLocalizedString["duration"]).Value = currentTieredEffect.buffData.Duration;
            else
                potionEffectDescriptionLocalizedString.Add("duration", new FloatVariable() { Value = currentTieredEffect.buffData.Duration });

            potionEffectDescriptionLocalizedString.StringChanged += OnPotionEffectLocalizationChange;
        }

        public override void Activate(Entity targetEntity)
        {
            if (targetEntity.HealthComponent || targetEntity.CastCostComponent)
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
            if ((!targetEntity.HealthComponent && !targetEntity.CastCostComponent) || !base.CanBeActivated(targetEntity))
                return false;

            bool canBeActivated = false;

            if (targetEntity.HealthComponent)
                canBeActivated |= targetEntity.HealthComponent.CurrentHitPoints < targetEntity.HealthComponent.MaxHitPoints;

            if (targetEntity.CastCostComponent)
                canBeActivated |= targetEntity.CastCostComponent.CurrentMindPower < targetEntity.CastCostComponent.MaxMindPower;

            return canBeActivated;
        }
    }
}