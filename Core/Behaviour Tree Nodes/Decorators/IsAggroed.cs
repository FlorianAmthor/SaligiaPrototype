using TheKiwiCoder;

namespace SuspiciousGames.Saligia.Core.BehaviourTreeNodes.Decorators
{
    public class IsAggroed : DecoratorNode
    {
        private bool _prevIsAggroed = false;
        protected override void OnStart()
        {
            //context.owner.Animator.ResetTrigger(context.owner.idleTriggerHash);
            //context.owner.Animator.ResetTrigger(context.owner.customIdleTriggerHash);
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            if (_prevIsAggroed != blackboard.isAggroed)
            {
                _prevIsAggroed = blackboard.isAggroed;
                blackboard.OnIsAggroedChanged.Invoke(blackboard.isAggroed);
            }
            if (blackboard.isAggroed && !blackboard.shouldReset)
                return child.Update();
            else
                return State.Failure;
        }
    }
}