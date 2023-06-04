using SuspiciousGames.Saligia.Core.Entities.Components;

namespace SuspiciousGames.Saligia.Core.Entities.Buffs
{
    public class BaseMpRegenMultiplierBuff : Buff
    {
        private readonly CastCostComponent _castCostComponent;

        public BaseMpRegenMultiplierBuff(BuffData buff, Entity buffTarget, Entity buffSource) : base(buff, buffTarget, buffSource)
        {
            _castCostComponent = buffTarget.GetComponent<CastCostComponent>();
        }

        protected override void ApplyEffect()
        {
            if (_castCostComponent != null)
                _castCostComponent.BaseMpRegenMultiplier.ApplyMultiplier(((BaseMpRegenMultiplierBuffData)BuffData).MpRegenMultiplier);
        }

        protected override void UndoEffect()
        {
            if (_castCostComponent != null)
                _castCostComponent.BaseMpRegenMultiplier.UndoMultiplier(((BaseMpRegenMultiplierBuffData)BuffData).MpRegenMultiplier);
        }
    }
}
