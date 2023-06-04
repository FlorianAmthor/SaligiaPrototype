using SuspiciousGames.Saligia.Core.Entities;
using TheKiwiCoder;

namespace SuspiciousGames.Saligia.Core.BehaviourTreeNodes.Decorators
{
    public class IsAtLeastInPhase : DecoratorNode
    {
        public BossPhase neededPhase;

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            if (blackboard.currentPhase >= neededPhase)
                return child.Update();
            else
                return State.Failure;
        }
    }
}