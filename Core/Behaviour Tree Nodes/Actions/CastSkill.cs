using SuspiciousGames.Saligia.Core.Skills;
using TheKiwiCoder;
using UnityEngine;

namespace SuspiciousGames.Saligia.Core.BehaviourTreeNodes.Actions
{
    public class CastSkill : ActionNode
    {
        [SerializeField] private BaseSkill _skillToCast;
        private bool _isCastSuccessfull;
        protected override void OnStart()
        {
            _isCastSuccessfull = false;
            if (context.owner.CastComponent)
                _isCastSuccessfull = context.owner.CastComponent.Cast(_skillToCast);
            context.owner.MovementComponent.Agent.ResetPath();
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (!_isCastSuccessfull)
                return State.Failure;
            if (blackboard.isInSkillAnimation || blackboard.isInCastAnimation)
                return State.Running;
            return State.Success;
        }
    }
}