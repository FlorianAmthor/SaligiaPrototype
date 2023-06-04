using TheKiwiCoder;
using UnityEngine;

namespace SuspiciousGames.Saligia.Core.BehaviourTreeNodes.Actions
{
    public class RegenerateHealthInfinite : ActionNode
    {
        [Range(0.0f, 1.0f)]
        public float healthPercentPerSecond;

        protected override void OnStart()
        {
            context.owner.HealthComponent.StartRegenerateUntilFull(healthPercentPerSecond);
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            if (context.owner.HealthComponent.CurrentHitPoints == context.owner.HealthComponent.MaxHitPoints)
                return State.Success;
            return State.Running;
        }
    }
}