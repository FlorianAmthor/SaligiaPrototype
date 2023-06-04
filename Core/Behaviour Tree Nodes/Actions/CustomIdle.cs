using UnityEngine;
using TheKiwiCoder;

namespace SuspiciousGames.Saligia.Core.BehaviourTreeNodes.Actions
{
    public class CustomIdle : ActionNode
    {
        public float customIdleCooldownMin;
        public float customIdleCooldownMax;
        private float _lastTimeCustomIdled = 0.0f;
        private float _randomIdleCooldown;
        protected override void OnStart()
        {
            _randomIdleCooldown = Random.Range(customIdleCooldownMin, customIdleCooldownMax);
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (Time.time - _lastTimeCustomIdled >= _randomIdleCooldown)
            {
                context.animator.SetTrigger(context.owner.customIdleTriggerHash);
                _lastTimeCustomIdled = Time.time;
                return State.Success;
            }
            return State.Failure;
        }
    }
}