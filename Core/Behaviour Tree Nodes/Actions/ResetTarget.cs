using TheKiwiCoder;

namespace SuspiciousGames.Saligia.Core.BehaviourTreeNodes.Actions
{
    public class ResetTarget : ActionNode
    {
        protected override void OnStart()
        {
            if (blackboard.followerData != null)
                blackboard.distributor.UnregisterFollower(blackboard.followerData);
            blackboard.isAggroed = false;
            blackboard.distributor = null;
            blackboard.followerData = null;
            blackboard.target = null;
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            return State.Success;
        }
    }
}