using SuspiciousGames.Saligia.Core.Entities;
using SuspiciousGames.Saligia.Core.Entities.Buffs;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace SuspiciousGames.Saligia.Core.Potions
{
    [CreateAssetMenu(menuName = "Saligia/Potions/PotionEffects/Regenerate Mind Power Potion Effect", fileName = "newRegenerateMindPowerPotionEffect")]
    public class RegenerateMpPotionEffect : BuffPotionEffect<ManaRegenBuffData>
    {
        public override void Init(PotionTier potionTier)
        {
            currentTieredEffect = buffTieredEffects[(int)potionTier];

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
            if (targetEntity.CastCostComponent)
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
            if (!targetEntity.CastCostComponent || !base.CanBeActivated(targetEntity))
                return false;
            return targetEntity.CastCostComponent.CurrentMindPower < targetEntity.CastCostComponent.MaxMindPower;
        }
    }
}