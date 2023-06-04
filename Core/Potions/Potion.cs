using SuspiciousGames.Saligia.Core.Entities;
using System.Collections.Generic;
using SuspiciousGames.Saligia.Core.Entities.Player;
using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Potions
{
    [System.Serializable]
    public class Potion
    {
        #region private fields
        [SerializeField] private PotionSettings _potionSettings;
        private string _name;
        [SerializeField] private PotionTier _tier;
        private int _currentNumberOfCharges;
        private int _maximumCharges;
        private int _chargesConsumedOnUse;
        private Sprite _potionIcon;
        #endregion

        #region Properties
        public PotionTier Tier => _tier;
        public int MaximumCharges => _maximumCharges;
        public Sprite PotionIcon => _potionIcon;
        public int CurrentNumberOfCharges => _currentNumberOfCharges;
        public float FillPercentage => _currentNumberOfCharges / (float)_maximumCharges;
        public int ChargesConsumedOnUse => _chargesConsumedOnUse;
        public PotionEffect PotionEffect => _potionSettings.PotionEffect;
        #endregion

        public Potion(PotionSettings potionSettings)
        {
            _potionSettings = potionSettings;
            _tier = PotionTier.Tier1;

            _name = _potionSettings.GetLocalizedName();
            UpdatePotion();
        }

        public bool IsUsable(Entity targetEntity)
        {
            return _potionSettings.PotionEffect.CanBeActivated(targetEntity) && _currentNumberOfCharges >= ChargesConsumedOnUse; ;
        }

        public void Use(Entity targetEntity)
        {
            if (_currentNumberOfCharges < _chargesConsumedOnUse)
                return;

            _currentNumberOfCharges -= ChargesConsumedOnUse;
            _potionSettings.PotionEffect.Activate(targetEntity);
        }

        public bool Upgrade()
        {
            if (_tier == PotionTier.Tier3)
                return false;

            _tier++;
            UpdatePotion();
            return true;
        }

        public void RestoreCharges(int amount)
        {
            _currentNumberOfCharges = Mathf.Clamp(_currentNumberOfCharges + amount, 0, _maximumCharges);
            
            if (PlayerEntity.Instance == null)
                return;
            
            if(_currentNumberOfCharges >= _chargesConsumedOnUse)
                PlayerEntity.Instance.PlayerInventory.events.potionEvents.onPotionChargesGained.Invoke();
        }

        public void UpdatePotion()
        {
            _maximumCharges = _potionSettings.GetMaximumChargesForTier(_tier);
            _currentNumberOfCharges = _maximumCharges;
            _potionIcon = _potionSettings.GetSpriteForTier(_tier);

            _potionSettings.PotionEffect.Init(_tier);
            _chargesConsumedOnUse = _potionSettings.ChargesConsumedOnUse;
        }

        public string GetPotionName()
        {
            _name = _potionSettings.GetLocalizedName();
            return _name;
        }

        public string GetPotionDescription()
        {
            return _potionSettings.PotionEffect.EffectDescription;
        }

        public Sprite GetEmptySprite()
        {
            return _potionSettings.GetEmpySpriteForTier(_tier);
        }

        public bool HasPotionSettings(PotionSettings potionSettings)
        {
            return _potionSettings.Equals(potionSettings);
        }
    }
}