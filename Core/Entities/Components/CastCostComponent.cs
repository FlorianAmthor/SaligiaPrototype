using SuspiciousGames.Saligia.Core.Skills;
using UnityEngine;
using UnityEngine.Events;

namespace SuspiciousGames.Saligia.Core.Entities.Components
{
    [RequireComponent(typeof(HealthComponent), typeof(CastComponent))]
    public class CastCostComponent : EntityComponent
    {
        [SerializeField] private float _maxMindPower;
        public float MaxMindPower => _maxMindPower;
        public float CurrentMindPower { get; private set; }
        public float MindPowerPercentage => CurrentMindPower / (float)MaxMindPower;
        [field: SerializeField] public Multiplier MpCostMultiplier { get; private set; }
        [field: SerializeField] public Multiplier HealthCostMultiplier { get; private set; }
        [field: SerializeField] public Multiplier BaseMpRegenMultiplier { get; private set; }
        [field: SerializeField] public Multiplier PrimaryAbilityMpRegenMultiplier { get; private set; }

        public UnityEvent<float> OnMpChanged;

        private void Awake()
        {
            CurrentMindPower = _maxMindPower;
            MpCostMultiplier.ResetMultiplier();
            HealthCostMultiplier.ResetMultiplier();
        }

        public void AddMindPower(float amount, bool sourceIsPrimaryAbility = false, bool ignoreRegenMultipliers = false)
        {
            if (!ignoreRegenMultipliers)
            {
                if (sourceIsPrimaryAbility)
                    amount *= PrimaryAbilityMpRegenMultiplier.Value * BaseMpRegenMultiplier.Value;
                else
                    amount *= BaseMpRegenMultiplier.Value;
            }

            CurrentMindPower = Mathf.Clamp(CurrentMindPower + ((int)(amount * 100.0f)) / 100.0f, 0, _maxMindPower);
            OnMpChanged.Invoke(MindPowerPercentage);
        }

        public bool CanPaySkillCost(SkillCost cost)
        {
            return Owner.HealthComponent.CurrentHitPoints > CalculateLifeCost(cost) && CurrentMindPower >= CalculateMpCost(cost);
        }

        public float CalculateMpCost(SkillCost cost)
        {
            return ((int)((cost.CurrentMpCost * MpCostMultiplier.Value * cost.MpCostMultiplier.Value) * 100.0f)) / 100.0f;
        }

        public float CalculateMpCost(float mp)
        {
            return ((int)(mp * 100.0f * MpCostMultiplier.Value)) / 100.0f;
        }

        public int CalculateLifeCost(SkillCost cost)
        {
            return Mathf.RoundToInt((cost.CurrentHealthCost * HealthCostMultiplier.Value * cost.HealthCostMultiplier.Value));
        }

        public void Pay(SkillCost cost)
        {
            if ((cost.CurrentMpCost * cost.MpCostMultiplier.Value * MpCostMultiplier.Value) == 0 && 
                (cost.CurrentHealthCost * cost.HealthCostMultiplier.Value * HealthCostMultiplier.Value) == 0)
                return;
            Owner.ApplyDamage(new DamageData(CalculateLifeCost(cost), isTrueDamage: true));
            CurrentMindPower -= CalculateMpCost(cost);
            OnMpChanged.Invoke(MindPowerPercentage);
        }

        public void Pay(float mp)
        {
            if (mp == 0)
                return;
            CurrentMindPower -= CalculateMpCost(mp);
            OnMpChanged.Invoke(MindPowerPercentage);
        }
    }
}
