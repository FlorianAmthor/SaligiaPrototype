using TheKiwiCoder;

namespace SuspiciousGames.Saligia.Core.BehaviourTreeNodes.Actions
{
    public class MoveToNavMeshPosition : ActionNode
    {
        protected override void OnStart()
        {
            context.owner.MovementComponent.SetDestination(blackboard.moveToPosition);
            context.animator.SetTrigger(context.owner.runTriggerHash);
        }

        protected override void OnStop()
        {
            context.animator.ResetTrigger(context.owner.runTriggerHash);
        }

        protected override State OnUpdate()
        {
            if (context.owner.MovementComponent.Agent.pathPending)
            {
                context.animator.SetTrigger(context.owner.runTriggerHash);
                return State.Running;
            }

            if (context.owner.MovementComponent.Agent.remainingDistance < context.owner.MovementComponent.Agent.stoppingDistance)
            {
                return State.Success;
            }

            if (context.owner.MovementComponent.Agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
            {
                return State.Failure;
            }

            context.animator.SetTrigger(context.owner.runTriggerHash);
            return State.Running;
        }
    }
}