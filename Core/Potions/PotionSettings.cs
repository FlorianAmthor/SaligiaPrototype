using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

namespace SuspiciousGames.Saligia.Core.Potions
{
    [CreateAssetMenu(menuName = "Saligia/Potions/Potion", fileName = "newPotion")]
    public class PotionSettings : ScriptableObject
    {
        [System.Serializable]
        private class TieredPotionCharges
        {
            public PotionTier potionTier;
            [Min(0)] public int maximumCharges;
        }

        [System.Serializable]
        private class TieredPotionTextures
        {
            public PotionTier potionTier;
            public Sprite potionIcon;
        }

        [SerializeField] private LocalizedString _potionNameLocalizedString;
        [SerializeField] private Sprite _emptyTier1PotionIcon;
        [SerializeField] private Sprite _emptyTier2PotionIcon;
        [SerializeField] private TieredPotionTextures[] _tieredPotionTextures = new TieredPotionTextures[3];
        private Dictionary<PotionTier, Sprite> _tieredPotionSpriteDict;
        [SerializeField] private TieredPotionCharges[] _tieredPotionCharges = new TieredPotionCharges[3];
        private Dictionary<PotionTier, int> _tieredPotionChargesDict;
        [SerializeField] private PotionEffect _potionEffect;
        public PotionEffect PotionEffect => _potionEffect;

        [field: SerializeField] public int ChargesConsumedOnUse { get; private set; }
        [HideInInspector] public UnityEvent<string> onLocalize;
        
        private void Init()
        {
            if (_tieredPotionCharges == null || _tieredPotionTextures == null)
                return;
            
            _tieredPotionChargesDict = new();
            _tieredPotionSpriteDict = new();

            foreach (var tieredPotionCharge in _tieredPotionCharges)
                _tieredPotionChargesDict.TryAdd(tieredPotionCharge.potionTier, tieredPotionCharge.maximumCharges);
            foreach (var tieredPotionTexture in _tieredPotionTextures)
                _tieredPotionSpriteDict.TryAdd(tieredPotionTexture.potionTier, tieredPotionTexture.potionIcon);
        }

        public string GetLocalizedName()
        {
            return _potionNameLocalizedString.GetLocalizedString();
        }

        public int GetMaximumChargesForTier(PotionTier potionTier)
        {
            if (_tieredPotionChargesDict == null)
                Init();
            if (_tieredPotionChargesDict.TryGetValue(potionTier, out int maximumCharges))
                return maximumCharges;
            return 0;
        }

        public Sprite GetEmpySpriteForTier(PotionTier potionTier)
        {
            if (potionTier == PotionTier.Tier1)
                return _emptyTier1PotionIcon;
            else
                return _emptyTier2PotionIcon;
        }

        public Sprite GetSpriteForTier(PotionTier potionTier)
        {
            if (_tieredPotionSpriteDict == null)
                Init();
            if (_tieredPotionSpriteDict.TryGetValue(potionTier, out var potionIcon))
                return potionIcon;
            return _tieredPotionSpriteDict.GetEnumerator().Current.Value;
        }
    }
}