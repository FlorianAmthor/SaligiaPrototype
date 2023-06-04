using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Entities.Buffs
{
    //TODO: what to do here, own CC category CastComponent.BlockCasting???
    public class StunBuff : Buff
    {
        private Entity _affectedEntity;
        private static readonly int IsStunned = Animator.StringToHash("IsStunned");

        public StunBuff(BuffData buffData, Entity buffTarget, Entity source) : base(buffData, buffTarget, source)
        {
        }

        protected override void ApplyEffect()
        {
            if (buffTarget.TryGetComponent(out _affectedEntity))
            {
                _affectedEntity.MovementComponent.BlockMovement(true);
                _affectedEntity.MovementComponent.BlockRotation(true);
                _affectedEntity.SetStunned(true);
                _affectedEntity.Animator.SetBool(IsStunned, true);
                //TODO add stun effects
            }
        }

        protected override void UndoEffect()
        {
            if (_affectedEntity)
            {
                _affectedEntity.MovementComponent.BlockMovement(false);
                _affectedEntity.MovementComponent.BlockRotation(false);
                _affectedEntity.SetStunned(false);
                _affectedEntity.Animator.SetBool(IsStunned, false);
                //TODO remove stun effects
            }
        }
    }
}
