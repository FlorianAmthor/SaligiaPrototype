using SuspiciousGames.Saligia.Core.Entities.Components;

namespace SuspiciousGames.Saligia.Core.Entities.Buffs
{
    public class WeaponMpRegenMultiplierBuff : Buff
    {
        private readonly CastCostComponent _castCostComponent;

        public WeaponMpRegenMultiplierBuff(BuffData buff, Entity buffTarget, Entity buffSource) : base(buff, buffTarget, buffSource)
        {
            _castCostComponent = buffTarget.GetComponent<CastCostComponent>();
        }

        protected override void ApplyEffect()
        {
            if (_castCostComponent != null)
                _castCostComponent.PrimaryAbilityMpRegenMultiplier.ApplyMultiplier(((WeaponMpRegenMultiplierBuffData)BuffData).MpRegenMultiplier);
        }

        protected override void UndoEffect()
        {
            if (_castCostComponent != null)
                _castCostComponent.PrimaryAbilityMpRegenMultiplier.UndoMultiplier(((WeaponMpRegenMultiplierBuffData)BuffData).MpRegenMultiplier);
        }
    }
}
