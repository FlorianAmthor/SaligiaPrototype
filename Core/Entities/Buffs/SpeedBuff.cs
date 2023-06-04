using SuspiciousGames.Saligia.Core.Entities.Components;

namespace SuspiciousGames.Saligia.Core.Entities.Buffs
{
    public class SpeedBuff : Buff
    {
        private readonly MovementComponent _movementComponent;

        public SpeedBuff(BuffData buff, Entity buffTarget, Entity buffSource) : base(buff, buffTarget, buffSource)
        {
            _movementComponent = buffTarget.GetComponent<MovementComponent>();
        }

        protected override void ApplyEffect()
        {
            if (_movementComponent != null)
                _movementComponent.MovementSpeedMultiplier.ApplyMultiplier(((SpeedBuffData)BuffData).SpeedMultiplier);
        }

        protected override void UndoEffect()
        {
            if (_movementComponent != null)
                _movementComponent.MovementSpeedMultiplier.UndoMultiplier(((SpeedBuffData)BuffData).SpeedMultiplier);
        }
    }
}
