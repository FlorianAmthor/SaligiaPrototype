using System;
using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Entities.Components
{
    [Serializable]
    public class Multiplier
    {
        [SerializeField] private float _baseValue = 1.0f;
        private float _sumMultipliers;
        public float Value { get; private set; }

        public Action<float> onMultiplierchange;

        public Multiplier()
        {
            _baseValue = 1.0f;
            _sumMultipliers = 0;
            Value = _baseValue;
        }

        public Multiplier(float baseValue)
        {
            _baseValue = baseValue;
            _sumMultipliers = 0;
            Value = _baseValue;
        }

        public void ApplyMultiplier(float multiplier)
        {
            _sumMultipliers += multiplier;
            CalculateCurrentValue();
        }

        public void UndoMultiplier(float multiplier)
        {
            _sumMultipliers -= multiplier;
            CalculateCurrentValue();
        }

        public void ResetMultiplier()
        {
            _sumMultipliers = 0;
            Value = _baseValue;
            onMultiplierchange?.Invoke(Value);
        }

        private void CalculateCurrentValue()
        {
            Value = _baseValue * (1 + _sumMultipliers);
            onMultiplierchange?.Invoke(Value);
        }
    }
}
