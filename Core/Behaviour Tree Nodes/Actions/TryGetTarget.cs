using TheKiwiCoder;
using UnityEngine;

namespace SuspiciousGames.Saligia.Core.BehaviourTreeNodes.Actions
{
    public class TryGetTarget : ActionNode
    {
        [SerializeField] private float _range;
        [SerializeField] private LayerMask _possibleTargets;

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (context.owner.TargetingComponent.FindTarget(_range, _possibleTargets))
            {
                context.owner.CastComponent.TargetData = new Entities.Components.TargetData(context.owner.TargetingComponent.Target.gameObject);
                return State.Success;
            }
            return State.Failure;
        }
    }
}