using TheKiwiCoder;
using UnityEngine;

namespace SuspiciousGames.Saligia.Core.BehaviourTreeNodes.Decorators
{
    public class IsTargetInLineOfSight : DecoratorNode
    {
        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (blackboard.target == null)
                return State.Failure;

            if (context.owner.TargetingComponent.IsInLineOfSight(blackboard.target))
                return child.Update();
            else
                return State.Failure;
        }
    }
}