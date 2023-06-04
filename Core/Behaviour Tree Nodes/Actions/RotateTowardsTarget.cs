using TheKiwiCoder;

namespace SuspiciousGames.Saligia.Core.BehaviourTreeNodes.Actions
{
    public class RotateTowardsTarget : ActionNode
    {
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            context.owner.MovementComponent.LookAt(blackboard.target.transform);

            return State.Success;
        }
    }
}