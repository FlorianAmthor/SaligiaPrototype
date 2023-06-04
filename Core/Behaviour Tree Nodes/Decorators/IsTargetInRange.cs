using TheKiwiCoder;
using UnityEngine;

namespace SuspiciousGames.Saligia.Core.BehaviourTreeNodes.Decorators
{
    public class IsTargetInRange : DecoratorNode
    {
        [SerializeField] private float _minRange;
        [SerializeField] private float _maxRange;

        private float _distanceToTarget;
        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (!blackboard.target)
            {
                Debug.LogError($"No target set in {name}!");
                return State.Failure;
            }
            _distanceToTarget = Vector3.Distance(blackboard.target.transform.position, context.transform.position);

            if (_distanceToTarget <= _maxRange &&
                _distanceToTarget >= _minRange)
                return child.Update();
            else
                return State.Failure;
        }
    }
}