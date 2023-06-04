using SuspiciousGames.Saligia.Core.Entities;
using SuspiciousGames.Saligia.Core.Entities.Buffs;
using SuspiciousGames.Saligia.Core.Entities.Components;
using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Potions
{
    public abstract class BuffPotionEffect<T> : PotionEffect where T : BuffData
    {
        [SerializeField] protected BuffPotionTieredEffect<T>[] buffTieredEffects = new BuffPotionTieredEffect<T>[3];

        protected BuffPotionTieredEffect<T> currentTieredEffect;
        public IReadOnlyList<BuffPotionTieredEffect<T>> RegenHealthTieredEffects => buffTieredEffects;

        [SerializeField] protected bool cancelOnDamage;
        protected Entity ownerEntity;

        private void OnEnable()
        {
            currentTieredEffect = buffTieredEffects[0];
            Init(currentTieredEffect.potionTier);
        }

        public override void Activate(Entity targetEntity)
        {
            if (targetEntity.BuffComponent)
                targetEntity.BuffComponent.AddBuff(currentTieredEffect.buffData);
        }

        public override bool CanBeActivated(Entity targetEntity)
        {
            return targetEntity.BuffComponent;
        }

        protected override void CancelEffect(Entity targetEntity)
        {
            if (targetEntity.BuffComponent)
                targetEntity.BuffComponent.RemoveBuff(currentTieredEffect.buffData);
        }

        protected void OnEntityReceiveDamage(DamageData damageData)
        {
            if (ownerEntity.HealthComponent)
                ownerEntity.HealthComponent.OnReceiveDamage.RemoveListener(OnEntityReceiveDamage);
            CancelEffect(ownerEntity);
        }
    }
}