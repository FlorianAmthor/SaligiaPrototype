using Gamekit3Dk;
using SuspiciousGames.Saligia.Core.Entities.Player;
using TheKiwiCoder;

namespace SuspiciousGames.Saligia.Core.BehaviourTreeNodes.Actions
{
    public class SetPlayerTarget : ActionNode
    {
        protected override void OnStart()
        {
            blackboard.target = PlayerEntity.Instance;
            if (blackboard.distributor != null && blackboard.followerData != null)
            {
                blackboard.distributor.UnregisterFollower(blackboard.followerData);
                blackboard.followerData = null;
            }
            blackboard.distributor = blackboard.target.GetComponent<TargetDistributor>();
            if (blackboard.followerData == null)
                blackboard.followerData = blackboard.distributor.RegisterNewFollower();
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            if (blackboard.target.Equals(PlayerEntity.Instance))
                return State.Success;
            else
                return State.Failure;
        }
    }
}