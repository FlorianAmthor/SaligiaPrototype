using TheKiwiCoder;
using UnityEngine;

namespace SuspiciousGames.Saligia.Core.BehaviourTreeNodes.Actions
{
    public class SetTargetData : ActionNode
    {
        private enum TargetType
        {
            Self,
            Target
        }

        [SerializeField] private TargetType _targetType;

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            switch (_targetType)
            {
                case TargetType.Self:
                    context.owner.CastComponent.TargetData = new(context.owner.gameObject);
                    return State.Success;
                case TargetType.Target:
                    if (!blackboard.target)
                    {
                        Debug.LogError($"Target not set! Source: {name}");
                        return State.Failure;
                    }
                    context.owner.CastComponent.TargetData = new(blackboard.target.gameObject);
                    return State.Success;
                default:
                    return State.Failure;
            }
        }
    }
}