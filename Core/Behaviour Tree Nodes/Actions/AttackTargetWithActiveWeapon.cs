using TheKiwiCoder;

namespace SuspiciousGames.Saligia.Core.BehaviourTreeNodes.Actions
{
    public class AttackTargetWithActiveWeapon : ActionNode
    {
        private bool _isAttackSuccessfull;
        protected override void OnStart()
        {
            _isAttackSuccessfull = false;
            if (context.owner.WeaponComponent)
                _isAttackSuccessfull = context.owner.WeaponComponent.AttackWithActiveWeapon();
            context.owner.MovementComponent.Agent.ResetPath();
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (!_isAttackSuccessfull)
                return State.Failure;
            if (blackboard.isInSkillAnimation)
                return State.Running;
            return State.Success;
        }
    }
}