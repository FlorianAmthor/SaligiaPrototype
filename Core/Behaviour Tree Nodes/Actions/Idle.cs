using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;

namespace SuspiciousGames.Saligia.Core.BehaviourTreeNodes.Actions
{
    public class Idle : ActionNode
    {

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            context.animator.SetTrigger(context.owner.idleTriggerHash);
            return State.Success;
        }
    }
}