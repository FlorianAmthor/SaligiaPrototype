using TheKiwiCoder;
using UnityEngine;

namespace SuspiciousGames.Saligia.Core.BehaviourTreeNodes.Actions
{
    public class MoveToTarget : ActionNode
    {
        [SerializeField] private bool _needsMeleeSlot;

        protected override void OnStart() { }

        protected override void OnStop()
        {
            //context.owner.MovementComponent.Agent.isStopped = true;
            context.animator.ResetTrigger(context.owner.runTriggerHash);
        }

        protected override State OnUpdate()
        {
            RequestTargetPosition();
            CalculateMoveToPosition();

            context.animator.SetTrigger(context.owner.runTriggerHash);
            if (blackboard.moveToPosition != context.owner.MovementComponent.Agent.destination)
                context.owner.MovementComponent.SetDestination(blackboard.moveToPosition);

            if (context.owner.MovementComponent.Agent.pathPending)
            {
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

            return State.Running;
        }

        private void CalculateMoveToPosition()
        {
            if (blackboard.followerData.assignedSlot != -1)
            {
                blackboard.moveToPosition = blackboard.target.transform.position +
                    blackboard.followerData.distributor.GetDirection(blackboard.followerData
                    .assignedSlot) * context.owner.MovementComponent.Agent.stoppingDistance * 0.9f;
            }
            else
            {
                blackboard.moveToPosition = blackboard.target.transform.position;
            }
        }

        private void RequestTargetPosition()
        {
            var fromTarget = context.owner.transform.position - blackboard.target.transform.position;
            fromTarget.y = 0;
            blackboard.followerData.requiredPoint = blackboard.target.transform.position + fromTarget.normalized * context.owner.MovementComponent.Agent.stoppingDistance * 0.9f;
            blackboard.followerData.requireSlot = _needsMeleeSlot;
            blackboard.distributor.DistributeTargetPosition(blackboard.followerData);
        }
    }
}