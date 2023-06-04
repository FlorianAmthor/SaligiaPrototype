using SuspiciousGames.Saligia.Core.Entities.Components;
using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Entities.Buffs
{
    public class GlobalDamageBuff : Buff
    {
        private readonly CastComponent _castComponent;

        public GlobalDamageBuff(BuffData buff, Entity buffTarget, Entity buffSource) : base(buff, buffTarget, buffSource)
        {
            _castComponent = buffTarget.GetComponent<CastComponent>();
        }

        protected override void ApplyEffect()
        {
            if (_castComponent != null)
                _castComponent.DamageMultiplier.ApplyMultiplier(((GlobalDamageBuffData)BuffData).DamageMultiplier);
        }

        protected override void UndoEffect()
        {
            if (_castComponent != null)
                _castComponent.DamageMultiplier.UndoMultiplier(((GlobalDamageBuffData)BuffData).DamageMultiplier);
        }
    }
}
