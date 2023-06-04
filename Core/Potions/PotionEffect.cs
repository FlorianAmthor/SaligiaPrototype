using SuspiciousGames.Saligia.Core.Entities;
using UnityEngine;
using UnityEngine.Localization;

namespace SuspiciousGames.Saligia.Core.Potions
{
    public abstract class PotionEffect : ScriptableObject
    {
        [SerializeField] protected LocalizedString potionEffectDescriptionLocalizedString;
        public string EffectDescription { get; protected set; }

        public abstract void Init(PotionTier potionTier);
        public abstract void Activate(Entity targetEntity);
        public abstract bool CanBeActivated(Entity targetEntity);
        protected abstract void CancelEffect(Entity targetEntity);
        protected void OnPotionEffectLocalizationChange(string value)
        {
            EffectDescription = value;
        }
    }
}